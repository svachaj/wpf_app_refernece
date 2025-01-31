using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace COS.Application.Shared
{
    public partial class Employee
    {
        public WorkGroup WorkGroup
        {
            get
            {
                return COSContext.Current.WorkGroups.FirstOrDefault(a => a.ID_RON == this.WorkGroupHR_ID);                
            }
            set
            {
                if (value != null)
                {
                    this.WorkGroupHR_ID = value.ID_RON;
                    OnPropertyChanged("WorkGroup");
                }
            }
        }

        public SalaryGroup SalaryGroup
        {
            get
            {
                return COSContext.Current.SalaryGroups.FirstOrDefault(a => a.ID_RON == this.SalaryGroupHR_ID);
            }
            set
            {
                if (value != null)
                {
                    this.WorkGroupHR_ID = value.ID_RON;
                    OnPropertyChanged("SalaryGroup");
                }
            }
        }

        public Shift Shift
        {
            get
            {
                return COSContext.Current.Shifts.FirstOrDefault(a => a.ID == this.Shift_ID);
            }
            set
            {
                if (value != null)
                {
                    this.Shift_ID = value.ID;
                    OnPropertyChanged("ShiftPattern");
                }
            }
        }

        public BonusGroup BonusGroup
        {
            get
            {
                return COSContext.Current.BonusGroups.FirstOrDefault(a => a.ID == this.BonusGroup_ID);
            }
            set
            {
                if (value != null)
                {
                    this.BonusGroup_ID = value.ID;
                    OnPropertyChanged("BonusGroup");
                }
            }
        }


        public WorkPosition WorkPosition
        {
            get
            {
                return COSContext.Current.WorkPositions.FirstOrDefault(a => a.ID == this.WorkPosition_ID);
            }
            set
            {
                if (value != null)
                {
                    this.WorkPosition_ID = value.ID;
                    OnPropertyChanged("WorkPosition");
                }
            }
        }

        public CostCenter CostCenter
        {
            get
            {
                return COSContext.Current.CostCenters.FirstOrDefault(a => a.ID_RON == this.CostCenterHR_ID);
            }
            set
            {
                if (value != null)
                {
                    this.CostCenterHR_ID = value.ID_RON;
                    OnPropertyChanged("CostCenter");
                }
            }
        }

        public Division DivisionHR
        {
            get
            {
                return COSContext.Current.Divisions.FirstOrDefault(a => a.ID_RON == this.DivisionHR_ID);
            }
            set
            {
                if (value != null)
                {
                    this.DivisionHR_ID = value.ID_RON;
                    OnPropertyChanged("DivisionHR");
                }
            }
        }

        public Employer EmployerHR
        {
            get
            {
                return COSContext.Current.Employers.FirstOrDefault(a => a.ID_RON == this.EmployerHR_ID);
            }
            set
            {
                if (value != null)
                {
                    this.EmployerHR_ID = value.ID_RON;
                    OnPropertyChanged("EmployerHR");
                }
            }
        }

        public string FullName
        {
            get
            {
                return this.Surname + " " + this.Name;
            }
        }

        public string ShortName
        {
            get
            {
                return this.Name.FirstOrDefault() + ". " + this.Surname;
            }
        }

        public string FullNameWithID
        {
            get
            {
                return this.Surname + " " + this.Name + " - " + this.HR_ID;
            }
        }

        public override string ToString()
        {
            return FullNameWithID;
        }

        //[EdmScalarPropertyAttribute(EntityKeyProperty = false, IsNullable = true)]
        [DataMemberAttribute()]
        public string WorkGroupDescription
        {
            get
            {
                return "";
            }
        }

    }
}
