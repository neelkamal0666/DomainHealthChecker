using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HealthChecker
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService2" in both code and config file together.
    [ServiceContract]
    public interface IHealthChecker
    {
        [OperationContract]
        [WebInvoke(
                    Method = "GET",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    UriTemplate = "/GetHealth?domain={domain}"
                   )]
        string GetHealth(string domain);

        [OperationContract]
        [WebInvoke(
                    Method = "GET",
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json,
                    UriTemplate = "/CheckEmailExistence?email={email}"
                   )]
        string CheckEmailExistence(string email);
    }
}
