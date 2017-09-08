using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.Models
{
    public class DatabaseSystem : EntityBase
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
