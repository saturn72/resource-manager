using System;
using System.Threading.Tasks;
using LabManager.WebService.Infrastructure;
using LabManager.WebService.Models.Runtime;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Runtime;
using Saturn72.Extensions;
using Saturn72.Core.Services;
using System.Linq;
using Saturn72.Core.Services.Web.Controllers;

namespace LabManager.WebService.Controllers
{
    public class RuntimeController : Saturn72ControllerBase
    {
        #region Fields

        private readonly IRuntimeManager _runtimeManager;

        #endregion

        #region ctor

        public RuntimeController(IRuntimeManager runtimeManager)
        {
            _runtimeManager = runtimeManager;
        }

        #endregion

        #region  Read

        [HttpGet("IsAssigned/{resourceId}")]
        public async Task<IActionResult> IsAssigned(long resourceId)
        {
            if (resourceId <= 0)
                return BadRequest(new
                {
                    message = "ResourceId is required"
                });
            var isAssigned = await _runtimeManager.IsAssigned(resourceId);
            return Ok(isAssigned);
        }

        #endregion
        #region POST
        [HttpPost]
        public async Task<IActionResult> RequestAssignment([FromBody] ResourceAssignmentRequestApiModel assignRequest)
        {
            var srvRes = await _runtimeManager.RequestResourceAssignmentAsync(assignRequest?.ToModel());

            return CheckAssignResponse(srvRes)
                ? Ok(new
                {
                    sessionId = srvRes.Model.SessionId,
                    resources = srvRes.Model.Resources.ToApiModel().ToArray()
                })
                : new NotFoundObjectResult(assignRequest) as IActionResult;
        }

        #endregion

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> AssignSessionAsync(string sessionId)
        {
            if (!sessionId.HasValue())
                return BadRequest(new
                {
                    message = "SessionId is required"
                });

            var srvRes = await _runtimeManager.AssignResourcesAsync(sessionId);
            return CheckAssignResponse(srvRes)
                   && srvRes.Model.Status == ResourceAssignmentStatus.Assigned
                ? Ok(
                    new
                    {
                        @sessionId = sessionId,
                        resources = srvRes.Model.Resources.Select(r=>r.Id).ToArray()
                    })
                : BadRequest(new
                {
                    @sessionId = sessionId,
                    message = srvRes?.ErrorMessage ?? "Unknown Error"
                }) as IActionResult;
            ;
        }

        #region Utilities

        private bool CheckAssignResponse(ServiceResponse<ResourceAssignmentResponse> serviceResponse)
        {
            var resModel = serviceResponse?.Model;

            return serviceResponse.NotNull()
                   && !serviceResponse.HasErrors()
                   && serviceResponse.Result == ServiceResponseResult.Success
                   && resModel.NotNull()
                   && resModel.SessionId.HasValue()
                   && !resModel.Resources.IsEmptyOrNull();
        }

        #endregion
    }
}