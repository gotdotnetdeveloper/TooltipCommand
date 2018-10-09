using System;
using System.Reflection;
using GalaSoft.MvvmLight.Helpers;

namespace ToolTipCommand
{
    /// <summary>
    /// Команда типизированная со слабой ссылкой.  Владелец может собирать мусор в любое время.
    /// Принцип работы со слабыми ссылками описан http://galasoft.ch/s/mvvmweakaction.
    /// </summary>
    /// <typeparam name="T">Тип передаваемых параметров.</typeparam>
    /// <typeparam name="TResult">Тип возращаемых параметров.</typeparam>
    /// <typeparam name="TCanExecuteInfo">Контекст команды для CanExecute.</typeparam>
    public class WeakFuncInfo<TCanExecuteInfo, T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult where TCanExecuteInfo : CanExecuteInfo
    {
        private Func<TCanExecuteInfo, T, TResult> _staticFunc;

        /// <summary>
        /// Имя слабой ссылки
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (_staticFunc != null)
                    return _staticFunc.GetMethodInfo().Name;
                return Method.Name;
            }
        }

       /// <summary>
       /// Получает значение, указывающее, жив ли еще владелец T, или если он был собран сборщиком мусора.
       /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null)
                    return false;
                if (_staticFunc == null)
                    return Reference.IsAlive;
                if (Reference != null)
                    return Reference.IsAlive;
                return true;
            }
        }

        /// <summary>Инициализация экземпляра со слабой ссылкой.</summary>
        /// <param name="func">Функция, которая ассоциирована с экземпляром.</param>
        /// <param name="keepTargetAlive"> True=если использовать как жесткую ссылку (требует ручного контроля, возможна утечка памяти)</param>
        public WeakFuncInfo(Func<TCanExecuteInfo, T, TResult> func, bool keepTargetAlive = false)
          : this(func?.Target, func, keepTargetAlive)
        {
        }

        /// <summary>Инициализация экземпляра со слабой ссылкой.</summary>
        /// <param name="target">Функция владельца.</param>
        /// <param name="func">Функция, которая ассоциирована с экземпляром.</param>
        /// <param name="keepTargetAlive">True=если использовать как жесткую ссылку (требует ручного контроля, возможна утечка памяти) </param>
        public WeakFuncInfo(object target, Func<TCanExecuteInfo ,T, TResult> func, bool keepTargetAlive = false)
        {
            if (func.GetMethodInfo().IsStatic)
            {
                _staticFunc = func;
                if (target == null)
                    return;
                Reference = new WeakReference(target);
            }
            else
            {
                Method = func.GetMethodInfo();
                FuncReference = new WeakReference(func.Target);
                LiveReference = keepTargetAlive ? func.Target : null;
                Reference = new WeakReference(target);
            }
        }

        /// <summary>
        /// Выполняет Func. Это происходит только в том случае, если владелец Func все еще жив. Параметр Func установлен в значение по умолчанию (T).
        /// </summary>
        /// <returns>Результат Func, сохраненный в качестве ссылки.</returns>
        public new TResult Execute()
        {
            return Execute(default(T));
        }

        /// <summary>
        /// Выполняет Func. Это происходит только в том случае, если владелец Func все еще жив.
        /// </summary>
        /// <param name="parameter">Параметр, который должен быть передан в действие.</param>
        /// <returns>Результат Func, сохраненный в качестве ссылки.</returns>
        public TResult Execute(T parameter)
        {
            return Execute(default(TCanExecuteInfo), default(T));
        }

        /// <summary>
        ///  Выполняет Func. Это происходит только в том случае, если владелец Func все еще жив.
        /// </summary>
        /// <param name="canExecuteInfo">Контекст команды для CanExecute</param>
        /// <param name="parameter">Параметр, который должен быть передан в действие.</param>
        /// <returns>Результат Func, сохраненный в качестве ссылки.</returns>
        public TResult Execute(TCanExecuteInfo canExecuteInfo, T parameter)
        {
            if (_staticFunc != null)
                return _staticFunc(canExecuteInfo, parameter);
            object funcTarget = FuncTarget;
            if (!IsAlive || Method == null || LiveReference == null && FuncReference == null || funcTarget == null)
                return default(TResult);
            // ReSharper disable once RedundantExplicitArraySize
            return (TResult)Method.Invoke(funcTarget, new object[2]
            {
                canExecuteInfo,  parameter
            });
        }

        /// <summary>
        /// Выполняет Func с параметром объекта type. Этот параметр будет передан в T.
        /// Может быть полезна, если хранится несколько экземпляров WeakFunc {T}, но заранее не знаете, какой тип T представляет.
        /// </summary>
        /// <param name="parameter">Параметр, который будет передан Func после того, как будет приведен к типу T.</param>
        /// <returns>Результат выполнения в качестве объекта, подлежащего передаче в T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            return Execute((T)parameter);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}
