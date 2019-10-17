using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VR_Measurement_tool : MonoBehaviour {

    private GameObject controller;
    public GameObject Object_toBe_measured;
    public GameObject pointToBeCloned;
    public GameObject[] points;
    public GameObject[] selected_points;
    private GameObject listOnLeftController;
    private Text collidingObjInfo_L;
    public GameObject reference_system;
    public GameObject referencePoint; //the measurement points will be created at the same location as the referencePoint on the LEFT controller
    public LineRenderer line;
    public float scaling_factor;
    public string distance_unit;
    public bool zUp;
    private float distance;
    private Vector3 left_controller_pos;
    //private Text measurement;
    private Canvas canvas;
    //private Canvas canvas_line;
    private double theta_value;
    private double phi_value;
    private double R_value;
    private Vector3 verticalAxis;
    private Vector3 pointProjectionOnXY;
    private float x_point;
    private float y_point;
    private float z_point;
    public bool left_trigger_pressed;
    public bool created;
    public bool left_grabbing;


    // Use this for initialization
    void Start()
    {
        controller = GameObject.Find("Controller (left)");
        created = false;
        //measurement = line.GetComponentInChildren<Text>();
        listOnLeftController = controller.GetComponent<ControllerGrabObjects>().canvasOnController;
        collidingObjInfo_L = controller.GetComponent<ControllerGrabObjects>().collidingObjInfo;

        scaling_factor = 1;
        distance_unit = " pc";
        zUp = true;

        if (scaling_factor == 0)
        {
            scaling_factor = 1;
        }
        if (distance_unit == " ")
        {
            Debug.Log("Please, specify your distance unit");
        }
    }

    // Update is called once per frame
    void Update()
    {
        left_trigger_pressed = controller.GetComponent<VR_inputs_controller>().trigger_pressed;     //right trigger used to place the measuring points in space
        left_grabbing = controller.GetComponent<ControllerGrabObjects>().grabbing;                  //when grabbing with the right handle, do not allow creating new points

        //create and set the position of the points in space when the LEFT controller's trigger is pressed
        if (left_trigger_pressed & created == false & left_grabbing == false & VR_UI_item.measuring == true)
        {
            //this 'if statement' is needed to avoid issues while creating and moving measurement points 
            if (listOnLeftController != null)
            {
                if (listOnLeftController.activeSelf == true & controller.GetComponent<VR_Measurement_tool>().isActiveAndEnabled)
                {
                    collidingObjInfo_L.text = "Hide the PARTS MENU" + "\n" + "before moving and creating" + "\n" + "measurement points.";

                    return;
                }
            }

            left_controller_pos = controller.transform.position;
            GameObject clone;
            clone = Instantiate(pointToBeCloned, referencePoint.transform.position, Quaternion.identity, reference_system.transform); //create the points attached to the ReferenceSystem: reference_system.transform
            created = true;
        }
        if (!left_trigger_pressed & created == true) //if the trigger is released, we wait until it will be pressed again to create more points 
        {
            created = false;
        }

        //select and destroy the points on the 'VR_select_objects' script attached to the 'selecting plane' of the LEFT controller
        //deselect   the   points   on   the   'VR_deselect_objects' script attached to the 'unselecting plane' of the LEFT controller

        points = GameObject.FindGameObjectsWithTag("point");
        foreach (GameObject point in points)
        {
            Display_point_info(point);
            //FPAV: if this is a poin I am currently "touching", keep the default colliding color.
            if (point.GetComponent<Renderer>().material.name != "ShowTouchedPoint (Instance)")
            {
                point.GetComponent<Renderer>().material.color = Color.white;
            }
            point.name = "Point";

            point.transform.GetChild(1).GetComponentInChildren<Text>().text = "";
            //FPAV: if the point is not selected, turn off the distance text AND the background image as well
            point.transform.GetChild(1).GetChild(0).GetComponentInChildren<Image>().enabled = false;
        }

        selected_points = GameObject.FindGameObjectsWithTag("selected_point"); //this is a list of all the selected points
        foreach (GameObject selected_point in selected_points)
        {
            Display_point_info(selected_point);
            //FPAV: if this is a poin I am currently "touching", keep the default colliding color.
            if (selected_point.GetComponent<Renderer>().material.name != "ShowTouchedPoint (Instance)")
            {
               selected_point.GetComponent<Renderer>().material.color = new Color(1.0f, 0.20f, 0.0f);
            }
            selected_point.name = "Selected point";

            selected_point.transform.GetChild(1).GetComponentInChildren<Text>().text = "";
            //FPAV: if the point is not selected, turn off the distance text AND the background image as well
            // FPAV: here, still keep it disabled, since there is no text set just yet ...
            // This is for the last point in the line, if I get it right ...
            selected_point.transform.GetChild(1).GetChild(0).GetComponentInChildren<Image>().enabled = false;
        }

        int sizeOfList;
        sizeOfList = selected_points.Count();
        if (sizeOfList < 2)
        {
            line.gameObject.SetActive(false); //if you do not have more than 2 selected points, the line won't be created (SetActive(false))
        }

        if (sizeOfList >= 2)
        {
            line.gameObject.SetActive(true);
            line.positionCount = sizeOfList;

            for (int i = 0; i < sizeOfList-1; i++)
            {
                line.SetPosition(i, selected_points[i].transform.position);
                line.SetPosition(i + 1, selected_points[i + 1].transform.position);
                distance = Vector3.Distance(selected_points[i].transform.position, selected_points[i + 1].transform.position) / scaling_factor / Object_toBe_measured.transform.localScale.x; //measure distance between two consecutive selected points and scale it accordingly to scaling factor and local scale of the model
                double distance_value = System.Math.Round(distance, 2); //round 'distance' to 2 decimal places
                Vector3 middlePosition = (selected_points[i].transform.position + selected_points[i + 1].transform.position) / 2;

                //display the measurement of the distance
                Transform distanceCanvas = selected_points[i].transform.GetChild(1);
                distanceCanvas.position = new Vector3(middlePosition.x, middlePosition.y + 0.02f, middlePosition.z);
                Text distanceText = distanceCanvas.GetComponentInChildren<Text>();
                distanceText.text = distance_value.ToString() + distance_unit;
                distanceText.color = new Color(1.0f, 0.20f, 0.0f); //FPAV: adjust color scheme
                //FPAV: if the point is not selected, turn off the distance text AND the background image as well
                selected_points[i].transform.GetChild(1).GetChild(0).GetComponentInChildren<Image>().enabled = true;

                Vector3 v = Camera.main.transform.position - distanceCanvas.transform.position;
                v.x = v.z = 0.0f;
                distanceCanvas.transform.LookAt(Camera.main.transform.position - v); //rotate the 'Canvas' component with the 'measurement.text' towards the camera (=towards the user)
                distanceCanvas.transform.Rotate(0, 180, 0);
            }
        }
    }

    //calculate and show the spatial parameters of each point; REMEMBER that the spatial and angular positions of the points are relative to the reference system
    void Display_point_info(GameObject point)
    {
        if (zUp)
        {
            verticalAxis = new Vector3(0, 1, 0);
            pointProjectionOnXY = new Vector3(point.transform.localPosition.x, 0, point.transform.localPosition.z);
            x_point = point.transform.localPosition.x; //|
            y_point = point.transform.localPosition.z; // > transform.localPosition gives you the position of the point relative to its parent (parent = ReferenceSystem)
            z_point = point.transform.localPosition.y; //|
        }
        else //meaning that Y axis is up
        {
            verticalAxis = new Vector3(0, 0, 1);
            pointProjectionOnXY = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, 0);
            x_point = point.transform.localPosition.x; 
            y_point = point.transform.localPosition.y;
            z_point = point.transform.localPosition.z;
        }

        float theta = Vector3.Angle(point.transform.localPosition, verticalAxis); //angle between the position of the point and the z-axis of the Reference System
        float phi = Vector3.Angle(pointProjectionOnXY, new Vector3(1, 0, 0)); //angle between the position of the point projected on the XY plane and the x-axis

        float referenceSystemScaling = reference_system.transform.localScale.x;
        double x_value = System.Math.Round(x_point / scaling_factor * referenceSystemScaling, 2); //reduce to 2 decimal places (NOTE: we multiply by the referenceSystem scale since the points are 'children' of the referenceSystem. This is done to be consistent with the units
        double y_value = System.Math.Round(y_point / scaling_factor * referenceSystemScaling, 2); 
        double z_value = System.Math.Round(z_point / scaling_factor * referenceSystemScaling, 2); 

        //value of THETA and PHI are +ve in clockwise direction wrt the axes
        if (y_point < 0)
        {
            phi_value = System.Math.Round(180 + 180 - phi, 2);  //round 'phi' to 2 decimal places --> 'phi' goes from 0 to 360 deg
        }
        else
        {
            phi_value = System.Math.Round(phi, 2);
        }
        theta_value = System.Math.Round(theta, 2);              //round 'theta' to 2 decimal places --> 'theta' goes from 0 to 180 deg
        float R = Vector3.Distance(reference_system.transform.localPosition, point.transform.localPosition) / scaling_factor * referenceSystemScaling;  //distance from the reference system to the point
        R_value = System.Math.Round(R, 2);                      //round distance 'R' to 2 decimal places
        
        //rotate the text of each point towards the user 
        canvas = point.GetComponentInChildren<Canvas>();
        Vector3 v = Camera.main.transform.position - canvas.transform.position;
        v.x = v.z = 0.0f;
        canvas.transform.LookAt(Camera.main.transform.position - v);
        canvas.transform.Rotate(0, 180, 0);
        Text point_info = point.GetComponentInChildren<Text>();
        point_info.text = "x: " + x_value.ToString() + ", y: " + y_value.ToString() + ", z: " + z_value.ToString() + distance_unit + "\n" +
                          "R: " + R_value.ToString() + distance_unit + "; θ: " + theta_value.ToString() + ", φ: " + phi_value.ToString() + " deg";
                           
                          //BEFORE --> "φ: " + phi_value.ToString() + ", θ: " + theta_value.ToString() + " deg, R: " + R_value.ToString() + distance_unit; 


        point.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f) / Object_toBe_measured.transform.localScale.x;  //in order to keep the points with a constant scaling of we have to divide them by the scaling of the model
    }
}