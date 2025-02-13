using Systems.FSM;
using UnityEngine;

//clase de ejemplo
public class Turnstile_Class : MonoBehaviour
{
    public enum TurnstileStates
    {
        LOCKED,
        UNLOCKED,
    }
    public class TurnstileLocked : State<TurnstileStates>
    {
        Turnstile_Class mTurnstile;
        public TurnstileLocked(Turnstile_Class turnstile)
            : base(TurnstileStates.LOCKED)
        {
            mTurnstile = turnstile;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Turnstile LOCKED. Press C key to insert a coin");
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Turnstile unlocking");
                    mTurnstile.mFsm.TransitionTo(TurnstileStates.UNLOCKED);
                }
                else
                {
                    Debug.Log("Incorrect coin");
                }
            }
        }
    }
    public class TurnstileUnlocked : State<TurnstileStates>
    {
        Turnstile_Class mTurnstile;
        public TurnstileUnlocked(Turnstile_Class turnstile)
            : base(TurnstileStates.UNLOCKED)
        {
            mTurnstile = turnstile;
        }

        public override void Enter()
        {
            Debug.Log("Turnstile LOCKED. Press C key to insert a coin");
            base.Enter();
        }

        public override void Update()
        {
            base.Update();
            if (Input.anyKeyDown)
            {
                if (!Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("Turnstile locking");
                    mTurnstile.mFsm.TransitionTo(TurnstileStates.LOCKED);
                }
            }
        }
    }

    FiniteStateMachine<TurnstileStates> mFsm = new FiniteStateMachine<TurnstileStates>();

    private void Start()
    {
        // Add the Locked state to the fsm
        mFsm.Add(new TurnstileLocked(this));

        // Add the Unlocked state to the fsm
        mFsm.Add(new TurnstileUnlocked(this));

        mFsm.TransitionTo(TurnstileStates.LOCKED);
    }

    private void Update()
    {
        mFsm.Update();
    }

    private void FixedUpdate()
    {
        mFsm.FixedUpdate();
    }
}
