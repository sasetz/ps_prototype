using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum CustomerState : byte {
    Entered,                // just entering the shop
    WentToItem,             // moving in the direction of an item
    LookedAtItem,           // standing and looking at an item
    DozedOff,               // idle doing nothing, thinking
    Mugged,                 // standing at the check out, waiting for the cashier to give the money
    StoleItem,              // stealing an item from the shelf
    RanOff,                 // running with the item/cash/from you
    FinishedWaitingInLine,         // waiting for check out
    CheckedOut,             // checking out
    Exited,                 // exiting the shop
}

public enum CustomerType : byte {
    SlowCustomer,
    RushingCustomer,
    JumpyCustomer,

    MuggingEnemy,
    StealerEnemy,
    ManiacEnemy,
}

public struct Customer {
    public NavMeshAgent agent;
    public float remainingTime;
    public UInt16 id;
    public CustomerState state;
    public CustomerType type;
}

// TODO: mob spawning
// TODO: finish mob behavior

public class AIManager : MonoBehaviour
{
    public const int MAX_SHELVES = 32;
    public const int MAX_CUSTOMERS = 256;

    public Transform[] items;
    public Transform first_exit;
    public Transform second_exit;
    public Transform[] checkout;
    public float distanceThreshold = 0.2f;

    private SwapbackArray<Customer> idle_customers, updating_customers;
    private CustomQueue<Customer> checkout_customers;
    private float queue_timer = 0f; // when expired, next customer in checkout_customers is added to the updating_customers array
    private SwapbackArray<Vector3> vacant_shelves; // TODO: consider making another data structure specifically for this
    private SwapbackArray<Vector3> occupied_shelves;

    public AIManager()
    {
        vacant_shelves = new SwapbackArray<Vector3>(MAX_SHELVES);
        occupied_shelves = new SwapbackArray<Vector3>(MAX_SHELVES);

        idle_customers = new SwapbackArray<Customer>(MAX_CUSTOMERS);
        updating_customers = new SwapbackArray<Customer>(MAX_CUSTOMERS);
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: add enemy spawning
        for (int i = 0; i < items.Length; i++)
        {
            vacant_shelves.Add(items[i].position);
        }
        checkout_customers = new CustomQueue<Customer>(checkout.Length);
    }

    void Update()
    {
        // update timers
        var deltaTime = Time.deltaTime;
        for (int i = 0; i < idle_customers.Count; i++)
        {
            if (idle_customers[i].remainingTime >= 0f)
            {
                idle_customers[i].remainingTime -= deltaTime;
            }
        }
        if (queue_timer >= 0f)
            queue_timer -= deltaTime;

        // determine which customers need to be updated
        for (int i = 0; i < idle_customers.Count;)
        {
            if (idle_customers[i].remainingTime <= 0f && idle_customers[i].agent.remainingDistance <= distanceThreshold)
            {
                updating_customers.Add(idle_customers[i]);
                idle_customers.RemoveAt(i);
            }
            else
            {
                // don't increment the index if we removed current customer, there's another one on that index
                i++;
            }
        }

        // destroy customers that have already exited
        for (int i = 0; i < updating_customers.Count;)
        {
            if (updating_customers[i].state == CustomerState.Exited)
            {
                Destroy(idle_customers[i].agent);
                updating_customers.RemoveAt(i);
            }
            else
            {
                // to avoid skipping any customers
                i++;
            }
        }

        // handle next in queue
        if (queue_timer <= 0f && checkout_customers.Count > 0)
        {
            updating_customers.Add(checkout_customers.Dequeue());
            for (int i = 0; i < checkout_customers.Count; i++)
            {
                checkout_customers[i].agent.destination = checkout[i].position;
            }
        }

        // main customer update loop
        for (int i = 0; i < updating_customers.Count; i++)
        {
            switch (updating_customers[i].type)
            {
            case CustomerType.RushingCustomer:
                updateRushingCustomer(i);
                break;
            default:
                Debug.LogError("An unknown customer type encountered!");
                break;
            }
        }

        // return the updating customers back to idle array
        // NOTE: customers after the queue don't go back into it, they return to idle
        // mode and wait until they exit basically
        for (int i = 0; i < updating_customers.Count; i++)
        {
            idle_customers.Add(updating_customers[i]);
        }
        updating_customers.Clear(); // all customers that needed it, were updated
    }

    void updateRushingCustomer(int index)
    {
        switch (updating_customers[index].state)
        {
            case CustomerState.Entered:
                if (occupyVacantShelf(index))
                {
                    updating_customers[index].state = CustomerState.WentToItem;
                }
                else
                {
                    updating_customers[index].state = CustomerState.Exited;
                    updating_customers[index].agent.destination = Random.Range(1,2) == 1 ? first_exit.position : second_exit.position;
                }
                break;
            case CustomerState.WentToItem:
                updating_customers[index].state = CustomerState.LookedAtItem;
                updating_customers[index].remainingTime = Random.Range(2, 5);
                break;
            case CustomerState.LookedAtItem:
                if (Random.Range(1, 4) == 1)
                {
                    // TODO: move this into a function
                    updating_customers[index].state = CustomerState.FinishedWaitingInLine;
                    int queue_position = checkout_customers.Enqueue(updating_customers[index]);
                    updating_customers[index].agent.destination = checkout[queue_position].position;
                }
                else if (occupyVacantShelf(index))
                {
                    updating_customers[index].state = CustomerState.WentToItem;
                }
                else
                {
                    updating_customers[index].state = CustomerState.Exited;
                    updating_customers[index].agent.destination = Random.Range(1, 2) == 1 ? first_exit.position : second_exit.position;
                }
                break;
            case CustomerState.FinishedWaitingInLine:
                // it's our checkout time, we need to set a timer and switch to the next state
                // TODO: move this into a function
                updating_customers[index].state = CustomerState.CheckedOut;
                queue_timer = Random.Range(5, 15);
                break;
            case CustomerState.CheckedOut:
                updating_customers[index].state = CustomerState.Exited;
                updating_customers[index].agent.destination = Random.Range(1, 2) == 1 ? first_exit.position : second_exit.position;
                break;
            default:
                Debug.LogError("An error state has occurred! This is probably a bug!");
                Debug.DebugBreak();
                updating_customers[index].state = CustomerState.Exited;
                updating_customers[index].agent.destination = first_exit.position;
                break;
        }
    }

    private bool occupyVacantShelf(int index)
    {
        if (vacant_shelves.Count <= 0)
        {
            return false;
        }

        var item = vacant_shelves.PopRandom();
        occupied_shelves.Add(item);
        updating_customers[index].agent.destination = item;
        return true;
    }
}

// WARN: кто тронет этот сраный скрипт, я ревертну всё к чертям
