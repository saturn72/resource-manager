using System.Collections.Generic;
using System.Linq;
using LabManager.Common.Domain.Resource;
using LabManager.WebService.Models.Resources;
using LabManager.WebService.Models.Runtime;
using LabManager.Services.Runtime;

namespace LabManager.WebService.Infrastructure
{
    internal static class MappingExtensions
    {
        #region ResourceMocel <==> Resource ApiModel
        public static IEnumerable<ResourceApiModel> ToApiModel(this IEnumerable<ResourceModel> source)
        {
            return source.Select(ToApiModel);
        }

        
        public static ResourceApiModel ToApiModel(this ResourceModel source)
        {
            return new ResourceApiModel
            {
                Id = source.Id,
                FriendlyName = source.FriendlyName,
                IpAddress = source.IpAddress,
                Status = source.Status,
                Active = source.Active,
            };
        }
       
        public static ResourceModel ToModel(this ResourceApiModel source)
        {
            return new ResourceModel
            {
                Id = source.Id,
                FriendlyName = source.FriendlyName,
                Active = source.Active,
                IpAddress = source.IpAddress,
            };
        }

        #endregion

        #region ResourceAssignment

        public static ResourceAssignmentRequest ToModel(this ResourceAssignmentRequestApiModel source)
        {
            return new ResourceAssignmentRequest
            {
                RequiredResources = source.RequiredResources.Select<ResourceApiModel, ResourceModel>(s => s.ToModel()),
                ClientReferenceCode = source.ClientReferenceCode
            };
        }
        #endregion
    }
}