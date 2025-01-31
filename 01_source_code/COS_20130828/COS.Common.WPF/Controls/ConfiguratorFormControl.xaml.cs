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
using Telerik.Windows.Controls;

namespace COS.Application.Controls
{
    /// <summary>
    /// Interaction logic for ConfiguratorFormControl.xaml
    /// </summary>
    public partial class ConfiguratorFormControl : UserControl, INotifyPropertyChanged
    {
        public ConfiguratorFormControl(Configurator config)
        {
            InitializeComponent();

            MyConfigurator = config;

            this.DataContext = this;

            this.Loaded += new RoutedEventHandler(ConfiguratorFormControl_Loaded);
        }

        private double? _result = null;
        public double? Result
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

        void ConfiguratorFormControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeControls();
        }

        private void InitializeControls()
        {
            cnvMainCanvas.Children.Clear();

            FormItemControl itemControl = null;

            if (MyConfigurator != null)
            {
                foreach (var item in MyConfigurator.FormItems)
                {
                    if (item.Input.SystemType != "Constant")
                    {
                        itemControl = new FormItemControl(item, cnvMainCanvas);
                        itemControl.IsEditable = true;
                        itemControl.IsFixed = true;

                        cnvMainCanvas.Children.Add(itemControl);
                    }
                }
            }
        }

        public Configurator MyConfigurator { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var propChanged = this.PropertyChanged;
            if (propChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dotMath.EqCompiler compiler = new dotMath.EqCompiler(true);
                compiler.SetFunction(MyConfigurator.Formula.ToLower());

                foreach (var itm in MyConfigurator.FormItems.Where(a => a.Input.SystemType == "System.Double" || a.Input.SystemType == "Constant" || a.Input.SystemType == "List"))
                {
                    compiler.SetVariable(itm.Name.ToLower(), itm.DoubleValue.HasValue ? itm.DoubleValue.Value : 0);
                }

                var res = Math.Round(compiler.Calculate(), 2);

                tbxResult.Text = res.ToString();
            }
            catch (Exception exc)
            {
                tbxResult.Text = exc.Message;
            }
        }

        private void ButtonUse_click(object sender, RoutedEventArgs e)
        {
            try
            {
                dotMath.EqCompiler compiler = new dotMath.EqCompiler(true);
                compiler.SetFunction(MyConfigurator.Formula.ToLower());

                foreach (var itm in MyConfigurator.FormItems.Where(a => a.Input.SystemType == "System.Double" || a.Input.SystemType == "Constant" || a.Input.SystemType == "List"))
                {
                    compiler.SetVariable(itm.Name.ToLower(), itm.DoubleValue.HasValue ? itm.DoubleValue.Value : 0);
                }

                Result = Math.Round(compiler.Calculate(), 2);

                RadWindow wnd = this.ParentOfType<RadWindow>();

                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Visibility = System.Windows.Visibility.Collapsed;
                    wnd.Close();
                }

            }
            catch (Exception exc)
            {
                tbxResult.Text = exc.Message;
            }
        }

        private void ButtonCancel_click(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("Canceled");

            RadWindow wnd = this.ParentOfType<RadWindow>();

            if (wnd != null)
            {
                wnd.DialogResult = false;
                wnd.Visibility = System.Windows.Visibility.Collapsed;
                wnd.Close();
            }

        }


    }

}
