using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Windows.Controls
{
    public static class TelerikExtensions
    {
        public static void ClearAllColumnFilters(this RadGridView rgv)
        {
            foreach (var itm in rgv.Columns)
                itm.ClearFilters();
        }
    }
}
