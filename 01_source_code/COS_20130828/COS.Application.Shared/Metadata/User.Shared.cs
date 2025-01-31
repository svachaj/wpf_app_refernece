using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace COS.Application.Shared
{
    public partial class User
    {
        public SysAccountType AccountType 
        {
            get 
            {
                return COSContext.Current.SysAccountTypes.FirstOrDefault(a => a.ID == this.AccountType_ID);
            }
            set
            {
                if (value != null)
                {
                    this.AccountType_ID = value.ID;
                    OnPropertyChanged("AccountType");
                }
            }
        }

        public SysGroup Group
        {
            get
            {
                return COSContext.Current.SysGroups.FirstOrDefault(a => a.ID == this.Group_ID);
            }
            set
            {
                if (value != null)
                {
                    this.Group_ID = value.ID;
                    OnPropertyChanged("Group");
                }
            }
        }

        [Required(ErrorMessage="Jméno je povinné.")]
        public string NameValid
        {
            get
            {
                return this.Name;
            }
            set
            {
                if (value != this.Name)
                {
                    this.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("NameValid");
                }
            }
        }

        public string FullName 
        {
            get             
            {
                return this.Name + " " + this.Surname;
            }
        }

        public string FullNameRev
        {
            get
            {
                return this.Surname + " " + this.Name;
            }
        }


        public Employee Employee 
        {
            get 
            {
                var empl = COSContext.Current.Employees.FirstOrDefault(a => a.HR_ID == this.HR_ID);

                return empl;
            }
        }
    }
}
