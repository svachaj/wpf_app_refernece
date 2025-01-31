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
using COS.Common.WPF;
using System.ComponentModel;
using COS.Application.Shared;
using COS.Application.Engeneering.Models;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Windows.Threading;
using COS.Application.Controls;

namespace COS.Application.Engeneering.Views
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class ConfiguratorsView : BaseUserControl
    {
        public ConfiguratorsView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new ConfiguratorsViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

                Loaded += new RoutedEventHandler(TpmPlanOverviewView_Loaded);


            }
        }



        void TpmPlanOverviewView_Loaded(object sender, RoutedEventArgs e)
        {
            Model.EditingMode = Common.EditMode.AllMode;

            grvConfigs.SelectedItem = COSContext.Current.Configurators.FirstOrDefault();

            Line line = null;
            double cn = 20;
            for (int i = 0; i < 100; i++)
            {
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.StrokeThickness = 0.5;
                line.X1 = 0;
                line.Y1 = i * cn;
                line.X2 = 1920;
                line.Y2 = i * cn;
                gridLines.Children.Add(line);

                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.StrokeThickness = 0.5;
                line.Y1 = 0;
                line.X1 = i * cn;
                line.Y2 = 1920;
                line.X2 = i * cn;
                gridLines.Children.Add(line);
            }

            //var cnvs = cnvMainCanvas;
            //gridCanvas.Children.Remove(cnvMainCanvas);
            //gridCanvas.Children.Add(cnvs);

            //var tbles = tblEsc;
            //gridCanvas.Children.Remove(tbles);
            //gridCanvas.Children.Add(tbles);

            //var btnup = btnUpdate;
            //gridCanvas.Children.Remove(btnup);
            //gridCanvas.Children.Add(btnup);
        }

        Engeneering.Models.ConfiguratorsViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateDataCompleted")
            {
                grvConfigs.Rebind();
            }
            else if (e.PropertyName == "Errors")
            {
                RadWindow.Alert(new DialogParameters() { Content = Model.Errors, Header = "Oznámení", Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }
            else if (e.PropertyName == "InsertDataCompleted")
            {
                tbxName.Focus();
            }
            else if (e.PropertyName == "CancelDataCompleted")
            {
                grvConfigs.SelectedItem = COSContext.Current.Configurators.FirstOrDefault();
            }
            else if (e.PropertyName == "DeleteDataCompleted")
            {
                grvConfigs.Rebind();
                grvConfigs.SelectedItem = COSContext.Current.Configurators.FirstOrDefault();
            }
            else if (e.PropertyName == "SelectedItem")
            {

                GenerateFormItems();

            }

        }

        ConfiguratorFormControl ctrl = null;
        private void testFormula_click(object sender, RoutedEventArgs e)
        {
            RadWindow wnd = new RadWindow();

            if (Model.SelectedItem != null)
                wnd.Header = Model.SelectedItem.Name;

            StyleManager.SetTheme(wnd, new Expression_DarkTheme());

            wnd.Owner = (RadWindow)COSContext.Current.RadMainWindow;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;


            ctrl = new ConfiguratorFormControl(Model.SelectedItem);
            ctrl.PropertyChanged += new PropertyChangedEventHandler(ctrl_PropertyChanged);

            wnd.Content = ctrl;
            wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();


        }

        void ctrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                //MessageBox.Show(ctrl.Result.ToString());
            }
        }


        private void useInput_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                FormInput input = btn.DataContext as FormInput;

                if (input != null)
                {
                    ConfiguratorFormItem newItem = COSContext.Current.ConfiguratorFormItems.CreateObject();

                    var newName = input.Name + (Model.SelectedItem.FormItems.Where(a => a.ID_input == input.ID).Count() + 1).ToString();

                    newItem.Input = input;
                    newItem.Name = newName;
                    newItem.LeftPosition = 40;
                    newItem.TopPosition = 190;
                    newItem.Width = 130;
                    newItem.Height = 24;
                    newItem.ListValues = input.ListValues;

                    if (input.SystemType == "Constant")
                    {
                        newItem.IsConstant = true;
                    }

                    if (!string.IsNullOrEmpty(input.DefaultValue))
                    {
                        if (input.SystemType == "System.Double")
                        {
                            newItem.DoubleValue = double.Parse(input.DefaultValue);
                        }
                        else if (input.SystemType == "Constant")
                        {
                            double cons = 0;
                            if (double.TryParse(input.DefaultValue, out cons))
                                newItem.DoubleValue = cons;
                            else if (double.TryParse(input.DefaultValue.Replace(".", ","), out cons))
                                newItem.DoubleValue = cons;
                        }
                        else if (input.SystemType == "Label")
                        {
                            newItem.Text = input.DefaultValue;
                            newItem.Width = 80;
                            newItem.Height = 24;
                        }
                    }

                    COSContext.Current.ConfiguratorFormItems.AddObject(newItem);

                    Model.SelectedItem.FormItems.Add(newItem);

                    Model.SelectedFormItem = newItem;

                    GenerateFormItems();


                }
            }
        }


        public void GenerateFormItems()
        {
            cnvMainCanvas.Children.Clear();



            FormItemControl itemControl = null;
            if (Model.SelectedItem != null)
            {
                foreach (var item in Model.SelectedItem.FormItems)
                {
                    itemControl = new FormItemControl(item, cnvMainCanvas);

                    itemControl.MouseDown += new MouseButtonEventHandler(itemControl_MouseDown);
                    itemControl.KeyDown += new KeyEventHandler(itemControl_KeyDown);
                    itemControl.PreviewKeyDown += new KeyEventHandler(itemControl_PreviewKeyDown);
                    cnvMainCanvas.Children.Add(itemControl);
                }
            }
        }

        void itemControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        void itemControl_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Delete)
            //{
            //    if (Model.SelectedFormItem != null) 
            //    {
            //        Model.SelectedItem.FormItems.Remove(Model.SelectedFormItem);
            //        COSContext.Current.ConfiguratorFormItems.DeleteObject(Model.SelectedFormItem);

            //        Model.SelectedFormItem = Model.SelectedItem.FormItems.FirstOrDefault();
            //    }
            //}
        }

        void itemControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

            foreach (var itm in cnvMainCanvas.Children)
            {
                FormItemControl fi = itm as FormItemControl;

                if (fi != null)
                {
                    //fi.mainBorder.BorderThickness = new Thickness(0);
                    fi.MainBorderBrush = Brushes.Transparent;
                }
            }

            FormItemControl fitm = sender as FormItemControl;

            if (fitm != null)
            {
                Model.SelectedFormItem = fitm.FormItem;
                fitm.Focus();
                //fitm.mainBorder.BorderThickness = new Thickness(2);
                fitm.MainBorderBrush = Brushes.Gold;
            }


        }



        private void BaseUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                DeleteFormItem();
            }
        }

        private void btnAligment_click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedItem != null)
            {
                foreach (var item in Model.SelectedItem.FormItems)
                {
                    double posX = ((int)(item.LeftPosition / 20)) * 20;
                    double posY = ((int)(item.TopPosition / 20)) * 20;

                    item.LeftPosition = posX;
                    item.TopPosition = posY;
                }
            }
        }

        private void cnvMainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            FormItemControl fitm = cnvMainCanvas.Children.OfType<FormItemControl>().FirstOrDefault(a => a.gridPressed);
            if (fitm != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed && fitm.gridPressed && !fitm.IsFixed)
                {
                    var pos = e.GetPosition(cnvMainCanvas);

                    Canvas.SetLeft(fitm, Math.Round(pos.X - fitm.grdConstX, 2));
                    Canvas.SetTop(fitm, Math.Round(pos.Y - fitm.grdConstY, 2));
                }
            }
        }



        private void cnvMainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void cnvMainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FormItemControl fitm = cnvMainCanvas.Children.OfType<FormItemControl>().FirstOrDefault(a => a.gridPressed);
            if (fitm != null)
            {
                fitm.gridPressed = false;
            }
        }

        private void cnvMainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //FormItemControl fitm = cnvMainCanvas.Children.OfType<FormItemControl>().FirstOrDefault(a => a.gridPressed);
            //if (fitm != null)
            //{
            //    fitm.gridPressed = false;
            //}
        }

        private void btnDelete_click(object sender, RoutedEventArgs e)
        {
            DeleteFormItem();
        }

        private void DeleteFormItem()
        {
            if (Model.SelectedFormItem != null)
            {
                Model.SelectedItem.FormItems.Remove(Model.SelectedFormItem);
                COSContext.Current.ConfiguratorFormItems.DeleteObject(Model.SelectedFormItem);

                GenerateFormItems();

                Model.SelectedFormItem = Model.SelectedItem.FormItems.FirstOrDefault(a => a.Name != "labour" && a.Name != "setuptime" && a.Name != "weight" && a.Name != "textlabour" && a.Name != "textsetuptime" && a.Name != "textweight" && a.Name != "itemdescription" && a.Name != "textitemdescription");
            }
        }

    }
}
