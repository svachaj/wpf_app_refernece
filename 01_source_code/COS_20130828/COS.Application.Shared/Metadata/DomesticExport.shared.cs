using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace COS.Application.Shared
{
    public partial class DomesticExport
    {

        public decimal VIVolumeCBM
        {
            get
            {
                decimal result = 0;
                try
                {
                    foreach (var itm in this.ExportDetails.Where(a => a.Customer != null && a.Customer.CustomerName == "VI - Comfort"))
                    {
                        result += itm.VolumeCBM;
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
                    foreach (var itm in this.ExportDetails.Where(a => a.Customer != null && a.Customer.CustomerName == "VA - ADS"))
                    {
                        result += itm.VolumeCBM;
                    }
                }
                catch { }

                return result;
            }
        }

        public string DestinationsString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.DestinationOrder))
                {
                    if (itm.Destination != null)
                    {
                        result += itm.Destination.DestinationName;
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += "; ";
                    }
                }

                return result;
            }
        }

        public string DetailsCustomersString 
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.DestinationOrder))
                {
                    if (itm.Customer != null)
                    {
                        result += itm.Customer.CustomerName;
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

                return result;
            }
        }

        public string DetailsDestinationsString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a=>a.DestinationOrder))
                {
                    if (itm.Destination != null)
                    {
                        result += itm.Destination.DestinationName;
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

                return result;
            }
        }

        public string DetailsTrpSosString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.DestinationOrder))
                {
                    if (itm.TrpSos != null)
                    {
                        result += itm.TrpSosString;
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

                return result;
            }
        }

        public string DetailsTrpString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.DestinationOrder))
                {
                    if (!itm.TRP.IsNullOrEmptyString())
                    {
                        result += itm.TRP;
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

                return result;
            }
        }


        public string DetailsDeliveryTimeString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.DestinationOrder))
                {
                    if (itm.DeliveryTime != null)
                    {
                        result += itm.DeliveryTime.Value.ToShortTimeString();
                        if (itm != this.ExportDetails.OrderBy(a => a.DestinationOrder).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

                return result;
            }
        }


        public string AddressString
        {
            get
            {
                string result = "";

                foreach (var itm in this.ExportDetails.OrderBy(a => a.Destination.CityAndStreet))
                {
                    if (!itm.Destination.CityAndStreet.IsNullOrEmptyString())
                    {
                        result += itm.Destination.CityAndStreet;
                        if (itm != this.ExportDetails.OrderBy(a => a.Destination.CityAndStreet).LastOrDefault())
                            result += Environment.NewLine;
                    }
                }

             
                return result;
            }
        }


        partial void OnID_driverChanging(Nullable<global::System.Int32> value)
        {
            //if (!value.HasValue)
            //    throw new ValidationException("Toto pole je poviné");
        }

        public void RefreshBinding(string prop) 
        {
            OnPropertyChanged(prop);
        }
       
    }
}
