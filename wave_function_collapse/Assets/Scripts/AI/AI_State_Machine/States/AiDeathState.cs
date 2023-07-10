using Pathfinding;
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
        agent.GetComponent<AIDestinationSetter>().enabled = false;
        agent.GetComponent<AIPath>().enabled = false;
        agent.GetComponent<Seeker>().enabled = false;
    }

    public void Update(AiAgent agent)
    {

        agent.GetComponent<AIDestinationSetter>().enabled = false;
        agent.GetComponent<AIPath>().enabled = false;
        agent.GetComponent<Seeker>().enabled = false; 
    }

    public void Exit(AiAgent agent)
    {
        Debug.Log("lalalal");
    }
}
