using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Game.FSM.States;
using Pathfinder;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class Miner : BaseAgent
{
    
    public int maxGold = 15;
    public int goldCollected = 0;
    public float goldExtractionSpeed = 1f;
    public int energy = 3;
    public int maxEnergy = 3;
    
    
    private void Start()
    {
        base.Start();
        
        SetIsMiner(true);

        fsm.AddBehaviour<IdleState>(States.Idle, onTickParameters: () => { return new object[] {this,  GetStart() }; });

        fsm.SetTransition(States.Idle, Flags.OnStart, States.MoveToMine);
        
        fsm.ForceState(States.Idle);
    }

    

    private object[] MoveToMineEnterParameters()
    {
        return new object[] { this , gameManager.GetDistanceBetweenNodes(), GetStartNode() };
    } 
    private object[] MoveToMineTickParameters()
    {
        return new object[] {this.transform, travelTime};
    }

    private object[] MineGoldEnterParameters()
    {
        return new object[] { this };
    }
    private object[] MineGoldTickParameters()
    {
        return new object[] { goldExtractionSpeed, maxGold };
    }

    private object[] EatFoodTickParameters()
    {
        return new object[] { this };
    }

    private object[] WaitForFoodTickParameters()
    {
        return new object[] { this };
    }

    private object[] DepositGoldTickParameters()
    {
        return new object[] { currentNode, travelTime };
    }
    private object[] DepositGoldEnterParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode(), gameManager.GetDistanceBetweenNodes() };
    }

    private object[] RespondToAlarmTickParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode(), gameManager.GetDistanceBetweenNodes() };
    }
    

    public void GetMapInputValues()
    {
        SetStart(true);
    }

    protected override void AddStates()
    {
        fsm.AddBehaviour<MoveToMineState>(States.MoveToMine, onEnterParameters: MoveToMineEnterParameters,onTickParameters: MoveToMineTickParameters);
        fsm.AddBehaviour<MineGoldState>(States.MineGold,onEnterParameters: MineGoldEnterParameters, onTickParameters: MineGoldTickParameters);
        fsm.AddBehaviour<EatFoodState>(States.EatFood, onTickParameters: EatFoodTickParameters);
        fsm.AddBehaviour<DepositGoldState>(States.DepositGold, onEnterParameters: DepositGoldEnterParameters,onTickParameters: DepositGoldTickParameters);
        fsm.AddBehaviour<WaitFoodState>(States.WaitFood, onTickParameters: WaitForFoodTickParameters);
        fsm.AddBehaviour<AlarmState>(States.Alarm, onTickParameters: RespondToAlarmTickParameters);
        // fsm.AddBehaviour<ReturnHomeState>(MinerStates.ReturnHome, onTickParameters: ReturnToUrbanCenterTickParameters);

    }

    public override void AddTransitions()
    {
        fsm.SetTransition(States.MoveToMine, Flags.OnMineFind, States.MineGold);
        fsm.SetTransition(States.MoveToMine, Flags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.MoveToMine, Flags.OnNoMoreMines, States.DepositGold);
        fsm.SetTransition(States.MineGold, Flags.OnFoodNeed, States.EatFood);
        fsm.SetTransition(States.MineGold, Flags.OnFullInventory, States.DepositGold);
        fsm.SetTransition(States.MineGold, Flags.OnMineEmpty, States.MoveToMine);
        fsm.SetTransition(States.MineGold, Flags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.EatFood, Flags.OnFoodEaten, States.MineGold);
        fsm.SetTransition(States.DepositGold, Flags.OnNoMoreMines, States.Idle);
        fsm.SetTransition(States.DepositGold, Flags.OnGoldDeposit, States.MoveToMine);
        fsm.SetTransition(States.DepositGold, Flags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.EatFood, Flags.OnMineEmptyOfFood, States.WaitFood);
        fsm.SetTransition(States.WaitFood, Flags.OnFoodAvailable, States.EatFood);
        fsm.SetTransition(States.WaitFood, Flags.OnAlarmTrigger, States.Alarm);
        fsm.SetTransition(States.Alarm, Flags.OnHome, States.Idle);
        fsm.SetTransition(States.Alarm, Flags.OnBackToWork, States.MoveToMine);
    }
    
    
    public int GetEnergy()
    {
        return energy;
    }
    
    public int GetGoldCollected()
    {
        return goldCollected;
    }
    
    public void SetEnergy(int energy)
    {
        this.energy = energy;
    }
    
    public int GetMaxEnergy()
    {
        return maxEnergy;
    }
    
    public void SetMaxEnergy(int maxEnergy)
    {
        this.maxEnergy = maxEnergy;
    }
    
    public void ResetEnergy()
    {
        energy = maxEnergy;
    }
    
}