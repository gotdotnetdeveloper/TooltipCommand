using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly WeakAction<T> _execute;
        private readonly WeakFuncInfo<CanExecuteInfo, T, bool> _canExecute;
        private string _disableReasonTip;
        private DisableReason _disableReason;

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="execute">Метод исполнения. ЗАМЕЧАНИЕ: Зависит от флага keepTargetAlive. Если нужна жесткая ссылка, то keepTargetAlive = true. </param>
        /// <param name="canExecuteSaveEmployeesMethod"></param>
        /// <param name="keepTargetAlive">Если true, Action будет оставлен жесткой ссылкой, иначе - слабой ссылкой (для лучшей работы сборщика памяти GC).</param>
        /// <exception cref="T:System.ArgumentNullException"> Если аргумент = null.</exception>
        public ViewModelCommand(Action<T> execute, object canExecuteSaveEmployeesMethod, bool keepTargetAlive = false)
          : this(execute, (Func<CanExecuteInfo, T, bool>)null, keepTargetAlive)
        {
        }

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
            this._execute = new WeakAction<T>(execute, keepTargetAlive);
            if (canExecute == null)
                return;
            this._canExecute = new WeakFuncInfo<CanExecuteInfo,T, bool>(canExecute, keepTargetAlive);
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


        /// <summary>
        /// Событие, зажигается, когда нужно проверить CanExecute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this._canExecute == null)
                    return;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (this._canExecute == null)
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
                CanExecuteInfo canExecuteInfo = new CanExecuteInfo((ICommand)this);
                if (this._canExecute == null)
                return true;

            if (this._canExecute.IsStatic || this._canExecute.IsAlive)
            {
                if (parameter == null && typeof(T).IsValueType)
                        flag = this._canExecute.Execute(canExecuteInfo,default(T));
                else if (parameter == null || parameter is T)
                    flag = this._canExecute.Execute(canExecuteInfo,(T)parameter);
                }
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
        public bool CanExecute(CanExecuteInfo canExecuteInfo, object parameter)
        {
            bool flag = false;
            try
            {
                if (this._canExecute == null)
                    return true;

                if (this._canExecute.IsStatic || this._canExecute.IsAlive)
                {
                    if (parameter == null && typeof(T).IsValueType)
                        flag = this._canExecute.Execute(canExecuteInfo, default(T));
                    else if (parameter == null || parameter is T)
                        flag = this._canExecute.Execute(canExecuteInfo, (T)parameter);
                }
                return flag;
            }
            catch (Exception ex)
            {
                return this.OnCanExecuteException(ex);
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
                parameter1 = Convert.ChangeType(parameter, typeof(T), (IFormatProvider)null);
            if (!this.CanExecute(parameter1) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
                return;
            if (parameter1 == null)
            {
                if (typeof(T).IsValueType)
                    this._execute.Execute(default(T));
                else
                    this._execute.Execute((T)parameter1);
            }
            else
                this._execute.Execute((T)parameter1);
        }

        /// <summary>Обработчик ошибки, возникшей в CanExecute()</summary>
        protected bool OnCanExecuteException(Exception e)
        {
            this.DisableReason = DisableReason.Error;
            this.DisableReasonTip = e.Message;
            return false;
        }
    }
}
