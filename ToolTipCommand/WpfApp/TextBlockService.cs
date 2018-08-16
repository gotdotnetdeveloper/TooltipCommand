using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp
{
    public class TextBlockService
    {
        #region Fields

        public static readonly DependencyProperty AutomaticToolTipEnabledProperty =
            DependencyProperty.RegisterAttached("AutomaticToolTipEnabled", typeof(bool), typeof(TextBlockService),
                                                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        private static readonly DependencyPropertyKey IsTextTrimmedKey =
            DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBlockService),
                                                        new PropertyMetadata(false));

        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        #endregion

        #region .ctor

        static TextBlockService()
        {
            // Register for the SizeChanged event on all TextBlocks, even if the event was handled.
            //EventManager.RegisterClassHandler(
            //	typeof (TextBlock),
            //	FrameworkElement.SizeChangedEvent,
            //	new SizeChangedEventHandler(OnTextBlockSizeChanged),
            //	true);

            EventManager.RegisterClassHandler(
                typeof(TextBlock),
                UIElement.MouseEnterEvent,
                new MouseEventHandler(OnTextBlockMouseEnter),
                true);
        }

        #endregion

        #region Methods

        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Boolean GetAutomaticToolTipEnabled(DependencyObject element)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(AutomaticToolTipEnabledProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static Boolean GetIsTextTrimmed(TextBlock target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        private static void OnTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (null == textBlock)
            {
                return;
            }

            SetIsTextTrimmed(textBlock, TextTrimming.None != textBlock.TextTrimming && CalculateIsTextTrimmed(textBlock));
        }

        /*
		private static void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e)
		{
			var textBlock = sender as TextBlock;
			if (null == textBlock)
			{
				return;
			}

			SetIsTextTrimmed(textBlock, TextTrimming.None != textBlock.TextTrimming && CalculateIsTextTrimmed(textBlock));
		}
		*/

        /*
		private static void OnTextMediatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!_inBindingOperation)
			{
				var textBlock = d as TextBlock;
				if (textBlock != null)
					SetIsTextTrimmed(textBlock, TextTrimming.None != textBlock.TextTrimming && CalculateIsTextTrimmed(textBlock));
			}
		}
		*/

        public static void SetAutomaticToolTipEnabled(DependencyObject element, bool value)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AutomaticToolTipEnabledProperty, value);
        }

        private static bool CalculateIsTextTrimmed(TextBlock textBlock)
        {
            /*
			if (!textBlock.IsArrangeValid)
			{
				return GetIsTextTrimmed(textBlock);
			}
			*/
            //var txt = textBlock.Text;
            if (!textBlock.Inlines.Any()/*string.IsNullOrEmpty(txt)*/)
                return false;

            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            return textBlock.DesiredSize.Width - textBlock.ActualWidth > -0.3;

            /*
			var typeface = new Typeface(
				textBlock.FontFamily,
				textBlock.FontStyle,
				textBlock.FontWeight,
				textBlock.FontStretch);

			// FormattedText is used to measure the whole width of the text held up by TextBlock container
			var formattedText = new FormattedText(
				textBlock.Text,
				System.Threading.Thread.CurrentThread.CurrentCulture,
				textBlock.FlowDirection,
				typeface,
				textBlock.FontSize,
				textBlock.Foreground) {
					MaxTextWidth = textBlock.ActualWidth
				};

			// When the maximum text width of the FormattedText instance is set to the actual
			// width of the textBlock, if the textBlock is being trimmed to fit then the formatted
			// text will report a larger height than the textBlock. Should work whether the
			// textBlock is single or multi-line.
			return (formattedText.Height > textBlock.ActualHeight);
			*/
        }

        private static void SetIsTextTrimmed(TextBlock target, Boolean value)
        {
            target.SetValue(IsTextTrimmedKey, value);
        }

        #endregion

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.RegisterAttached("TextWrapping", typeof(TextWrapping), typeof(TextBlockService), new PropertyMetadata(default(TextWrapping)));

        public static void SetTextWrapping(UIElement element, TextWrapping value)
        {
            element.SetValue(TextWrappingProperty, value);
        }

        public static TextWrapping GetTextWrapping(UIElement element)
        {
            return (TextWrapping)element.GetValue(TextWrappingProperty);
        }

        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.RegisterAttached("TextDecorations", typeof(TextDecorationCollection), typeof(TextBlockService), new PropertyMetadata(null));


        public static void SetTextDecorations(UIElement element, TextDecorationCollection value)
        {
            element.SetValue(TextDecorationsProperty, value);
        }

        public static TextDecorationCollection GetTextDecorations(UIElement element)
        {
            return (TextDecorationCollection)element.GetValue(TextDecorationsProperty);
        }
    }
}
