using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.DAL.Models
{
    public class LoadingLock
    {
        public Guid Id { get; set; }
        public bool IsLoading { get; set; }
    }
}
