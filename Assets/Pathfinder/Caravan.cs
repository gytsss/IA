using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;



public class Caravan : BaseAgent
{
    public int food = 10;
    
    private void Start()
    {
        base.Start();

        SetIsMiner(false);
        
        fsm.AddBehaviour<IdleState>(States.Idle, onTickParameters: () => { return new object[] {  GetStart() }; });

        fsm.SetTransition(States.Idle, Flags.OnStart, States.WaitMine);

        fsm.ForceState(States.Idle);
    }
    
    private object[] MoveToMineTickParameters()
    {
        return new object[]
            { this as Caravan, this.transform, travelTime, distanceBetweenNodes, GetStartNode() };
    }
    
    private object[] DepositFoodTickParameters()
    {
        return new object[] { this, currentMine };
    }
    
    
    private object[] AlarmTickParameters()
    {
        return new object[] { this };
    }

    public void GetMapInputValues()
    {
        SetStart(true);
    }


    protected override void AddStates()
    {
       fsm.AddBehaviour<WaitMineState>(States.WaitMine, onTickParameters: () => { return new object[] { gameManager }; });
        fsm.AddBehaviour<MoveToMineState>(States.MoveToMine, onTickParameters: MoveToMineTickParameters);
    }

    public override void AddTransitions()
    {
        fsm.SetTransition(States.WaitMine, Flags.OnMineFind, States.MoveToMine);
        //fsm.SetTransition(CaravanStates.DepositFood, CaravanFlags.OnFoodDeposit, CaravanStates.ReturnHome);
        //fsm.SetTransition(CaravanStates.ReturnHome, CaravanFlags.OnHome, CaravanStates.Idle);

    }
    
}

    