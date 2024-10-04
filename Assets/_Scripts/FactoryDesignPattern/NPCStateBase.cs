using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCStateBase
{
    protected FactoryDesignPattern fsm;

    public NPCStateBase(FactoryDesignPattern fsm)
    {
        this.fsm = fsm;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}




