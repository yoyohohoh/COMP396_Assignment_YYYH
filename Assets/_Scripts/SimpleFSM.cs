using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;



public class SimpleFSM : MonoBehaviour
{
    public NPCState currentState;

    [Header("Conditions")]
    [SerializeField] public bool IsPlayerDetected;
    [SerializeField] public bool IsPlayerInRange;
    [SerializeField] public bool HasMsgDelivered;

    [Header("Messages")]
    [SerializeField] public GameObject IdlingMsg;
    [SerializeField] public GameObject GuideMsg;

    public enum NPCState
    {
        Idle,
        Patrol,
        Chase,
        Guide,
        Attack
    }

    private void Start()
    {
        currentState = NPCState.Idle;
        StartCoroutine(IdleToPatrol());

        HasMsgDelivered = false;
        IsPlayerDetected = false;
        IsPlayerInRange = false;
        IdlingMsg.SetActive(true);
        GuideMsg.SetActive(false);   
    }

    private void FixedUpdate()
    {
        Debug.Log($"Current Status: {currentState}");
    }

    void Update()
    {
        switch(currentState)
        {
            case NPCState.Idle:
                HandleIdle();
                break;
            case NPCState.Patrol:
                HandlePatrol();
                break;
            case NPCState.Chase:
                HandleChase();
                break;
            case NPCState.Guide:
                HandleGuide();
                break;
            case NPCState.Attack:
                HandleAttack();
                break;
            
            default:
                Debug.Log($"NPC State Update Error: {currentState} is invalid");
                break;

        }
    }

    private void HandleIdle()
    {
        IdlingMsg.SetActive(true);
        this.GetComponent<Renderer>().material.color = Color.gray;
        HasMsgDelivered = false;
    }
    private void HandlePatrol()
    {
        this.GetComponent<NPCController>().FollowPath();
        IdlingMsg.SetActive(false);
        this.GetComponent<Renderer>().material.color = Color.red;
        if (IsPlayerDetected)
        {
            currentState = NPCState.Chase;
        }
        else if (!IsPlayerDetected && !IsPlayerInRange)
        {
            // Random time before going back to Idle
            StartCoroutine(PatrolToIdle());
        }
    }
    private void HandleChase()
    {
        this.GetComponent<NPCController>().Chase();
        if (IsPlayerInRange)
        {
            int randomProbability = Random.Range(0, 100);
            Debug.Log($"The probabilty is {randomProbability}");
            if(randomProbability < 20)
            {
                currentState = NPCState.Attack;
            }
            else
            {
                currentState = NPCState.Guide;
            }
        }
        else if (!IsPlayerDetected && !IsPlayerInRange)
        {
            currentState = NPCState.Patrol;
        }
    }
    private void HandleGuide()
    {
        GuideMsg.SetActive(true);
        Invoke("CloseGuide", 3.0f);
    }

    private void CloseGuide()
    {
        HasMsgDelivered = true;
        GuideMsg.SetActive(false);
        if (HasMsgDelivered)
        {
            currentState = NPCState.Idle;
        }
    }
    private void HandleAttack()
    {
        if (!IsPlayerInRange)
        {
            currentState = NPCState.Patrol;
        }
    }

    private IEnumerator IdleToPatrol()
    {
        while (true) // Keep looping while in idle
        {
            float waitTime = Random.Range(20f, 30f); // Random time between 2 to 5 seconds
            yield return new WaitForSeconds(waitTime); // Wait for the random time
            currentState = NPCState.Patrol; // Change state to patrol
        }
    }

    private IEnumerator PatrolToIdle()
    {
        float waitTime = Random.Range(20f, 30f); // Random time between 2 to 5 seconds
        yield return new WaitForSeconds(waitTime); // Wait for the random time
        currentState = NPCState.Idle; // Change state to idle
    }

}
