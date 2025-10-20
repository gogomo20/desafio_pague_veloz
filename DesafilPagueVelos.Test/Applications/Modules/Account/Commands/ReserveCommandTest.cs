using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using Moq;

namespace DesafilPagueVelos.Test.Applications;

public class ReserveCommandTest
{
    private readonly Mock<IReadableRepository<Account>> _repositoryMock;
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReserveCommandHandler _handler;

    public ReserveCommandTest()
    {
        _repositoryMock = new Mock<IReadableRepository<Account>>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new ReserveCommandHandler(
            _repositoryMock.Object,
            _currencyRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Deve_Executar_Com_Sucesso_Quando_Dados_Forem_Validos()
    {
        // Arrange
        var moedaConta = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente1", 100, moedaConta, 100, 0);
        conta.Credit(conta.Operations.First().Value);
        conta.Operations.First().Complete();
        var moedaOperacao = new Currency("Real", "BRL", 1);

        var command = new ReserveCommand(
            Value: 50,
            Description: "Reserva de teste",
            Currency: "BRL"
        )
        {
            AccountId = Guid.NewGuid()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync(conta);
        _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
            .ReturnsAsync(moedaOperacao);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(resultado.Success);
        Assert.Equal("Operação solicitada com sucesso!", resultado.Message);
        Assert.NotNull(resultado.Data);
        Assert.IsType<OperationResponse>(resultado.Data);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Conta_Nao_Existir()
    {
        // Arrange
        var command = new ReserveCommand(
            Value: 100,
            Description: "Reserva inválida",
            Currency: "BRL"
        )
        {
            AccountId = Guid.NewGuid()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A conta informada não existe", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Moeda_Nao_Existir()
    {
        // Arrange
        var conta = new Account("cliente2", 200, new Currency("Real", "BRL", 1), 50, 0);
        var command = new ReserveCommand(
            Value: 100,
            Description: "Reserva em moeda inexistente",
            Currency: "USD"
        )
        {
            AccountId = Guid.NewGuid()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync(conta);
        _currencyRepositoryMock.Setup(x => x.GetAsync("USD"))
            .ReturnsAsync((Currency?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Não possuimos essa moeda em nossa base", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Saldo_For_Insuficiente()
    {
        // Arrange
        var moedaConta = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente3", 30, moedaConta, 0, 0);
        var moedaOperacao = new Currency("Real", "BRL", 1);

        var command = new ReserveCommand(
            Value: 100,
            Description: "Reserva acima do saldo",
            Currency: "BRL"
        )
        {
            AccountId = Guid.NewGuid()
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync(conta);
        _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
            .ReturnsAsync(moedaOperacao);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Saldo insuficiente", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}