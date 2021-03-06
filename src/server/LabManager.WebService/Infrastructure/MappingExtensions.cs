using System.Collections.Generic;
using System.IO;
using System.Linq;
using LabManager.Common.Domain.Resource;
using LabManager.WebService.Models.Resources;
using LabManager.WebService.Models.Runtime;
using LabManager.Services.Runtime;
using Saturn72.Extensions;

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
                SshUsername = source.SshUsername,
                SshPassword = source.SshPassword,
                SquishServerPort = source.SquishServerPort,
                SquishServerLocalPath = source.SquishServerLocalPath,
                ObjectMapFilePath = source.ObjectMapFilePath
            };
        }

        public static ResourceModel ToModel(this ResourceApiModel source)
        {
            if (source.SquishServerLocalPath.HasValue())
            {
                while (source.SquishServerLocalPath.EndsWith('\\'))
                    source.SquishServerLocalPath = source.SquishServerLocalPath.Substring(0, source.SquishServerLocalPath.Length - 1);
            }

            return new ResourceModel
            {
                Id = source.Id,
                FriendlyName = source.FriendlyName,
                Active = source.Active,
                IpAddress = source.IpAddress,
                SshUsername = source.SshUsername,
                SshPassword = source.SshPassword,
                SquishServerPort = source.SquishServerPort,
                SquishServerLocalPath = source.SquishServerLocalPath,
                ObjectMapFilePath = source.ObjectMapFilePath
            };
        }

        #endregion

        #region ResourceAssignment

        public static ResourceAssignmentRequest ToModel(this ResourceAssignmentRequestApiModel source)
        {
            return new ResourceAssignmentRequest
            {
                RequiredResources = source.Filter?.Select(ToModel),
                ClientReferenceCode = source.ClientReferenceCode,
                ResourceCount = source.ResourceCount,
            };
        }
        #endregion
    }
}