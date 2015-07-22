using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HealthChecker.Lib
{
    public class WhoIsLookup
    {
        public enum RecordType { domain, nameserver, registrar };

        /// <summary>
        /// retrieves whois information
        /// </summary>
        /// <param name="domainname">The registrar or domain or name server whose whois information to be retrieved</param>
        /// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
        /// <returns>The string containg the whois information</returns>
        public static string lookup(string domainname, RecordType recordType)
        {
            List<string> res = lookup(domainname, recordType, "whois.internic.net");
            string result = "";
            foreach (string st in res)
            {
                result += st + "\n";
            }
            return result;
        }        /// <summary>
        /// retrieves whois information
        /// </summary>
        /// <param name="domainname">The registrar or domain or name server whose whois information to be retrieved</param>
        /// <param name="recordType">The type of record i.e a domain, nameserver or a registrar</param>
        /// <param name="returnlist">use "whois.internic.net" if you dont know whoisservers</param>
        /// <returns>The string list containg the whois information</returns>
        public static List<string> lookup(string domainname, RecordType recordType, string whois_server_address)
        {
            if (whois_server_address == "")
                whois_server_address = "whois.internic.net";
            TcpClient tcp = new TcpClient();
            tcp.Connect(whois_server_address, 43);
            string strDomain = recordType.ToString() + " " + domainname + "\r\n";
            byte[] bytDomain = Encoding.ASCII.GetBytes(strDomain.ToCharArray());
            Stream s = tcp.GetStream();
            s.Write(bytDomain, 0, strDomain.Length);
            StreamReader sr = new StreamReader(tcp.GetStream(), Encoding.ASCII);
            string strLine = "";
            List<string> result = new List<string>();
            while (null != (strLine = sr.ReadLine()))
            {
                result.Add(strLine);
            }
            tcp.Close();
            return result;
        }
    }
}