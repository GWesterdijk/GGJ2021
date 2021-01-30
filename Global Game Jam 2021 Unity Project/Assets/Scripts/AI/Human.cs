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

    [SerializeField] string subtitleName = "Stefan de Kattenwasser";

    public enum State
    {
        Walking,
        Searching,
        Alert,
        Chasing
    }
    public State CurrentState = State.Walking;

    [SerializeField] private Transform playerRaycastTarget;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float sightFov = 0.5f;

    float timer = 0;
    [SerializeField] private float alertTime = 4;
    [SerializeField] private RoomWaypoint currentRoom;
    [SerializeField] private float catSpotTime = 2;
    private float catSpotTimer = 0;

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

                if (catSpotTimer > catSpotTime)
                {
                    StartChasingCat();
                }

                if (timer <= 0)
                {
                    WalkToNewRoom();
                }
                break;
            case State.Alert:
                // Look for that darn cat in current spot for certain time
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance)
                {
                    NavMeshAgent.isStopped = true;
                }

                if (catSpotTimer > catSpotTime)
                {
                    StartChasingCat();
                }

                if (NavMeshAgent.isStopped)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        ContinueOnPath();
                    }
                }
                break;
            case State.Chasing:
                // Chase that darn cat
                break;
            default:
                break;
        }

        Ray ray = new Ray(raycastOrigin.position, (playerRaycastTarget.position - raycastOrigin.position).normalized); ;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, ~(1 << 8)))
        {
            if (hit.transform.tag == "Player")
            {
                float dot = Vector3.Dot(raycastOrigin.forward, (hit.point - raycastOrigin.position).normalized);
                if (dot > sightFov)
                {
                    // Actually spot the player
                    OnSpotCat();

                    Debug.DrawRay(raycastOrigin.position, (hit.point - raycastOrigin.position), Color.red);
                }
                else
                {
                    Debug.DrawRay(raycastOrigin.position, (hit.point - raycastOrigin.position), new Color(1.0f, 0.64f, 0.0f));
                }
            }
            else
                Debug.DrawRay(raycastOrigin.position, playerRaycastTarget.position - raycastOrigin.position, Color.green);
        }
    }

    /// <summary>
    /// Makes the human run to the room the cat is in
    /// </summary>
    public void TriggerHearingCat()
    {
        BecomeAlert();
    }

    public void OnSpotCat()
    {
        switch (CurrentState)
        {
            case State.Walking:
                BecomeAlert();
                break;
            case State.Searching:
                BecomeAlert();
                //catSpotTimer += Time.deltaTime;
                break;
            case State.Alert:
                catSpotTimer += Time.deltaTime;
                break;
            case State.Chasing:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Starts alert state and sets timer
    /// </summary>
    public void BecomeAlert()
    {
        // TODO: trigger searching animation

        SubtitleUI.instance.ShowSubtitle(subtitleName, "Is that you over there?", 3f);
        NavMeshAgent.SetDestination(playerRaycastTarget.position);
        timer = alertTime;
        CurrentState = State.Alert;
        catSpotTimer += Time.deltaTime;
    }

    /// <summary>
    /// Gets a new waitpoint and starts navigating to it
    /// </summary>
    public void WalkToNewRoom()
    {
        catSpotTimer = 0;
        currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        NavMeshAgent.isStopped = false;
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Maybe somewhere else in the house", 3f);
        NavMeshAgent.SetDestination(currentRoom.transform.position);
        CurrentState = State.Walking;
    }

    public void ContinueOnPath()
    {
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Huh, must have been the wind", 3f);

        timer = 0;
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

        SubtitleUI.instance.ShowSubtitle(subtitleName, "Hmm must be here somewhere", 3f);
        // TODO: Play searching animation
        timer = currentRoom.WaitTime;
        CurrentState = State.Searching;
    }

    /// <summary>
    /// Makes the human chase the cat
    /// </summary>
    public void StartChasingCat()
    {
        catSpotTimer = 0;

        SubtitleUI.instance.ShowSubtitle(subtitleName, "I've got you now!", 3f);

        Debug.Log("LOSE GAME");
        Debug.Break();
        Application.Quit();
    }
}
