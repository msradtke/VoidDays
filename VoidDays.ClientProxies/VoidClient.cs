using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Contracts.Data;
using VoidDays.Contracts.Services;

namespace VoidDays.ClientProxies
{
    public class VoidClient : ClientBase<IVoidService>, IVoidService, IVoidClient
    {
        public bool ChangePassword(string username, string password, string newPassword)
        {
            return Channel.ChangePassword(username, password, newPassword);
        }

        public bool CreateUser(string username, string password)
        {
            return Channel.CreateUser(username, password);
        }

        public DayDTO GetCurrentDay()
        {
            return Channel.GetCurrentDay();
        }

        public List<GoalItemDTO> GetCurrentGoalItems()
        {
            return Channel.GetCurrentGoalItems();
        }

        public bool Login(string username, string password)
        {
            return Channel.Login(username, password);
        }
    }

    public interface IVoidClient
    {
        bool CreateUser(string username, string password);
        bool Login(string username, string password);
        bool ChangePassword(string username, string password, string newPassword);
        List<GoalItemDTO> GetCurrentGoalItems();
    }
}
