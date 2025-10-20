using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using Moq;

namespace DesafilPagueVelos.Test.Applications;

public class TransferCommandTest
{
    private readonly Mock<IReadableRepository<Account>> _repositoryMock;
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TransferCommandHandler _handler;

    public TransferCommandTest()
    {
        _repositoryMock = new Mock<IReadableRepository<Account>>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new TransferCommandHandler(
            _repositoryMock.Object,
            _currencyRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Deve_Executar_Com_Sucesso_Quando_Dados_Forem_Validos()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var contaOrigem = new Account("cliente1", 200, currency, 100, 0) { Id = Guid.NewGuid()};
        contaOrigem.Credit(contaOrigem.Operations.First().Value);
        contaOrigem.Operations.First().Complete();
        var contaDestino = new Account("cliente2", 50, currency, 100, 0) { Id = Guid.NewGuid() };
        contaDestino.Credit(contaDestino.Operations.First().Value);
        contaDestino.Operations.First().Complete();
        
        var command = new TransferCommand(
            To: contaDestino.Id,
            Value: 100,
            Currency: "BRL",
            Description: "Transferência teste"
        )
        {
            AccountId = contaOrigem.Id
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(contaOrigem.Id))
            .ReturnsAsync(contaOrigem);
        _repositoryMock.Setup(x => x.GetByIdAsync(contaDestino.Id))
            .ReturnsAsync(contaDestino);
        _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
            .ReturnsAsync(currency);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(resultado.Success);
        Assert.Equal("Solicitação de transferência solicitada com sucesso!", resultado.Message);
        Assert.NotNull(resultado.Data);
        Assert.IsType<OperationResponse>(resultado.Data);
        Assert.Contains(contaOrigem.Operations, o => o.OperationType == OperationType.transfer);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Conta_Origem_Nao_Existir()
    {
        // Arrange
        var command = new TransferCommand(
            To: Guid.NewGuid(),
            Value: 100,
            Currency: "BRL",
            Description: "Transferência inválida"
        )
        {
            AccountId = Guid.NewGuid()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A conta informada não exite", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Conta_Destino_Nao_Existir()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var contaOrigem = new Account("cliente1", 200, currency, 100, 0);

        var command = new TransferCommand(
            To: Guid.NewGuid(),
            Value: 100,
            Currency: "BRL",
            Description: "Transferência inválida"
        )
        {
            AccountId = contaOrigem.Id
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(contaOrigem.Id))
            .ReturnsAsync(contaOrigem);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.To))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A conta no qual deseja realizar a transferência não existe", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Moeda_Nao_Existir()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var contaOrigem = new Account("cliente1", 200, currency, 100, 0);
        var contaDestino = new Account("cliente2", 50, currency, 100, 0);

        var command = new TransferCommand(
            To: contaDestino.Id,
            Value: 100,
            Currency: "USD",
            Description: "Transferência moeda inválida"
        )
        {
            AccountId = contaOrigem.Id
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(contaOrigem.Id))
            .ReturnsAsync(contaOrigem);
        _repositoryMock.Setup(x => x.GetByIdAsync(contaDestino.Id))
            .ReturnsAsync(contaDestino);
        _currencyRepositoryMock.Setup(x => x.GetAsync("USD"))
            .ReturnsAsync((Currency?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Não possuimos essa moeda em nossa base", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Saldo_Insuficiente()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var contaOrigem = new Account("cliente1", 50, currency, 0, 0);
        var contaDestino = new Account("cliente2", 50, currency, 100, 0);

        var command = new TransferCommand(
            To: contaDestino.Id,
            Value: 100,
            Currency: "BRL",
            Description: "Transferência saldo insuficiente"
        )
        {
            AccountId = contaOrigem.Id
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(contaOrigem.Id))
            .ReturnsAsync(contaOrigem);
        _repositoryMock.Setup(x => x.GetByIdAsync(contaDestino.Id))
            .ReturnsAsync(contaDestino);
        _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
            .ReturnsAsync(currency);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Saldo insuficiente", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}