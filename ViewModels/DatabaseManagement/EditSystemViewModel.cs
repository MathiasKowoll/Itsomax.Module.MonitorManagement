using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement
{
    public class EditSystemViewModel
    {
        [Required]
        public long Id { get; set; }
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
