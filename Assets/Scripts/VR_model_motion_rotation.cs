using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_model_motion_rotation : MonoBehaviour {

    public GameObject objectToBeRotated;
    public bool touchpad_pressed;
    public Vector2 controller_axis;
    private Quaternion initial_rotation;
    public float rotate_speed;

    private Sprite rotation_sprite;
    private Canvas[] all_canvas;
    public static Canvas upper_canvas;
    private Image motion_rotation_image;

    // Use this for initialization
    void Start()
    {
        initial_rotation = objectToBeRotated.transform.rotation;
        rotate_speed = 0.4f;

        //load the image that will be shown on the controller  
        //FPAV: not needed anymore
        //rotation_sprite = Resources.Load<Sprite>("Sprites/motion_rotation"); //DO NOT put .png when writing the path for the sprite
    }

    private void OnEnable()
    {
        Display_motion_rotation_image();
    }

    // Update is called once per frame
    void Update()
    {
        controller_axis = GetComponent<VR_inputs_controller>().controller_axis;
        touchpad_pressed = GetComponent<VR_inputs_controller>().touchpad_pressed;

        //moving the object with the touchpad(press-up/down --> move_up/down; press-left/right --> move_left/right; release --> stop)
        if (touchpad_pressed == true)
        {
            if (controller_axis.y > 0.7f)
            {
                //print("Rotating Upwards");
                objectToBeRotated.transform.Rotate(Camera.main.transform.right, rotate_speed, Space.World);
            }
            if (controller_axis.y < -0.7f)
            {
                //print("Rotating Downwards");
                objectToBeRotated.transform.Rotate(Camera.main.transform.right, -rotate_speed, Space.World);
            }
            if (controller_axis.x > 0.7f)
            {
                //print("Rotating Anti-clockwise");
                objectToBeRotated.transform.Rotate(Vector3.up, -rotate_speed, Space.World);
            }

            if (controller_axis.x < -0.7f)
            {
                //print("Rotating Clockwise");
                objectToBeRotated.transform.Rotate(Vector3.up, +rotate_speed, Space.World);
            }
        }
        else if (touchpad_pressed == false)
        {
            objectToBeRotated.transform.Rotate(0, 0, 0);
        }

        //RESET rotation of the model to its initial rotation by pressing the touchpad in the centre
        if (touchpad_pressed)
        {
            if (controller_axis.y < 0.4f & controller_axis.y > -0.4f & controller_axis.x < 0.4f & controller_axis.x > -0.4f)
            {
                objectToBeRotated.transform.rotation = initial_rotation;
            }
        }
    }

    void Display_motion_rotation_image()
    {
        all_canvas = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in all_canvas)
        {
            if (canvas.name == "Canvas_upper")
            {
                upper_canvas = canvas;
            }
        }
        motion_rotation_image = upper_canvas.GetComponentInChildren<Image>();
        //motion_rotation_image.sprite = rotation_sprite; //FPAV: this does not work the first time around (shows a blank square) -> load the image each time.
        motion_rotation_image.sprite = Resources.Load<Sprite>("Sprites/motion_rotation"); ;
    }
}