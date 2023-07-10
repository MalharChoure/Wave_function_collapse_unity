using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChaseState : AiState
{
	public AIPath aiPath;
	public AiStateType GetStateType()
	{
		return AiStateType.Chase;
	}

	public void Enter(AiAgent agent)
	{
		aiPath = agent.GetComponent<AIPath>();
		agent.GetComponent<AIDestinationSetter>().enabled = true;
		agent.GetComponent<AIPath>().enabled = true;
		agent.GetComponent<Seeker>().enabled = true;
	}

	public void Update(AiAgent agent)
	{
		agent.GetComponent<AIDestinationSetter>().enabled = true;
		agent.GetComponent<AIPath>().enabled = true;
		agent.GetComponent<Seeker>().enabled = true;
		if (aiPath.reachedEndOfPath)
		{
			// agent.transform.LookAt(agent.GetComponent<AIDestinationSetter>().target);
		}
	}

	public void Exit(AiAgent agent)
	{
        agent.GetComponent<AIDestinationSetter>().enabled = false;
        agent.GetComponent<AIPath>().enabled = false;
        agent.GetComponent<Seeker>().enabled = false;
    }
}
