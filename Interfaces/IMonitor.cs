using Itsomax.Module.MonitorManagement.Models.DatabaseManagement;
using Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itsomax.Module.MonitorManagement.Interfaces
{
    public interface IMonitor
    {
        bool CreateSystem(CreateSystemViewModel model, string Username);
        IEnumerable<SystemListViewModel> GetSystemList(string userName);
        EditSystemViewModel GetSystemForEdit(long Id, string userName);
        DatabaseSystem GetSystem(long Id, string userName);
        DatabaseSystem GetSystem(long Id);
        bool EditSystem(EditSystemViewModel model, string userName);
        bool DeleteSystem(long Id, string userName);
        bool DisableEnableSystem(long Id, string userName);
    }
}
