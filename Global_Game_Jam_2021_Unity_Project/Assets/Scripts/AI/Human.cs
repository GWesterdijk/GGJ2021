using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private TMP_Text gameOverUIScore;
    [SerializeField] private GameObject winGameUI;
    [SerializeField] private GameObject pauseGameUI;
    [SerializeField] private TMP_Text winGameUIScore;
    [HideInInspector] public bool IsGameOver = false;

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

    private Animator _animator;
    public Animator Animator
    {
        get
        {
            if (_animator == null)
                _animator = transform.GetChild(0).GetComponent<Animator>();

            return _animator;
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

    [SerializeField] private List<string> onHearingCatDialogue = new List<string>();
    [SerializeField] private List<string> onWalkToNewRoomDialogue = new List<string>();
    [SerializeField] private List<string> onStopAlertCatDialogue = new List<string>();
    [SerializeField] private List<string> onStartSearchingRoomDialogue = new List<string>();
    [SerializeField] private List<string> onStartRunningDialogue = new List<string>();

    private string GetRandomDialogue(List<string> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 3.5f;

    [SerializeField] private Transform playerRaycastTarget;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float sightFov = 0.5f;

    float timer = 0;
    [SerializeField] private float alertTime = 4;
    [SerializeField] private float unreachableLoseGameTime = 2;
    [SerializeField] private Queue<RoomWaypoint> comingRooms = new Queue<RoomWaypoint>();
    [SerializeField] private RoomWaypoint currentRoom;
    [SerializeField] private RoomWaypoint firstRoom;
    [SerializeField] private float catSpotTime = 2;
    private float catSpotTimer = 0;
    [SerializeField] private float reactionTime = 1;

    private void Start()
    {
        //currentRoom = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        WalkToNewRoom(true);
        SubtitleUI.instance.ShowSubtitle(subtitleName, "Here kitty kitty kitty");
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            PauseGame();
        }

        Animator.SetFloat("Speed", NavMeshAgent.velocity.magnitude);

        Ray ray;
        RaycastHit hit;

        switch (CurrentState)
        {
            case State.Walking:
                NavMeshAgent.speed = walkingSpeed;

                // Walk toward selected waypoint
                if (NavMeshAgent.remainingDistance < NavMeshAgent.stoppingDistance && delayedSetDestinationRoutine == null)
                {
                    StartSearhingForCat();
                }
                break;
            case State.Searching:
                NavMeshAgent.speed = walkingSpeed;

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
                NavMeshAgent.speed = runningSpeed;

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
                NavMeshAgent.speed = runningSpeed;

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
                if (dot > sightFov || CurrentState == State.Chasing || CurrentState == State.Alert)
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
        //onHearingCatDialogue
        SubtitleUI.instance.ShowSubtitle(subtitleName, GetRandomDialogue(onHearingCatDialogue)); //"Hey! What was that?") ;
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
            //SubtitleUI.instance.ShowSubtitle(subtitleName, "Is that you over there?", 3f);

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

        if (comingRooms.Count <= 0)
        {
            comingRooms = new Queue<RoomWaypoint>(RoomWaypoint.Waypoints.Count);
            comingRooms.Enqueue(firstRoom);
            for (int i = 0; i < RoomWaypoint.Waypoints.Count; i++)
            {
                RoomWaypoint room = RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count - i)];
                if (room != firstRoom)
                {
                    comingRooms.Enqueue(room);
                }
            }
        }

        currentRoom = comingRooms.Dequeue(); //RoomWaypoint.Waypoints[Random.Range(0, RoomWaypoint.Waypoints.Count)];
        comingRooms.Enqueue(currentRoom);

        NavMeshAgent.isStopped = false;

        //
        if (!skipSubtitle)
            SubtitleUI.instance.ShowSubtitle(subtitleName, GetRandomDialogue(onWalkToNewRoomDialogue), 3f);
        //NavMeshAgent.SetDestination(currentRoom.transform.position);
        NavMeshAgent.destination = currentRoom.transform.position;
        CurrentState = State.Walking;
        NavMeshAgent.speed = walkingSpeed;
    }

    public void ContinueOnPath()
    {
        //
        SubtitleUI.instance.ShowSubtitle(subtitleName, GetRandomDialogue(onStopAlertCatDialogue), 3f);

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

        //
        SubtitleUI.instance.ShowSubtitle(subtitleName, GetRandomDialogue(onStartSearchingRoomDialogue), 3f);
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
            //
            SubtitleUI.instance.ShowSubtitle(subtitleName, GetRandomDialogue(onStartRunningDialogue), 3f);
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

    [Button(25)]
    public void LoseGame()
    {
        gameOverUI.SetActive(true);
        gameOverUIScore.text = "Score: " + Chaos.TotalChaosScore;
        IsGameOver = true;

        Debug.Log("LOSE GAME");
        //Debug.Break();
        //Application.Quit();
    }

    [Button(25)]
    public void WinGame()
    {
        winGameUI.SetActive(true);
        winGameUIScore.text = "Score: " + Chaos.TotalChaosScore;
        IsGameOver = true;

        Debug.Log("WIN GAME");
        //Debug.Break();
        //Application.Quit();
    }

    [Button(25)]
    public void PauseGame()
    {
        pauseGameUI.SetActive(true);

        Debug.Log("PAUSE GAME");
        //Debug.Break();
        //Application.Quit();
    }
}
