using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimpleFSM;

public class FactoryDesignPattern : MonoBehaviour
{
    public NPCStateBase currentState;

    [Header("Conditions")]
    [SerializeField] public bool IsIdling;    
    [SerializeField] public bool IsAttaking;
    [SerializeField] public bool IsPlayerDetected;
    [SerializeField] public bool IsPlayerInRange;
    [SerializeField] public bool HasMsgDelivered;

    [Header("Messages")]
    [SerializeField] public GameObject IdlingMsg;
    [SerializeField] public GameObject GuideMsg;

    private void Start()
    {
        SetState(new IdleState(this));
        HasMsgDelivered = false;
        IsPlayerDetected = false;
        IsPlayerInRange = false;
        IdlingMsg.SetActive(true);
        GuideMsg.SetActive(false);
    }

    private void Update()
    {
        currentState?.Update();
        Debug.Log($"{currentState}");
    }

    public void SetState(NPCStateBase newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void CloseGuide()
    {
        HasMsgDelivered = true;
        if (HasMsgDelivered)
        {
            SetState(new IdleState(this));
        }
    }

    public IEnumerator IdleToPatrol()
    {
        while (true)
        {
            float waitTime = Random.Range(10f, 20f);
            yield return new WaitForSeconds(waitTime); 
            SetState(new PatrolState(this));
        }
    }

    public IEnumerator PatrolToIdle()
    {
        float waitTime = Random.Range(20f, 30f);
        yield return new WaitForSeconds(waitTime);
        SetState(new IdleState(this));
    }
}

