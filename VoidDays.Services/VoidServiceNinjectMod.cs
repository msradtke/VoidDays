using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Core;

namespace VoidDays.Services
{
    public class VoidServiceNinjectMod : NinjectModule
    {
        public override void Load()
        {
            Bind<IService>().To<VoidService>().InTransientScope();
        }
    }
}
