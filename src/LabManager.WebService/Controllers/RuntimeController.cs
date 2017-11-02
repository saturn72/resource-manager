using System.Threading.Tasks;
using LabManager.WebService.Infrastructure;
using LabManager.WebService.Models.Runtime;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Runtime;
using QAutomation.Core.Services;
using QAutomation.Extensions;
using System.Linq;

namespace LabManager.WebService.Controllers
{
    public class RuntimeController : ControllerBase
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

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromBody] ResourceAssignmentRequestApiModel assignRequest)
        {
            var srvRes = await _runtimeManager.RequestResourceAssignmentAsync(assignRequest.ToModel());

            return CheckAssignResponse(srvRes)
                ? Ok(new
                {
                    sessionId = srvRes.Model.SessionId,
                    resources = srvRes.Model.Resources.ToApiModel().ToArray()
                })
                : new NotFoundObjectResult(assignRequest) as IActionResult;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string sessionId)
        {
            var srvRes = await _runtimeManager.AssignResourcesAsync(sessionId);
            return CheckAssignResponse(srvRes)
                   && srvRes.Model.Status == ResourceAssignmentStatus.Assigned
                ? Ok(sessionId)
                : BadRequest(new
                {
                    sessionId = sessionId,
                    message = srvRes?.ErrorMessage ?? "Unknown Error"
                }) as IActionResult;
            ;
        }

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
    }
}