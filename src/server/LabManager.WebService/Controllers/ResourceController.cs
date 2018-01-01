#region using

using System;
using System.Threading.Tasks;
using LabManager.WebService.Models.Resources;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Resources;
using LabManager.WebService.Infrastructure;
using Saturn72.Extensions;
using Saturn72.Core.Services;

#endregion

namespace LabManager.WebService.Controllers
{
    public class ResourceController : ControllerBase
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
        public async Task<IActionResult> Create([FromBody] ResourceApiModel apiModel)
        {
            if (apiModel.IsNull())
                return BadRequest(apiModel);

            var model = apiModel.ToModel();
            var srvRes = await _resourceService.CreateAsync(model);
            return !srvRes.HasErrors() && srvRes.Result == ServiceResponseResult.Success
                ? Created(srvRes.Model.Id.ToString() as string, srvRes.Model.ToApiModel())
                : BadRequest(apiModel) as IActionResult;
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

        // PUT api/values/5
        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] ResourceApiModel resource)
        {
            if(resource.IsNull() || resource.Id <=0)
                return BadRequest("Missing Resource Id");

            var srvRes = await _resourceService.UpdateAsync(resource.ToModel());
            return !srvRes.HasErrors() && srvRes.Result == ServiceResponseResult.Success
                ? Ok(srvRes.Model.ToApiModel())
                : BadRequest(resource) as IActionResult;
        }

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}