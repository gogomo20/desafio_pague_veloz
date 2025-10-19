using System.Text.Json;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Context;
using DesafioPagueVeloz.Persistense.Repositories;
using DesafioPagueVeloz.Persistense.Strategies.Operations;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Capture;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Credit;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Debit;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Reserve;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Reverse;
using DesafioPagueVeloz.Persistense.Strategies.Operations.Transfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DesafioPagueVeloz.Persistense.Workers;

public class OperationWorkerService : BackgroundService
{
    private readonly ILogger<OperationWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OperationWorkerService(ILogger<OperationWorkerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando worker de operações");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Realizando operações");
                var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var readableRepository = scope.ServiceProvider.GetRequiredService<IReadableRepository<Operation>>();
                var strategies = new Dictionary<OperationType, IOperationStrategy>()
                {
                    { OperationType.capture, new CaptureStrategy(readableRepository) },
                    { OperationType.credit, new CreditStrategy(readableRepository) },
                    { OperationType.debit, new DebitStrategy(readableRepository) },
                    { OperationType.transfer, new TransferStrategy(readableRepository) },
                    { OperationType.reserve, new ReserveStrategy(readableRepository) },
                    { OperationType.reverse, new ReverseStrategy(readableRepository) }
                };
                var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                var operations = await context.Set<Operation>()
                    .Where(x => x.Status == OperationStatus.pending)
                    .OrderBy(x => x.CreatedAt)
                    .Take(10)
                    .ToListAsync(stoppingToken);
                foreach (var operation in operations)
                {
                    try
                    {
                        await strategies[operation.OperationType].ExecuteAsync(operation);
                        await unitOfWork.SaveAsync(stoppingToken);
                    }
                    catch (Exception e)
                    {
                        unitOfWork.ClearChanges();
                        _logger.LogInformation($"Erro ao realizar operação {operation.OperationType}");
                        var log = new ErrorLogs(
                            "Erro ao realizar operação",
                            JsonSerializer.Serialize(
                                operation,
                                new JsonSerializerOptions
                                {
                                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition
                                        .WhenWritingNull,
                                    WriteIndented = false
                                }
                            ),
                            e.ToString()
                        );
                        operation.PushCounter();
                        if (operation.RetryCounter > 3)
                            operation.SetError(log);
                        context.Set<ErrorLogs>().Add(log);
                        await unitOfWork.SaveAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar operação");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}