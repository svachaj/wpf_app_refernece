using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace COS.Direction
{
    public class DistanceClass
    {
        public int Distance { set; get; }
        public int Duration { set; get; }

        public string OriginAddress { set; get; }
        public string DestinationAddress { set; get; }

        public override string ToString()
        {
            if (Distance > 0)
            {
                var res = "Délka: " + (Distance / 1000).ToString() + "km";

                res += Environment.NewLine;
                res += "Doba: " + (Duration / 60).ToString() + " min";

                res += Environment.NewLine;
                res += "Místo odjezdu: " + OriginAddress;

                res += Environment.NewLine;
                res += "Místo příjezdu: " + DestinationAddress;

                return res;
            }
            else
                return base.ToString();
        }

        public decimal DistanceKM
        {
            get
            {
                return Math.Round((decimal)this.Distance / 1000, 2);
            }
        }

        public static List<DistanceClass> GetDirection(string origin, List<string> destinations, string username, string password, string domain, string prxyurl)
        {
            List<DistanceClass> result = new List<DistanceClass>();

            DistanceClass item = null;

            try
            {
                item = new DistanceClass();

                WebClient wc = new WebClient();

                WebProxy wp = new WebProxy();

                if (!string.IsNullOrEmpty(prxyurl))
                {
                    wp.Address = new Uri(prxyurl);
                }
                else 
                {
                    wp = WebProxy.GetDefaultProxy();
                }
                

                wc.Proxy = wp;

                wc.Proxy.Credentials = new NetworkCredential(username, password, domain);


                var wayps = "";

                foreach (var itm in destinations)
                    wayps += "|" + itm;

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(System.Text.Encoding.UTF8.GetString(wc.DownloadData("http://maps.googleapis.com/maps/api/directions/xml?origin=" + origin + "&waypoints=optimize:false" + wayps + "&sensor=false")));

                var status = xmldoc.ChildNodes[1].ChildNodes[0].FirstChild.Value;

                if (!string.IsNullOrEmpty(status) && status == "OK")
                {
                    var legs = xmldoc.ChildNodes[1].ChildNodes[1].ChildNodes.OfType<XmlElement>().Where(a => a.Name == "leg");

                    foreach (var itm in legs)
                    {
                        item = new DistanceClass();

                        var distance = itm.SelectSingleNode("distance/value").FirstChild.Value;
                        item.Distance = int.Parse(distance);

                        var duration = itm.SelectSingleNode("duration/value").FirstChild.Value;
                        item.Duration = int.Parse(duration);

                        var startadr = itm.SelectSingleNode("start_address").FirstChild.Value;
                        item.OriginAddress = startadr;

                        var endadr = itm.SelectSingleNode("end_address").FirstChild.Value;
                        item.DestinationAddress = endadr;

                        result.Add(item);
                    }
                }
                else
                    result = null;




            }
            catch (Exception exc)
            {
                result = null;
                throw exc;
            }


            return result;
        }

        public static string GetDirectionUrl(string origin, List<string> destinations)
        {
            string result = null;

            var fn = new FileInfo("ResultPage.htm");

            var respath = "file:///" + fn.FullName;

            var querystring = "start=" + origin;
            querystring += "&end=" + destinations.Last();

            if (destinations.Count > 1)
            {
                querystring += "&wps=";
                for (int i = 0; i < destinations.Count - 1; i++)
                    querystring += destinations[i] + ";";

                querystring = querystring.Remove(querystring.Length - 1, 1);
            }

            result = respath + "?" + querystring;

            return result;
        }

        //public static Image GetMapImage(string origin, List<string> destinations)
        //{
        //    Image img = null;

        //    string wayps = "";
        //    foreach (var itm in destinations)
        //        wayps += "|" + itm;

        //    string url = "http://maps.googleapis.com/maps/api/staticmap?size=512x512&markers=size:tiny|color:blue|" + origin + wayps + "&sensor=false";


        //    WebClient wc = new WebClient();

        //    var bdat = wc.DownloadData(url);

        //    using (MemoryStream ms = new MemoryStream(bdat)) 
        //    {
        //        img = Image.FromStream(ms);
        //        ms.Close();
        //    }

        //    return img;
        //}

        public static BitmapImage GetMapImage(string origin, List<string> destinations, string username, string password, string domain, string prxyurl)
        {

            string wayps = "";
            foreach (var itm in destinations)
                wayps += "|" + itm;

            string url = "http://maps.googleapis.com/maps/api/staticmap?size=512x512&markers=color:blue|" + origin + wayps + "&sensor=false";



            WebClient wc = new WebClient();

            WebProxy wp = new WebProxy();

            if (!string.IsNullOrEmpty(prxyurl))
            {
                wp.Address = new Uri(prxyurl);
            }
            else
            {
                wp = WebProxy.GetDefaultProxy();
            }


            wc.Proxy = wp;

            wc.Proxy.Credentials = new NetworkCredential(username, password, domain);

            var bdat = wc.DownloadData(url);

            return LoadImage(bdat);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            //image.Freeze();
            return image;
        }

    }
}
