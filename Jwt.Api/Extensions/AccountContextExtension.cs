using MediatR;
using System.Net;

namespace Jwt.Api.Extensions;
public static class AccountContextExtension
{
    public static void AddAccountContext(this WebApplicationBuilder builder)
    {
        //registra todas as dependencias 
        #region Criação 

        builder.Services.AddTransient<
            Jwt.Core.Contexts.AccountContext.UseCases.Create.Contracts.IRepository,
            Jwt.Infra.Contexts.AccountContext.UseCases.Create.Repository>();

        builder.Services.AddTransient<
            Jwt.Core.Contexts.AccountContext.UseCases.Create.Contracts.IService,
            Jwt.Infra.Contexts.AccountContext.UseCases.Create.Service>();

        #endregion

        #region Autenticação

        builder.Services.AddTransient<
            Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Contracts.IRepository,
            Jwt.Infra.Contexts.AccountContext.UseCases.Authenticate.Repository>();

        #endregion
    }

    public static void MapAccountEndpoints(this WebApplication app)
    {
        //Registra todos os endpoints
        #region Criação

        app.MapPost("api/v1/users", async (
             Jwt.Core.Contexts.AccountContext.UseCases.Create.Request request,
             IRequestHandler<
                 Jwt.Core.Contexts.AccountContext.UseCases.Create.Request,
                 Jwt.Core.Contexts.AccountContext.UseCases.Create.Response> handler) =>
        {
            var result = await handler.Handle(request, new CancellationToken());
            return result.IsSuccess
                ? Results.Created($"api/v1/users/{result.Data?.Id}", result)
                : Results.Json(result, statusCode: result.Status);
        });

        #endregion

        #region Autenticação

        app.MapPost("api/v1/authenticate", async (
            Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Request request,
            IRequestHandler<
                Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Request,
                Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Response> handler) =>
        {
            var result = await handler.Handle(request, new CancellationToken());
            if (!result.IsSuccess)
                return Results.Json(result, statusCode: result.Status);

            if (result.Data is null)
                return Results.Json(result, statusCode: (int)HttpStatusCode.InternalServerError);

            result.Data.Token = JwtExtension.Generate(result.Data);
            return Results.Ok(result);

        });
        #endregion
    }
}