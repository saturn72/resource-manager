using System;
using System.Collections.Generic;
using LabManager.Common.Domain.Resource;
using QAutomation.Extensions;

namespace LabManager.Services.Resources
{
    public static class ResourceRepositoryExtensions
    {
        public static IEnumerable<ResourceModel> GetBy(this IResourceRepository reourseRepository, ResourceModel filter)
        {
            if (filter.IsNull())
                throw new InvalidOperationException("Filter cannot be null");

            if (filter.Id > 0)
            {
                var requestedResource = reourseRepository.GetById(filter.Id);
                return requestedResource.NotNull() ? new[] {requestedResource} : null;
            }

            if(filter.IpAddress.HasValue())
                return reourseRepository.GetByIpAddress(filter.IpAddress);


            Func<ResourceModel, bool> query = rs => !filter.FriendlyName.HasValue() || rs.FriendlyName.Equals(filter.FriendlyName, StringComparison.InvariantCultureIgnoreCase);

            return reourseRepository.GetBy(query);
        }

        public static IEnumerable<ResourceModel> GetByIpAddress(this IResourceRepository resourceRepository, string ipAddress)
        {
            return resourceRepository.GetBy(rs => rs.IpAddress == ipAddress);
        }
    }
}