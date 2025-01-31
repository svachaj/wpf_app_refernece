using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace COS.Resources.Localization
{
    public class CustomLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            switch (key)
            {
                #region GRIDVIEW
                case "GridViewAlwaysVisibleNewRow":
                    return ResourceHelper.GetResource<string>("GridViewAlwaysVisibleNewRow");
                case "GridViewClearFilter":
                    return ResourceHelper.GetResource<string>("GridViewClearFilter");
                case "GridViewFilter":
                    return ResourceHelper.GetResource<string>("GridViewFilter");
                case "GridViewFilterAnd":
                    return ResourceHelper.GetResource<string>("GridViewFilterAnd");
                case "GridViewFilterContains":
                    return ResourceHelper.GetResource<string>("GridViewFilterContains");
                case "GridViewFilterDoesNotContain":
                    return ResourceHelper.GetResource<string>("GridViewFilterDoesNotContain");
                case "GridViewFilterEndsWith":
                    return ResourceHelper.GetResource<string>("GridViewFilterEndsWith");
                case "GridViewFilterIsContainedIn":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsContainedIn");
                case "GridViewFilterIsEqualTo":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsEqualTo");
                case "GridViewFilterIsGreaterThan":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsGreaterThan");
                case "GridViewFilterIsGreaterThanOrEqualTo":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsGreaterThanOrEqualTo");
                case "GridViewFilterIsNotContainedIn":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsNotContainedIn");
                case "GridViewFilterIsLessThan":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsLessThan");
                case "GridViewFilterIsLessThanOrEqualTo":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsLessThanOrEqualTo");
                case "GridViewFilterIsNotEqualTo":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsNotEqualTo");
                case "GridViewFilterMatchCase":
                    return ResourceHelper.GetResource<string>("GridViewFilterMatchCase");
                case "GridViewFilterOr":
                    return ResourceHelper.GetResource<string>("GridViewFilterOr");
                case "GridViewFilterSelectAll":
                    return ResourceHelper.GetResource<string>("GridViewFilterSelectAll");
                case "GridViewFilterShowRowsWithValueThat":
                    return ResourceHelper.GetResource<string>("GridViewFilterShowRowsWithValueThat");
                case "GridViewFilterStartsWith":
                    return ResourceHelper.GetResource<string>("GridViewFilterStartsWith");
                case "GridViewFilterIsNull":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsNull");
                case "GridViewFilterIsNotNull":
                    return ResourceHelper.GetResource<string>("GridViewFilterIsNotNull");
                case "GridViewGroupPanelText":
                    return ResourceHelper.GetResource<string>("GridViewGroupPanelText");
                case "GridViewGroupPanelTopText":
                    return ResourceHelper.GetResource<string>("GridViewGroupPanelTopText");
                case "GridViewGroupPanelTopTextGrouped":
                    return ResourceHelper.GetResource<string>("GridViewGroupPanelTopTextGrouped");
                #endregion

                #region PANE
                case "Hide":
                    return ResourceHelper.GetResource<string>("Hide");
                case "Auto_hide":
                    return ResourceHelper.GetResource<string>("Auto_hide");
                case "Floating":
                    return ResourceHelper.GetResource<string>("Floating");
                case "Dockable":
                    return ResourceHelper.GetResource<string>("Dockable");
                case "Tabbed_document":
                    return ResourceHelper.GetResource<string>("Tabbed_document");
                #endregion

                #region PAGER
                case "RadDataPagerEllipsisString":
                    return ResourceHelper.GetResource<string>("RadDataPagerEllipsisString");
                case "RadDataPagerOf":
                    return ResourceHelper.GetResource<string>("RadDataPagerOf");
                case "RadDataPagerPage":
                    return ResourceHelper.GetResource<string>("RadDataPagerPage");
                #endregion



            }
            return base.GetStringOverride(key);
        }
    }
}
