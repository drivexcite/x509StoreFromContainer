using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace x509storeFromContainer
{
    public static class HostExtensions
    {
        public static IHost AddCertificatesToStore(this IHost host) 
        {
            var logger = host.Services.GetService<ILogger<Program>>();

            try
            {                
                var certificatesPath = Path.Combine(Environment.CurrentDirectory, "certs");                

                var certificates = (
                    from file in new DirectoryInfo(certificatesPath).GetFiles("*.pfx") 
                    let certificateLocation = file.FullName
                    let base64Representation = File.ReadAllText(certificateLocation)
                    let binaryRepresentation = Convert.FromBase64String(base64Representation)
                    select new X509Certificate2(binaryRepresentation)
                ).ToList();

                logger.LogInformation($"{certificates.Count} certificates found in {certificatesPath}");

                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);

                foreach(var certificate in certificates)
                {                                                        
                    store.Add(certificate);
                    logger.LogInformation($"Added Friendly Name: {certificate.FriendlyName}, Subject Name: {certificate.SubjectName?.Name}, Thumbprint {certificate.Thumbprint}");
                }
            }
            catch(Exception e)
            {
                logger.LogWarning(e, "Loading the certificates didn't work");
            }

            return host;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {            
            CreateHostBuilder(args)
                .Build()
                .AddCertificatesToStore()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
