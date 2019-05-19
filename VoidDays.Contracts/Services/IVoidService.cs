using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Contracts.Services
{
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface IVoidService
    {
        [OperationContract]
        bool CreateUser(string username, string password);
        [OperationContract]
        bool Login(string username, string password);
        [OperationContract]
        bool ChangePassword(string username, string password, string newPassword);

    }
}
