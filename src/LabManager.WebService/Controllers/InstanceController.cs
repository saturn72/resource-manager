using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Instance;
using Saturn72.Core.Services;

namespace LabManager.WebService.Controllers
{
    public class InstanceController : ControllerBase
    {
        #region Fields

        private readonly IInstanceService _instanceService;

        #endregion

        #region ctor

        public InstanceController(IInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        #endregion

        #region Start

        [HttpPost("start")]
        public async Task<IActionResult> StartAsync(long resourceId)
        {
            return await RunResourceCommandAsync(resourceId, rId => _instanceService.StartAsync(rId));
        }

        #endregion

        [HttpPost("stop")]
        public async Task<IActionResult> StopAsync(long resourceId)
        {
            return await RunResourceCommandAsync(resourceId, rId => _instanceService.StopAsync(rId));
        }

        #region Utilities

        private async Task<IActionResult> RunResourceCommandAsync(long resourceId, Func<long, Task<ServiceResponse<ResourceExecutionResponseData>>> func)
        {
            if (resourceId <= 0)
                return BadRequest(new
                {
                    @resourceId = resourceId,
                    message = "ResourceId must be greater than 0."
                });

            var serviceResponse = await func(resourceId);
            return serviceResponse.Model.Executed
                ? Ok() as IActionResult
                : BadRequest(new
                {
                    @resourceId = resourceId,
                    message = string.Format("Failed To start resource: {0}\n{1}", resourceId, serviceResponse.ErrorMessage)
                });
        }
        #endregion
    }
}
