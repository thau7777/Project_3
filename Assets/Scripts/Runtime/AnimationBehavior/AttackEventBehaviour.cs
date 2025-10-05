using UnityEngine;

public class AttackEventBehaviour : StateMachineBehaviour
{
    private bool _hasTriggered;
    private PlayerTopDownStateDriver _stateDriver;
    [SerializeField]
    [Range(0,1f)]
    private float EndTime = 0.8f;
    [SerializeField]
    private bool isDashAtStart = true;
    // Called on first frame the state is active
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hasTriggered = false;
        _stateDriver = animator.GetComponent<PlayerTopDownStateDriver>();
        _stateDriver.OnAttackAnimStart();
    }

    // Called every frame while this state is active
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Example: fire event when reaching halfway (0.5 normalized time)
        if (!_hasTriggered && stateInfo.normalizedTime >= EndTime)
        {
            _hasTriggered = true;
            _stateDriver.ExecuteNextAttack(); // or whatever function you want
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _stateDriver.OnAttackAnimEnd();
    }
}
