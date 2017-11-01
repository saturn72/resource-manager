using System;
using System.Threading.Tasks;
using QAutomation.Extensions;
using LabManager.WebService.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Resources;
using LabManager.WebService.Infrastructure;
using LabManager.Services.Runtime;
using QAutomation.Core.Services;
using LabManager.Common.Domain.Runtime;

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
        public async Task<IActionResult> GetAsync([FromBody]ResourceApiModel filter)
        {
            var srvRes = await _runtimeManager.AssignResourceAsync(filter?.ToModel());

            return CheckIfReponseSucceseed(srvRes)
                ? Accepted(srvRes.Model.SessionId as string, srvRes.Model.Resource.ToApiModel())
                : new NotFoundObjectResult(filter) as IActionResult;
        }

        private bool CheckIfReponseSucceseed(ServiceResponse<RuntimeSession>  serviceResponse)
        {
            var resModel = serviceResponse?.Model;

            return serviceResponse.NotNull() 
                   && serviceResponse.Result == ServiceResponseResult.Success 
                   && resModel.NotNull() 
                   && resModel.SessionId.HasValue() 
                   && resModel.Resource.NotNull();
        }
    }
}