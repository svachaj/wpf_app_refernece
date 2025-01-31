using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace COS.Application.Shared
{
    public partial class TpmPlan
    {
        public SolidColorBrush StatusBackground
        {
            get
            {
                if (this.TpmStatus.Code == null)
                {
                    return new SolidColorBrush(Color.FromRgb(37, 160, 218));
                }
                else if (this.TpmStatus.Code == "P")
                {
                    return new SolidColorBrush(Color.FromRgb(37, 160, 218));
                }
                else if (this.TpmStatus.Code == "I")
                {
                    return new SolidColorBrush(Color.FromRgb(243, 179, 1));
                }
                else if (this.TpmStatus.Code == "F")
                {
                    return new SolidColorBrush(Color.FromRgb(86, 177, 14));
                }
                else if (this.TpmStatus.Code == "C")
                {
                    return new SolidColorBrush(Colors.Crimson);
                }
                else return new SolidColorBrush(Color.FromRgb(37, 160, 218));


            }
        }

        public void RefreshStatusBackground()
        {
            OnPropertyChanged("StatusBackground");
            OnPropertyChanged("StatusForeground");
        }

        public SolidColorBrush StatusForeground
        {
            get
            {
                if (this.TpmStatus.Code == null)
                {
                    return new SolidColorBrush(Color.FromRgb(50, 50, 50));
                }
                else if (this.TpmStatus.Code == "P")
                {
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else if (this.TpmStatus.Code == "I")
                {
                    return new SolidColorBrush(Color.FromRgb(50, 50, 50));
                }
                else if (this.TpmStatus.Code == "F")
                {
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else if (this.TpmStatus.Code == "C")
                {
                    return new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                else return new SolidColorBrush(Color.FromRgb(50, 50, 50));

            }
        }


    }
}
