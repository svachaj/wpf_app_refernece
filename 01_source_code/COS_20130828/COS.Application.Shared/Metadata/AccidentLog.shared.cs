using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COS.Application.Shared
{
    public partial class AccidentLog
    {
        public void CalculateNumberOfCalendarDaysOfIncapacity()
        {
            if (this.IncapacityFrom.HasValue && this.IncapacityTo.HasValue)
                this.NoOfCalDaysOfIncapacity = Math.Round((decimal)(this.IncapacityTo.Value - this.IncapacityFrom.Value).TotalDays, 1);
        }

        public void CalculateNumberOfWorkDaysOfIncapacity()
        {
            if (this.IncapacityFrom.HasValue && this.IncapacityTo.HasValue)
            {
                this.NoOfWrkDaysOfIncapacity = 0;

                var days = (int)(this.IncapacityTo.Value - this.IncapacityFrom.Value).TotalDays;

                for (int i = 0; i < days; i++)
                {
                    var dd = this.IncapacityFrom.Value.AddDays(i);

                    if (dd.IsWorkDay())
                        this.NoOfWrkDaysOfIncapacity++;
                }
            }
        }
              
    }

    public partial class AreaOfAccident
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result) && this.ID_localization_description.HasValue)
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class TypeOfAccident
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class TypeOfInjury
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class InjPartOfBody
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class MeasurePreventType
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class SourceOfAccident
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class CauseOfAccident
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }

    public partial class HealtInsuranceComp
    {
        public string Description
        {
            get
            {
                string result = null;

                if (this.SysLocalize != null)
                    result = COSContext.Current.Language == "cs-CZ" ? this.SysLocalize.cs_Czech : this.SysLocalize.en_English;

                if (string.IsNullOrEmpty(result))
                    result = "Not translated (" + this.ID_localization_description.ToString() + ")";

                return result;
            }
        }
    }



}
