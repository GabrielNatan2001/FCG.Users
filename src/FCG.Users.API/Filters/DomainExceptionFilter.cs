using System.Net;
using FCG.Users.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FCG.Users.API.Filters;

/// <summary>
/// Converte DomainException em 400 via IActionResult, para o prometheus-net
/// registrar o status correto (evita o 200 fantasma do ExceptionMiddleware).
/// </summary>
public sealed class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainException domainException)
            return;

        context.Result = new ObjectResult(new { message = domainException.Message })
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
        context.ExceptionHandled = true;
    }
}
