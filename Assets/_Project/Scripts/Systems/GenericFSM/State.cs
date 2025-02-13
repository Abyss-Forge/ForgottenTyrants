using System;

namespace Systems.FSM
{
    public class State<EState> where EState : Enum
    {
        public EState ID { get; private set; }

        public State(EState id)
        {
            ID = id;
        }

        public delegate void DelegateNoArg();
        public DelegateNoArg OnEnter, OnExit, OnUpdate, OnFixedUpdate, OnLateUpdate;

        public State(EState id,
            DelegateNoArg onEnter,
            DelegateNoArg onExit = null,
            DelegateNoArg onUpdate = null,
            DelegateNoArg onFixedUpdate = null,
            DelegateNoArg onLateUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
            OnLateUpdate = onLateUpdate;
        }

        virtual public void Enter() => OnEnter?.Invoke();
        virtual public void Exit() => OnExit?.Invoke();
        virtual public void Update() => OnUpdate?.Invoke();
        virtual public void FixedUpdate() => OnFixedUpdate?.Invoke();
        virtual public void LateUpdate() => OnLateUpdate?.Invoke();
    }
}