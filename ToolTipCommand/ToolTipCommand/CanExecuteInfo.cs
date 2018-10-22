using System.Windows.Input;

namespace ToolTipCommand
{
    /// <summary>Контекст команды для CanExecute</summary>
    public class CanExecuteInfo
    {
        /// <summary>
        /// Конструктор CanExecuteInfo
        /// </summary>
        /// <param name="command"> Команда</param>
        internal CanExecuteInfo(ICommand command)
        {
            Command = command;
        }

        #region Публичные свойства
        /// <summary>Сообщение, отображаемое на экране.</summary>
        public string DisableReasonTip { get; set; }

        /// <summary>Причина недоступности</summary>
        public DisableReason DisableReason { get; set; }

        /// <summary>Обрабатываемая команда</summary>
        public ICommand Command { get; }

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
            return EnableIf(condition, DisableReason.Error, disableReasonTip);
        }

        /// <summary>Блокировать команду  </summary>
        /// <param name="disableReasonTip">Текстовая причина</param>
        public bool Disable(string disableReasonTip)
        {
            return Disable(DisableReason.Error, disableReasonTip);
        }

        /// <summary>
        /// Блокировать команду   
        /// </summary>
        /// <param name="disableReason">Тип причины</param>
        /// <param name="disableReasonTip"></param>
        /// <returns>всегда false</returns>
        public bool Disable(DisableReason disableReason, string disableReasonTip)
        {
            DisableReason = DisableReason.Error;
            DisableReasonTip = disableReasonTip;
            return false;
        }
        #endregion

    }
}
