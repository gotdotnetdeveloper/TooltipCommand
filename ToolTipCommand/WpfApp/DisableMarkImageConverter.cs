using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ToolTipCommand;


namespace WpfApp
{
    public class DisableMarkImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DisableReason))
                return null;
            var dr = (DisableReason)value;
            if (dr == DisableReason.None)
                return null;

            var key = (dr == DisableReason.Security) ? "SmallLock"
                : (dr == DisableReason.Error) ? "SmallError"
                : "SmallRedStop";

            return Application.Current.FindResource(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
