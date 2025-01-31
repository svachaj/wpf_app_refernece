using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
//using System.ComponentModel.DataAnnotations;
using System.Windows;
using Telerik.Windows.Controls;
using COS.Resources;
using System.Transactions;

namespace COS.Application.Production.Models.PlanningVA4H
{
    public partial class cbProdPlanStateDetailViewModel : ValidationViewModelBase
    {
        public cbProdPlanStateDetailViewModel(COSContext dataContext)
            : base()
        {
            this.dataContext = dataContext;
        }

        COSContext dataContext;

        private ProdPlanState _selectedItem = null;
        public ProdPlanState SelectedItem
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
                        dataContext.ProdPlanStates.AddObject(SelectedItem);
                    }
                    else
                    {

                    }
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }))
                    {
                        try
                        {
                            dataContext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                            scope.Complete();
                        }
                        catch (Exception exc)
                        {
                            Logging.LogException(exc, LogType.ToFileAndEmail);
                            scope.Dispose();
                            dataContext.RejectChanges();

                            RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)dataContext.RadMainWindow });
                        }

                    }

                    OnPropertyChanged("Save");
                }
                else
                {
                    RadWindow.Alert(new DialogParameters() { Content = customErrors + Error, Header = ResourceHelper.GetResource<string>("m_Header2_E"), Owner = (RadWindow)dataContext.RadMainWindow });
                }
           
        }


        public void Cancel()
        {
            OnPropertyChanged("Cancel");
        }

        public string IsValid()
        {
            string err = "";


            return err;
        }

        internal ProdPlanState CreateNewItem()
        {
            ProdPlanState res = dataContext.ProdPlanStates.CreateObject();
            SysLocalize tempLocalize = new SysLocalize();
            tempLocalize.ID = dataContext.SysLocalizes.Max(a => a.ID) + 1;
            dataContext.SysLocalizes.AddObject(tempLocalize);
            res.SysLocalize = tempLocalize;

            return res;
        }
    }
}
