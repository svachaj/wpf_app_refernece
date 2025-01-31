using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace COS.Application.Shared
{
    public partial class TpmPlanExecution
    {
        public Employee Employee
        {
            get
            {
                return COSContext.Current.Employees.FirstOrDefault(a => a.HR_ID == this.HR_ID);
            }
        }

        public void RefreshEmployee()
        {
            OnPropertyChanged("Employee");
        }

      
    }
}
