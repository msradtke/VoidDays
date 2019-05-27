using Mobile.Services;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using VoidDays.Contracts.Data;

namespace Mobile.Providers
{
    public class VoidProvider : IVoidProvider
    {
        VoidServiceClient _client;
        public VoidProvider()
        {
            var add = new EndpointAddress("http://10.0.0.57:8733/Design_Time_Addresses/VoidDays.Services/VoidService/");
            _client = new VoidServiceClient(new BasicHttpBinding(), add);
        }

        public bool CreateUser(string username, string password)
        {
            var test = _client.CreateUser(username, password);
            return test;
        }

        public DayDTO GetCurrentDay()
        {
            return _client.GetCurrentDay();
        }
        public List<GoalItemDTO> GetCurrentGoalItems()
        {
            return _client.GetCurrentGoalItems();
        }

    }

    public interface IVoidProvider
    {
        DayDTO GetCurrentDay();
        List<GoalItemDTO> GetCurrentGoalItems();
    }
}
