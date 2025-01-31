using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;

namespace COS.Application.Administration.Models
{
    public partial class SecurityViewModel : ValidationViewModelBase
    {
        public SecurityViewModel()
            : base()
        {
           
        }


        public SysPwdPolicy SelectedItem 
        {
            get 
            {
                return COSContext.Current.SysPwdPolicies.FirstOrDefault();
            }
        }

    }
}
