using System.Windows.Input;

namespace ToolTipCommand
{
    /// <summary>Контекст команды для CanExecute</summary>
    public class CanExecuteInfo
    {
        #region Приватные свойства
        private readonly ICommand _command;
        private DisableReason _disableReason;
        #endregion


        /// <summary>
        /// Конструктор CanExecuteInfo
        /// </summary>
        /// <param name="command"> Команда</param>
        internal CanExecuteInfo(ICommand command)
        {
            _command = command;
        }

        #region Публичные свойства
        /// <summary>Сообщение, отображаемое на экране.</summary>
        public string DisableReasonTip { get; set; }

        /// <summary>Причина недоступности</summary>
        public DisableReason DisableReason
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DisableReasonTip) || _disableReason != DisableReason.None)
                    return _disableReason;
                return DisableReason.BusinessRule;
            }
            set => _disableReason = value;
        }

        /// <summary>Обрабатываемая команда</summary>
        public ICommand Command => _command;

        /// <summary>Команда резрешена, если выполняется условие condition</summary>
        /// <param name="condition">Условие</param>
        /// <param name="disableReason">Тип причины</param>
        /// <param name="disableReasonTip">Сообщение</param>
        /// <returns></returns>
        public bool EnableIf(bool condition, DisableReason disableReason, string disableReasonTip)
        {
            if (!condition)
            {
                DisableReason = disableReason;
                DisableReasonTip = disableReasonTip;
            }
            return condition;
        }
        /// <summary>Команда резрешена, если выполняется условие condition</summary>
        /// <param name="condition">Условие</param>
        /// <param name="disableReasonTip">Сообщение</param>
        /// <returns>true = Если команда разрешена. false =  Если команда запрещена</returns>
        public bool EnableIf(bool condition, string disableReasonTip)
        {
            return EnableIf(condition, DisableReason.BusinessRule, disableReasonTip);
        }

        /// <summary>Блокировать команду  </summary>
        /// <param name="disableReasonTip">Текстовая причина</param>
        public bool Disable(string disableReasonTip)
        {
            return Disable(DisableReason.BusinessRule, disableReasonTip);
        }

        /// <summary>
        /// Блокировать команду   
        /// </summary>
        /// <param name="disableReason">Тип причины</param>
        /// <param name="disableReasonTip"></param>
        /// <returns>всегда false</returns>
        public bool Disable(DisableReason disableReason, string disableReasonTip)
        {
            DisableReason = DisableReason.BusinessRule;
            DisableReasonTip = disableReasonTip;
            return false;
        }
        #endregion

    }
}
