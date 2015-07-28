using System;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using ARSoft.Tools.Net.Dns;
using System.Collections.Generic;

namespace HealthChecker.Lib
{
    public class EmailChecker
    {
        public string checkMailBoxExistence(string emailId, string mailServer, string sender)
        {
            try
            {
                TcpClient tClient = new TcpClient(mailServer, 25);
                string CRLF = "\r\n";
                byte[] dataBuffer;
                string ResponseString;
                NetworkStream netStream = tClient.GetStream();
                StreamReader reader = new StreamReader(netStream);
                ResponseString = reader.ReadLine();
                /* Perform HELO to SMTP Server and get Response */
                dataBuffer = BytesFromString("HELO NowFloats Here" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                dataBuffer = BytesFromString("MAIL FROM:" + sender + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                /* Read Response of the RCPT TO Message to know from google if it exist or not */
                dataBuffer = BytesFromString("RCPT TO:<" + emailId + ">" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                ResponseString = reader.ReadLine();
                if (GetResponseCode(ResponseString) == 550 || GetResponseCode(ResponseString) == 0)
                {
                    return "Email Address Does not Exist ! " + ResponseString;
                }
                /* QUITE CONNECTION */
                dataBuffer = BytesFromString("QUITE" + CRLF);
                netStream.Write(dataBuffer, 0, dataBuffer.Length);
                tClient.Close();
                return "valid";
            }
            catch (Exception)
            {
                return "Not sure about validity";
                throw;
            }
        }
        private byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        private int GetResponseCode(string ResponseString)
        {

            try
            {
                return int.Parse(ResponseString.Substring(0, 3));
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
            
           
        }
        public static string NulltoString(string Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;


        }
        public static bool isEmail(string inputEmail)
        {
            inputEmail = NulltoString(inputEmail);
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
        public string checkValidDomain(string email) 
        {
            try
            {
                string[] host = (email.Split('@'));
                string hostname = host[1];

                IPHostEntry IPhst = Dns.Resolve(hostname);
                IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], 25);
                Socket s = new Socket(endPt.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                s.Connect(endPt);
            }
            catch (Exception)
            {

                return "Not a valid email address";
            }
            return "valid";
        }
        public List<string> getMXRecord(string email, string hostname)
        {
           
            List<string> mailServer = new List<string>();
            try
            {
                DnsMessage dnsMessage = DnsClient.Default.Resolve(hostname, RecordType.Mx);
                if ((dnsMessage == null) || ((dnsMessage.ReturnCode != ReturnCode.NoError) && (dnsMessage.ReturnCode != ReturnCode.NxDomain)))
                {
                    throw new Exception("DNS request failed");
                }
                else
                {
                    foreach (DnsRecordBase dnsRecord in dnsMessage.AnswerRecords)
                    {
                        MxRecord mxRecord = dnsRecord as MxRecord;
                        if (mxRecord != null)
                        {
                          //mailServer = mailServer+ "," + mxRecord.ExchangeDomainName.ToString();
                            mailServer.Add(mxRecord.ExchangeDomainName.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // return ex.ToString();
                throw;
            }
            return mailServer;
        }
        public string checkEmailExistence(string emailId) 
        {
            if (isEmail(emailId) == false) {
                return "Invalid Email Address";
            }
            string[] host = (emailId.Split('@'));
            string hostname = host[1];
            string mailServer = String.Empty;
            string sender = "neelkamal0666@gmail.com";
            if(hostname == "gmail.com")
            {
                mailServer = "gmail-smtp-in.l.google.com";
                return checkMailBoxExistence(emailId, mailServer, sender);
            }
            List<string> mailServers = getMXRecord(emailId, hostname);
            if (mailServers.Count == 0) 
            {
                return "Invalid Email Address";
            }
            foreach (string mail_server in mailServers) // Loop through List with foreach.
            {
                mailServer = mail_server;
                string status = checkMailBoxExistence(emailId, mailServer, sender);
                if(status == "valid"){
                    return status;
                }
            }
            return "Not Sure";
            //
           
        }

    }
}