using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement
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
        [Required]
        public IEnumerable<SelectListItem> VendorList { get; set; }
        [Required]
        public IEnumerable<SelectListItem> ConfigTypeList { get; set; }
    }
}
