using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement
{
    public class CreateServiceViewModel
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [MaxLength(50)]
        [Required]
        public string IP { get; set; }
        [MaxLength(200)]
        [Required]
        public string Hostname { get; set; }
        [MaxLength(100)]
        public string Named { get; set; }
        [MaxLength(100)]
        [Required]
        public string LoginName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string LoginPassword { get; set; }
        [Required]
        public bool Active { get; set; }
        public IEnumerable<SelectListItem> SystemList { get; set; }
    }
}
