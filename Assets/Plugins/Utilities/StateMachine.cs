using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Plugins.Utilities
{
    public interface IStateMachine<T> where T : IMachineState
    {
        T Current { get; }
        TState Get<TState>() where TState : MonoBehaviour, T;
        void SetState<TState>() where TState : MonoBehaviour, T;
        void SetState(T state);
    }

    public class StateMachine<T> : MonoBehaviour, IStateMachine<T> where T : IMachineState
    {
        [SerializeField] private T _current;
        [SerializeField] private List<MonoBehaviour> _states = new List<MonoBehaviour>();

        public T Current
        {
            get { return _current; }
            protected set { _current = value; }
        }

        public TState Get<TState>() where TState : MonoBehaviour, T
        {
            var state = _states.OfType<TState>().FirstOrDefault();
            if (state != null) return state;
            state = GetComponent<TState>() ?? gameObject.AddComponent<TState>();
            _states.Add(state);
            return state;
        }

        public void SetState<TState>() where TState : MonoBehaviour, T
        {
            SetState(Get<TState>());
        }

        public virtual void SetState(T state)
        {
            if (Current != null) Current.enabled = false;
            Current = state;
            Current.enabled = true;
        }
    }

    public interface IMachineState
    {
        bool enabled { get; set; }
    }
}
