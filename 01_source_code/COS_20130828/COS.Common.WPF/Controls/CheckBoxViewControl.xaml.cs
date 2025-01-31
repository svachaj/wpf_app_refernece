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

namespace COS.Common.WPF.Controls
{
    /// <summary>
    /// Interaction logic for CheckBoxViewControl.xaml
    /// </summary>
    public partial class CheckBoxViewControl : UserControl, INotifyPropertyChanged
    {
        public CheckBoxViewControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsCheckedProperty =
  DependencyProperty.Register("IsChecked", typeof(bool?), typeof(CheckBoxViewControl), new PropertyMetadata(false, OnIsCheckedPropertyChanged));


        public bool? IsChecked
        {
            set
            {
                SetValue(IsCheckedProperty, value);
            }
            get
            {
                return (bool?)GetValue(IsCheckedProperty);
            }
        }

        private static void OnIsCheckedPropertyChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs e)
        {
            CheckBoxViewControl myUserControl = dependencyObject as CheckBoxViewControl;
            myUserControl.OnPropertyChanged("IsChecked");
            myUserControl.OnIsCheckedPropertyChanged(e);
        }

        private void OnIsCheckedPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsChecked.HasValue && IsChecked.Value)
            {
                imgMain.Visibility = System.Windows.Visibility.Visible;
                imgMain.Width = 14;
            }
            else
            {
                imgMain.Visibility = System.Windows.Visibility.Collapsed;
                imgMain.Width = 0;
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
