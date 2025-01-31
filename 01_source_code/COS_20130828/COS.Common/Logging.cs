using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace COS.Common
{
    public static class Logging
    {
        public static string LogException(Exception exc, LogType loggingType, Dictionary<string, string> extendedInfo)
        {
            if (loggingType == LogType.ToFile)
            {
                StreamWriter sw = null;
                try
                {
                    using (sw = CheckLogFolderAndFile())
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        var innerExc = exc.InnerException;
                        while (innerExc != null)
                        {
                            sw.WriteLine(innerExc.Message);
                            sw.WriteLine(innerExc.StackTrace);
                            sw.WriteLine();
                            sw.WriteLine();
                            innerExc = innerExc.InnerException;
                        }
                        sw.WriteLine(exc.Message);
                        sw.WriteLine(exc.StackTrace);
                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
            else if (loggingType == LogType.ToEmail)
            {

            }
            else if (loggingType == LogType.ToFileAndEmail)
            {
                StreamWriter sw = null;
                try
                {
                    using (sw = CheckLogFolderAndFile())
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        var innerExc = exc.InnerException;
                        while (innerExc != null)
                        {
                            sw.WriteLine(innerExc.Message);
                            sw.WriteLine(innerExc.StackTrace);
                            sw.WriteLine();
                            sw.WriteLine();
                            innerExc = innerExc.InnerException;
                        }
                        sw.WriteLine(exc.Message);
                        sw.WriteLine(exc.StackTrace);
                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }


                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                    message.To.Add("dashboard@lindab.cz");

                    message.Subject = "Error COS App";
                    message.From = new System.Net.Mail.MailAddress("lindab@calch.cz");
                    message.Body = "Error from COS application:";
                    message.Body += Environment.NewLine;
                    message.Body += Environment.NewLine;
                    message.Body += DateTime.Now.ToString();
                    message.Body += Environment.NewLine;
                    message.Body += Environment.NewLine;

                    if (extendedInfo != null)
                    {
                        foreach (var itm in extendedInfo)
                        {
                            message.Body += itm.Key + ": " + itm.Value;
                            message.Body += Environment.NewLine;
                        }
                    }

                    message.Body += Environment.NewLine;
                    message.Body += Environment.NewLine;

                    message.Body += exc.Message;
                    message.Body += Environment.NewLine;
                    message.Body += exc.StackTrace;

                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("se861057");


                    //System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential("helpdesk@calch.cz", "Calch12349");
                    smtp.UseDefaultCredentials = true;
                    //smtp.Credentials = SMTPUserInfo;


                    smtp.Send(message);
                }
                catch (Exception mex)
                {
                    try
                    {
                        using (sw = CheckLogFolderAndFile())
                        {
                            sw.WriteLine(DateTime.Now.ToString());
                            var innerExc = mex.InnerException;
                            while (innerExc != null)
                            {
                                sw.WriteLine(innerExc.Message);
                                sw.WriteLine(innerExc.StackTrace);
                                sw.WriteLine();
                                sw.WriteLine();
                                innerExc = innerExc.InnerException;
                            }
                            sw.WriteLine(mex.Message);
                            sw.WriteLine(mex.StackTrace);
                            sw.WriteLine();
                            sw.WriteLine();
                        }
                    }
                    finally
                    {
                        if (sw != null)
                            sw.Close();
                    }
                }

            }

            string result = "";

            SqlException sqlexc = exc as SqlException;
            if (sqlexc == null && exc.InnerException != null)
                sqlexc = exc.InnerException as SqlException;

            if (sqlexc != null && sqlexc.Number == 547)
            {
                result = "Na tuto položku je vázán jeden nebo více jiných záznamů v dalších modulech.";
            }

            return result;
        }


        public static string LogException(Exception exc, LogType loggingType)
        {
            return LogException(exc, loggingType, null);
        }


        public static void LogException(string errorMessage, LogType loggingType)
        {
            if (loggingType == LogType.ToFile)
            {
                StreamWriter sw = null;
                try
                {
                    using (sw = CheckLogFolderAndFile())
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        sw.WriteLine(errorMessage);

                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
            else if (loggingType == LogType.ToEmail)
            {

            }
            else if (loggingType == LogType.ToFileAndEmail)
            {
                StreamWriter sw = null;
                try
                {
                    using (sw = CheckLogFolderAndFile())
                    {
                        sw.WriteLine(DateTime.Now.ToString());
                        sw.WriteLine(errorMessage);

                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }

        public static void WriteToFile()
        {

        }

        public static StreamWriter CheckLogFolderAndFile()
        {
            string logFolder = @"LOGS";

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            StreamWriter log;

            if (!File.Exists(logFolder + @"\logs.txt"))
            {
                log = new StreamWriter(logFolder + @"\logs.txt");
            }
            else
            {
                log = File.AppendText(logFolder + @"\logs.txt");
            }

            return log;
        }
    }

    public enum LogType
    {
        ToFile = 1,
        ToEmail = 2,
        ToFileAndEmail = 3
    }
}
