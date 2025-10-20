using DesafioPagueVeloz.Domain.DomainExceptions;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Domain.Entities.Accounts;

namespace DesafilPagueVelos.Test.Domains.Entities;

public class OperationsTest
{
    public Currency _currency;
    public Account _account;

    public OperationsTest()
    {
        _currency = new Currency("Real", "BRL", 1);
        _account = new Account("cliente123", 100, _currency, 100, 0);
        _account.Credit(_account.Operations.First().Value);
        _account.Operations.First().Complete();
    }
    [Fact]
    public void Construtor_Deve_Criar_Operacao_Valida_Para_Tipos_Aceitos()
    {
        // Arrange
        var operacao = new Operation(OperationType.debit, _currency, "Compra", 100);

        // Assert
        Assert.Equal(OperationType.debit, operacao.OperationType);
        Assert.Equal(OperationStatus.pending, operacao.Status);
        Assert.Equal("Compra", operacao.Description);
        Assert.Equal(100, operacao.Value);
    }

    [Fact]
    public void Construtor_Deve_Lancar_Excecao_Quando_Tipo_For_Invalido()
    {
        // Assert
        Assert.Throws<DomainException>(() =>
            new Operation(OperationType.reverse, _currency, "Operação inválida", 50));
    }

    [Fact]
    public void Construtor_De_Transferencia_Deve_Criar_Transferencia_Valida()
    {
        var operacao = new Operation(OperationType.transfer, _currency, "Transferência", 200, _account);

        // Assert
        Assert.Equal(OperationType.transfer, operacao.OperationType);
        Assert.Equal(_account, operacao.TransferAccount);
        Assert.False(operacao.IsReversable);
    }

    [Fact]
    public void Construtor_De_Transferencia_Deve_Lancar_Excecao_Se_Tipo_Nao_For_Transfer()
    {

        Assert.Throws<DomainException>(() =>
            new Operation(OperationType.debit, _currency, "Erro", 100, _account));
    }

    [Fact]
    public void SetPreviousBalance_Deve_Funcionar_Quando_Status_For_Pendente()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);
        operacao.SetPreviousBalance(500);

        Assert.Equal(500, operacao.PreviousBalance);
    }

    [Fact]
    public void SetPreviousBalance_Deve_Lancar_Excecao_Quando_Status_For_Concluido()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);
        operacao.Complete();

        Assert.Throws<DomainException>(() => operacao.SetPreviousBalance(500));
    }

    [Fact]
    public void Complete_Deve_Definir_Status_Como_Concluido()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        operacao.Complete();

        Assert.Equal(OperationStatus.completed, operacao.Status);
        Assert.NotNull(operacao.ExecutionDate);
    }

    [Fact]
    public void Cancel_Deve_Definir_Status_Como_Cancelado_Quando_Razao_For_Informada()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        operacao.Cancel("Cliente desistiu");

        Assert.Equal(OperationStatus.canceled, operacao.Status);
        Assert.Equal("Cliente desistiu", operacao.Description);
    }

    [Fact]
    public void Cancel_Deve_Lancar_Excecao_Quando_Razao_For_Vazia()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        Assert.Throws<DomainException>(() => operacao.Cancel(""));
    }

    [Fact]
    public void UndoOperation_Deve_Retornar_Operacao_De_Reversao_Quando_Concluida()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);
        operacao.Complete();

        var reversao = operacao.UndoOperation();

        Assert.Equal(OperationType.reverse, reversao.OperationType);
        Assert.Equal(operacao, reversao.Undo);
    }

    [Fact]
    public void UndoOperation_Deve_Lancar_Excecao_Quando_Operacao_Nao_Estiver_Concluida()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        Assert.Throws<DomainException>(() => operacao.UndoOperation());
    }

    [Fact]
    public void SetIrreversible_Deve_Definir_Operacao_Como_Irreversivel()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        operacao.SetIrreversible();

        Assert.False(operacao.IsReversable);
    }

    [Fact]
    public void PushCounter_Deve_Incrementar_RetryCounter()
    {
        var operacao = new Operation(OperationType.debit, _currency, "Teste", 100);

        operacao.PushCounter();
        operacao.PushCounter();

        Assert.Equal(2, operacao.RetryCounter);
    }
}