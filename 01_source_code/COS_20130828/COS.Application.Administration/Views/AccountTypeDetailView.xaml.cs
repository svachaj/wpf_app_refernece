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
using COS.Application.Administration.Models;
using System.ComponentModel;
using Telerik.Windows.Controls;

namespace COS.Application.Administration.Views
{
    /// <summary>
    /// Interaction logic for UserDetailView.xaml
    /// </summary>
    public partial class AccountTypeDetailView : BaseUserControl
    {
        public AccountTypeDetailView()
            : base()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Model = new  AccountTypeDetailViewModel();
                this.DataContext = Model;

                Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);

            }
        }

       
        void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Cancel")
            {
                if (RaiseWindow != null)
                {
                    RaiseWindow.DialogResult = false;
                    RaiseWindow.Close();
                }
            }
            else if (e.PropertyName == "Save")
            {
                if (RaiseWindow != null)
                {
                   
                    RaiseWindow.DialogResult = true;
                    RaiseWindow.Close();
                }
            }
          

        }

        void SelectedUser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
        }

        public AccountTypeDetailViewModel Model = null;
        public RadWindow RaiseWindow = null;

      

    }
}
