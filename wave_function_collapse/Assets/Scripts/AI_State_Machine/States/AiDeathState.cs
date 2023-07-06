using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDeathState : AiState
{
    public AiStateType GetStateType()
    {
	    return AiStateType.Dead;
    }

    public void Enter(AiAgent agent)
    {
	    throw new System.NotImplementedException();
    }

    public void Update(AiAgent agent)
    {
	    throw new System.NotImplementedException();
    }

    public void Exit(AiAgent agent)
    {
	    throw new System.NotImplementedException();
    }
}
