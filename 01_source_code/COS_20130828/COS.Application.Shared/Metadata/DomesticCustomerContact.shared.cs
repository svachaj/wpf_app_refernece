using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class DomesticCustomerContact
    {

        public string DisplayName
        {
            get
            {
                string result = null;

                result = this.Surname;
                result += " ";
                result += this.Name;

                if (this.ContactType != null)
                {
                    result += " - ";
                    result += this.ContactType.Description;
                }

                if (!string.IsNullOrEmpty(this.PhoneNumber))
                {
                    result += " (";
                    result += this.PhoneNumber;
                    result += ")";
                }

                return result;
            }
        }

        
    }
}
