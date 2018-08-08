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
        /// A command whose sole purpose is to relay its functionality to other
        /// objects by invoking delegates. The default return value for the CanExecute
        /// method is 'true'.  This class does not allow you to accept command parameters in the
        /// Execute and CanExecute callback methods.
        /// </summary>
        /// <remarks>If you are using this class in WPF4.5 or above, you need to use the
        /// GalaSoft.MvvmLight.CommandWpf namespace (instead of GalaSoft.MvvmLight.Command).
        /// This will enable (or restore) the CommandManager class which handles
        /// automatic enabling/disabling of controls based on the CanExecute delegate.</remarks>
        public class AlertCommand : ObservableObject, ICommand 
        {
            private string _disableReasonTip;
            private DisableReason _disableReason;
            private readonly WeakAction _execute;
            private readonly WeakFunc<CanExecuteInfo, bool> _canExecute;
            private EventHandler _requerySuggestedLocal;

            /// <summary>
            /// Initializes a new instance of the RelayCommand class that
            /// can always execute.
            /// </summary>
            /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
            /// you must set keepTargetAlive to true to avoid side effects. </param>
            /// <param name="keepTargetAlive">If true, the target of the Action will
            /// be kept as a hard reference, which might cause a memory leak. You should only set this
            /// parameter to true if the action is causing a closure. See
            /// http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
            public AlertCommand(Action execute, bool keepTargetAlive = false)
              : this(execute, (Func<bool>)null, keepTargetAlive)
            {
            }

            /// <summary>Initializes a new instance of the RelayCommand class.</summary>
            /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
            /// you must set keepTargetAlive to true to avoid side effects. </param>
            /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
            /// you must set keepTargetAlive to true to avoid side effects. </param>
            /// <param name="keepTargetAlive">If true, the target of the Action will
            /// be kept as a hard reference, which might cause a memory leak. You should only set this
            /// parameter to true if the action is causing a closures. See
            /// http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
            public AlertCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
                : this(execute, canExecute == null ? (Func<CanExecuteInfo, bool>)null : (Func<CanExecuteInfo, bool>)(ctx => canExecute()), keepTargetAlive)
            {

            }
            /// <summary>Initializes a new instance of the RelayCommand class.</summary>
            /// <param name="execute">The execution logic. IMPORTANT: If the action causes a closure,
            /// you must set keepTargetAlive to true to avoid side effects. </param>
            /// <param name="canExecute">The execution status logic.  IMPORTANT: If the func causes a closure,
            /// you must set keepTargetAlive to true to avoid side effects. </param>
            /// <param name="keepTargetAlive">If true, the target of the Action will
            /// be kept as a hard reference, which might cause a memory leak. You should only set this
            /// parameter to true if the action is causing a closures. See
            /// http://galasoft.ch/s/mvvmweakaction. </param>
            /// <exception cref="T:System.ArgumentNullException">If the execute argument is null.</exception>
            public AlertCommand(Action execute, Func<CanExecuteInfo, bool> canExecute, bool keepTargetAlive = false)
            {
                if (execute == null)
                    throw new ArgumentNullException(nameof(execute));
                this._execute = new WeakAction(execute, keepTargetAlive);
                if (canExecute == null)
                    return;
                this._canExecute = new WeakFunc<CanExecuteInfo, bool>(canExecute, keepTargetAlive);
            }

            /// <summary>
            /// Occurs when changes occur that affect whether the command should execute.
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
            /// Raises the <see cref="E:GalaSoft.MvvmLight.CommandWpf.RelayCommand.CanExecuteChanged" /> event.
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }

            /// <summary>
            /// Defines the method that determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">This parameter will always be ignored.</param>
            /// <returns>true if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                bool flag = false;

                if (this._canExecute == null)
                    return true;

                if (this._canExecute.IsStatic || this._canExecute.IsAlive)
                    flag = this._canExecute.Execute();

                try
                {
                    CanExecuteInfo canExecuteInfo = new CanExecuteInfo((ICommand)this);
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
            /// <param name="e"></param>
            /// <returns></returns>
            protected bool OnCanExecuteException(Exception e)
            {
                this.DisableReason = DisableReason.Error;
                this.DisableReasonTip = e.Message;
                return false;
            }

            /// <summary>
            /// Defines the method to be called when the command is invoked.
            /// </summary>
            /// <param name="parameter">This parameter will always be ignored.</param>
            public virtual void Execute(object parameter)
            {
                if (!this.CanExecute(parameter) || this._execute == null || !this._execute.IsStatic && !this._execute.IsAlive)
                    return;
                this._execute.Execute();
            }
        }
    }
}
