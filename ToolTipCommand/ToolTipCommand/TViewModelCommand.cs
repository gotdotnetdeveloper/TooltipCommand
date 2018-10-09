using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Helpers;

namespace ToolTipCommand
{
    /// <summary>
    /// Команда типизированная.
    /// </summary>
    /// <typeparam name="T">Тип параметров для команды.</typeparam>
    /// <remarks>CommandManager класс автоматически вкл/выкл CanExecute.</remarks>
    public class ViewModelCommand<T> : ObservableObject, ICommand
    {
        #region Приватные свойства
        private readonly WeakAction<T> _execute;
        private readonly WeakFuncInfo<CanExecuteInfo, T, bool> _canExecute;
        private string _disableReasonTip;
        private DisableReason _disableReason;
        #endregion

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="execute">Метод исполнения. ЗАМЕЧАНИЕ: Зависит от флага keepTargetAlive. Если нужна жесткая ссылка, то keepTargetAlive = true. </param>
        /// <param name="keepTargetAlive">Если true, Action будет оставлен жесткой ссылкой, иначе - слабой ссылкой (для лучшей работы сборщика памяти GC).</param>
        /// <exception cref="T:System.ArgumentNullException"> Если аргумент = null.</exception>
        public ViewModelCommand(Action<T> execute, bool keepTargetAlive = false)
          : this(execute, null, keepTargetAlive)
        {}

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="execute">Метод исполнения. ЗАМЕЧАНИЕ: Зависит от флага keepTargetAlive. Если нужна жесткая ссылка, то keepTargetAlive = true. </param>
        /// <param name="canExecute"> Метод проверки возможности исполнения команды.</param>
        /// <param name="keepTargetAlive">Если true, Action будет оставлен жесткой ссылкой, иначе - слабой ссылкой (для лучшей работы сборщика памяти GC).</param>
        /// <exception cref="T:System.ArgumentNullException"> Если аргумент = null.</exception>
        public ViewModelCommand(Action<T> execute, Func<CanExecuteInfo, T, bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            _execute = new WeakAction<T>(execute, keepTargetAlive);
            if (canExecute == null)
                return;
            _canExecute = new WeakFuncInfo<CanExecuteInfo,T, bool>(canExecute, keepTargetAlive);
        }

        #region Публичные свойства
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


        /// <summary>
        /// Событие, зажигается, когда нужно проверить CanExecute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute == null)
                    return;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute == null)
                    return;
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Метод зажигает проверку выполнения команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Определение метода, которые проверяет возможность выполнения команды
        /// </summary>
        /// <param name="parameter">Параметр проверки.</param>
        /// <returns>true если проверка пройдена. false = проверка не пройдена.</returns>
        public bool CanExecute(object parameter)
        {
            bool flag = false;
            try
            {
                CanExecuteInfo canExecuteInfo = new CanExecuteInfo(this);
                if (_canExecute == null)
                    return true;

                if (_canExecute.IsStatic || _canExecute.IsAlive)
                {
                    if (parameter == null && typeof(T).IsValueType)
                        flag = _canExecute.Execute(canExecuteInfo, default(T));
                    else if (parameter == null || parameter is T)
                        flag = _canExecute.Execute(canExecuteInfo, (T)parameter);
                }
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
        /// <summary>
        /// Возможность исполнения команды.
        /// </summary>
        /// <param name="canExecuteInfo">Контекст команды для CanExecute.</param>
        /// <param name="parameter">Параметр команды для CanExecute.</param>
        /// <returns>True = Можно исполнить команду. False=нельзя исполнить.</returns>
        public bool CanExecute(CanExecuteInfo canExecuteInfo, object parameter)
        {
            bool flag = false;
            try
            {
                if (_canExecute == null)
                    return true;

                if (_canExecute.IsStatic || _canExecute.IsAlive)
                {
                    if (parameter == null && typeof(T).IsValueType)
                        flag = _canExecute.Execute(canExecuteInfo, default(T));
                    else if (parameter == null || parameter is T)
                        flag = _canExecute.Execute(canExecuteInfo, (T)parameter);
                }
                return flag;
            }
            catch (Exception ex)
            {
                return OnCanExecuteException(ex);
            }
        }

        /// <summary>
        /// Метод команды
        /// </summary>
        /// <param name="parameter">Параметры исполнения команды</param>
        public virtual void Execute(object parameter)
        {
            object parameter1 = parameter;
            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
                parameter1 = Convert.ChangeType(parameter, typeof(T), null);
            if (!CanExecute(parameter1) || _execute == null || !_execute.IsStatic && !_execute.IsAlive)
                return;
            if (parameter1 == null)
            {
                _execute.Execute(default(T));
                //if (typeof(T).IsValueType)
                //    _execute.Execute(default(T));
                //else
                //    _execute.Execute((T)parameter1);
            }
            else
                _execute.Execute((T)parameter1);
        }

        /// <summary>Обработчик ошибки, возникшей в CanExecute()</summary>
        protected bool OnCanExecuteException(Exception e)
        {
            DisableReason = DisableReason.Error;
            DisableReasonTip = e.Message;
            return false;
        }
        #endregion
    }
}
