using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Human : MonoBehaviour
{
    public static Human instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


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

    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 3.5f;

    [SerializeField] private Transform playerRaycastTarget;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float sightFov = 0.5f;

    float timer = 0;
    [SerializeField] private float alertTime = 4;
    [SerializeField] private float unreachableLoseGameTime = 2;
    [SerializeField] private RoomWaypoint currentRoom;
    [SerializeField] private float catSpotTime = 2;
    private float catSpotTimer = 0;
    [SerializeField] private float reactionTime = 1;

    private void Start()
    {
        currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        WalkToNewRoom(true);
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Here kitty kitty kitty");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        RaycastHit hit;

        switch (CurrentState)
        {
            case State.Walking:
                // Walk toward selected waypoint
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance && delayedSetDestinationRoutine == null)
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
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance && delayedSetDestinationRoutine == null)
                {
                    NavMeshAgent.isStopped = true;
                }

                if (catSpotTimer > catSpotTime)
                {
                    StartChasingCat();
                }

                if (NavMeshAgent.isStopped && delayedSetDestinationRoutine == null)
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
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance && delayedSetDestinationRoutine == null)
                {
                    //LoseGame();
                    ray = new Ray(transform.position + Vector3.up * NavMeshAgent.height, (playerRaycastTarget.position - (transform.position + Vector3.up * NavMeshAgent.height)).normalized);
                    if (Physics.Raycast(ray, out hit, 100f, ~(1 << 8)))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            timer += Time.deltaTime;
                            if (timer > unreachableLoseGameTime)
                                LoseGame();
                        }
                    }
                }
                break;
            default:
                break;
        }

        ray = new Ray(CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position,
            (playerRaycastTarget.position - (CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position)).normalized);
        //RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, ~(1 << 8)))
        {
            if (hit.transform.tag == "Player")
            {
                float dot = Vector3.Dot(raycastOrigin.forward, (hit.point - raycastOrigin.position).normalized);
                if (dot > sightFov || CurrentState == State.Chasing)
                {
                    // Actually spot the player
                    OnSpotCat();

                    Debug.DrawRay(CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position, 
                        (hit.point - (CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position)), Color.red);
                }
                else
                {
                    Debug.DrawRay(CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position, 
                        (hit.point - (CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position)), new Color(1.0f, 0.64f, 0.0f));
                    if (CurrentState == State.Chasing && NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance)
                    {

                        //NavMeshAgent.speed = walkingSpeed;
                        timer = alertTime;
                        CurrentState = State.Alert;
                        catSpotTimer += Time.deltaTime;

                        NavMeshAgent.SetDestination(GetPlayerPosition());

                        //ContinueOnPath();
                        //BecomeAlert(true);
                    }
                }
            }
            else
            {
                Debug.DrawRay(CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position,
                    playerRaycastTarget.position - (CurrentState == State.Chasing ? transform.position + Vector3.up * NavMeshAgent.height : raycastOrigin.position), Color.green);
                if (CurrentState == State.Chasing && NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance)
                {
                    //NavMeshAgent.speed = walkingSpeed;
                    timer = alertTime;
                    CurrentState = State.Alert;
                    catSpotTimer += Time.deltaTime;

                    //NavMeshAgent.SetDestination(GetPlayerPosition());
                    NavMeshAgent.destination = GetPlayerPosition();

                    //ContinueOnPath();
                    //BecomeAlert(true);
                }
            }
        }
    }

    /// <summary>
    /// Makes the human run to the room the cat is in
    /// </summary>
    public void TriggerHearingCat(Vector3 position)
    {
        BecomeAlert(true, position);
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Hey! What was that?");
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
                BecomeAlert();
                //catSpotTimer += Time.deltaTime;
                break;
            case State.Chasing:
                StartChasingCat();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Starts alert state and sets timer
    /// </summary>
    public void BecomeAlert(bool skipSubtitle = false)
    {
        BecomeAlert(skipSubtitle, GetPlayerPosition());

        // TODO: trigger searching animation

        //if (!skipSubtitle)
        //    SubtitleUI.instance.ShowSubtitle(subtitleName, "Is that you over there?", 3f);
        ////NavMeshAgent.SetDestination(playerRaycastTarget.position);
        //NavMeshAgent.isStopped = true;
        //delayedSetDestinationRoutine = StartCoroutine(DelayedSetDestination(playerRaycastTarget.position, reactionTime));

        //timer = alertTime;
        //CurrentState = State.Alert;
        //catSpotTimer += Time.deltaTime;
    }
    public void BecomeAlert(bool skipSubtitle, Vector3 position)
    {
        // TODO: trigger searching animation

        //if (!skipSubtitle)
        //    SubtitleUI.instance.ShowSubtitle(subtitleName, "Is that you over there?", 3f);

        //NavMeshAgent.SetDestination(playerRaycastTarget.position);
        if (CurrentState != State.Alert || CurrentState != State.Chasing)
        {
            NavMeshAgent.isStopped = true;
            delayedSetDestinationRoutine = StartCoroutine(DelayedSetDestination(position, reactionTime));
        }
        else if (delayedSetDestinationRoutine == null)
        {
            //NavMeshAgent.SetDestination(GetPlayerPosition());
            NavMeshAgent.destination = (GetPlayerPosition());
        }

        //NavMeshAgent.speed = walkingSpeed;
        timer = alertTime;
        CurrentState = State.Alert;
        catSpotTimer += Time.deltaTime; // *
            //Mathf.Clamp01(Vector3.Dot(transform.forward, ((playerRaycastTarget.position - transform.position).normalized)) *
            //Mathf.Clamp(1 - Vector3.Distance(transform.position, playerRaycastTarget.position), 0.5f, 5));
    }

    /// <summary>
    /// Gets a new waitpoint and starts navigating to it
    /// </summary>
    public void WalkToNewRoom(bool skipSubtitle = false)
    {
        catSpotTimer = 0;
        currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        NavMeshAgent.isStopped = false;
        if (!skipSubtitle)
            SubtitleUI.instance.ShowSubtitle(subtitleName, "Maybe somewhere else in the house", 3f);
        //NavMeshAgent.SetDestination(currentRoom.transform.position);
        NavMeshAgent.destination = currentRoom.transform.position;
        CurrentState = State.Walking;
        NavMeshAgent.speed = walkingSpeed;
    }

    public void ContinueOnPath()
    {
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Huh, must have been the wind...", 3f);

        timer = 0;
        NavMeshAgent.isStopped = false;
        //NavMeshAgent.SetDestination(currentRoom.transform.position);
        NavMeshAgent.destination = currentRoom.transform.position;
        CurrentState = State.Walking;
        NavMeshAgent.speed = walkingSpeed;
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

        NavMeshAgent.speed = walkingSpeed;
    }

    /// <summary>
    /// Makes the human chase the cat
    /// </summary>
    public void StartChasingCat()
    {
        catSpotTimer = 0;

        if (CurrentState != State.Chasing)
        {
            timer = 0;
            SubtitleUI.instance.ShowSubtitle(subtitleName, "I've got you now!", 3f);
        }

        CurrentState = State.Chasing;
        NavMeshAgent.isStopped = false;
        if (delayedSetDestinationRoutine != null)
        {
            StopCoroutine(delayedSetDestinationRoutine);
        }
        NavMeshAgent.destination = GetPlayerPosition();
        NavMeshAgent.speed = runningSpeed;
    }

    private Coroutine delayedSetDestinationRoutine = null;
    private IEnumerator DelayedSetDestination(Vector3 target, float time)
    {
        yield return new WaitForSeconds(time);

        NavMeshAgent.isStopped = false;
        //NavMeshAgent.SetDestination(target);
        NavMeshAgent.destination = target;
        delayedSetDestinationRoutine = null;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (CurrentState == State.Chasing || CurrentState == State.Alert)
            {
                LoseGame();
            }
            else
            {
                BecomeAlert();
            }
        }
    }

    public Vector3 GetPlayerPosition()
    {
        //return playerRaycastTarget.position;

        Vector3 result = playerRaycastTarget.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(new Vector3(playerRaycastTarget.position.x, transform.position.y, playerRaycastTarget.position.z), out hit, NavMeshAgent.height, NavMesh.AllAreas))
        {
            result = hit.position;
        }
        return result;
    }

    public void LoseGame()
    {
        Debug.Log("LOSE GAME");
        Debug.Break();
        Application.Quit();
    }
}
