using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using ToolTipCommand;

namespace WpfApp
{
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



    public class TooltipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 1 && values[0] != null)
            {
                if (values[0] is string text)
                {
                    return text;
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
