using Systems.FSM;

public abstract class AbilityState<T> : State<EAbilityState> where T : AbilityStateMachine
{
    protected T _ability;

    public AbilityState(T ability, EAbilityState id) : base(id)
    {
        _ability = ability;
    }
}