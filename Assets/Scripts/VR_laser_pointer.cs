using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PointerEventArguments //we need this to send info to other functions, in particular when interacting with the UI and the objects in the scene
{
    public float distance;
    public Transform target;
}

public class VR_laser_pointer : MonoBehaviour
{
    public int laser_counter = 0;
    public Color color;
    public float thickness = 0.002f;
    public GameObject holder;
    public GameObject pointer;
    public GameObject touchingPoint;
    public bool laserIsActive = true;  //with this boolean you can activate or disactivate the laser beam
    public bool addRigidBody = false;
    public bool trigger_pressed;
    public Transform previousContact = null;
    public GameObject selectedObject;

    //create two events
    public delegate void LaserPointerEvent(object sender, PointerEventArguments e);
    public static event LaserPointerEvent PointerIn;   //note that the event variable PointerIn is static so that we can use it outside the class without instantiating an object of this class
    public static event LaserPointerEvent PointerOut;

    // Use this for initialization
    void Start ()
    {
        holder = new GameObject();   //create a new GameObject to hold the laser beam
        holder.name = "laser_ray_holder";
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;

        touchingPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        touchingPoint.GetComponent<SphereCollider>().enabled = false; //disable the 'SphereCollider' component, otherwise it interferes with the laser beam; the Sphere for the touchingPoint at the end of the laser beam has been added just for aesthetics
        touchingPoint.name = "laser_TouchingPoint";
        touchingPoint.transform.parent = holder.transform;
        touchingPoint.transform.localPosition = new Vector3(0f, 0f, 100f);
        touchingPoint.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube); //create the "physical" laser beam
        pointer.name = "laser_beam";
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f); //the laser beam has a length of 100 (z-axis)
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        if (addRigidBody)
        {
            if (collider)
            {
                collider.isTrigger = true;
            }
            Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
        }
        else
        {
            if (collider)
            {
                Object.Destroy(collider);
            }
        }
        //Material newMaterial = new Material(Shader.Find("Unlit/Color"));  
        Material newMaterial = Resources.Load<Material>("Materials/LaserBeam");

        //choose the color of the laser beam from the Inspector
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;
        touchingPoint.GetComponent<MeshRenderer>().material = newMaterial;
    }

    public virtual void OnPointerIn(PointerEventArguments e)
    {
        if (PointerIn != null)  //Unity causes an error if we invoke an event with no subscribers, therefore we should always make sure that the event != null before invoking it
            PointerIn(this, e);
    }

    public virtual void OnPointerOut(PointerEventArguments e)
    {
        if (PointerOut != null)
            PointerOut(this, e);
    }

    //use the OnGripButtonPressed event to activate or disactivate the laser
    void OnEnable()
    {
        VR_inputs_controller.OnGripButtonPressed += EnableLaser;
    }

    void OnDisable()
    {
        VR_inputs_controller.OnGripButtonPressed -= EnableLaser;
    }

    // Update is called once per frame
    void Update ()
    {
        trigger_pressed = GetComponent<VR_inputs_controller>().trigger_pressed;

        if (laserIsActive)
        {
            transform.Find("laser_ray_holder").gameObject.SetActive(true);  
        }
        else
        {
            transform.Find("laser_ray_holder").gameObject.SetActive(false);
        }

        //draw the ray
        float dist = 100f;  //length of the ray
        Ray raycast = new Ray(transform.position, transform.forward);  //specify the origin and direction of the ray
        RaycastHit hit;  //this variable stores anything that gets hit by the ray
        bool bHit = Physics.Raycast(raycast, out hit);  //boolean to check for collisions with any colliders. You can also specify the float maxDistance = max distance the ray should check for collisions; since it is not specified, it will be automatically set to infinity

        if (previousContact && previousContact != hit.transform)
        {
            PointerEventArguments args = new PointerEventArguments();
            args.distance = 0f;
            args.target = previousContact;
            OnPointerOut(args);
            previousContact = null;
        }
        if (bHit && previousContact != hit.transform)
        {
            PointerEventArguments argsIn = new PointerEventArguments();
            argsIn.distance = hit.distance;
            argsIn.target = hit.transform;
            OnPointerIn(argsIn);
            previousContact = hit.transform;
        }

        if (bHit)  //if the ray hits a collider
        {
            dist = hit.distance;
            pointer.transform.localScale = new Vector3(thickness, thickness, dist); //the ray has as length the distance from the ray's origin to the impact point
            touchingPoint.transform.localPosition = new Vector3(0, 0, dist);
            if (trigger_pressed) //if the controller's trigger is pressed while the ray hits a collider
            {
                //Debug.Log(hit.collider.name);  //return the name of the collider
                pointer.transform.localScale = new Vector3(thickness * 2f, thickness * 2f, hit.distance); //increase the thickness of the laser beam
                selectedObject = hit.transform.gameObject; //select an object
            }
        }
        else if (!bHit) //if the ray does not hit any collider
        {
            previousContact = null;
            pointer.transform.localScale = new Vector3(thickness, thickness, dist); //the ray has full length
            touchingPoint.transform.localPosition = new Vector3(0, 0, dist);
            if (trigger_pressed)
            {
                selectedObject = null; //if the user presses the trigger in free space, there won't be any selected object
            }
        }
        pointer.transform.localPosition = new Vector3(0f, 0f, dist/2f);
    }

    public void EnableLaser(string controllerName)
    {

        if (controllerName == "Controller (right)")
        {
            laser_counter++;
            if (laser_counter % 2 == 0)
            {
                //Debug.Log("Laser Enabled");
                laserIsActive = true;
            }
            else
            {
                //Debug.Log("Laser Disabled");
                laserIsActive = false;
            }
        }
    }
}

//https://unity3d.college/2017/06/17/steamvr-laser-pointer-menus/