using Itsomax.Data.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Itsomax.Module.MonitorManagement.Models
{
    public class VendorProducts : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength]
        public string Description { get; set; }
        [Required]
        public long VendorId { get; set; }
        public Vendor Vendor { get; set; }

    }
}
