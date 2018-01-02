using System.Threading.Tasks;
using LabManager.WebService.Infrastructure;
using LabManager.WebService.Models.Command;
using Microsoft.AspNetCore.Mvc;
using Saturn72.Core.Services.Web.Controllers;
using Saturn72.Extensions;
using LabManager.Services.Command;
using Saturn72.Core.Services;

namespace LabManager.WebService.Controllers
{
    public class CommandController:Saturn72ControllerBase
    {
        #region Fields

        private readonly ICommandResourceService _commandResourceService;

        #endregion

        #region ctor

        public CommandController(ICommandResourceService commandResourceService)
        {
            _commandResourceService = commandResourceService;
        }

            #endregion

        #region POST

        [HttpPost]
        public async Task<IActionResult> SendCommand(CommandApiModel apiModel)
        {
            if (apiModel.IsNull() || 
                !apiModel.SessionId.HasValue() ||
                !apiModel.ResourceId.HasValue() )
                return BadRequest(new
                {
                    message = "Missing command data",
                    model = apiModel
                });
            var command = apiModel.ToModel();
            var srvRes = await _commandResourceService.SendCommand(command);
            return srvRes.HasErrors() || !srvRes.IsFullySuccess()?
                BadRequest(new {
                    message = srvRes.ErrorMessage,
                    data = apiModel
                }):
                Ok(srvRes.Model) as IActionResult;
        }
        #endregion
    }
}
