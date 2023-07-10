using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAgent : MonoBehaviour
{
    public AiStateMachine stateMachine;
    public AiStateType InitialStateType;
    public Transform playerTransform;
    [SerializeField] public float maxSightDistance = 17f;

    // Start is called before the first frame update
    void Start() 
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChaseState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiIdeState());
        stateMachine.ChangeState(InitialStateType);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
