using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class ForeignExportDetail
    {

        public string TransportPaymentString
        {
            get
            {
                return this.TransportPayment.Description;
            }
        }

        public void RefreshTransportPayment()
        {
            OnPropertyChanged("TransportPaymentString");
        }


        public string OrderedByString
        {
            get
            {
                return this.OrderedBy.CustomerName;
            }
        }

        public void RefreshOrderedBy()
        {
            OnPropertyChanged("OrderedByString");
        }

        public string CustomerNumber
        {
            get
            {
                string result = "";

                if (this.Connection != null && this.Connection.Destination != null)
                {
                    var r = this.Connection.Destination.t_log_foreignExport_Zone_CustomerNumber.FirstOrDefault(a => a.ID_OrderedBy == this.ID_OrderedBy);

                    if (r != null)
                        result = r.cNumber;
                }

                return result;
            }
        }

    }
}
