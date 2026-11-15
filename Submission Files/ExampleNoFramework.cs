
void UpdateState()
{
    if (Health < 30)
    {
        ChangeState(State.Retreat);
    }
    else if (Ally.Health < 40)
    {
        ChangeState(State.HealAlly);
    }
    else if (Enemy.IsVisible)
    {
        if (Enemy.IsInRange)
        {
            ChangeState(State.Attack);
        }
        else
        {
            ChangeState(State.Approach);
        }
    }
    else
    {
        ChangeState(State.Patrol);
    }
}



GameContext context;        // Received from Character/Agent Object
List<State> validStates;    // Received from State Object

void UpdateState()
{
    StateDecision decision = RAG_StateSelector.SelectState( 
        context, 
        behaviorProfile, 
        validStates
    );

    if (validStates.Contains(decision.State))
        ChangeState(decision.State);
}



