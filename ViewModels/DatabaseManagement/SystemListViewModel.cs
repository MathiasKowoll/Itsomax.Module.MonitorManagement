﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement
{
    public class SystemListViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
