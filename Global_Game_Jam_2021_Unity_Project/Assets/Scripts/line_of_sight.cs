using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class line_of_sight : MonoBehaviour
{
    //private bool is_insight = false;
    public Transform Player;

    void Update()
    {
        //RaycastHit hit;
        //Ray sightRay = new Ray(transform.position, (transform.position-Player.position));

       
        //    if (Physics.Raycast(sightRay, out hit, LayerMask.NameToLayer("Default")))
        //    {
        //        if(hit.collider.tag == "Player")
        //        {
        //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //        is_insight = true;
        //            Debug.Log("True");
        //        }
        //        else
        //        {
        //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //         }
        //    }
        
    }
}
