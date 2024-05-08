using Jwt.Core.Contexts.AccountContext.Entities;
using Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Contracts;
using MediatR;
using System.Net;

namespace Jwt.Core.Contexts.AccountContext.UseCases.Authenticate;
public class Handler : IRequestHandler<Request, Response>
{
    private readonly IRepository _repository;

    public Handler(IRepository repository) => _repository = repository;

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        #region 01 - Valida o Request

        try
        {
            var res = Specification.Ensure(request);
            if (!res.IsValid)
                return new Response("Requisição inválida", (int)HttpStatusCode.BadRequest, res.Notifications);
        }
        catch
        {
            return new Response("Não foi possível validar sua requisição", (int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 02 - Obtem o perfil

        User? user;
        try
        {
            user = await _repository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return new Response("Perfil não encontrado", (int)HttpStatusCode.NotFound);
        }
        catch (Exception e)
        {
            return new Response("Não foi possível recuperar seu perfil", (int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 03 -  Verifica se a senha é válida

        if (!user.Password.Challenge(request.Password))
            return new Response("Usuário ou senha inválidos", (int)HttpStatusCode.BadRequest);

        #endregion

        #region 04 -  Verifica se a conta está verificada

        try
        {
            if (!user.Email.Verification.IsActive)
                return new Response("Conta inativa", (int)HttpStatusCode.BadRequest);
        }
        catch
        {
            return new Response("Não foi possível verificar seu perfil", (int)HttpStatusCode.InternalServerError);
        }

        #endregion

        #region 05 - Retorna os dados

        try
        {
            var data = new ResponseData
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Roles = user.Roles.Select(x => x.Name).ToArray()
            };

            return new Response(string.Empty, data);
        }
        catch
        {
            return new Response("Não foi possível obter os dados do perfil", (int)HttpStatusCode.InternalServerError);
        }

        #endregion
    }
}
