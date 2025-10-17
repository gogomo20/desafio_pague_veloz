using DesafioPagueVeloz.Application;
using DesafioPagueVeloz.Application.Modules.Account.Queries.Get;
using DesafioPagueVeloz.Application.Modules.Account.Responses;
using DesafioPagueVeloz.Application.Response;
using DesafioPagueVeloz.Persistense.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DesafioPagueVeloz.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<AccountCreatedResponse>))]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await _mediator.Send(command, cancellationToken);
        return Created(nameof(GetAccount), response);
    }
    [HttpPost("{id:guid}/debit")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterDebit(
            [FromRoute] Guid id,
            [FromBody] DebitCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }
    [HttpPost("{id:guid}/reserve")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterReserve(
            [FromRoute] Guid id,
            [FromBody] ReserveCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }
    [HttpPost("{id:guid}/reverse")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterReverse(
            [FromRoute] Guid id,
            [FromBody] ReverseCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }
    [HttpPost("{id:guid}/capture")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterCapture(
            [FromRoute] Guid id,
            [FromBody] CaptureCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }

    [HttpPost("{id:guid}/credit")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterCredit(
            [FromRoute] Guid id,
        [FromBody] CreditCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }
    [HttpPost("{id:guid}/transfer")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenericResponse<OperationResponse>))]
    public async Task<IActionResult> RegisterTransfer(
            [FromRoute] Guid id,
            [FromBody] TransferCommand command
        )
    {
        command.AccountId = id;
        var response = await _mediator.Send(command, CancellationToken.None);
        return Created("", response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<AccountView>))]
    public async Task<IActionResult> GetAccount(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAccount(id);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    [HttpGet("client/{clientId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<ICollection<AccountView>>))]
    public async Task<IActionResult> GetAccounts(string clientId, CancellationToken cancellationToken)
    {
        var query = new GetAccounts(clientId);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<ICollection<HistoryView>>))]
    public async Task<IActionResult> GetHistory(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetHistory(id);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
}