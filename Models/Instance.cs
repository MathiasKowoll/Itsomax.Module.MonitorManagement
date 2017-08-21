using Itsomax.Data.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Itsomax.Module.MonitorManagement.Models
{
    class Instance : EntityBase
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}