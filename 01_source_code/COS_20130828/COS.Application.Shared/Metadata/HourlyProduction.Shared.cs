using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace COS.Application.Shared
{
    public partial class HourlyProduction
    {
        public List<ProductionAsset> Assets
        {
            get
            {
                return COSContext.Current.ProductionAssets.Where(aa => aa.ID_HP == this.ID_HP).ToList();
            }
        }

        public List<ProductionHRResource> Operators
        {
            get
            {
                return COSContext.Current.ProductionHRResources.Where(aa => aa.ID_HP == this.ID_HP).ToList();
            }
        }

    }
}
