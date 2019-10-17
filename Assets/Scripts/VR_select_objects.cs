using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_select_objects : MonoBehaviour
{
    public bool left_grip_pressed;
    private GameObject controller_left;
    public GameObject unselecting_plane;
    public bool selecting_plane_touched;
    public bool is_unselecting_plane_touched;

    public void Start()
    {
        controller_left = transform.parent.gameObject;
    }

    public void Update()
    {
        left_grip_pressed = controller_left.GetComponent<VR_inputs_controller>().grip_pressed;
    }

    void OnTriggerEnter(Collider other)  //the Collider other is the point that is going to be selected with the RIGHT controller
    {
        //delete the points
        if (left_grip_pressed & (other.tag == "point" || other.tag == "selected_point"))
        {
            other.gameObject.SetActive(false); //if the right grip is pressed while selecting the point (called other), the point is destroyed
            selecting_plane_touched = false;
            VR_unselect_objects.unselecting_plane_touched = false;
            return;
        }

        //selected points turn green
        selecting_plane_touched = true;
        is_unselecting_plane_touched = VR_unselect_objects.unselecting_plane_touched;
        if (is_unselecting_plane_touched == true)
        {
            is_unselecting_plane_touched = false;
            return;
        }

        if (other.tag == "point") //check if it is a point, otherwise you will turn green all the objects you select
        {
            other.tag = "selected_point";
        }

        if (other.tag == "selected_point")
        { return; }
    }
    void OnTriggerExit(Collider other)
    {
        selecting_plane_touched = false;
    }
}

//remember to insert the Unselecting_plane for the 'public GameObject unselecting_plane'