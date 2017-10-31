﻿#region Usings

using QAutomation.Core.Domain;
using QAutomation.Extensions;

#endregion

namespace QAutomation.Core.Services
{
    public static class ServiceResponseExtensions
    {
        public static bool HasErrors<TDomainModel>(this ServiceResponse<TDomainModel> response)
            where TDomainModel : DomainModelBase
        {
            return response.ErrorMessage.HasValue();
        }
    }
}