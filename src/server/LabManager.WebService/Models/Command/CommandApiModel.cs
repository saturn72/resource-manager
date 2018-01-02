using System.ComponentModel.DataAnnotations;

namespace LabManager.WebService.Models.Command
{
    public class CommandApiModel
    {
        [Required]
        public string SessionId { get; set; }

        [Required]
        public string ResourceId { get; set; }
    }
}
