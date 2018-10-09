using System;
using System.Reflection;

namespace ToolTipCommand
{
    /// <summary>
    /// Слабая ссылка с CanExecuteInfo
    /// </summary>
    /// <typeparam name="TResult">Тип возращаемых параметров.</typeparam>
    /// <typeparam name="TCanExecuteInfo">Информация о CanExecute</typeparam>
    public class WeakFuncInfo<TCanExecuteInfo, TResult> where TCanExecuteInfo : CanExecuteInfo
    {
        private Func<TCanExecuteInfo, TResult> _staticFunc;

        /// <summary>
        /// Описание метода <see cref="T:System.Reflection.MethodInfo"/> соответствующие WeakFunc. Метод передается в конструктор.
        /// </summary>
        protected MethodInfo Method { get; set; }

        /// <summary>
        /// Является ли слабая ссылка статичным типом static
        /// </summary>
        public bool IsStatic => _staticFunc != null;

        /// <summary>
        /// Имя метода который соответствует слабой ссылки.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                    return _staticFunc.GetMethodInfo().Name;
                return Method.Name;
            }
        }

        /// <summary>
        /// Получает или задает значение WeakReference для этой цели действия WeakFunc. Это не обязательно то же самое, что
        /// </summary>
        protected WeakReference FuncReference { get; set; }

        /// <summary>
        /// Сохраняет как жесткую ссылку. Это используется в отношении конструктора этого экземпляра и только если
        /// параметр keepTargetAlive конструктора является истинным.
        /// </summary>
        protected object LiveReference { get; set; }

        /// <summary>
        /// Получает или задает значение WeakReference для цели, переданной при построении WeakFunc, например, если метод анонимный.
        /// </summary>
        protected WeakReference Reference { get; set; }

        /// <summary>Инициализирует пустой экземпляр класса WeakFunc.</summary>
        protected WeakFuncInfo()
        {}

        /// <summary>Инициализирует новый экземпляр класса WeakFunc.</summary>
        /// <param name="func">Функция, которая ассоциирована с экземпляром.</param>
        /// <param name="keepTargetAlive">True=если использовать как жесткую ссылку (требует ручного контроля, возможна утечка памяти) </param>
        public WeakFuncInfo(Func<TCanExecuteInfo,TResult> func, bool keepTargetAlive = false)
            // ReSharper disable once MergeConditionalExpression
          : this(func == null ? null : func.Target, func, keepTargetAlive)
        {
        }

        /// <summary>Инициализирует новый экземпляр класса WeakFunc.</summary>
        /// <param name="target">Функция владельца.</param>
        /// <param name="func">Func, который будет связан с этим экземпляром</param>
        /// <param name="keepTargetAlive">True=если использовать как жесткую ссылку (требует ручного контроля, возможна утечка памяти). </param>
        public WeakFuncInfo(object target, Func<TCanExecuteInfo,TResult> func, bool keepTargetAlive = false)
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
        /// Получает значение, указывающее, жив ли еще владелец T, или если он был собран сборщиком мусора.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null && LiveReference == null)
                    return false;
                if (_staticFunc != null)
                {
                    if (Reference != null)
                        return Reference.IsAlive;
                    return true;
                }
                if (LiveReference != null)
                    return true;
                if (Reference != null)
                    return Reference.IsAlive;
                return false;
            }
        }

        /// <summary>
        /// Получает владельца Func. Этот объект хранится как WeakReference
        /// </summary>
        public object Target
        {
            get
            {
                if (Reference == null)
                    return null;
                return Reference.Target;
            }
        }

        /// <summary>
        /// Получает владельца Func, который был передан как параметр. Это не обязательно то же самое, что, например, если метод анонимный.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (LiveReference != null)
                    return LiveReference;
                if (FuncReference == null)
                    return null;
                return FuncReference.Target;
            }
        }

        /// <summary>
        /// Выполняет Func. Это происходит только в том случае, если владелец Func все еще жив. Параметр Func установлен в значение по умолчанию (T).
        /// </summary>
        /// <returns>Результат Func, сохраненный в качестве ссылки.</returns>
        public TResult Execute()
        {
            return Execute(default(TCanExecuteInfo));
        }

        /// <summary>
        /// Выполняет Func. Это происходит только в том случае, если владелец Func все еще жив. Параметр Func установлен в значение по умолчанию (T).
        /// </summary>
        /// <param name="parameter">Информация о CanExecute.</param>
        /// <returns>Результат Func, сохраненный в качестве ссылки.</returns>
        public TResult Execute(TCanExecuteInfo parameter)
        {
            if (_staticFunc != null)
                return _staticFunc(parameter);
            object funcTarget = FuncTarget;

            if (!IsAlive || Method == null || LiveReference == null && FuncReference == null ||
                funcTarget == null)
                return default(TResult);

            // ReSharper disable once RedundantExplicitArraySize
            return (TResult) Method.Invoke(funcTarget, new object[1]
            {
                parameter
            });
        }

        /// <summary>Устанавливает ссылку, которую этот экземпляр сохраняет в null.</summary>
        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            LiveReference = null;
            Method = null;
            _staticFunc = null;
        }
    }
}
