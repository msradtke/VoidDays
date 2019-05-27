using Mobile.Providers;
using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new VoidProvider();
            provider.CreateUser("stand","stand");
        }
    }
}
