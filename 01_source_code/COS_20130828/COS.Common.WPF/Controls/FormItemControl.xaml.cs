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
using COS.Common.WPF.Controls;

namespace COS.Application.Controls
{
    /// <summary>
    /// Interaction logic for FormItemControl.xaml
    /// </summary>
    public partial class FormItemControl : UserControl, INotifyPropertyChanged
    {
        public FormItemControl(ConfiguratorFormItem item, Canvas canvas)
        {
            InitializeComponent();

            FormItem = item;
            this.DataContext = this;

            MainCanvas = canvas;

            this.Loaded += new RoutedEventHandler(FormItemControl_Loaded);
        }

        public Canvas MainCanvas { set; get; }




        private bool _isEditable = false;
        public bool IsEditable
        {
            set
            {
                if (_isEditable != value)
                {
                    _isEditable = value;
                    OnPropertyChanged("IsEditable");
                }
            }
            get
            {
                return _isEditable;
            }
        }

        private Brush _mainBorderBrush = Brushes.Transparent;
        public Brush MainBorderBrush
        {
            set
            {
                if (_mainBorderBrush != value)
                {
                    _mainBorderBrush = value;
                    OnPropertyChanged("MainBorderBrush");
                }
            }
            get
            {
                return _mainBorderBrush;
            }
        }

        public bool IsFixed { set; get; }

        void FormItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCurrentControl();

            if (IsFixed)
                this.mainBorder.Cursor = Cursors.Arrow;
        }

        private void InitializeCurrentControl()
        {
            if (FormItem != null && FormItem.Input != null)
            {
                if (FormItem.Input.SystemType == "List")
                {
                    RadComboBox cmb = new RadComboBox();
                    StyleManager.SetTheme(cmb, new Expression_DarkTheme());

                    if (!string.IsNullOrEmpty(FormItem.ListValues))
                    {
                        var items = FormItem.ListValues.Split(';');

                        double value = 0;
                        foreach (var itm in items)
                        {
                            if (double.TryParse(itm, out value))
                            {
                                cmb.Items.Add(value);
                            }
                        }
                    }

                    Binding bind = new Binding("FormItem.DoubleValue");
                    bind.Mode = BindingMode.TwoWay;
                    cmb.SetBinding(RadComboBox.SelectedValueProperty, bind);


                    bind = new Binding("FormItem.Width");
                    bind.Mode = BindingMode.TwoWay;
                    cmb.SetBinding(RadNumericUpDown.WidthProperty, bind);


                    bind = new Binding("FormItem.Height");
                    bind.Mode = BindingMode.TwoWay;
                    cmb.SetBinding(RadNumericUpDown.HeightProperty, bind);


                    mainGrid.Children.Add(cmb);

                }
                else if (FormItem.Input.SystemType == "System.String")
                {
                    TextBox tb = new TextBox();
                    Binding bind = new Binding("FormItem.StringValue");
                    bind.Mode = BindingMode.TwoWay;

                    tb.SetBinding(TextBox.TextProperty, bind);

                    StyleManager.SetTheme(tb, new Expression_DarkTheme());

                    bind = new Binding("FormItem.Width");
                    bind.Mode = BindingMode.TwoWay;
                    tb.SetBinding(RadNumericUpDown.WidthProperty, bind);


                    bind = new Binding("FormItem.Height");
                    bind.Mode = BindingMode.TwoWay;
                    tb.SetBinding(RadNumericUpDown.HeightProperty, bind);


                    mainGrid.Children.Add(tb);

                }
                else if (FormItem.Input.SystemType == "System.Int32")
                {

                }
                else if (FormItem.Input.SystemType == "Label")
                {
                    TextBlock tbl = new TextBlock();
                    tbl.TextWrapping = TextWrapping.Wrap;
                    tbl.Foreground = new SolidColorBrush(Colors.White);
                    Binding bind = new Binding("FormItem.Text");
                    bind.Mode = BindingMode.TwoWay;
                    tbl.SetBinding(TextBlock.TextProperty, bind);


                    bind = new Binding("FormItem.Width");
                    bind.Mode = BindingMode.TwoWay;
                    tbl.SetBinding(RadNumericUpDown.WidthProperty, bind);


                    bind = new Binding("FormItem.Height");
                    bind.Mode = BindingMode.TwoWay;
                    tbl.SetBinding(RadNumericUpDown.HeightProperty, bind);


                    mainGrid.Children.Add(tbl);
                }
                else if (FormItem.Input.SystemType == "System.Double")
                {
                    RadNumericUpDown num = new RadNumericUpDown();
                    StyleManager.SetTheme(num, new Expression_DarkTheme());
                    Binding bind = new Binding("FormItem.DoubleValue");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.ValueProperty, bind);


                    bind = new Binding("FormItem.Width");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.WidthProperty, bind);


                    bind = new Binding("FormItem.Height");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.HeightProperty, bind);


