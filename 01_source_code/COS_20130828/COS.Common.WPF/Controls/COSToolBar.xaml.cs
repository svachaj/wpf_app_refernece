using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for COSToolBar.xaml
    /// </summary>
    public partial class COSToolBar : RadToolBar
    {
        public COSToolBar()
            : base()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(COSToolBar_Loaded);

        }

        void COSToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SubmoduleName))
            {
                if (InsertButtonVisibility == System.Windows.Visibility.Visible)
                {
                    if (!Helpers.HasRightForOperation(SubmoduleName, "Insert"))
                        InsertButtonVisibility = System.Windows.Visibility.Collapsed;
                }

                if (UpdateButtonVisibility == System.Windows.Visibility.Visible)
                {
                    if (!Helpers.HasRightForOperation(SubmoduleName, "Update"))
                        UpdateButtonVisibility = System.Windows.Visibility.Collapsed;
                }

                if (DeleteButtonVisibility == System.Windows.Visibility.Visible)
                {
                    if (!Helpers.HasRightForOperation(SubmoduleName, "Delete"))
                        DeleteButtonVisibility = System.Windows.Visibility.Collapsed;
                }


            }

        }

        public string SubmoduleName { set; get; }


        public Visibility InsertButtonVisibility
        {
            set
            {
                btnInsert.Visibility = value;
            }
            get
            {
                return btnInsert.Visibility;
            }
        }
        public Visibility UpdateButtonVisibility
        {
            set
            {
                btnUpdate.Visibility = value;
            }
            get
            {
                return btnUpdate.Visibility;
            }
        }
        public Visibility DeleteButtonVisibility
        {
            set
            {
                btnDelete.Visibility = value;
            }
            get
            {
                return btnDelete.Visibility;
            }
        }
        public Visibility CancelButtonVisibility
        {
            set
            {
                btnCancel.Visibility = value;
            }
            get
            {
                return btnCancel.Visibility;
            }
        }

        public bool UpdateButtonEnabledForce
        {
            set
            {
                btnUpdate.IsEnabled = value;
            }
            get
            {
                return btnUpdate.IsEnabled;
            }
        }
    }

    public class EditModeEnabledConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = false;

            if (value != null)
            {
                EditMode mode = (EditMode)value;

                switch (mode)
                {
                    case EditMode.View:
                        if (parameter == null)
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "insert")
                        {
                            result = true;
                        }
                        else if (parameter.ToString().ToLower() == "update")
                        {                           
                                result = false;
                        }
                        else if (parameter.ToString().ToLower() == "delete")
                        {
                            result = true;
                        }
                        else if (parameter.ToString().ToLower() == "cancel")
                        {
                            result = false;
                        }
                        break;
                    case EditMode.New:
                        if (parameter == null)
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "insert")
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "update")
                        {
                            result = true;
                        }
                        else if (parameter.ToString().ToLower() == "delete")
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "cancel")
                        {
                            result = true;
                        }
                        break;
                    case EditMode.Edit:
                        if (parameter == null)
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "insert")
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "update")
                        {
                            result = true;
                        }
                        else if (parameter.ToString().ToLower() == "delete")
                        {
                            result = false;
                        }
                        else if (parameter.ToString().ToLower() == "cancel")
                        {
                            result = true;
                        }
                        break;
                    case EditMode.AllMode:
                        result = true;
                        break;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
