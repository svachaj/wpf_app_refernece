using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using COS.Common;
using System.Windows;
using System.Windows.Media;

namespace COS.Resources
{
    public class EditModeEgpsConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.New)
            {
                return false;
            }
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class VisibilityBoolConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                bool vis = (bool)value;

                if (vis)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
                return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class VisibilityReverseBoolConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                bool vis = (bool)value;

                if (!vis)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            else
                return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }


    public class ConectivityBrushConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                long vis = (long)value;

                if (vis > 120)
                    return Brushes.Red;
                else if (vis > 60)
                    return Brushes.Orange;
                else if (vis >= 0)
                    return Brushes.Green;
                else
                    return Brushes.Red;
            }
            else
                return Brushes.Red;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EmptyStringEnabledConvertor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsSetHPReverseConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                return !(bool)value;
            }
            else
                return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class TimePlanModeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "OneTime")
                {
                    return true;
                }
                else if (mode == 2 && parameter.ToString() == "Recurrency")
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool modeVal = (bool)value;

                if (modeVal && parameter.ToString() == "OneTime")
                {
                    return 1;
                }
                else if (modeVal && parameter.ToString() == "Recurrency")
                {
                    return 2;
                }
                else
                    return 1;

            }
            else
                return 1;
        }


        #endregion
    }


    public class TimePlanModeEnabledConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "OneTime")
                {
                    return true;
                }
                else if (mode == 2 && parameter.ToString() == "Recurrency")
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool modeVal = (bool)value;

                if (modeVal && parameter.ToString() == "OneTime")
                {
                    return 1;
                }
                else if (modeVal && parameter.ToString() == "Recurrency")
                {
                    return 2;
                }
                else
                    return 1;

            }
            else
                return 1;
        }


        #endregion
    }

    public class TimePlanModeHeightConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "OneTime")
                {
                    return double.NaN;
                }
                else if (mode == 2 && parameter.ToString() == "Recurrency")
                {
                    return double.NaN;
                }
                else
                    return 0;

            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeEnabledConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.New)
            {
                return true;
            }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class EditModeVisibilityConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EditMode mode = (EditMode)value;

            if (mode == EditMode.Edit)
            {
                return Visibility.Visible;
            }
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }


    public class RecurrencyModeEnabledConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "1")
                {
                    return true;
                }
                else if (mode == 2 && parameter.ToString() == "2")
                {
                    return true;
                }
                else if (mode == 3 && parameter.ToString() == "3")
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool modeVal = (bool)value;

                if (modeVal && parameter.ToString() == "1")
                {
                    return 1;
                }
                else if (modeVal && parameter.ToString() == "2")
                {
                    return 2;
                }
                else if (modeVal && parameter.ToString() == "3")
                {
                    return 3;
                }
                else
                    return 1;

            }
            else
                return 1;
        }


        #endregion
    }


    public class RecurrencyModeHeightConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "1")
                {
                    return double.NaN;
                }
                else if (mode == 2 && parameter.ToString() == "2")
                {
                    return double.NaN;
                }
                else if (mode == 3 && parameter.ToString() == "3")
                {
                    return double.NaN;
                }
                else
                    return 0;

            }
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }


    public class RecurrencyStringExistsConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }



    public class RecurrencyModeBorderBrushConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                int mode = (int)value;

                if (mode == 1 && parameter.ToString() == "1")
                {
                    return new SolidColorBrush(new Color() { R = 101, G = 173, B = 20, A = 255 });
                }
                else if (mode == 2 && parameter.ToString() == "2")
                {
                    return new SolidColorBrush(new Color() { R = 101, G = 173, B = 20, A = 255 });
                }
                else if (mode == 3 && parameter.ToString() == "3")
                {
                    return new SolidColorBrush(new Color() { R = 101, G = 173, B = 20, A = 255 });
                }
                else
                    return new SolidColorBrush(Colors.Gray);

            }
            else
                return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class ForwarderEnabledConvertor : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }

    public class BooleanReverseConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return !(bool)value;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }


    public class NullToVisibilityConverter : IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeToLocalizeDayConverter : IValueConverter 
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string day = "";

            if (value is DateTime) 
            {
                day = Helpers.Days[(int)((DateTime)value).DayOfWeek];
            }

            return day;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

   
}




