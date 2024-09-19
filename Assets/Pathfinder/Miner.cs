using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Pathfinder;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MinerStates
{
    Idle,
    MoveToMine,
    MineGold,
    EatFood,
    DepositGold,
    WaitFood,
    Alarm
}

public enum MinerFlags
{
    OnStart,
    OnMineFind,
    OnFoodNeed,
    OnFoodEaten,
    OnFoodAvailable,
    OnFullInventory,
    OnMineEmpty,
    OnMineEmptyOfFood,
    OnGoldDeposit,
    OnNoMoreMines,
    OnAlarmTrigger,
    OnHome
}


public class Miner : BaseAgent<MinerStates, MinerFlags>
{
    
    public int maxGold = 15;
    public int goldCollected = 0;
    public float goldExtractionSpeed = 1f;
    public int energy = 3;
    public int maxEnergy = 3;
    


    private void Start()
    {
        base.Start();

        fsm.AddBehaviour<IdleState>(MinerStates.Idle, onTickParameters: () => { return new object[] { GetStart() }; });

        fsm.SetTransition(MinerStates.Idle, MinerFlags.OnStart, MinerStates.MoveToMine);
        
        fsm.ForceState(MinerStates.Idle);
    }

    

    private object[] MoveToMineTickParameters()
    {
        return new object[] { this as Miner, this.transform, travelTime, gameManager.GetDistanceBetweenNodes() };
    }

    private object[] MineGoldTickParameters()
    {
        return new object[] { this, goldExtractionSpeed, maxGold };
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
        return new object[] { this, currentNode, gameManager.GetUrbanCenterNode(), travelTime, gameManager.GetDistanceBetweenNodes() };
    }

    private object[] ReturnToUrbanCenterTickParameters()
    {
        return new object[] { this, gameManager.GetUrbanCenterNode() };
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
        fsm.AddBehaviour<MoveToMineState>(MinerStates.MoveToMine, onTickParameters: MoveToMineTickParameters);
        fsm.AddBehaviour<MineGoldState>(MinerStates.MineGold, onTickParameters: MineGoldTickParameters);
        fsm.AddBehaviour<EatFoodState>(MinerStates.EatFood, onTickParameters: EatFoodTickParameters);
        fsm.AddBehaviour<DepositGoldState>(MinerStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        fsm.AddBehaviour<WaitFoodState>(MinerStates.WaitFood, onTickParameters: WaitForFoodTickParameters);
        fsm.AddBehaviour<AlarmState>(MinerStates.Alarm, onTickParameters: RespondToAlarmTickParameters);
        // fsm.AddBehaviour<ReturnHomeState>(MinerStates.ReturnHome, onTickParameters: ReturnToUrbanCenterTickParameters);

    }

    public override void AddTransitions()
    {
        fsm.SetTransition(MinerStates.MoveToMine, MinerFlags.OnMineFind, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.MoveToMine, MinerFlags.OnAlarmTrigger, MinerStates.Alarm);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFoodNeed, MinerStates.EatFood);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnFullInventory, MinerStates.DepositGold);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnMineEmpty, MinerStates.MoveToMine);
        fsm.SetTransition(MinerStates.MineGold, MinerFlags.OnAlarmTrigger, MinerStates.Alarm);
        fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnFoodEaten, MinerStates.MineGold);
        fsm.SetTransition(MinerStates.DepositGold, MinerFlags.OnNoMoreMines, MinerStates.Idle);
        fsm.SetTransition(MinerStates.DepositGold, MinerFlags.OnGoldDeposit, MinerStates.MoveToMine);
        fsm.SetTransition(MinerStates.EatFood, MinerFlags.OnMineEmptyOfFood, MinerStates.WaitFood);
        fsm.SetTransition(MinerStates.WaitFood, MinerFlags.OnFoodAvailable, MinerStates.EatFood);
        fsm.SetTransition(MinerStates.WaitFood, MinerFlags.OnAlarmTrigger, MinerStates.Alarm);
        fsm.SetTransition(MinerStates.Alarm, MinerFlags.OnHome, MinerStates.Idle);
        // fsm.SetTransition(MinerStates.ReturnHome, MinerFlags.OnAlarmTrigger, MinerStates.MoveToMine);

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

public static class MinerEvents
{
    public static Action<Miner> OnNeedFood;
}