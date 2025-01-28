using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState : byte {
    Entering,               // just entering the shop
    GoingToItem,            // moving in the direction of an item
    LookingAtItem,          // standing and looking at an item
    DozingOff,              // idle doing nothing, thinking
    GoingToCheckOut,        // going to check out
    CheckingOut,            // checking out
    Exiting,                // exiting the shop
}

public enum EnemyState : byte {
    Entering,
    GoingToItem,
    LookingAtItem,
    DozingOff,
    GoingToCheckOut,
    CheckingOut,
    Mugging,
    GrabbingItem,
    RunningOff,
    Exiting,
}

public struct Customer {
    public NavMeshAgent agent;
    public CustomerState state;
}

public struct Enemy {
    public NavMeshAgent agent;
    public EnemyState state;
}

public class AIManager : MonoBehaviour
{
    public Transform[] low_value_items, mid_value_items, high_value_items;
    public Transform[] exits;

    private Customer[] rushing_customers, slow_customers, jumpy_customers;
    private Enemy[] stealer_enemies, mugging_enemies, maniac_enemies;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: add enemy spawning
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < rushing_customers.Length; i++)
        {
            switch(rushing_customers[i].state)
            {
            case CustomerState.Entering:
                rushing_customers[i].agent.destination = exits[0].position;
                break;
            // TODO: add state switching conditions
            }
        }
    }
}

// WARN: кто тронет этот сраный скрипт, я ревертну всё к чертям
