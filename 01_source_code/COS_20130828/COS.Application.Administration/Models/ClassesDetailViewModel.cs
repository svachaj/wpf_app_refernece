using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;

namespace COS.Application.Administration.Models
{
    public partial class ClassesDetailViewModel : ValidationViewModelBase
    {
        public ClassesDetailViewModel()
            : base()
        {

        }

        private SysClass _selectedItem = null;
        public SysClass SelectedItem
        {
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
            get
            {
                return _selectedItem;
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(param => this.Save());
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(param => this.Cancel());
            }
        }


        public void Save()
        {

            string customErrors = "";

            customErrors = IsValid();

            if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(customErrors))
            {
                if (SelectedItem.ID == 0)
                {
                    COSContext.Current.SysClasses.AddObject(SelectedItem);
                }
                else
                {

                }

                if (SelectedGroupClass != null && SelectedGroupClass.ParentItem != null)
                {
                    SelectedGroupClass.ParentItem.RelatedItems.Add(new AdminGroupClass(SelectedItem, SelectedGroupClass.GroupId));
                    SelectedGroupClass = null;
                }


                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
                        COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                        scope.Complete();
                    }
                    catch (Exception exc)
                    {
                        Logging.LogException(exc, LogType.ToFileAndEmail);
                        scope.Dispose();
                        COSContext.Current.RejectChanges();

                        RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                    }

                }

                OnPropertyChanged("Save");
            }
            else
            {
                RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
            }

        }

        public AdminGroupClass SelectedGroupClass { set; get; }

        public SysClass CopyClass(SysClass selectedClass)
        {
            SysClass result = null;

            if (selectedClass != null)
            {
                result = COSContext.Current.SysClasses.CreateObject();

                result.t_sys_localization = SysLocalize.CreateNewLocalize();

                result.ID_Parent = selectedClass.ID_Parent;

                result.ID = COSContext.Current.SysClasses.Max(a => a.ID) + 1;
                result.TableName = selectedClass.TableName;
                result.FullName = selectedClass.FullName;
                result.UserControlTag = "copy_" + selectedClass.UserControlTag;
                result.Name = "copy_" + selectedClass.Name;
                result.t_sys_localization.cs_Czech = "copy_" + selectedClass.t_sys_localization.cs_Czech;
                result.t_sys_localization.en_English = "copy_" + selectedClass.t_sys_localization.en_English;

                SysEGP egp = null;
                foreach (var itm in selectedClass.t_sys_egp)
                {
                    egp = COSContext.Current.SysEGPs.CreateObject();
                    egp.Action = itm.Action;
                    egp.Granted = itm.Granted;
                    egp.SysGroup = itm.SysGroup;

                    result.t_sys_egp.Add(egp);
                }

            }

            return result;
        }

        public SysClass NewClass(SysClass parentClass)
        {
            SysClass result = null;


            result = COSContext.Current.SysClasses.CreateObject();

            result.t_sys_localization = SysLocalize.CreateNewLocalize();

            if(parentClass != null)
                result.ID_Parent = parentClass.ID;

            result.ID = COSContext.Current.SysClasses.Max(a => a.ID) + 1;                  

            return result;
        }


        public void Cancel()
        {
            //COSContext.Current.RejectChanges();
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";


            if (string.IsNullOrEmpty(SelectedItem.Name))
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000005");
                err += Environment.NewLine;
            }


            if (string.IsNullOrEmpty(SelectedItem.t_sys_localization.cs_Czech))
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                err += Environment.NewLine;
            }

            if (string.IsNullOrEmpty(SelectedItem.t_sys_localization.en_English))
            {
                err += ResourceHelper.GetResource<string>("m_Body_ADM00000006");
                err += Environment.NewLine;
            }

            return err;
        }
    }
}
