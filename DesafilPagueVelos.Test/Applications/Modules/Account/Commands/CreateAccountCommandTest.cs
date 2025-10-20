using DesafilPagueVelos.Test.Helpers;
using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Context;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DesafilPagueVelos.Test.Applications;

public class CreateAccountCommandTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWriteableRepository<Account>> _repositoryMock;
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IWriteableRepository<Account>>();
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();

        _handler = new CreateAccountCommandHandler(
            _unitOfWorkMock.Object,
            _repositoryMock.Object,
            _currencyRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Deve_Criar_Conta_Com_Dados_Validos()
    {
        // Arrange
        var command = new CreateAccountCommand(
            ClientId: "cliente123",
            Balance: 100,
            Currency: "BRL",
            CreditLimit: 200,
            ReservedAmount: 0
        );

        var moeda = new Currency("Real", "BRL", 1);
        _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
            .ReturnsAsync(moeda);

        // Act
        var resultado = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(resultado);
        Assert.True(resultado.Success);
        Assert.Equal("Conta criada com sucesso", resultado.Message);
        Assert.IsType<AccountCreatedResponse>(resultado.Data);
        Assert.Equal(command.Currency, resultado.Data.Currency);
        Assert.Equal(command.Balance, resultado.Data.Balance);
    }

    [Fact]
    public async Task Deve_Lancar_Excecao_Quando_Moeda_Nao_Existir()
    {
        // Arrange
        var command = new CreateAccountCommand(
            ClientId: "clienteInvalido",
            Balance: 100,
            Currency: "USD",
            CreditLimit: 50,
            ReservedAmount: 0
        );

        _currencyRepositoryMock.Setup(x => x.GetAsync("USD"))
            .ReturnsAsync((Currency?)null);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<AppException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("Moeda informada não existe", excecao.Message);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}