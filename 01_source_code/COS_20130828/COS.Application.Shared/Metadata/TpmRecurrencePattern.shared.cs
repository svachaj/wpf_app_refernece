using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class TpmRecurrencePattern
    {
        private int _reccurencyMode = 1;
        /// <summary>
        /// 1 = daily
        /// 2 = weekley
        /// 3 = monthly
        /// </summary>       
        public int RecurrencyMode
        {
            get
            {
                if (this.isDaily)
                    return 1;
                else if (this.isWeekly)
                    return 2;
                else if (this.isMonthly)
                    return 3;
                else
                    return _reccurencyMode;
            }
            set
            {
                if (_reccurencyMode != value)
                {
                    _reccurencyMode = value;

                    if (_reccurencyMode == 1)
                    {
                        this.isDaily = true;
                        this.isMonthly = false;
                        this.isWeekly = false;
                    }
                    else if (_reccurencyMode == 2)
                    {
                        this.isDaily = false;
                        this.isMonthly = false;
                        this.isWeekly = true;
                    }
                    else if (_reccurencyMode == 3)
                    {
                        this.isDaily = false;
                        this.isMonthly = true;
                        this.isWeekly = false;
                    }

                    OnPropertyChanged("RecurrencyMode");
                }
            }
        }


    }
}
