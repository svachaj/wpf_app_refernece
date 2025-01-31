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
using COS.Application.Shared;
using Telerik.Windows.Controls;
using System.ComponentModel;

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for MatrixConfiguratorGrid.xaml
    /// </summary>
    public partial class MatrixConfiguratorGrid : UserControl, INotifyPropertyChanged
    {
        public MatrixConfiguratorGrid()
        {
            InitializeComponent();

            this.DataContext = this;

            
        }

        private void canvasMain_MouseMove(object sender, MouseEventArgs e)
        {
            var posY = (int)(e.GetPosition(canvasMain).Y / 30) * 30;

            Canvas.SetTop(rectMovingHorizontal, posY);

            var posX = (int)(e.GetPosition(canvasMain).X / 50) * 50;

            Canvas.SetLeft(rectMovingVertical, posX);
        }

        List<ConfiguratorFormItem> FormItems { set; get; }

        public void GenerateControls(List<ConfiguratorFormItem> formItems)
        {
            FormItems = formItems;

            canvasMain.Children.RemoveRange(4, canvasMain.Children.Count - 4);

            rectSelectedVertical.Visibility = System.Windows.Visibility.Collapsed;
            rectSelectedHorizontal.Visibility = System.Windows.Visibility.Collapsed;

            MatrixConfiguratorItemControl item = null;
            if (FormItems != null)
            {
                foreach (var itm in FormItems)
                {
                    item = new MatrixConfiguratorItemControl();
                    item.MouseLeftButtonUp += new MouseButtonEventHandler(item_MouseLeftButtonUp);
                    item.DataItem = itm;

                    if (itm.DecimalValue.HasValue)
                    {
                        item.IsHeader = false;
                        item.ItemText = itm.DecimalValue.Value.ToString();
                        item.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        item.IsHeader = true;
                        item.ItemText = itm.StringValue;
                    }
                    canvasMain.Children.Add(item);
                    Canvas.SetTop(item, (double)(itm.TopPosition * 30));
                    Canvas.SetLeft(item, (double)(itm.LeftPosition * 50));
                }

                if (FormItems.Count > 0)
                {
                    canvasMain.MinWidth = FormItems.Max(a => a.LeftPosition).Value * 50 + 54;
                    canvasMain.MinHeight = FormItems.Max(a => a.TopPosition).Value * 30 + 34;
                }
            }
        }

        private bool _isComputeMode = false;
        public bool IsComputeMode
        {
            set
            {
                _isComputeMode = value;
                if (_isComputeMode)
                {
                    buttonStack.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    buttonStack.Visibility = System.Windows.Visibility.Collapsed;
            }
            get
            {
                return _isComputeMode;
            }
        }

        private ConfiguratorFormItem _result = null;
        public ConfiguratorFormItem Result
        {
            set
            {
                if (_result != value)
                {
                    _result = value;
                    OnPropertyChanged("Result");
                }
            }
            get
            {
                return _result;
            }
        }

        public ConfiguratorFormItem TempResult
        { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        void item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MatrixConfiguratorItemControl item = sender as MatrixConfiguratorItemControl;

            if (item != null)
            {
                if (item.DataItem != null)
                {
                    if (item.DataItem.DecimalValue.HasValue)
                    {
                        if (IsComputeMode)
                        {
                            TempResult = item.DataItem;
                        }
                        else
                        {
                            string res = COS.Resources.ResourceHelper.GetResource<string>("eng_ConfMatPcs") + " " + item.DataItem.DecimalValue.Value.ToString();
                            res += Environment.NewLine;
                            res += COS.Resources.ResourceHelper.GetResource<string>("eng_ConfMatOps") + " " + item.DataItem.Labours.Value.ToString();
                            res += Environment.NewLine;
                            res += COS.Resources.ResourceHelper.GetResource<string>("eng_ConfMatSetup") + " " + item.DataItem.SetupTime.Value.ToString();

                            RadWindow.Alert(new DialogParameters() { Content = res, Header = COS.Resources.ResourceHelper.GetResource<string>("eng_ConfMatResult"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                        }

                        SelectWithCross(item.DataItem);
                    }
                }
            }
        }

        private void SelectWithCross(ConfiguratorFormItem configuratorFormItem)
        {
            if (configuratorFormItem != null)
            {
                var posY = configuratorFormItem.LeftPosition * 50;

                Canvas.SetLeft(rectSelectedVertical, (int)posY);

                var posX = configuratorFormItem.TopPosition * 30;

                Canvas.SetTop(rectSelectedHorizontal, (int)posX);

                rectSelectedVertical.Visibility = System.Windows.Visibility.Visible;
                rectSelectedHorizontal.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempResult != null)
            {
                Result = TempResult;
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = COS.Resources.ResourceHelper.GetResource<string>("m_Body_ENG00000022"), Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
        }
    }
}
