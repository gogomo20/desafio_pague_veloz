using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;
using DesafioPagueVeloz.Persistense.Repositories;
using Moq;

namespace DesafilPagueVelos.Test.Applications;

public class CreditCommandTest
{
    private readonly Mock<IReadableRepository<Account>> _accountRepositoryMock;
        private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreditCommandHandler _handler;

        public CreditCommandTest()
        {
            _accountRepositoryMock = new Mock<IReadableRepository<Account>>();
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreditCommandHandler(
                _accountRepositoryMock.Object,
                _currencyRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Deve_Executar_Com_Sucesso_Quando_Dados_Forem_Validos()
        {
            // Arrange
            var conta = new Account("cliente1", 100, new Currency("Real", "BRL", 1), 50, 0);
            var moeda = new Currency("Real", "BRL", 1);

            var command = new CreditCommand(
                Description: "Crédito de valor",
                Value: 200,
                Currency: "BRL"
            )
            {
                AccountId = Guid.NewGuid()
            };

            _accountRepositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
                .ReturnsAsync(conta);
            _currencyRepositoryMock.Setup(x => x.GetAsync("BRL"))
                .ReturnsAsync(moeda);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(resultado.Success);
            Assert.Equal("Solicitação de crédito solicitada com sucesso!", resultado.Message);
            Assert.NotNull(resultado.Data);
            Assert.IsType<OperationResponse>(resultado.Data);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Conta_Nao_Existir()
        {
            // Arrange
            var command = new CreditCommand(
                Description: "Crédito inválido",
                Value: 150,
                Currency: "BRL"
            )
            {
                AccountId = Guid.NewGuid()
            };

            _accountRepositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
                .ReturnsAsync((Account?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("A conta informada não exite", ex.Message);
            _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Moeda_Nao_Existir()
        {
            // Arrange
            var conta = new Account("cliente2", 100, new Currency("Real", "BRL", 1), 50, 0);
            var command = new CreditCommand(
                Description: "Crédito inválido",
                Value: 100,
                Currency: "USD"
            )
            {
                AccountId = Guid.NewGuid()
            };

            _accountRepositoryMock.Setup(x => x.GetByIdAsync(command.AccountId))
                .ReturnsAsync(conta);
            _currencyRepositoryMock.Setup(x => x.GetAsync("USD"))
                .ReturnsAsync((Currency?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Equal("Não possuimos essa moeda em nossa base", ex.Message);
            _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
}