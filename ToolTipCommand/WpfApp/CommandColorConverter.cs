using System;
using System.Globalization;
using System.Windows.Data;
using ToolTipCommand;

namespace WpfApp
{
    /// <summary>
    /// Определить цвет по DisableReason
    /// </summary>
    public class CommandColorConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisableReason dr)
            {
                if (dr != DisableReason.None)
                    return System.Windows.Media.Brushes.Red ;
            }
            return System.Windows.Media.Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}