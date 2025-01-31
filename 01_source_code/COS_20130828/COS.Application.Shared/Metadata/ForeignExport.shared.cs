using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class ForeignExport
    {

        public decimal VIVolumeCBM
        {
            get
            {
                decimal result = 0;
                try
                {
                    foreach (var conn in this.Connections)
                    {
                        foreach (var itm in conn.ExportDetails.Where(a => a.OrderedBy != null && a.OrderedBy.CustomerName == "VI - Comfort"))
                        {
                            result += itm.VolumeCbm;
                        }
                    }
                }
                catch { }

                return result;
            }
        }

        public decimal VAVolumeCBM
        {
            get
            {
                decimal result = 0;
                try
                {
                    foreach (var conn in this.Connections)
                    {
                        foreach (var itm in conn.ExportDetails.Where(a => a.OrderedBy != null && a.OrderedBy.CustomerName == "VA - ADS"))
                        {
                            result += itm.VolumeCbm;
                        }
                    }
                }
                catch { }

                return result;
            }
        }

        public decimal VA4HVolumeCBM
        {
            get
            {
                decimal result = 0;
                try
                {
                    foreach (var conn in this.Connections)
                    {
                        foreach (var itm in conn.ExportDetails.Where(a => a.OrderedBy != null && a.OrderedBy.CustomerName == "VA - 4H"))
                        {
                            result += itm.VolumeCbm;
                        }
                    }
                }
                catch { }

                return result;
            }
        }

        public decimal VAinc4HVolumeCBM
        {
            get
            {
                decimal result = 0;
                try
                {
                    foreach (var conn in this.Connections)
                    {
                        foreach (var itm in conn.ExportDetails.Where(a => a.OrderedBy != null && (a.OrderedBy.CustomerName == "VA - ADS" || a.OrderedBy.CustomerName == "VA - 4H")))
                        {
                            result += itm.VolumeCbm;
                        }
                    }
                }
                catch { }

                return result;
            }
        }

    }
}
