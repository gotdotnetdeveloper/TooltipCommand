using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToolTipCommand
{
    /// <summary>
    /// Слабая ссылка с CanExecuteInfo
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TCanExecuteInfo">Информация о CanExecute</typeparam>
    public class WeakFuncInfo<TCanExecuteInfo, TResult> where TCanExecuteInfo : CanExecuteInfo
    {
        private Func<TCanExecuteInfo, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Get a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this._staticFunc != null;
            }
        }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                    return this._staticFunc.GetMethodInfo().Name;
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's action's target.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Reference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference FuncReference { get; set; }

        /// <summary>
        /// Saves the <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference" /> as a hard reference. This is
        /// used in relation with this instance's constructor and only if
        /// the constructor's keepTargetAlive parameter is true.
        /// </summary>
        protected object LiveReference { get; set; }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.FuncReference" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected WeakReference Reference { get; set; }

        /// <summary>Initializes an empty instance of the WeakFunc class.</summary>
        protected WeakFuncInfo()
        {
        }

        /// <summary>Initializes a new instance of the WeakFunc class.</summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures. See
        /// http://galasoft.ch/s/mvvmweakaction. </param>
        public WeakFuncInfo(Func<TCanExecuteInfo,TResult> func, bool keepTargetAlive = false)
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
        public WeakFuncInfo(object target, Func<TCanExecuteInfo,TResult> func, bool keepTargetAlive = false)
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
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && this.Reference == null && this.LiveReference == null)
                    return false;
                if (this._staticFunc != null)
                {
                    if (this.Reference != null)
                        return this.Reference.IsAlive;
                    return true;
                }
                if (this.LiveReference != null)
                    return true;
                if (this.Reference != null)
                    return this.Reference.IsAlive;
                return false;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (this.Reference == null)
                    return (object)null;
                return this.Reference.Target;
            }
        }

        /// <summary>
        /// Gets the owner of the Func that was passed as parameter.
        /// This is not necessarily the same as
        /// <see cref="P:GalaSoft.MvvmLight.Helpers.WeakFunc`1.Target" />, for example if the
        /// method is anonymous.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (this.LiveReference != null)
                    return this.LiveReference;
                if (this.FuncReference == null)
                    return (object)null;
                return this.FuncReference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute()
        {
            return this.Execute(default(TCanExecuteInfo));
        }


        public TResult Execute(TCanExecuteInfo parameter)
        {
            if (this._staticFunc != null)
                return this._staticFunc(parameter);
            object funcTarget = this.FuncTarget;

            if (!this.IsAlive || this.Method == null || this.LiveReference == null && this.FuncReference == null ||
                funcTarget == null)
                return default(TResult);

            return (TResult) this.Method.Invoke(funcTarget, new object[1]
            {
                (object) parameter
            });
        }

        /// <summary>Sets the reference that this instance stores to null.</summary>
        public void MarkForDeletion()
        {
            this.Reference = (WeakReference)null;
            this.FuncReference = (WeakReference)null;
            this.LiveReference = (object)null;
            this.Method = (MethodInfo)null;
            this._staticFunc = (Func<TCanExecuteInfo,TResult>)null;
        }
    }
}
