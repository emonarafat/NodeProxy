using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using NodeProxy.Features.SoapFeature;
using NodeProxy.Models;

using System.Threading.Tasks;

namespace NodeProxy.Controllers
{
    [Route("api/proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProxyController(IMediator mediator) => _mediator = mediator;
        
        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]SoapCommand command)
        {
            Request.Headers.TryGetValue("Authorization", out var authHeader);
            Request.Headers.TryGetValue("soapaction", out var soapaction);
            command.Headers.Add("authorization", authHeader);
            command.Headers.Add("soapaction", soapaction);
            var result = await _mediator.Send(command).ConfigureAwait(false);
            return Ok(result);


        }
    }
}
