using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();

            return _navMeshAgent;
        }
    }

    public enum State
    {
        Walking,
        Searching,
        Chasing
    }
    public State CurrentState = State.Walking;

    float timer = 0;
    [SerializeField] private RoomWaypoint currentRoom;

    private void Start()
    {
        currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        WalkToNewRoom();
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case State.Walking:
                // Walk toward selected waypoint
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance)
                {
                    StartSearhingForCat();
                }
                break;
            case State.Searching:
                // Wait for certain time and find new waypoint
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    WalkToNewRoom();
                }
                break;
            case State.Chasing:
                // Chase that darn cat
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Makes the human run to the room the cat is in
    /// </summary>
    public void TriggerHearingCat()
    {

    }

    /// <summary>
    /// Gets a new waitpoint and starts navigating to it
    /// </summary>
    public void WalkToNewRoom()
    {
        currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        NavMeshAgent.isStopped = false;
        NavMeshAgent.SetDestination(currentRoom.transform.position);
        CurrentState = State.Walking;
    }

    /// <summary>
    /// Checks to see if cat is in line of sight and if so starts chasing
    /// </summary>
    public void StartSearhingForCat()
    {
        NavMeshAgent.isStopped = true;
        // TODO: Play searching animation
        timer = currentRoom.WaitTime;
        CurrentState = State.Searching;
    }

    /// <summary>
    /// Makes the human chase the cat
    /// </summary>
    public void StartChasingCat()
    {

    }
}
