#region using

using System.Threading.Tasks;
using LabManager.WebService.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Resources;
using LabManager.WebService.Infrastructure;
using QAutomation.Extensions;
using QAutomation.Core.Services;

#endregion

namespace LabManager.WebService.Controllers
{
    [Route("api/[controller]")]
    public class ResourceController : Controller
    {
        #region Fields
        private readonly IResourceService _resourceService;
        #endregion
        #region ctor
        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ResourceApiModel apiModel)
        {
            if (apiModel.IsNull())
                return BadRequest();

            var model = apiModel.ToModel();
            var srvRes = await _resourceService.CreateAsync(model);
            return !srvRes.HasErrors()&& srvRes.Result == ServiceResponseResult.Success?
                Created(srvRes.Model.Id.ToString()as string, srvRes.Model.ToApiModel()) :
                BadRequest() as IActionResult;
        }
        #endregion

        #region read
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var resources = await _resourceService.GetAllAsync();
            return Ok(resources.ToApiModel());
        }

       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var res = await _resourceService.GetById(id);
            if (res.IsNull())
                return BadRequest();
            return Ok(res.ToApiModel());
        }
        #endregion
        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody]string value)
        // {
        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
