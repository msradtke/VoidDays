using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.ViewModels
{
    public class CreateUserViewModel
    {
        public CreateUserViewModel()
        {

        }

        public SecureString SecurePassword { private get; set; }
        public SecureString SecureVerifyPassword { private get; set; }
        public string Message { get; set; }
        void CreateUser()
        {
            if(SecurePassword != SecureVerifyPassword)
            {
                Message = "Passwords must match";
                return;
            }
            Message = "";

        }

        
    }
}
