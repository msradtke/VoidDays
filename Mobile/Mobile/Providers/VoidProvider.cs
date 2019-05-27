using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace Mobile.Providers
{
    public class VoidProvider
    {
        VoidServiceClient _client;
        public VoidProvider()
        {
            var add = new EndpointAddress("http://3.16.24.128:8733/Design_Time_Addresses/VoidDays.Services/VoidService/");
            _client = new VoidServiceClient(new BasicHttpBinding(), add);
        }

        public bool CreateUser(string username, string password)
        {
            var test = _client.CreateUser(username, password);
            return test;
        }

    }

    public interface IVoidProvider
    {

    }
}
