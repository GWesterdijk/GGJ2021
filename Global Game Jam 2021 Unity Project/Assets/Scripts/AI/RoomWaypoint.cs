using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWaypoint : MonoBehaviour
{
    public static List<RoomWaypoint> Waypoints = new List<RoomWaypoint>();

    public float WaitTime = 1;

    private void OnEnable()
    {
        Waypoints.Add(this);
    }

    private void OnDisable()
    {
        Waypoints.Remove(this);
    }
}
