using System;
using Assets.Plugins.Utilities;
using Assets.UI.MenuStates.Intro;
using UnityEngine;

namespace Assets.UI.MenuStates
{
    public class MenuStateMachine : StateMachine<MenuState>
    {
        public void Init()
        {
            SetState(Get<DownloadingDatabase>().Init(this));
        }
    }

    public abstract class MenuState : MonoBehaviour, IMachineState
    {
        protected MenuStateMachine StateMachine;
        public event Action Enabled;
        public event Action Disabled;

        public virtual MenuState Init(MenuStateMachine stateMachine)
        {
            StateMachine = stateMachine;
            return this;
        }

        protected virtual void OnEnable()
        {
            if (Enabled != null) Enabled();
        }

        protected virtual void OnDisable()
        {
            if (Disabled != null) Disabled();
        }
    }
}
