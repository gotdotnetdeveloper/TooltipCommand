using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp
{
    /// <summary>
    /// Расширение TextBlock
    /// </summary>
    public class TextBlockService
    {
        #region Fields
        /// <summary>
        /// Автоматически включать тултипы
        /// </summary>
        public static readonly DependencyProperty AutomaticToolTipEnabledProperty =
            DependencyProperty.RegisterAttached("AutomaticToolTipEnabled", typeof(bool), typeof(TextBlockService),
                                                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
        /// <summary>
        /// Обрезка текста тултипа
        /// </summary>
        private static readonly DependencyPropertyKey IsTextTrimmedKey =
            DependencyProperty.RegisterAttachedReadOnly("IsTextTrimmed", typeof(bool), typeof(TextBlockService),
                                                        new PropertyMetadata(false));
        /// <summary>
        /// Обрезка текста тултипа
        /// </summary>
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedKey.DependencyProperty;

        #endregion

        #region .ctor

        static TextBlockService()
        {

            EventManager.RegisterClassHandler(
                typeof(TextBlock),
                UIElement.MouseEnterEvent,
                new MouseEventHandler(OnTextBlockMouseEnter),
                true);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Атач - проперти для AutomaticToolTipEnabled
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Boolean GetAutomaticToolTipEnabled(DependencyObject element)
        {
            if (null == element)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (bool)element.GetValue(AutomaticToolTipEnabledProperty);
        }
        /// <summary>
        /// Атач - проперти для IsTextTrimmed
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(TextBlock))]
        public static Boolean GetIsTextTrimmed(TextBlock target)
        {
            return (Boolean)target.GetValue(IsTextTrimmedProperty);
        }

        /// <summary>
        /// Событие при наведении мышки
        /// </summary>
        private static void OnTextBlockMouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (null == textBlock)
            {
                return;
            }

            SetIsTextTrimmed(textBlock, TextTrimming.None != textBlock.TextTrimming && CalculateIsTextTrimmed(textBlock));
        }


        /// <summary>
        /// Установка свойства AutomaticToolTipEnabled
        /// </summary>
        public static void SetAutomaticToolTipEnabled(DependencyObject element, bool value)
        {
            if (null == element)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(AutomaticToolTipEnabledProperty, value);
        }

        /// <summary>
        /// Вычислить, что нужено обрезать текст тултипа.
        /// </summary>
        /// <param name="textBlock">Целевой текстблок</param>
        /// <returns>Результат вычисления</returns>
        private static bool CalculateIsTextTrimmed(TextBlock textBlock)
        {
            if (!textBlock.Inlines.Any()/*string.IsNullOrEmpty(txt)*/)
                return false;

            textBlock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            return textBlock.DesiredSize.Width - textBlock.ActualWidth > -0.3;
        }
        /// <summary>
        /// Установка текст тултипа.
        /// </summary>
        /// <param name="target">текст блок</param>
        /// <param name="value"></param>
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
