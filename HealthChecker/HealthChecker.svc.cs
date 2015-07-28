using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using HealthChecker.Lib;

namespace HealthChecker
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service2.svc or Service2.svc.cs at the Solution Explorer and start debugging.
    public class HealthChecker : IHealthChecker
    {
        public string GetHealth(string domain)
        {

            string status = String.Empty;
            string info = GetDomainHealth.GetDomainHealthStatus(domain);
            return info;
        }
        public string CheckEmailExistence(string email)
        {
            var obj = new EmailChecker();
            return obj.checkEmailExistence(email);
        }
       
    }
}
