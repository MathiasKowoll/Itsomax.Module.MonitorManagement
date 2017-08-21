using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.Models
{
    class Instance : EntityBase
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string IP { get; set; }
        [MaxLength(100)]
        public string InstanceName { get; set; }
    }
}