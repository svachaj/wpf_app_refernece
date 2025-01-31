using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class DomesticDestination
    {

        public string DistanceComputeString
        {
            get
            {
                string result = null;

                if (!string.IsNullOrEmpty(this.GPS))
                    result = this.GPS;
                else 
                {
                    if (!string.IsNullOrEmpty(this.City))
                        result = this.City;

                    if (!string.IsNullOrEmpty(this.Street))
                    {
                        if (!string.IsNullOrEmpty(result))
                            result += "+";

                        result += this.Street;
                    }                     
                }

                return result;
            }
        }

        public string CityAndStreet 
        {
            get 
            {
                string result = "";

                result = this.City;

                if (!string.IsNullOrEmpty(this.Street))
                    result += ", " + this.Street;

                return result;
            }
        }


        public override string ToString()
        {
            return this.DestinationName;
        }
               
    }
}
