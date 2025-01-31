using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class Forwarder
    {
        public override string ToString()
        {
            return this.Name ;
        }


        public string ForwarderToEmails 
        {
            get 
            {
                var result = "";

                var emails = this.t_log_forwarderEmails.Where(a => a.EmailType.TypeName == "Sender");
                               

                foreach (var itm in emails)
                    result += itm.Email + "; ";

                result = result.Trim(' ', ';');

                return result;
            }
        }

        public void RefreshForwarderToEmails() 
        {
            OnPropertyChanged("ForwarderToEmails");
        }

        public string RecieptEmails
        {
            get
            {
                var result = "";

                var emails = this.t_log_forwarderEmails.Where(a => a.EmailType.TypeName == "Reciept");


                foreach (var itm in emails)
                    result += itm.Email + "; ";

                result = result.Trim(' ', ';');

                return result;
            }
        }

        public void RefreshRecieptEmails()
        {
            OnPropertyChanged("RecieptEmails");
        }
    }
}
