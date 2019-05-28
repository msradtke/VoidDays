using Mobile.Providers;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile
{
    public class VoidNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IVoidProvider>().To<VoidProvider>().InTransientScope();
        }
    }
}
