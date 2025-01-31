using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class TpmCheckList
    {
        public TpmCheckList Copy()
        {
            TpmCheckList newitem = new TpmCheckList();

            newitem.IsPreImage = false;
            newitem.CheckListName = this.CheckListName;
            newitem.TpmPlannedlabour = this.TpmPlannedlabour;
            newitem.TpmPlannedTime = this.TpmPlannedTime;
            newitem.Description = this.Description;

            foreach (var itm in this.Items)
                newitem.Items.Add(new TpmCheckList_CheckListItem() { CheckList = newitem, CheckListItems = itm.CheckListItems });

            return newitem;
        }
    }
}
