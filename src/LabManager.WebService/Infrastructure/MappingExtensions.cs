using System.Collections.Generic;
using System.Linq;
using LabManager.Common.Domain.Resource;
using LabManager.WebService.Models.Resources;

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

        public static ResourceModel ToModel(this ResourceApiModel source, bool includeId = false)
        {
            return new ResourceModel
            {
                Id = includeId ? source.Id : default(long),
                
                FriendlyName = source.FriendlyName,
                Active = source.Active,
                IpAddress = source.IpAddress,
            };
        }

        #endregion
    }
}