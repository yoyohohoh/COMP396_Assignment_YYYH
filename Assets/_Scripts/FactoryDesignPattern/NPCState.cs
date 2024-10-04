using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : NPCStateBase
{
    public IdleState(FactoryDesignPattern fsm) : base(fsm) { }

    public override void Enter()
    {
        fsm.IsIdling = true;
        fsm.IdlingMsg.SetActive(true);
        fsm.GetComponent<Renderer>().material.color = Color.gray;
        fsm.StartCoroutine(fsm.IdleToPatrol());
    }

    public override void Exit()
    {
        fsm.IsIdling = false;
        fsm.IdlingMsg.SetActive(false);
    }

    public override void Update()
    {

    }
}

public class PatrolState : NPCStateBase
{
    public PatrolState(FactoryDesignPattern fsm) : base(fsm) { }

    public override void Enter()
    {       
        fsm.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void Exit() { }

    public override void Update()
    {
        fsm.GetComponent<NPCController>().FollowPath();
        if (fsm.IsPlayerDetected)
        {
            fsm.SetState(new ChaseState(fsm));
        }
        else if (!fsm.IsPlayerDetected && !fsm.IsPlayerInRange)
        {
            // Random time before going back to Idle
            fsm.StartCoroutine(fsm.PatrolToIdle());
        }
    }
}

public class ChaseState : NPCStateBase
{
    public ChaseState(FactoryDesignPattern fsm) : base(fsm) { }

    public override void Enter() { }

    public override void Exit() { }

    public override void Update()
    {
        fsm.GetComponent<NPCController>().Chase();
        if (fsm.IsPlayerInRange)
        {
            int randomProbability = Random.Range(0, 100);
            if (randomProbability < 20)
            {
                fsm.SetState(new AttackState(fsm));
            }
            else
            {
                fsm.SetState(new GuideState(fsm));
            }
        }
        else if (!fsm.IsPlayerDetected && !fsm.IsPlayerInRange)
        {
            fsm.SetState(new PatrolState(fsm));
        }
    }
}

public class GuideState : NPCStateBase
{
    public GuideState(FactoryDesignPattern fsm) : base(fsm) { }

    public override void Enter()
    {
        fsm.GuideMsg.SetActive(true);
        fsm.Invoke("CloseGuide", 3.0f);
    }

    public override void Exit()
    {
        fsm.GuideMsg.SetActive(false);
    }

    public override void Update() { }
}

public class AttackState : NPCStateBase
{
    public AttackState(FactoryDesignPattern fsm) : base(fsm) { }

    public override void Enter() { fsm.IsAttaking = true; }

    public override void Exit() { fsm.IsAttaking = false; }

    public override void Update()
    {
        if (!fsm.IsPlayerInRange)
        {
            fsm.SetState(new PatrolState(fsm));
        }
    }
}

