using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerGrabObjects : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject collidingObject;
    private GameObject objectInHand;
    public Material collidingMaterial;
    public Material originalMat;
    public bool grabbing;
    public Text collidingObjInfo;
    public string onlyGrabObjWith_TAG1;
    public string onlyGrabObjWith_TAG2;
    public GameObject canvasOnController;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>(); //track the controller to which the script is attached

        //set the collidingMaterial
        // FPAV: added a different colliding Material for the "Touched Points" of the left controller.
        if (trackedObj.name == "Controller (right)")
        {
            collidingMaterial = Resources.Load<Material>("Materials/ShowSelectedObj");
        }

        else if (trackedObj.name == "Controller (left)")
        {
            collidingMaterial = Resources.Load<Material>("Materials/ShowTouchedPoint");
        }
    }

    // When the trigger collider enters another, this sets up the other collider as a potential grab target.
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // It ensures that the target is set when the player holds a controller over an object for a while.
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // When the collider exits an object, abandoning an ungrabbed target, this code removes its target by setting it to null
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject.GetComponent<Renderer>().material = originalMat;
        collidingObject = null;
        originalMat = null;
    }

    private void SetCollidingObject(Collider col)
    {
        // Doesn’t make the GameObject a potential grab target if the player is already holding something or the object has no rigidbody.
        if (collidingObject || !col.GetComponent<Rigidbody>())    
        {
            return;
        }
        
        // Assigns the object as a potential grab target. NOTE: the 'tag' assigned to each object allows to distinguish what the LEFT and RIGHT controllers can grab or not
        if (col.gameObject.tag == onlyGrabObjWith_TAG1 || col.gameObject.tag == onlyGrabObjWith_TAG2)
        {
            collidingObject = col.gameObject;
            originalMat = collidingObject.GetComponent<Renderer>().material;
            collidingObject.GetComponent<Renderer>().material = collidingMaterial;

            //show the collidingObjInfo on the controller
            if (collidingObject.GetComponent<Text>() != null)
            {
                collidingObjInfo.text = collidingObject.name + ": " + collidingObject.GetComponent<Text>().text;
            }
            else
            {
                collidingObjInfo.text = collidingObject.name; //show the name of the touched object
                //collidingObjInfo.text = trackedObj.name;    //show the name of the controller
                //collidingObjInfo.text = "";                 //show nothing  
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Controller.GetHairTriggerDown()) // When the player squeezes the trigger and there’s a potential grab target, this grabs it
        {
            //this 'if statement' is needed to avoid issues while creating and moving measurement points 
            if (canvasOnController!=null)
            {
                if (canvasOnController.activeSelf == true & trackedObj.GetComponent<VR_Measurement_tool>().isActiveAndEnabled)
                {
                    collidingObjInfo.text = "Please, hide the Canvas on this" + "\n" + "controller by pressing the 'Menu Button'" + "\n" + "before moving and creating new points";
                    return;
                }
            }

            if (collidingObject) //if there is a collidingObject
            {
                GrabObject();
                grabbing = true;
            }
        }

        if (Controller.GetHairTriggerUp()) // If the player releases the trigger and there’s an object attached to the controller, this releases it
        {
            if (objectInHand)
            {
                ReleaseObject();
                grabbing = false;
            }
        }

        if (collidingObject)
        {
            if (collidingObject.activeSelf == false) //if the collidingObject gets destroyed (ex. when you destroy the points that you don't need anymore for measurement) the 'collidingObj' and 'originalMat'
            {                                        //are set to "null"; you also need to set the 'grabbing' bool to false for the 'VR_Measurement_tool' script to work as expected
                collidingObject = null;
                originalMat = null;
                grabbing = false;
            }
        }
    }

    private void GrabObject()
    {
        //collidingObject.GetComponent<Renderer>().material = originalMat; //uncomment this line (and comment the next line) if you want to keep the original material while grabbing the object
        collidingObject.GetComponent<Renderer>().material = collidingMaterial;
        objectInHand = collidingObject;
        //collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    // This function removes the grabbed object’s fixed joint and controls its speed and rotation when the player tosses it away.
    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            //objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;  //you need these last two lines if you want to throw away objects and give them some velocity
            //objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;  // Remove the reference to the formerly attached object.
    }
}

// IMPORTANT TO REMEMBER: the object you want to grab needs to have a 'RigidBody' component attached to it and a mesh collider in the same place.
//                        The controller also needs to have a 'RigidBody' component attached to it with useGravity=false and isKinematic=true