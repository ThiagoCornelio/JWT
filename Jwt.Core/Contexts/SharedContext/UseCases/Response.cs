using Flunt.Notifications;
using System.Net;

namespace Jwt.Core.Contexts.SharedContext.UseCases;
public abstract class Response
    //Uma classe de resposta padrão para as apis
{
    public string Message { get; set; } = string.Empty;
    public int Status { get; set; } = (int)HttpStatusCode.BadRequest;
    public bool IsSuccess => Status is >= (int)HttpStatusCode.OK and <= (int)HttpStatusCode.IMUsed;
    public IEnumerable<Notification>? Notifications { get; set; }
}