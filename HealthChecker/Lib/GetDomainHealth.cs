using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace HealthChecker.Lib
{
    public class GetDomainHealth
    {
        public static string GetExpirationDate(string domain) 
        {
            try
            {
                string info = WhoIsLookup.lookup(domain, WhoIsLookup.RecordType.domain);
                string[] words = Regex.Split(info, "Expiration Date: ");
                if (words.Count() < 2)
                {
                    return "Domain Expired";
                }
                else
                {

                    string[] word = words[1].Split('\n');
                    if (word.Count() < 2)
                    {
                        return "Domain Expired";
                    }
                    else
                    {
                        return word[0];
                    }
                }
            }
            catch (OverflowException)
            {
                return "Domain Expired";
            }
            
        }
        public static string CheckServerHealth(string domain) 
        {
            try
            {
                WebRequest request = WebRequest.Create("http://" + domain);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return "server down or too busy";
                }
                else
                {
                    return "OK";
                }
            }
            catch (Exception)
            {

                return "Invalid domain or server down. Don't add http in doamin";
            }
        }
        public static string CheckDomainExpiration(string domain) 
        {
            try
            {
                string expirationDate = GetExpirationDate(domain);
                if (expirationDate == "Domain Expired")
                {
                    return expirationDate;
                }
                string[] arr = expirationDate.Split('-');
                if (arr.Count() < 3)
                {
                    return "Domain Expired";
                }
                string[] monthArr = { "null", "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
                var today = DateTime.UtcNow.ToString("dd'-'MM'-'yyy", DateTimeFormatInfo.InvariantInfo);

                string[] arr1 = today.Split('-');
                if (Int32.Parse(arr[2]) > Int32.Parse(arr1[2]))
                {
                    return "OK";
                }
                else if (Int32.Parse(arr[2]) == Int32.Parse(arr1[2]))
                {
                    if (Array.IndexOf(monthArr, arr[1]) == Int32.Parse(arr1[1]))
                    {
                        if (Int32.Parse(arr[0]) < Int32.Parse(arr1[0]))
                        {
                            return "OK";
                        }
                        else if (Int32.Parse(arr[0]) == Int32.Parse(arr1[0]))
                        {
                            return "Domain will expire today";
                        }
                        else
                        {
                            return "Domain Expired";
                        }

                    }
                    else if (Array.IndexOf(monthArr, arr[1]) > Int32.Parse(arr1[1]))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "Doamin Expired";
                    }
                }
                else
                {
                    return "Doamin Expired";
                }
            }
            catch (OverflowException)
            {

                return "Domain Expired";
            }
        }

        public static string GetDomainHealthStatus(string domain) 
        {
            string domainStatus = CheckDomainExpiration(domain);
            if (domainStatus == "OK")
            {
                string serverStatus = CheckServerHealth(domain);
                return serverStatus;
            }
            else 
            {
                return domainStatus;
            }
            
        }

    }
}