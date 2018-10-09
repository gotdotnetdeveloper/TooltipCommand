using System;
using GalaSoft.MvvmLight.Helpers;
using System.Threading;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace ToolTipCommand
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
            private readonly WeakFuncInfo<CanExecuteInfo, bool> _canExecute;
            private EventHandler _requerySuggestedLocal;

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. 
            /// Вы должны установить это параметр true, если действие вызывает замыкание </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, bool keepTargetAlive = false)
              : this(execute, (Func<bool>)null, keepTargetAlive)
            { }

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="canExecute">Функция: можно ли выполнить команду</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. Вы должны установить это
            /// параметр true, если действие вызывает замыкание </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
                : this(execute, canExecute == null ? null : (Func<CanExecuteInfo, bool>)(ctx => canExecute()), keepTargetAlive)
            { }

            /// <summary>
            /// Конструктор команды
            /// </summary>
            /// <param name="execute">Если действие вызывает закрытие,
            /// нужно установить keepTargetAlive в true, чтобы избежать побочных эффектов.</param>
            /// <param name="canExecute">Можно ли выполнить команду</param>
            /// <param name="keepTargetAlive">Если = true, действия будет
            /// сохраненна как жесткая ссылка, которая может вызвать утечку памяти. Вы должны установить это параметр true, если действие вызывает замыкание. </param>
            /// <exception cref="T:System.ArgumentNullException">Эксепшен, если аргументы = null.</exception>
            public ViewModelCommand(Action execute, Func<CanExecuteInfo, bool> canExecute, bool keepTargetAlive = false)
            {
                if (execute == null)
                    throw new ArgumentNullException(nameof(execute));
                _execute = new WeakAction(execute, keepTargetAlive);
                if (canExecute == null)
                    return;
                _canExecute = new WeakFuncInfo<CanExecuteInfo, bool>(canExecute, keepTargetAlive);
            }

            /// <summary>
            /// Событие, происходит когда CanExecute изменился.
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add
                {
                    if (_canExecute == null)
                        return;
                    EventHandler eventHandler = _requerySuggestedLocal;
                    EventHandler comparand;
                    do
                    {
                        comparand = eventHandler;
                        eventHandler = Interlocked.CompareExchange(ref _requerySuggestedLocal, comparand + value, comparand);
                    }
                    while (eventHandler != comparand);
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    if (_canExecute == null)
                        return;
                    EventHandler eventHandler = _requerySuggestedLocal;
                    EventHandler comparand;
                    do
                    {
                        comparand = eventHandler;
                        // ReSharper disable once DelegateSubtraction
                        eventHandler = Interlocked.CompareExchange(ref _requerySuggestedLocal, comparand - value, comparand);
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
                    CanExecuteInfo canExecuteInfo = new CanExecuteInfo(this);
                    if (_canExecute == null)
                    return true;

                if (_canExecute.IsStatic || _canExecute.IsAlive)
                    flag = _canExecute.Execute(canExecuteInfo );

                 
                    if (flag)
                    {
                        DisableReason = DisableReason.None;
                        DisableReasonTip = null;
                    }
                    else
                    {
                        DisableReasonTip = canExecuteInfo.DisableReasonTip;
                        DisableReason = canExecuteInfo.DisableReason;
                    }
                    return flag;
                }
                catch (Exception ex)
                {
                    return OnCanExecuteException(ex);
                }
            }
            /// <summary>Подсказка о причине недоступности контрола</summary>
            public string DisableReasonTip
            {
                get => _disableReasonTip;
                set
                {
                    if (_disableReasonTip == value)
                        return;
                    _disableReasonTip = value;
                    RaisePropertyChanged(nameof(DisableReasonTip));
                }
            }

            /// <summary>Вид причины недоступности</summary>
            public DisableReason DisableReason
            {
                get => _disableReason;
                set
                {
                    if (_disableReason == value)
                        return;
                    _disableReason = value;
                    RaisePropertyChanged(nameof(DisableReason));
                }
            }

            /// <summary>Обработчик ошибки, возникшей в CanExecute()</summary>
            protected bool OnCanExecuteException(Exception e)
            {
                DisableReason = DisableReason.Error;
                DisableReasonTip = e.Message;
                return false;
            }

            /// <summary>
            /// Определяет метод, который вызывается при вызове команды.
            /// </summary>
            public virtual void Execute(object parameter)
            {
                if (!CanExecute(parameter) || _execute == null || !_execute.IsStatic && !_execute.IsAlive)
                    return;
                _execute.Execute();
            }
        }
}
