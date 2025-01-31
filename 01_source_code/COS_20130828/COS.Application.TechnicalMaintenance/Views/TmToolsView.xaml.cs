﻿using System;
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
using COS.Application.Shared;
using COS.Application.TechnicalMaintenance.Models;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using COS.Common;
using COS.Resources;
using System.Data.SqlClient;

namespace COS.Application.TechnicalMaintenance.Views
{
    /// <summary>
    /// Interaction logic for TpmEquipmentView.xaml
    /// </summary>
    public partial class TmToolsView : BaseUserControl
    {
        public TmToolsView()
            : base()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new TmToolsViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

                COSContext.Current.PropertyChanged += new PropertyChangedEventHandler(Current_PropertyChanged);

            }

        }

        TmToolsViewModel Model = null;

        void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Language")
            {
                grvCodebook.Rebind();
            }
        }

        private TmToolsDetailView detailView = null;
        public RadWindow DetailWindow = null;

        TmTool newItem = null;
        bool newAdded = false;

        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AddNew")
            {
                newItem = new TmTool();
                newAdded = true;

                if (DetailWindow == null)
                {
                    DetailWindow = new RadWindow();


                    DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                    DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                    DetailWindow.Header = COS.Resources.ResourceHelper.GetResource<string>("tm_EquipmentHeader");
                    DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailView = new TmToolsDetailView();
                    detailView.RaiseWindow = DetailWindow;
                    DetailWindow.Content = detailView;

                    StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                }

                SysLocalize tempLocalize = new SysLocalize();
                tempLocalize.ID = COSContext.Current.SysLocalizes.Max(a => a.ID) + 1;
                COSContext.Current.SysLocalizes.AddObject(tempLocalize);
                newItem.SysLocalize = tempLocalize;

                detailView.Model.SelectedItem = newItem;
                DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                DetailWindow.ShowDialog();
            }
            else if (e.PropertyName == "Delete")
            {
                RadWindow.Confirm(new DialogParameters()
                {
                    OkButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_Y"),
                    CancelButtonContent = COS.Resources.ResourceHelper.GetResource<string>("m_General_N"),
                    DialogStartupLocation = WindowStartupLocation.CenterOwner,
                    Content = COS.Resources.ResourceHelper.GetResource<string>("m_General_Delete"),
                    Header = COS.Resources.ResourceHelper.GetResource<string>("m_Header1_A"),
                    Closed = new EventHandler<WindowClosedEventArgs>(OnConfirmClosed)
                });

            }
        }

        private void OnConfirmClosed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                if (grvCodebook.SelectedItem != null)
                {
                    TmTool item = grvCodebook.SelectedItem as TmTool;

                    if (item != null)
                    {
                        try
                        {
                            Model.LocalTmTools.Remove(item);
                            COSContext.Current.DeleteObject(item);
                            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);                         

                            grvCodebook.Rebind();

                        }
                        catch (Exception exc)
                        {
                            string err = Logging.LogException(exc, LogType.ToFileAndEmail);

                            if (!string.IsNullOrEmpty(err))
                                MessageBox.Show(err);
                        }
                    }
                }
            }
        }


        //SysLocalize tempLocalize = null;

        void DetailWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.HasValue && e.DialogResult.Value)
            {
                if (newAdded && newItem != null)
                {
                    newAdded = false;
                    Model.LocalTmTools.Add(newItem);
                }
                grvCodebook.Rebind();

            }
            else
            {
                COSContext.Current.RejectChanges();
            }
        }


        private void grvCodeBook_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (COS.Common.WPF.Helpers.HasRightForOperation("TpmTool", "Update"))
            {
                FrameworkElement originalSender = e.OriginalSource as FrameworkElement;
                if (originalSender != null)
                {
                    var row = originalSender.ParentOfType<GridViewRow>();
                    if (row != null)
                    {
                        TmTool item = grvCodebook.SelectedItem as TmTool;
                        newAdded = false;

                        if (item != null)
                        {
                            if (DetailWindow == null)
                            {
                                DetailWindow = new RadWindow();

                                DetailWindow.ResizeMode = ResizeMode.CanMinimize;
                                DetailWindow.Closed += new EventHandler<WindowClosedEventArgs>(DetailWindow_Closed);
                                DetailWindow.Header = ResourceHelper.GetResource<string>("tm_EquipmentHeader");
                                DetailWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                detailView = new TmToolsDetailView();
                                detailView.RaiseWindow = DetailWindow;
                                DetailWindow.Content = detailView;

                                StyleManager.SetTheme(DetailWindow, new Expression_DarkTheme());
                            }

                            detailView.Model.SelectedItem = item;
                            DetailWindow.Owner = (RadWindow)COSContext.Current.RadMainWindow;
                            DetailWindow.ShowDialog();
                        }
                    }
                }
            }
        }


    }
}
