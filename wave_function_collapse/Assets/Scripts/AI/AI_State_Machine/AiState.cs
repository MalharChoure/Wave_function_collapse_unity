using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiStateType
{
	Idle,
	Patrol,
	Chase,
	Attack,
	Dead
}

public interface AiState 
{
    AiStateType GetStateType();
	void Enter(AiAgent agent);
	void Update(AiAgent agent);
	void Exit(AiAgent agent);
}
