using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetFactory
{
    public class PredicateAction
    {
        public PredicateAction(Action execute) : this(execute, () => true) { }
        public PredicateAction(Action execute, Func<bool> predicate)
        {
            Execute = execute;
            Predicate = predicate;
        }

        public Action Execute { get; }
        public Func<bool> Predicate { get; }

        public bool TryExecute()
        {
            bool predicate = Predicate.Invoke();

            if (predicate)
                Execute.Invoke();

            return predicate;
        }
    }
    public class PredicateAction<T>
    {
        public PredicateAction(Action<T> execute) : this(execute, _ => true) { }
        public PredicateAction(Action<T> execute, Predicate<T> predicate)
        {
            Execute = execute;
            Predicate = predicate;
        }

        public Action<T> Execute { get; }
        public Predicate<T> Predicate { get; }

        public bool TryExecute(T arg)
        {
            bool predicate = Predicate.Invoke(arg);

            if (predicate)
                Execute.Invoke(arg);

            return predicate;
        }
    }
}
