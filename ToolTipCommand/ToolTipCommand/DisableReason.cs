using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Причина - безопастность
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
