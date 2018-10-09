namespace ToolTipCommand
{
    /// <summary>
    /// Причина блокирования комманды
    /// </summary>
    public enum DisableReason
    {
        /// <summary>
        /// Не задано
        /// </summary>
        None,
        /// <summary>
        /// Причина блокировки - безопастность
        /// </summary>
        Security,
        /// <summary>
        /// Бизнес-правило
        /// </summary>
        BusinessRule,
        /// <summary>
        /// Ошибка
        /// </summary>
        Error,
    }
}
