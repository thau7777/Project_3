using System.Collections; 
using UnityEngine;


namespace Turnbase
{
    public class CharacterStateMachine : MonoBehaviour
    {
        public Character character;

        public BattleManager battleManager;

        public BaseState currentState;
        public WaitingState waitingState;
        public ReadyState readyState;
        public ReadyStateSkill readyStateSkill;
        public AttackingState attackingState;
        public TakingDamageState takingDamageState;
        public DeadState deadState;
        public ParryingState parryingState;
        public InterruptedState interruptedState;
        public StunnedState stunnedState;
        public AvoidState avoidState;

        private InputActions controls;
        public PlayerTurnBasedActions inputLogic = new PlayerTurnBasedActions();



        private void Awake()
        {
            character = GetComponent<Character>();

            battleManager = FindFirstObjectByType<BattleManager>();

            controls = new InputActions();

            controls.PlayerTurnBased.SetCallbacks(inputLogic);


            waitingState = new WaitingState(this);
            //readyState = new ReadyState(this);
            attackingState = new AttackingState(this);
            takingDamageState = new TakingDamageState(this);
            deadState = new DeadState(this);
            parryingState = new ParryingState(this);
            interruptedState = new InterruptedState(this);
            stunnedState = new StunnedState(this);
            avoidState = new AvoidState(this);
        }

        void Start()
        {
            readyState = new ReadyState(this, inputLogic);
            SwitchState(waitingState);

        }

        void Update()
        {
            if (currentState != null)
            {
                currentState.OnUpdate();
            }
        }

        public void SwitchState(BaseState newState)
        {
            if (currentState is DeadState)
            {
                return;
            }

            if (currentState != null)
            {
                currentState.OnExit();
            }
            currentState = newState;
            currentState.OnEnter();
        }

        void OnEnable()
        {
            if (controls == null) controls = new InputActions();
            controls.Enable();
        }

        void OnDisable()
        {
            controls.Disable();
        }



    }

}
