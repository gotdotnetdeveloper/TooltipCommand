using ToolTipCommand;

namespace WpfApp
{
    /// <summary>
    /// Клас для кастомного биндинга wpf причины блокировки команды
    /// </summary>
    public class TooltipWithDisableReason
    {
        /// <summary>
        /// Оригинальный тултип заданный из верстки View, не связанный с блокировкой.
        /// </summary>
        public object OriginalTooltip { get; set; }
        /// <summary>
        /// Причина блокировки команды
        /// </summary>
        public DisableReason DisableReason { get; set; }
        /// <summary>
        /// Тултип валидации связанный с блокировкой, задается из CanExecute
        /// </summary>
        public string DisableReasonTip { get; set; }
    }
}
