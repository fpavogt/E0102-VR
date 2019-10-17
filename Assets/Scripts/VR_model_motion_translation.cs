using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_model_motion_translation : MonoBehaviour {

    public GameObject objectToBeMoved;
    //public float sensitivity;
    public bool touchpad_pressed;
    public Vector2 controller_axis;
    private Vector3 initial_position;

    private Sprite translation_sprite;
    private Canvas[] all_canvas;
    public static Canvas upper_canvas;
    private Image motion_translation_image;

    // Use this for initialization
    void Start()
    {
        initial_position = objectToBeMoved.transform.position;

        //load the image that will be shown on the controller  
        // FPAV: not needed .... load it every time ... is it bad ?
        //translation_sprite = Resources.Load<Sprite>("Sprites/motion_translation"); //DO NOT put .png when writing the path for the sprite
    }

    private void OnEnable()
    {
        Display_motion_translation_image();
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
                //print("Moving Up");
                objectToBeMoved.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
            }
            if (controller_axis.y < -0.7f)
            {
                //print("Moving Down");
                objectToBeMoved.transform.Translate(Vector3.down * Time.deltaTime, Space.World);
            }
            if (controller_axis.x > 0.7f)
            {
                //print("Moving Right");
                objectToBeMoved.transform.Translate(Vector3.right * Time.deltaTime, Camera.main.transform); 
            }

            if (controller_axis.x < -0.7f)
            {
                //print("Moving left");
                objectToBeMoved.transform.Translate(Vector3.left * Time.deltaTime, Camera.main.transform); 
            }
        }
        else if (touchpad_pressed == false)
        {
            objectToBeMoved.transform.position += new Vector3(0, 0, 0);
        }

        //RESET position of the model to its initial position by pressing the touchpad in the centre
        if (touchpad_pressed)
        {
            if (controller_axis.y < 0.4f & controller_axis.y > -0.4f & controller_axis.x < 0.4f & controller_axis.x > -0.4f)
            {
                objectToBeMoved.transform.position = initial_position;
            }
        }
    }

    void Display_motion_translation_image()
    {
        all_canvas = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in all_canvas)
        {
            if (canvas.name == "Canvas_upper")
            {
                upper_canvas = canvas;
            }
        }
        motion_translation_image = upper_canvas.GetComponentInChildren<Image>();
        //motion_translation_image.sprite = translation_sprite; //FPAV: for reasons I do not understand, this was showing a white square the first time around ...
        motion_translation_image.sprite = Resources.Load<Sprite>("Sprites/motion_translation");

    }
}

