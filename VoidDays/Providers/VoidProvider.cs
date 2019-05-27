using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ClientProxies;

namespace VoidDays.Providers
{
    public class VoidProvider : IVoidProvider
    {
        private readonly IVoidClient _voidClient;

        public VoidProvider(IVoidClient voidClient)
        {
            _voidClient = voidClient;
        }

        public bool CreateUser(string username, string password)
        {
            return _voidClient.CreateUser(username, password);
        }
    }

    public interface IVoidProvider
    {
        bool CreateUser(string username, string password);
    }
}
