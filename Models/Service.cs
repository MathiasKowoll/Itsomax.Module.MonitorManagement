using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.Models
{
    class Service : EntityBase
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
        public string LoginPassword { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public long SystemId { get; set; }
        public DatabaseSystem System { get; set; }
    }
}