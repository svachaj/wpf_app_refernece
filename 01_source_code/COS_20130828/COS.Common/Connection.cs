using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace COS.Common
{
    public static class Connection
    {
        public static bool CheckServerSource(string serverPath)
        {
            bool result = false;

            if (Directory.Exists(serverPath))
                result = true;

            return result;
        }

        public static bool IsConnectedToServer(string dBServerNameOrIP, out long roundtripTime)
        {
            roundtripTime = 800;
            bool result = false;
            Ping p = new Ping();
            PingReply reply = null;

            try
            {
                reply = p.Send(dBServerNameOrIP, 800);

                if (reply.Status == IPStatus.Success)
                {
                    roundtripTime = reply.RoundtripTime;
                    return true;
                }
            }
            catch
            {

            }
            finally
            {
                p.Dispose();
            }

            return result;
        }


        public static string DBServerNameOrIP
        {
            get
            {
                string cryptname = System.Configuration.ConfigurationManager.AppSettings["dbServerName"];

                if (!string.IsNullOrEmpty(cryptname))
                {
                    return Crypto.DecryptString(cryptname, COS.Security.SecurityHelper.SecurityKey);
                }
                else
                    return null;
            }
        }


    }
}
