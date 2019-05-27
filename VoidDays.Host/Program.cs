using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Contracts.Services;
using VoidDays.Services;

namespace VoidDays.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(binDir);
            Environment.SetEnvironmentVariable("PATH", path + ";" + binDir);
            Uri voidServiceAddress = new Uri("http://localhost:8733/Design_Time_Addresses/VoidDays.Services/");
            // Step 2 Create a ServiceHost instance  
            ServiceHost voidServiceHost = new ServiceHost(typeof(VoidService), voidServiceAddress);
            try
            {
                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = 10485760;
                binding.MaxBufferSize = 10485760;
                binding.MaxBufferPoolSize = 10485760;
                binding.TransferMode = TransferMode.Streamed;
                binding.ReaderQuotas.MaxArrayLength = 10485760;
                binding.ReaderQuotas.MaxDepth = 10485760;

                voidServiceHost.AddServiceEndpoint(typeof(IVoidService), binding, "VoidService");
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                smb.HttpGetEnabled = true;
                voidServiceHost.Description.Behaviors.Add(smb);

                voidServiceHost.AddServiceEndpoint(
                                                    ServiceMetadataBehavior.MexContractName,
                                                    MetadataExchangeBindings.CreateMexHttpBinding(),
                                                    "mex"
                                                );

                voidServiceHost.Open();
                Console.WriteLine("Service opened.");
                Console.ReadLine();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                voidServiceHost.Abort();
            }
        }
    }
}
