using Jwt.Core.Contexts.AccountContext.Entities;
using Jwt.Core.Contexts.AccountContext.UseCases.Create.Contracts;
using Jwt.Core.Contexts.AccountContext.ValueObjects;
using MediatR;
using System.Net;

namespace Jwt.Core.Contexts.AccountContext.UseCases.Create;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IRepository _repository;
    private readonly IService _service;

    public Handler(IRepository repository, IService service)
    {
        _repository = repository;
        _service = service;
    }


    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        #region 01 -  Valida a requisição

        try
        {
            var res = Specification.Ensure(request);
            if (!res.IsValid)
                return new Response("Requisição inválida", (int)HttpStatusCode.BadRequest, res.Notifications);
        }
        catch
        {
            return new Response("Não foi possível validar sua requisição",(int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 02 - Gerar os Objetos

        Email email;
        Password password;
        User user;

        try
        {
            email = new Email(request.Email);
            password = new Password(request.Password);
            user = new User(request.Name, email, password);
        }
        catch (Exception ex)
        {
            return new Response(ex.Message, (int)HttpStatusCode.BadRequest);
        }

        #endregion

        #region 03 - Verifica se o usuário existe no banco

        try
        {
            var exists = await _repository.AnyAsync(request.Email, cancellationToken);
            if (exists)
                return new Response("Este E-mail já está em uso", (int)HttpStatusCode.BadRequest);
        }
        catch
        {
            return new Response("Falha ao verificar E-mail cadastrado", (int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 04 - Persiste os dados

        try
        {
            await _repository.SaveAsync(user, cancellationToken);
        }
        catch
        {
            return new Response("Falha ao persistir dados", (int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 05 - Envia E-mail de ativação

        try
        {
            await _service.SendVerificationEmailAsync(user, cancellationToken);
        }
        catch
        {
            // Do nothing
        }

        #endregion

        return new Response(
            "Conta criada",
            new ResponseData(user.Id, user.Name, user.Email));
    }
}