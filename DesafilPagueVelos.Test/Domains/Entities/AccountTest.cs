using DesafioPagueVeloz.Domain.DomainExceptions;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;

namespace DesafilPagueVelos.Test.Domains.Entities;

public class AccountTest
{
    private readonly Currency _real = new Currency("Real", "BRL", 1);

    [Fact]
    public void Deve_Criar_Com_Dados_Validos()
    {
        //Act
        var ex = Record.Exception(() =>
            new Account(
                "testevalidos",
                50,
                _real,
                100,
                50
            ));
        //Assert
        Assert.Null(ex);
    }

    [Fact]
    public void Nao_deve_Criar_Com_Saldo_Negativo()
    {
        //Act
        var ex = Assert.Throws<DomainException>(() =>
            new Account(
                "testesaldo",
                -50,
                _real,
                100,
                50
            ));
        //Assert
        Assert.NotNull(ex);
        Assert.Equal("A conta não pode ser aberta com valores negativos", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Criar_Com_Limite_Negativo()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Account(
                "cliente123",
                10,
                _real,
                -100,
                0
            ));

        Assert.Equal("A conta não pode ser aberta com valores negativos", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Criar_Sem_ClientId()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Account(
                "",
                10,
                _real,
                100,
                0
            ));

        Assert.Equal("Id do cliente deve ser informado", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Criar_Sem_Currency()
    {
        var ex = Assert.Throws<DomainException>(() =>
            new Account(
                "cliente123",
                10,
                null!,
                100,
                0
            ));

        Assert.Equal("Moeda deve ser informada", ex.Message);
    }

    [Fact]
    public void Deve_Reservar_Valor_Com_Sucesso()
    {
        var account = new Account("cliente123", 100, _real, 100, 0);
        account.Credit(account.Operations.First().Value);
        account.Operations.First().Complete();
        
        account.Reserve(50);

        Assert.Equal(50, account.Balance);
        Assert.Equal(50, account.ReservedAmount);
    }

    [Fact]
    public void Nao_Deve_Reservar_Valor_Negativo()
    {
        var account = new Account("cliente123", 100, _real, 100, 0);

        var ex = Assert.Throws<ArgumentException>(() => account.Reserve(-10));

        Assert.Equal("Valor reservado deve ser maior ou igual a zero", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Reservar_Valor_Maior_Que_Saldo()
    {
        var account = new Account("cliente123", 50, _real, 100, 0);

        var ex = Assert.Throws<ArgumentException>(() => account.Reserve(100));

        Assert.Equal("Saldo insuficiente", ex.Message);
    }

    [Fact]
    public void Deve_Transferir_Com_Sucesso()
    {
        var origem = new Account("clienteOrigem", 100, _real, 100, 0);
        origem.Credit(origem.Operations.First().Value);
        origem.Operations.First().Complete();
        var destino = new Account("clienteDestino", 0, _real, 100, 0);

        origem.Transfer(50, destino);

        Assert.Equal(50, origem.Balance);
        Assert.Single(destino.Operations);
        Assert.Equal("Transferencia enviada", destino.Operations.First().Description);
    }

    [Fact]
    public void Nao_Deve_Transferir_Valor_Negativo()
    {
        var origem = new Account("clienteOrigem", 100, _real, 100, 0);
        var destino = new Account("clienteDestino", 0, _real, 100, 0);

        var ex = Assert.Throws<ArgumentException>(() => origem.Transfer(-10, destino));

        Assert.Equal("Valor transferido deve ser maior ou igual a zero", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Transferir_Valor_Maior_Que_Saldo()
    {
        var origem = new Account("clienteOrigem", 50, _real, 100, 0);
        var destino = new Account("clienteDestino", 0, _real, 100, 0);

        var ex = Assert.Throws<ArgumentException>(() => origem.Transfer(100, destino));

        Assert.Equal("Saldo insuficiente", ex.Message);
    }

    [Fact]
    public void Deve_Capturar_Valor_Com_Sucesso()
    {
        var account = new Account("cliente123", 100, _real, 100, 50);
        account.Credit(account.Operations.First().Value);
        account.Operations.First().Complete();
        
        account.Capture(30);

        Assert.Equal(130, account.Balance);
        Assert.Equal(20, account.ReservedAmount);
    }

    [Fact]
    public void Nao_Deve_Capturar_Valor_Negativo()
    {
        var account = new Account("cliente123", 100, _real, 100, 50);

        var ex = Assert.Throws<ArgumentException>(() => account.Capture(-10));

        Assert.Equal("O valor de captura deve ser maior ou igual a zero", ex.Message);
    }

    [Fact]
    public void Nao_Deve_Capturar_Sem_Saldo_Reservado()
    {
        var account = new Account("cliente123", 100, _real, 100, 20);

        var ex = Assert.Throws<ArgumentException>(() => account.Capture(50));

        Assert.Equal("Saldo reservado insuficiente", ex.Message);
    }

    [Fact]
    public void Deve_Debitar_Valor_Dentro_Do_Saldo()
    {
        var account = new Account("cliente123", 100, _real, 100, 0);
        account.Credit(account.Operations.First().Value);
        account.Operations.First().Complete();  
        
        account.Debit(60);

        Assert.Equal(40, account.Balance);
    }

    [Fact]
    public void Deve_Debitar_Usando_Credito()
    {
        var account = new Account("cliente123", 50, _real, 100, 0);

        account.Debit(80);

        Assert.Equal(0, account.Balance);
        Assert.True(account.AvaliableCredit < 0); // como o código reduz o crédito disponível
    }

    [Fact]
    public void Nao_Deve_Debitar_Valor_Maior_Que_Saldo_E_Credito()
    {
        var account = new Account("cliente123", 50, _real, 20, 0);

        var ex = Assert.Throws<ArgumentException>(() => account.Debit(100));

        Assert.Equal("Saldo insuficience!", ex.Message);
    }

    [Fact]
    public void Deve_Creditar_Valor_Com_Sucesso()
    {
        var account = new Account("cliente123", 100, _real, 100, 0);
        account.Credit(account.Operations.First().Value);
        account.Operations.First().Complete();
        
        account.Credit(50);

        Assert.Equal(150, account.Balance);
    }

    [Fact]
    public void Deve_Adicionar_Operacao()
    {
        var account = new Account("cliente123", 0, _real, 100, 0);
        var operation = new Operation(OperationType.credit, _real, "Teste", 10);

        account.AddOperation(operation);

        Assert.Single(account.Operations);
        Assert.Equal("Teste", account.Operations.First().Description);
    }
}