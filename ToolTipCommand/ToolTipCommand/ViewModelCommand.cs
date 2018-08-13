using System;
using GalaSoft.MvvmLight.Helpers;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace ToolTipCommand
{

namespace GalaSoft.MvvmLight.CommandWpf
    {
        /// <summary>
        /// Mvvm Команда, наследник ICommand. Единственная цель которой - передать функциональность (ExecuteCommand) другим
        /// объектам путем вызова делегатов. 
        /// </summary>
        /// CommandManager автоматически  вкл/выкл CanExecute для WPF.
        public class ViewModelCommand : ObservableObject, ICommand 
        {
            private string _disableReasonTip;
            private DisableReason _disableReason;
            private readonly WeakAction _execute;
            //private readonly WeakFunc<CanExecuteInfo, bool> _canExecute;
            private readonly WeakFuncInfo<CanExecuteInfo, bool> _canExecute;
            private EventHandler _requerySuggestedLocal;

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. Вы должны установить это
            /// параметр true, если действие вызывает замыкание
            /// http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, bool keepTargetAlive = false)
              : this(execute, (Func<bool>)null, keepTargetAlive)
            {
            }

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. Вы должны установить это
            /// параметр true, если действие вызывает замыкание
            /// http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
                : this(execute, canExecute == null ? (Func<CanExecuteInfo, bool>)null : (Func<CanExecuteInfo, bool>)(ctx => canExecute()), keepTargetAlive)
            {

            }

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="canExecute">Можно ли выполнить команду</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. Вы должны установить это
            /// параметр true, если действие вызывает замыкание  http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, Func<CanExecuteInfo, bool> canExecute, bool keepTargetAlive = false)
            {
                if (execute == null)
                    throw new ArgumentNullException(nameof(execute));
                this._execute = new WeakAction(execute, keepTargetAlive);
                if (canExecute == null)
                    return;
                this._canExecute = new WeakFuncInfo<CanExecuteInfo, bool>(canExecute, keepTargetAlive);
            }

            /// <summary>
            /// Событие, происходит когда CanExecute изменился.
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add
                {
                    if (this._canExecute == null)
                        return;
                    EventHandler eventHandler = this._requerySuggestedLocal;
                    EventHandler comparand;
                    do
                    {
                        comparand = eventHandler;
                        eventHandler = Interlocked.CompareExchange<EventHandler>(ref this._requerySuggestedLocal, comparand + value, comparand);
                    }
                    while (eventHandler != comparand);
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    if (this._canExecute == null)
                        return;
                    EventHandler eventHandler = this._requerySuggestedLocal;
                    EventHandler comparand;
                    do
                    {
                        comparand = eventHandler;
                        eventHandler = Interlocked.CompareExchange<EventHandler>(ref this._requerySuggestedLocal, comparand - value, comparand);
                    }
                    while (eventHandler != comparand);
                    CommandManager.RequerySuggested -= value;
                }
            }

            /// <summary>
            /// Поднять событие CanExecute.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }

            /// <summary>
            /// Определяет метод, который определяет, может ли команда выполняться в текущем состоянии.
            /// </summary>
            /// <param name="parameter">Этот параметр всегда будет игнорироваться.</param>
            /// <returns>true, если эта команда может быть выполнена; в противном случае - false.</returns>
            public bool CanExecute(object parameter)
            {
                bool flag = false;

                try
                {
                    CanExecuteInfo canExecuteInfo = new CanExecuteInfo((ICommand)this);
                    if (this._canExecute == null)
                    return true;

                if (this._canExecute.IsStatic || this._canExecute.IsAlive)
                    flag = this._canExecute.Execute(canExecuteInfo );

                 
                    if (flag)
                    {
                        this.DisableReason = DisableReason.None;
                        this.DisableReasonTip = (string)null;
                    }
                    else
                    {
                        this.DisableReasonTip = canExecuteInfo.DisableReasonTip;
                        this.DisableReason = canExecuteInfo.DisableReason;
                    }
                    return flag;
                }
                catch (Exception ex)
                {
                    return this.OnCanExecuteException(ex);
                }
            }
            /// <summary>Подсказка о причине недоступности контрола</summary>
            public string DisableReasonTip
            {
                get
                {
                    return this._disableReasonTip;
                }
                set
                {
                    if (!(this._disableReasonTip != value))
                        return;
                    this._disableReasonTip = value;
                    this.RaisePropertyChanged(nameof(DisableReasonTip));
                }
            }

            /// <summary>Вид причины недоступности</summary>
            public DisableReason DisableReason
            {
                get
                {
                    return this._disableReason;
                }
                set
                {
                    if (this._disableReason == value)
                        return;
                    this._disableReason = value;
                    this.RaisePropertyChanged(nameof(DisableReason));
                }
            }

            /// <summary>Обработчик ошибки, возникшей в CanExecute()</summary>
            protected bool OnCanExecuteException(Exception e)
            {
                this.DisableReason = DisableReason.Error;
                this.DisableReasonTip = e.Message;
                return false;
            }

            /// <summary>
            /// Определяет метод, который вызывается при вызове команды.
            /// </summary>
            public virtual void Execute(object parameter)
            {
                if (!this.CanExecute(parameter) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
                    return;
                this._execute.Execute();
            }
        }
    }
}
