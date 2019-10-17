using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_change_transparency : MonoBehaviour {

    public static float r, g, b, transparency; //the values have to be between 0 and 1;
    public float change_transparency_sensitivity;
    public GameObject[] objectsChangeTransparency;
    public bool touchpad_pressed;
    public Vector2 controller_axis;
 
    private Sprite transparency_sprite;
    private Canvas[] all_canvas;
    public static Canvas upper_canvas;
    private Image transparency_image;
    public Text transparencyInfo;
    //private int internalCounter = 0;

    // Use this for initialization
    void Start()
    {
        change_transparency_sensitivity = 0.005f;

        //load the image that will be shown on the controller  
        transparency_sprite = Resources.Load<Sprite>("Sprites/transparency"); //DO NOT put .png when writing the path for the sprite
    }

    private void OnEnable()
    {
        DisplayTransparencyImage();
    }

    // Update is called once per frame
    void Update()
    {
        controller_axis = GetComponent<VR_inputs_controller>().controller_axis;
        touchpad_pressed = GetComponent<VR_inputs_controller>().touchpad_pressed;

        objectsChangeTransparency = GameObject.FindGameObjectsWithTag("selected_obj");

        if (touchpad_pressed & objectsChangeTransparency != null)
        {
            foreach (GameObject objChangeTransparency in objectsChangeTransparency)
            {
                if (objChangeTransparency.GetComponent<Renderer>() != null) //check if the object has a 'Shader' (objects without a 'Shader', ex. buttons, cannot change transparency)
                {
                    ChangeTransparency(objChangeTransparency);
                }
                else
                {
                    print(objChangeTransparency.name + " does not have a 'Renderer' component attached to it'");
                }
            }
        }

        var localTransparency = System.Math.Round(transparency, 2);
        transparencyInfo.text = "Transparency : " + localTransparency.ToString(); //display the transparency level on the 'Tools Menu'
    }

    public void DisplayTransparencyImage() //display the transparency image on the controller used to change transparency
    {
        all_canvas = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in all_canvas)
        {
            if (canvas.name == "Canvas_upper")
            {
                upper_canvas = canvas;
            }
        }
        transparency_image = upper_canvas.GetComponentInChildren<Image>();
        transparency_image.sprite = Resources.Load<Sprite>("Sprites/transparency"); //DO NOT put .png when writing the path for the sprite
    }

    public void ChangeTransparency(GameObject selectedObj)
    {
        Material m = selectedObj.GetComponent<Renderer>().material;

        ////change from 'Standard' to 'Transparent' Rendering Mode
        //m.SetFloat("_Mode", 3f);
        //m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //m.SetInt("_ZWrite", 0);
        //m.DisableKeyword("_ALPHATEST_ON");
        //m.EnableKeyword("_ALPHABLEND_ON");
        //m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //m.renderQueue = 3000 + internalCounter;

        r = m.color.r;
        g = m.color.g;
        b = m.color.b;
        transparency = m.color.a;

        m.color = new Color(r, g, b, transparency);
        selectedObj.GetComponent<Renderer>().material = m;

        transparency = Mathf.Clamp(transparency, 0.1f, 1.0f); //transparency (= 'a' value) has an upper limit of 1.0 and a lower limit of 0.1 

        if (controller_axis.y > 0.7f || controller_axis.y < -0.7f)
        {
            transparency += controller_axis.y * change_transparency_sensitivity;
            m.color = new Color(r, g, b, transparency);
            selectedObj.GetComponent<Renderer>().material = m;
        }
        //internalCounter++;
    }
}
//https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html

