using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using ToolTipCommand;

namespace WpfApp
{
    /// <summary>
    /// Сборка тултипа из заданного тултипа и из за причины блокировки
    /// </summary>
    public class DisableControlTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TooltipWithDisableReason drt = null;
            if (values == null || values.Length == 0)
                return null;

            // исходный тултип
            if (values[0] != null)
            {
                // ToolTip не должен иметь родителя, поэтому если исходный тултип был задан как объект ToolTip, то берем от него только Content
                var originalToolTip = values[0] as ToolTip;
                drt = new TooltipWithDisableReason
                {
                    OriginalTooltip = originalToolTip != null ? originalToolTip.Content : values[0]
                };
            }

            if (values.Length >= 3 && values[2] != null)
            {
                var s = values[2] as string;
                if (!string.IsNullOrEmpty(s))
                {
                    if (drt == null)
                        drt = new TooltipWithDisableReason();
                    if (values[1] is DisableReason)
                        drt.DisableReason = (DisableReason)(values[1]);
                    drt.DisableReasonTip = s;
                }
            }

            if (drt == null)
                return null;

            if (drt.DisableReasonTip == null)
                return drt.OriginalTooltip;

            return drt; //new ContentPresenter{ Content = drt, ContentTemplate = (DataTemplate)parameter};

            //return new ToolTip
            //{
            //	Style = (Style)parameter,
            //	Content = drt
            //};
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class MultiTooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TooltipWithDisableReason drt = null;
            if (values == null || values.Length == 0)
                return parameter;

            if(values.Length >= 2)
            {
                if (values[0] is DisableReason disableReason)
                {
                    drt = new TooltipWithDisableReason();
                    drt.DisableReason = disableReason;
                }

                if (values[1] is string s && !string.IsNullOrEmpty(s) )
                {
                    if (drt == null)
                        drt = new TooltipWithDisableReason();
                    drt.DisableReasonTip = s;
                }

                // исходный тултип
                if (parameter is string originalToolTip)
                {
                    if (drt == null)
                        drt = new TooltipWithDisableReason();
                    drt.OriginalTooltip = originalToolTip;
                }
            }
            return drt; 
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер Tooltip
    /// </summary>
    public class TooltipConverter : IValueConverter
    {
        /// <summary>
        /// Получить текст тултипа 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string || parameter is string)
            {
                var drt = new TooltipWithDisableReason();

                if (value is string stringValue)
                {
                    drt.DisableReasonTip = stringValue;
                }
                if (parameter is string stringParameter && !string.IsNullOrEmpty(stringParameter))
                {
                    drt.OriginalTooltip = stringParameter;
                }

                return drt;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
