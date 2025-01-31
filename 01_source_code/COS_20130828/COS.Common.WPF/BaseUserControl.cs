using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using COS.Application.Shared;
using Telerik.Windows.Controls;

namespace COS.Common.WPF
{
    public class BaseUserControl : UserControl
    {
        public BaseUserControl()
            : base()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Loaded += new System.Windows.RoutedEventHandler(BaseUserControl_Loaded);
                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                LocalizationManager.Manager = new COS.Resources.Localization.CustomLocalizationManager();

                this.Language = System.Windows.Markup.XmlLanguage.GetLanguage(COSContext.Current.Language);
            }
        }

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language") 
            {
                Helpers.LoadLocalizeResources(this);
            }
        }

        void BaseUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Helpers.LoadLocalizeResources(this);

            Helpers.ApplyAllRights(this);

            RefreshData();
        }

        public virtual void RefreshData()
        {
            //přepsat v příslušním view pro reload dat
        }
    }
}
