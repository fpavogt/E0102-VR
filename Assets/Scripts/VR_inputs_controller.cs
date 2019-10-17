using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_inputs_controller : MonoBehaviour {

    // Create a public reference to the object being tracked. In this case, the controller.
    public SteamVR_TrackedObject trackedObj;

    // A Device property to provide easy access to the controller. It uses the tracked object’s index to return the controller’s input.
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public Vector3 controller_position;
    public Vector2 controller_axis;
    public bool trigger_pressed;
    public bool grip_pressed;
    public bool touchpad_pressed;
    public bool menu_pressed;

    public delegate void PressAction(string controllerName);
    public static event PressAction OnTriggerPressed;
    public static event PressAction OnGripButtonPressed;
    public static event PressAction OnMenuPressed;
    public static event PressAction OnTouchpadUpPressed;
    public static event PressAction OnTouchpadDownPressed;
    public static event PressAction OnTouchpadRightPressed;
    public static event PressAction OnTouchpadLeftPressed;

    //public delegate void HandleMenu(string controllerName);

    // Use this for initialization
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        controller_position = trackedObj.transform.position;

        // 1: TOUCHPAD
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Debug.Log("touchpad pressed");
            touchpad_pressed = true;

            if (Controller.GetAxis() != Vector2.zero)
            {
                //Debug.Log(gameObject.name + Controller.GetAxis());

                controller_axis = Controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
                if (controller_axis.y > 0.6f)
                {
                    if (OnTouchpadUpPressed != null)
                        OnTouchpadUpPressed(trackedObj.name);
                }

                else if (controller_axis.y < -0.6f)
                {
                    if (OnTouchpadDownPressed != null)
                        OnTouchpadDownPressed(trackedObj.name);
                }

                if (controller_axis.x > 0.6f)
                {
                    if (OnTouchpadRightPressed != null)
                        OnTouchpadRightPressed(trackedObj.name);
                }

                else if (controller_axis.x < -0.6f)
                {
                    if (OnTouchpadLeftPressed != null)
                        OnTouchpadLeftPressed(trackedObj.name);
                }
            }
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Debug.Log("touchpad released");
            touchpad_pressed = false;
        }

        //2 : TRIGGER
        if (Controller.GetHairTriggerDown())
        {
            //Debug.Log(trackedObj.name + " Trigger Press");
            trigger_pressed = true;

            if (OnTriggerPressed != null)
                OnTriggerPressed(trackedObj.name);
        }
        if (Controller.GetHairTriggerUp())
        {
            //Debug.Log(trackedObj.name + " Trigger Release");
            trigger_pressed = false;
        }

        //3 : GRIP BUTTON
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            //Debug.Log(gameObject.name + " Grip Press");
            grip_pressed = true;

            if (OnGripButtonPressed != null)
                OnGripButtonPressed(trackedObj.name);
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            //Debug.Log(gameObject.name + " Grip Release");
            grip_pressed = false;
        }

        //4 : MENU BUTTON
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            //Debug.Log("Menu pressed by: " + trackedObj.name);
            menu_pressed = true;

            if (OnMenuPressed != null)
                OnMenuPressed(trackedObj.name);
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            //Debug.Log(gameObject.name + " Menu Release");
            menu_pressed = false;
        }
    }
}
