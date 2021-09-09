using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodeProxy.Models;
using NodeProxy.Features.SoapFeature;

using System.Threading.Tasks;

namespace NodeProxy.Controllers
{
    [Route("api/proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProxyController(IMediator mediator) => _mediator = mediator;

        ///<summary>
        ///   Post to Soap endpoint
        /// <param name="command"><see cref="ProxyCommand"/></param>
        /// </summary>
        /// <returns>Response Result</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK,Type =typeof(ResponseModel.WithData))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResponseModel.NoData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel.NoData))]
        public async Task<IActionResult> Post(ProxyCommand command)
        {
            Request.Headers.TryGetValue("Authorization", out var authHeader);
            Request.Headers.TryGetValue("soapaction", out var soapaction);

            command.Authorization = authHeader;
            command.SoapAction = soapaction;

            var result = await _mediator.Send(command).ConfigureAwait(false);

            return result.IsSuccess.GetValueOrDefault()
                ? Ok(result) : BadRequest(result);

        }
    }
}
