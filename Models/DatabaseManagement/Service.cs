using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.Models.DatabaseManagement
{
    public class Service : EntityBase
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
        public byte[] LoginPassword { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public long DatabaseSystemId { get; set; }
        public DatabaseSystem DatabaseSystem { get; set; }
    }
}