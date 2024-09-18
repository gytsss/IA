using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CaravanStates
{
    Idle,
    CaravanMoveToMine,
    DepositFood,
    ReturnHome,
    Alarm
}

public enum CaravanFlags
{
    OnFoodNeed,
    OnMineFind,
    OnFoodDeposit,
    OnHome,
    OnAlarmTrigger
}


public class Caravan : BaseAgent<CaravanStates, CaravanFlags>
{
    private bool start = false;


    public Miner miner;

    private void OnEnable()
    {
        MinerEvents.OnNeedFood += OnFoodNeed;
    }
    
    private void OnDisable()
    {
        MinerEvents.OnNeedFood -= OnFoodNeed;
    }

    private void OnFoodNeed(Miner obj)
    {
        Debug.Log("Caravan moving to mine");
        fsm.AddBehaviour<CaravanMoveToMineState>(CaravanStates.CaravanMoveToMine, onTickParameters: CaravanMoveToMineTickParameters);

        fsm.ForceState(CaravanStates.CaravanMoveToMine);
    }

    private void Start()
    {
        base.Start();

        fsm.AddBehaviour<IdleState>(CaravanStates.Idle, onTickParameters: () => { return new object[] { start }; });

        fsm.SetTransition(CaravanStates.Idle, CaravanFlags.OnFoodNeed, CaravanStates.CaravanMoveToMine);

        fsm.ForceState(CaravanStates.Idle);
    }
    

    private object[] CaravanMoveToMineTickParameters()
    {
        return new object[]
            { this as Caravan, this.transform, destinationNode, startNode, travelTime, distanceBetweenNodes, path };
    }


    private object[] DepositFoodTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] ReturnHomeTickParameters()
    {
        return new object[] { this, currentMine };
    }

    private object[] AlarmTickParameters()
    {
        return new object[] { this };
    }

    private void Update()
    {
        fsm.Tick();
    }

    public void GetMapInputValues()
    {
        start = true;
    }
    
    
    protected override void AddStates()
    {
        //fsm.AddBehaviour<CaravanMoveToMineState>(CaravanStates.CaravanMoveToMine, onTickParameters: CaravanMoveToMineTickParameters);
        //fsm.AddBehaviour<MineGoldState>(CaravanStates.DepositGold, onTickParameters: MineGoldTickParameters);
        //fsm.AddBehaviour<EatFoodState>(CaravanStates.EatFood, onTickParameters: EatFoodTickParameters);
        //fsm.AddBehaviour<DepositGoldState>(CaravanStates.DepositGold, onTickParameters: DepositGoldTickParameters);
        
    }

    public override void AddTransitions()
    {
        //fsm.SetTransition(CaravanStates.CaravanMoveToMine, CaravanFlags.OnMineFind, CaravanStates.DepositFood);
        //fsm.SetTransition(CaravanStates.DepositFood, CaravanFlags.OnFoodDeposit, CaravanStates.ReturnHome);
        //fsm.SetTransition(CaravanStates.ReturnHome, CaravanFlags.OnHome, CaravanStates.Idle);

    }
    
}

    