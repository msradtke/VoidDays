using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string DataKeyCipher { get; set; }
        public string IntermediateKeyCipher { get; set; }
        public string Message { get; set; }

        [NotMapped]
        public string Password { get; set; }
        [NotMapped]
        public string DataKey { get; set; }
        [NotMapped]
        public string IntermediateKey { get; set; }
    }
}
