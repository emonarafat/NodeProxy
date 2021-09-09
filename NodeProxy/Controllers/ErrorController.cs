using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NodeProxy.Models;

namespace NodeProxy.Controllers
{

    [ApiController]
    [ApiExplorerSettings(IgnoreApi =true)]
    public class ErrorController : ControllerBase
    {
        [Route("error")]
        public IActionResult Error() => Problem();
        [Route("error-local-development")]
        public IActionResult ErrorLocal([FromServices] IWebHostEnvironment env)
        {
            switch (env.EnvironmentName)
            {
                case "Development":
                    {
                        var errorFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
                        return Problem(errorFeature.Error.StackTrace, title: errorFeature.Error.Message);
                    }

                default:
                    return new ObjectResult(new ApiResponse(500, "This should not invoked in no development environment"));
            }
        }
    }
}