                    mainGrid.Children.Add(num);

                }
                else if (FormItem.Input.SystemType == "Constant")
                {


                    RadNumericUpDown num = new RadNumericUpDown();
                    StyleManager.SetTheme(num, new Expression_DarkTheme());
                    num.Margin = new Thickness(20, 10, 0, 0);
                    num.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                    Binding bind = new Binding("FormItem.DoubleValue");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.ValueProperty, bind);

                    TextBlock tbl = new TextBlock();
                    bind = new Binding("FormItem.Name");
                    bind.Mode = BindingMode.TwoWay;
                    tbl.SetBinding(TextBlock.TextProperty, bind);
                    tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    tbl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    tbl.FontSize = 9;
                    tbl.Margin = new Thickness(20, -1, 2, 1);
                    tbl.Foreground = new SolidColorBrush(Colors.Black);
                    mainGrid.Children.Add(tbl);
                    mainGrid.Background = new SolidColorBrush(Colors.White);

                    bind = new Binding("FormItem.Width");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.WidthProperty, bind);


                    bind = new Binding("FormItem.Height");
                    bind.Mode = BindingMode.TwoWay;
                    num.SetBinding(RadNumericUpDown.HeightProperty, bind);

                    Image img = new Image();
                    var uriSource = new Uri(@"/COSResources;component/Images/allsize1.png", UriKind.Relative);
                    img.Source = new BitmapImage(uriSource);
                    img.Width = 18;
                    img.Height = 18;
                    img.Margin = new Thickness(1, 0, 0, 0);
                    img.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    mainGrid.Children.Add(img);
                                     

                    mainGrid.Children.Add(num);
                    this.IsEditable = true;
                }
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed && gridPressed && !IsFixed)
            //{

            //    var pos = e.GetPosition(MainCanvas);

            //    Canvas.SetLeft(this, Math.Round(pos.X - grdConstX, 2));
            //    Canvas.SetTop(this, Math.Round(pos.Y - grdConstY, 2));

            //}
        }

        public double grdConstX = 0;
        public double grdConstY = 0;

        public bool gridPressed = false;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsFixed)
            {
                gridPressed = true;
                var grdPos = e.GetPosition(this);

                grdConstX = grdPos.X;
                grdConstY = grdPos.Y;


            }
        }


        private bool _isSelected = false;
        public bool IsSelected
        {
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
            get
            {
                return _isSelected;
            }
        }

        private ConfiguratorFormItem _formItem = null;
        public ConfiguratorFormItem FormItem
        {
            set
            {
                if (_formItem != value)
                {
                    _formItem = value;
                    this.OnPropertyChanged("FormItem");
                }
            }
            get
            {
                return _formItem;
            }
        }


        private Type _controlType = null;
        public Type ControlType
        {
            get
            {
                return _controlType;
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

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Grid_mouseUp(object sender, MouseButtonEventArgs e)
        {

        }




    }
}
