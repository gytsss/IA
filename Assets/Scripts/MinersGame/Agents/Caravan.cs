using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Game.FSM.States;
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
        
        fsm.AddBehaviour<IdleState>(States.Idle, onTickParameters: () => { return new object[] { this, GetStart() }; });

        fsm.SetTransition(States.Idle, BaseAgentsFlags.OnStart, States.WaitMine);

        fsm.ForceState(States.Idle);
    }
    
    private object[] MoveToMineEnterParameters()
    {
        return new object[] { this , gameManager.GetDistanceBetweenNodes(), GetStartNode() };
    }
    private object[] MoveToMineTickParameters()
    {
        return new object[]
            {  this.transform, travelTime };
    }
    
    private object[] DepositFoodTickParameters()
    {
        return new object[] { this, gameManager, food };
    }
    private object[] ReturnHomeTickParameters()
    {
        return new object[] { this, GetCurrentNode(), gameManager.GetUrbanCenterNode(), travelTime, gameManager.GetDistanceBetweenNodes() };
    }
    
    
    private object[] AlarmTickParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode(), gameManager.GetDistanceBetweenNodes() };

    }

    public void GetMapInputValues()
    {
        SetStart(true);
    }


    protected override void AddStates()
    {
       fsm.AddBehaviour<WaitMineState>(States.WaitMine,onTickParameters: () => { return new object[] { gameManager }; });
        fsm.AddBehaviour<MoveToMineState>(States.MoveToMine, onEnterParameters: MoveToMineEnterParameters ,onTickParameters: MoveToMineTickParameters);
        fsm.AddBehaviour<DepositFoodState>(States.DepositFood, onTickParameters: DepositFoodTickParameters);
        fsm.AddBehaviour<ReturnHomeState>(States.ReturnHome, onTickParameters: ReturnHomeTickParameters);
        fsm.AddBehaviour<AlarmState>(States.Alarm, onTickParameters: AlarmTickParameters);
    }

    public override void AddTransitions()
    {
        fsm.SetTransition(States.WaitMine, BaseAgentsFlags.OnMineFind, States.MoveToMine);
        fsm.SetTransition(States.WaitMine, BaseAgentsFlags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.MoveToMine, BaseAgentsFlags.OnFoodDeposit, States.DepositFood);
        fsm.SetTransition(States.MoveToMine, BaseAgentsFlags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.DepositFood, BaseAgentsFlags.OnFoodDeposit, States.ReturnHome);
        fsm.SetTransition(States.DepositFood, BaseAgentsFlags.OnNoMoreMines, States.ReturnHome);
        fsm.SetTransition(States.ReturnHome, BaseAgentsFlags.OnHome, States.WaitMine);
        fsm.SetTransition(States.ReturnHome, BaseAgentsFlags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.Alarm, BaseAgentsFlags.OnHome, States.Idle);
        fsm.SetTransition(States.Alarm, BaseAgentsFlags.OnBackToWork, States.WaitMine);
    }
    
    public int GetFood()
    {
        return food;
    }
    
}

    