using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.ViewModels
{
    public class CreateSystemViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
