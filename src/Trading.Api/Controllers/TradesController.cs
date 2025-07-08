using MediatR;
using Microsoft.AspNetCore.Mvc;
using Trading.Api.Models;
using Trading.Api.Mappers;
using Trading.Application.Commands;
using Trading.Application.Queries;
using Mapster;

namespace Trading.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ApiResult<TradeResponse>>> ExecuteTrade([FromBody] ExecuteTradeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResult<TradeResponse>.Fail("Invalid request payload."));
            try
            {
                TradeApiValidator.Validate(request);
                var command = request.Adapt<ExecuteTradeCommand>();
                var result = await mediator.Send(command);
                var response = result.Adapt<TradeResponse>();
                return Ok(ApiResult<TradeResponse>.Ok(response));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResult<TradeResponse>.Fail(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResult<TradeResponse>.Fail("An unexpected error occurred."));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResult<List<TradeResponse>>>> GetTrades()
        {
            try
            {
                var result = await mediator.Send(new GetTradesQuery());
                var response = result.ConvertAll(trade => trade.Adapt<TradeResponse>());
                return Ok(ApiResult<List<TradeResponse>>.Ok(response));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResult<List<TradeResponse>>.Fail("An unexpected error occurred."));
            }
        }
    }
} 