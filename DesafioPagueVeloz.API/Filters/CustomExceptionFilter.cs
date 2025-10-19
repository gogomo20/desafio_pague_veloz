using System.ComponentModel.DataAnnotations;
using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Domain.DomainExceptions;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = FluentValidation.ValidationException;

namespace DesafioPagueVeloz.Filters;

public class CustomExceptionFilter(
    ILogger<CustomExceptionFilter> logger,
    IWriteableRepository<ErrorLogs> repository,
    IUnitOfWork unitOfWork)
    : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var errorResponse = new GenericResponseNoData();
        switch (context.Exception)
        {
            case ValidationException validationExeption:
                var errors = validationExeption.Errors.Select(e => e.ErrorMessage).ToList();
                errorResponse.PushRangeErrors(errors, "Dados inválidos!");
                var validationResult = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status400BadRequest };
                context.Result = validationResult;
                context.ExceptionHandled = true;
                break;
            case ArgumentException argumentException:
                errorResponse.PushError(argumentException.Message, "Dados inválidos!");
                var argumentResult = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status400BadRequest };
                context.Result = argumentResult;
                context.ExceptionHandled = true;
                break;
            case DomainException:
                errorResponse.PushError(context.Exception.Message, "Dados inválidos!");
                var domainResult = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status400BadRequest };
                context.Result = domainResult;
                context.ExceptionHandled = true;
                break;
            case AppException appException:
                context.Result = OnAppException(appException, errorResponse);
                context.ExceptionHandled = true;
                break;
            default:
                await LogError(context);
                errorResponse.PushError("Erro ao realizar a ação.", "Erro inesperado");
                var defaultResult = new ObjectResult(errorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
                context.Result = defaultResult;
                context.ExceptionHandled = true;
                break;
        }
    }

    private ObjectResult OnAppException(AppException exception, GenericResponseNoData response)
    {
        switch (exception.Type)
        {
            case ApplicationTypeErrors.BadRequest:
                response.PushError(exception.Message, "Erro");
                return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            case ApplicationTypeErrors.NotFound:
                response.PushError(exception.Message, "Erro");
                return new ObjectResult(response) { StatusCode = StatusCodes.Status404NotFound };
            case ApplicationTypeErrors.InternalServerError:
                response.PushError(exception.Message, "Erro");
                return new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError };
            case ApplicationTypeErrors.Invalid:
                response.PushError(exception.Message, "Erro");
                return new ObjectResult(response) { StatusCode = StatusCodes.Status400BadRequest };
            default:
                response.PushError(exception.Message, "Erro");
                return new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    private async Task LogError(ExceptionContext context)
    {
        logger.LogError("Um erro inesperado ocorreu");
        using var reader = new StreamReader(context.HttpContext.Request.Body, leaveOpen: true);
        var payload = reader.ReadToEndAsync().Result;
        var command = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        var log = new ErrorLogs(
            command,
            payload,
            context.Exception.ToString()
        );
        await repository.AddAsync(log);
        await unitOfWork.SaveAsync();
    }
}