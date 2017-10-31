using FluentValidation;
using LabManager.WebService.Models.Resources;

namespace LabManager.WebService.Validators.Resources
{
    public class ResourceApiModelValidator:AbstractValidator<ResourceApiModel>
    {
        public ResourceApiModelValidator()
        {
            //fetch cache and validate there is not identical resource.
            //The following ,ust be unique:
            //name, 
        }
    }
}
