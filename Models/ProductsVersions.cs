using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Itsomax.Module.MonitorManagement.Models
{
    public class ProductsVersions
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string Version { get; set; }
        [Required]
        public int Mayor { get; set; }
        [Required]
        public int Minor { get; set; }
        [Required]
        public int Patch { get; set; }
        public long VendorProductsId { get; set; }

    }
}
