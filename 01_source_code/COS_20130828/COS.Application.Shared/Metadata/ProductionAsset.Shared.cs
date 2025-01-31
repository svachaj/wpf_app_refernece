using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace COS.Application.Shared
{
    public partial class ProductionAsset
    {
        public HourlyProduction GetProduction(List<HourlyProduction> productions)
        {
            return productions.FirstOrDefault(a => a.ID_HP == this.ID_HP);
        }


    }
}
