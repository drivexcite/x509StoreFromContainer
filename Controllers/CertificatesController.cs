using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace x509storeFromContainer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificatesController : ControllerBase
    {
        private readonly ILogger<CertificatesController> _logger;

        public CertificatesController(ILogger<CertificatesController> logger)
        {
            _logger = logger;
        }

        private IEnumerable<object> GetCertificateInfo()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            foreach(var certificate in store.Certificates)
            {
                yield return new 
                {
                    certificate.IssuerName,
                    certificate.FriendlyName,                    
                    certificate.Subject,
                    certificate.SubjectName, 
                    certificate.HasPrivateKey,
                    certificate.NotAfter,
                    certificate.NotBefore,           
                    certificate.Thumbprint
                };
            }

            store.Close();
        }

        [HttpGet]
        public List<object> Get()
        {
            try
            {
                return GetCertificateInfo().ToList();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Something is not quite working");
            }

            return null;
        }
    }
}
