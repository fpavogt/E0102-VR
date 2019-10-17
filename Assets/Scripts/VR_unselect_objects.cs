using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_unselect_objects : MonoBehaviour
{
    public GameObject selecting_plane;
    public bool is_selecting_plane_touched;
    public static bool unselecting_plane_touched;

    private void OnTriggerEnter(Collider other) //the Collider other is the point that is going to be unselected with the RIGHT controller
    {
        unselecting_plane_touched = true;
        is_selecting_plane_touched = selecting_plane.GetComponent<VR_select_objects>().selecting_plane_touched;
        if (is_selecting_plane_touched)
        {
            is_selecting_plane_touched = false;
            return;
        }

        //unselect the points
        if (other.tag == "selected_point")
        {
            other.tag = "point";
        }

        if (other.tag == "point") //it the point that gets hit by the collider has not yet been selected, just exit the function
        { return; }
    }

    private void OnTriggerExit(Collider other)
    {
        unselecting_plane_touched = false;
    }
}

//remember to insert the Selecting_plane for the 'public GameObject selecting_plane'