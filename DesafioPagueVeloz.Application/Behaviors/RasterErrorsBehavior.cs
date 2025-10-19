using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using DesafioPagueVeloz.Domain.DomainExceptions;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DesafioPagueVeloz.Application.Exceptions;

public class RasterErrorsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IWriteableRepository<ErrorLogs> _repository;
    private readonly ILogger<RasterErrorsBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;
    public RasterErrorsBehavior(
        IWriteableRepository<ErrorLogs> repository,
        ILogger<RasterErrorsBehavior<TRequest, TResponse>> logger,
        IUnitOfWork unitOfWork
        )
    {
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var command = typeof(TRequest).Name;
        try
        {
            return await next(cancellationToken);
        }
        catch(Exception ex)
        {
            _unitOfWork.ClearChanges();
            switch (ex)
            {
                case ValidationException validationException:
                    throw;
                case AppException:
                    throw;
                case DomainException:
                    throw;
                default:
                    var payload = JsonSerializer.Serialize(
                        request,
                        new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                    );
                    var log = new ErrorLogs(
                        command,
                        payload,
                        ex.Message
                    );
                    await _repository.AddAsync(log);
                    await _unitOfWork.SaveAsync(cancellationToken);
                    throw AppException.InternalServerError("Erro ao realizar a ação");
            }
        }
    }
}