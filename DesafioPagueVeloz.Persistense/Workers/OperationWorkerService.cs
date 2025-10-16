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
using DesafioPagueVeloz.Persistense.Strategies.Operations.Transfer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DesafioPagueVeloz.Persistense.Workers;

public class OperationWorkerService : BackgroundService
{
    private readonly ILogger<OperationWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<OperationType, IOperationStrategy> _strategies;
    private readonly ApplicationContext _context;

    public OperationWorkerService(ILogger<OperationWorkerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var readableRepository = serviceProvider.GetRequiredService<IReadableRepository<Account>>();
        _context = serviceProvider.GetRequiredService<ApplicationContext>();
        _strategies = new Dictionary<OperationType, IOperationStrategy>()
        {
            { OperationType.capture, new CaptureStrategy(readableRepository, unitOfWork) },
            { OperationType.credit, new CreditStrategy(readableRepository, unitOfWork) },
            { OperationType.debit, new DebitStrategy(readableRepository, unitOfWork) },
            { OperationType.transfer, new TransferStrategy(readableRepository, unitOfWork) },
            { OperationType.reserve, new ReserveStrategy(readableRepository, unitOfWork) }
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando worker de operações");
        while (!stoppingToken.IsCancellationRequested)
        {
            var operations = _context.Set<Operation>()
                .Where(x => x.Status == OperationStatus.pending)
                .OrderBy(x => x.CreatedAt)
                .Take(10);
            foreach (var operation in operations)
            {
                try
                {
                    await _strategies[operation.OperationType].ExecuteAsync(operation);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Erro ao realizar operação {operation.OperationType}");
                    var log = new ErrorLogs(
                        "Erro ao realizar operação",
                        JsonSerializer.Serialize(operation),
                        e.InnerException.ToString()
                    );
                    _context.Set<ErrorLogs>().Add(log);
                    await _context.SaveChangesAsync(stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(0.5), stoppingToken);
        }
    }
}