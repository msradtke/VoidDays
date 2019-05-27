using AutoMapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Contracts.Data;
using VoidDays.Contracts.Services;
using VoidDays.Core;
using VoidDays.DAL.Models;
using VoidDays.DAL.Providers;

namespace VoidDays.Services
{
    public class VoidService : IVoidService, IService
    {
        EFDbContext _dbContext;
        public bool ChangePassword(string username, string password, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool CreateUser(string username, string password)
        {
            var userService = new UserProvider();
            return userService.CreateUser(username, password);
        }
        public DayDTO GetCurrentDay()
        {
            using (_dbContext = new EFDbContext())
            {
                var day = _dbContext.Days.FirstOrDefault(x => x.DayNumber == _dbContext.Days.Max(y => y.DayNumber));
                return Mapper.Map<DayDTO>(day);
            }
        }
        public List<GoalItemDTO> GetCurrentGoalItems()
        {
            using (_dbContext = new EFDbContext())
            {
                var day = _dbContext.GoalItems.Where(x => x.DayNumber == _dbContext.Days.Max(y => y.DayNumber)).ToList();
                return Mapper.Map<List<GoalItemDTO>>(day);
            }
        }
        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
