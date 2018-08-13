using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Helpers;

namespace ToolTipCommand
{
    /// <summary>
    /// Stores an Func without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    /// <typeparam name="TResult">The type of the Func's return value.</typeparam>
    /// <typeparam name="TCanExecuteInfo">Контекст команды для CanExecute</typeparam>
    public class WeakFuncInfo<TCanExecuteInfo, T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult where TCanExecuteInfo : CanExecuteInfo
    {
        private Func<TCanExecuteInfo, T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the name of the method that this WeakFunc represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                    return this._staticFunc.GetMethodInfo().Name;
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && this.Reference == null)
                    return false;
                if (this._staticFunc == null)
                    return this.Reference.IsAlive;
                if (this.Reference != null)
                    return this.Reference.IsAlive;
                return true;
            }
        }

        /// <summary>Initializes a new instance of the WeakFunc class.</summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        public WeakFuncInfo(Func<TCanExecuteInfo, T, TResult> func, bool keepTargetAlive = false)
          : this(func == null ? (object)null : func.Target, func, keepTargetAlive)
        {
        }

        /// <summary>Initializes a new instance of the WeakFunc class.</summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        public WeakFuncInfo(object target, Func<TCanExecuteInfo ,T, TResult> func, bool keepTargetAlive = false)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                this._staticFunc = func;
                if (target == null)
                    return;
                this.Reference = new WeakReference(target);
            }
            else
            {
                this.Method = func.GetMethodInfo();
                this.FuncReference = new WeakReference(func.Target);
                this.LiveReference = keepTargetAlive ? func.Target : (object)null;
                this.Reference = new WeakReference(target);
            }
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive. The Func's parameter is set to default(T).
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public new TResult Execute()
        {
            return this.Execute(default(T));
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute(T parameter)
        {
            return this.Execute(default(TCanExecuteInfo), default(T));
        }

        public TResult Execute(TCanExecuteInfo canExecuteInfo, T parameter)
        {
            if (this._staticFunc != null)
                return this._staticFunc(canExecuteInfo, parameter);
            object funcTarget = this.FuncTarget;
            if (!this.IsAlive || this.Method == null || this.LiveReference == null && this.FuncReference == null || funcTarget == null)
                return default(TResult);
            return (TResult)this.Method.Invoke(funcTarget, new object[2]
            {
                canExecuteInfo,  parameter
            });
        }

        /// <summary>
        /// Executes the Func with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="M:GalaSoft.MvvmLight.Helpers.IExecuteWithObject.ExecuteWithObject(System.Object)" />
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the Func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            return (object)this.Execute((T)parameter);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            this._staticFunc = (Func<TCanExecuteInfo, T, TResult>)null;
            base.MarkForDeletion();
        }
    }
}
