﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LabManager.Services.Instance;

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

        [HttpPost]
        public async Task<IActionResult> StartAsync(int resourceId)
        {
            if (resourceId <= 0)
                return BadRequest(new
                {
                    @resourceId = resourceId,
                    message = "ResourceId must be greater than 0."
                });

            var serviceResponse = await _instanceService.Start(resourceId);
            return serviceResponse.Model.Started
                ? Ok() as IActionResult
                : BadRequest(new
                {
                    @resourceId = resourceId,
                    message = string.Format("Failed To start resource: {0}\n{1}",  resourceId, serviceResponse.ErrorMessage)
                });
        }
    }
}
