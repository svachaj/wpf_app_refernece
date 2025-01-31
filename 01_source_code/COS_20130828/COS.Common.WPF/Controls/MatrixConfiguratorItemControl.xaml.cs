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
using System.ComponentModel;
using COS.Application.Shared;

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for MatrixConfiguratorItemControl.xaml
    /// </summary>
    public partial class MatrixConfiguratorItemControl : UserControl, INotifyPropertyChanged
    {
        public MatrixConfiguratorItemControl()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public ConfiguratorFormItem DataItem { set; get; }

        private string _ItemText = null;
        public string ItemText
        {
            set
            {
                if (_ItemText != value)
                {
                    _ItemText = value;
                    if (_ItemText.Length > 9)
                        tblMain.FontSize = 9;
                    else if (_ItemText.Length > 7)
                        tblMain.FontSize = 9.5;
                    else if (_ItemText.Length > 4)
                        tblMain.FontSize = 11;
                    else
                        tblMain.FontSize = 12;

                    OnPropertyChanged("ItemText");
                }
            }
            get
            {
                return _ItemText;
            }
        }

        private bool _isHeader = false;
        public bool IsHeader
        {
            set
            {
                if (_isHeader != value)
                {
                    _isHeader = value;
                    if (_isHeader)
                    {
                        tblMain.FontWeight = FontWeights.Bold;
                        borderMain.BorderBrush = Brushes.White;
                    }
                    else
                    {
                        tblMain.FontWeight = FontWeights.Normal;
                        borderMain.BorderBrush = Brushes.Transparent;
                    }

                    OnPropertyChanged("IsHeader");
                }
            }
            get
            {
                return _isHeader;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
