using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public class StandardExportItem
    {
        public StandardExportItem() { }

        public string ItemNumber { set; get; }
        public string WorkCenter { set; get; }
        public string WorkGroup { set; get; }

        public bool IsConfig { set; get; }
        public int Labour { set; get; }
        public int Weight_Kg { set; get; }
        public int SetupTime_mm { set; get; }
        public int PcsPerMinute { set; get; }
        public int PcsPerHour { set; get; }





    }
}
