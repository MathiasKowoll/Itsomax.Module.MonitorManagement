using Itsomax.Module.MonitorManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Itsomax.Module.MonitorManagement.Models.DatabaseManagement;
using Itsomax.Module.MonitorManagement.ViewModels.DatabaseManagement;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Interfaces;
using System.Linq;

namespace Itsomax.Module.MonitorManagement.Services
{
    public class MonitorServices : IMonitor
    {
        private readonly IRepository<DatabaseSystem> _systemRepository;
        private readonly ILogginToDatabase _logger;

        public MonitorServices(IRepository<DatabaseSystem> systemRepository, ILogginToDatabase logger)
        {
            _systemRepository = systemRepository;
            _logger = logger;
        }

        public bool CreateSystem(CreateSystemViewModel model, string userName)
        {
            try
            {
                var dbSysten = new DatabaseSystem()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Active = model.Active
                };

                _systemRepository.Add(dbSysten);
                _systemRepository.SaveChange();
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Create Database System", ex.InnerException.Message, userName);
                return false;
            }

        }

        public IEnumerable<SystemListViewModel> GetSystemList(string userName)
        {
            try
            {
                var list = _systemRepository.Query().ToList().Select(x => new SystemListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Active = x.Active
                });
                return list;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Get System List", ex.InnerException.Message, userName);
                return null;
            }

        }
        public EditSystemViewModel GetSystemForEdit(long Id, string userName)
        {
            try
            {
                var system = _systemRepository.Query().FirstOrDefault(x => x.Id == Id);
                var editSystem = new EditSystemViewModel()
                {
                    Id = system.Id,
                    Name = system.Name,
                    Description = system.Description,
                    Active = system.Active
                };
                return editSystem;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Get System", ex.InnerException.Message);
                return null;
            }

        }

        public DatabaseSystem GetSystem(long Id, string userName)
        {
            try
            {
                var system = _systemRepository.Query().FirstOrDefault(x => x.Id == Id);
                
                return system;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Get System", ex.InnerException.Message);
                return null;
            }

        }

        public DatabaseSystem GetSystem(long Id)
        {
            return GetSystem(Id, string.Empty);
        }

        public bool EditSystem(EditSystemViewModel model, string userName)
        {
            try
            {
                var oldsystem = GetSystem(model.Id);
                oldsystem.Name = model.Name;
                oldsystem.Description = model.Description;
                oldsystem.Active = model.Active;
                _systemRepository.SaveChange();
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Edit System", ex.InnerException.Message, userName);
                return false;
            }
        }

        public bool DeleteSystem(long Id,string userName)
        {
            try
            {
                var deleteSystem = GetSystem(Id);
                _systemRepository.Remove(deleteSystem);
                _systemRepository.SaveChange();
                return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Delete System", ex.InnerException.Message, userName);
                return false;
            }
        }

        public bool DisableEnableSystem(long Id,string userName)
        {
            try
            {
                var systemEna = GetSystem(Id);
                if(systemEna.Active)
                {
                    systemEna.Active = false;
                    _systemRepository.SaveChange();
                    return true;
                }
                else
                {
                    systemEna.Active = true;
                    _systemRepository.SaveChange();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Enable/Disable System", ex.InnerException.Message, userName);
                return false;
            }
        }
    }
}
