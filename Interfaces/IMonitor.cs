﻿using Itsomax.Module.MonitorManagement.Models;
using Itsomax.Module.MonitorManagement.ViewModels;
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
    }
}
