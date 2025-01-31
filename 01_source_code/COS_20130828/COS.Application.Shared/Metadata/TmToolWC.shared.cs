using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Input;
using COS.Common;
using System.Transactions;

namespace COS.Application.Shared
{
    public partial class TmToolWC
    {


        public decimal DiffPcs
        {
            get
            {
                decimal result = 0;

                if (this.Tool != null && this.Tool.ServiceLifePcs != 0)
                {
                    result = this.ActualPcs / this.Tool.ServiceLifePcs * 100;
                }

                return Math.Round(result, 2);
            }
        }


        public ICommand RestartCommand
        {
            get
            {
                return new RelayCommand(param => this.Restart());
            }
        }

        private void Restart()
        {
            ToolHistory newitem = COSContext.Current.ToolHistories.CreateObject();
            newitem.ChangeDate = COSContext.Current.DateTimeServer;
            newitem.User = COSContext.Current.CurrentUser;
            newitem.StatePcs = this.ActualPcs;
            newitem.ToolWC = this;

            COSContext.Current.ToolHistories.AddObject(newitem);

            this.ActualPcs = 0;
            this.IsLimitNotifySend = false;
            this.IsOverflowNotifySend = false;

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

                    //RadWindow.Alert(new DialogParameters() { Content = ResourceHelper.GetResource<string>("m_General_SaveFail"), Header = ResourceHelper.GetResource<string>("m_Header1_A"), Owner = (RadWindow)COSContext.Current.RadMainWindow });
                }
            }

            OnPropertyChanged("Restarted");
        }
    }
}
