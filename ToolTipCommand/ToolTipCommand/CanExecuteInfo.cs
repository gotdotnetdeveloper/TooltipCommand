using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToolTipCommand
{
    /// <summary>Контекст команды для CanExecute</summary>
    public class CanExecuteInfo
    {
       // private static readonly CanExecuteInfo _empty = new CanExecuteInfo((ICommand)null);
        private readonly ICommand _command;
        private DisableReason _disableReason;

        internal CanExecuteInfo(ICommand command)
        {
            this._command = command;
        }

        ///// <summary>Empty CanExecuteInfo for Fake calls</summary>
        //public static CanExecuteInfo Empty
        //{
        //    get
        //    {
        //        return CanExecuteInfo._empty;
        //    }
        //}

        /// <summary>Сообщение</summary>
        public string DisableReasonTip { get; set; }

        /// <summary>Причина недоступности</summary>
        public DisableReason DisableReason
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.DisableReasonTip) || this._disableReason != DisableReason.None)
                    return this._disableReason;
                return DisableReason.BusinessRule;
            }
            set
            {
                this._disableReason = value;
            }
        }

        /// <summary>Обрабатываемая команда</summary>
        public ICommand Command
        {
            get
            {
                return this._command;
            }
        }

        /// <summary>Команда резрешена, если выполняется условие condition</summary>
        /// <param name="condition">Условие</param>
        /// <param name="disableReason">Тип причины</param>
        /// <param name="disableReasonTip">Сообщение</param>
        /// <returns></returns>
        public bool EnableIf(bool condition, DisableReason disableReason, string disableReasonTip)
        {
            if (!condition)
            {
                this.DisableReason = disableReason;
                this.DisableReasonTip = disableReasonTip;
            }
            return condition;
        }

        /// <summary>Команда резрешена, если выполняется условие condition</summary>
        /// <param name="condition">Условие</param>
        /// <param name="disableReasonTip">Сообщение</param>
        /// <returns></returns>
        public bool EnableIf(bool condition, string disableReasonTip)
        {
            return this.EnableIf(condition, DisableReason.BusinessRule, disableReasonTip);
        }

        /// <summary>Блокировать команду  </summary>
        /// <param name="disableReasonTip">Текстовая причина</param>
        public bool Disable(string disableReasonTip)
        {
            return this.Disable(DisableReason.BusinessRule, disableReasonTip);
        }

        /// <summary>
        /// Блокировать команду   
        /// </summary>
        /// <param name="disableReason">Тип причины</param>
        /// <param name="disableReasonTip"></param>
        /// <returns>всегда false</returns>
        public bool Disable(DisableReason disableReason, string disableReasonTip)
        {
            this.DisableReason = DisableReason.BusinessRule;
            this.DisableReasonTip = disableReasonTip;
            return false;
        }
    }
}
