using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ToolTipCommand;


namespace WpfApp
{
    /// <summary>
    /// Конвертер для картинки валидации.
    /// </summary>
    public class DisableMarkImageConverter : IValueConverter
    {
        /// <summary>
        /// Ключ картинки. Желтый замочек.
        /// </summary>
        public const string SmallLockKeyPictute = "SmallLock";
        /// <summary>
        /// Ключ картинки. Желтый треугольник с восклицательным знаком.
        /// </summary>
        public const string SmallErrorKeyPictute = "SmallError";
        /// <summary>
        /// Ключ картинки. Красный кирпич.
        /// </summary>
        public const string SmallRedStopKeyPictute = "SmallRedStop";

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DisableReason))
                return null;
            var dr = (DisableReason)value;
            if (dr == DisableReason.None)
                return null;

            var key = (dr == DisableReason.Security) ? SmallLockKeyPictute
                : (dr == DisableReason.Error) ? SmallErrorKeyPictute
                : SmallRedStopKeyPictute;

            var returnValue = Application.Current.FindResource(key);

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
