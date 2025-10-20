using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using Moq;

namespace DesafilPagueVelos.Test.Applications;

public class ReverseCommandTest
{
    private readonly Mock<IReadableRepository<Account>> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReverseCommandHandler _handler;

    public ReverseCommandTest()
    {
        _accountRepositoryMock = new Mock<IReadableRepository<Account>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new ReverseCommandHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Deve_Executar_Com_Sucesso_Quando_Operacao_Existir_E_For_Completed()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente1", 100, currency, 50, 0);
        var operacao = new Operation(OperationType.credit, currency, "Depósito", 50) { Id = Guid.NewGuid() };
        operacao.Complete();
        conta.AddOperation(operacao);

        var command = new ReverseCommand(operacao.Id)
        {
            AccountId = conta.Id
        };

        _accountRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(resultado.Success);
        Assert.Equal("Operação solicitada com sucesso!", resultado.Message);
        Assert.NotNull(resultado.Data);
        Assert.IsType<OperationResponse>(resultado.Data);
        Assert.Contains(conta.Operations, o => o.OperationType == OperationType.reverse);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Conta_Nao_Existir()
    {
        // Arrange
        var command = new ReverseCommand(Guid.NewGuid())
        {
            AccountId = Guid.NewGuid()
        };

        _accountRepositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A conta informada não existe", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Operacao_Nao_Existir()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente2", 100, currency, 50, 0);

        var command = new ReverseCommand(Guid.NewGuid())
        {
            AccountId = conta.Id
        };

        _accountRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A operação informada não existe", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Operacao_Nao_Foi_Completed()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente3", 100, currency, 50, 0);
        var operacao = new Operation(OperationType.credit, currency, "Depósito", 50); // Status pending
        conta.AddOperation(operacao);

        var command = new ReverseCommand(operacao.Id)
        {
            AccountId = conta.Id
        };

        _accountRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("A operação informada não pode ser desfeita pois ela ainda não foi processada", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Ja_Existe_Reversao_Para_Essa_Operacao()
    {
        // Arrange
        var currency = new Currency("Real", "BRL", 1);
        var conta = new Account("cliente4", 100, currency, 50, 0);
        var operacaoOriginal = new Operation(OperationType.credit, currency, "Depósito", 50);
        operacaoOriginal.GenerateId();
        operacaoOriginal.Complete();
        conta.AddOperation(operacaoOriginal);

        var reversaoExistente = operacaoOriginal.UndoOperation();
        reversaoExistente.GenerateId();
        conta.AddOperation(reversaoExistente);

        var command = new ReverseCommand(operacaoOriginal.Id)
        {
            AccountId = conta.Id
        };

        _accountRepositoryMock.Setup(x => x.GetByIdAsync(conta.Id))
            .ReturnsAsync(conta);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Já existe uma solicitação registrada para desfazer a operação informada", ex.Message);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}