using UnityEngine;
using UnityEngine.UI;

public class VR_scale_model : MonoBehaviour
{
    public GameObject objectToBeScaled;
    public float sensitivity;
    private Vector3 initial_scaling;
    private float scaling_factor;
    public bool touchpad_pressed;
    public Vector2 controller_axis;
    public Text scalingInfo;

    private Sprite scaling_sprite;
    private Canvas[] all_canvas;
    public static Canvas upper_canvas;
    private Image scaling_image;

    public void OnEnable()
    {
        //load the image that will be shown on the controller  
        scaling_sprite = Resources.Load<Sprite>("Sprites/scaling"); //DO NOT put .png when writing the path for the sprite
        Display_scaling_image();

        //in order to avoid performance issues (especially in 'Physics'), we disable the colliders of all che children of the objects inside 'ContainerOfAllObj', except the ReferenceSystem
        foreach (Transform parent in objectToBeScaled.transform)
        {
            if (parent.name != "ReferenceSystem")
            {
                foreach (Transform child in parent)
                {
                    child.GetComponent<MeshCollider>().enabled = false;
                }
            }
        }
    }

    public void OnDisable()
    {
        //we enable the colliders that were disabled
        foreach (Transform parent in objectToBeScaled.transform)
        {
            if (parent.name != "ReferenceSystem")
            {
                foreach (Transform child in parent)
                {
                    var myMeshCollider = child.GetComponent<MeshCollider>();
                    if (child.GetComponent<MeshRenderer>().enabled == false)
                    {
                        myMeshCollider.enabled = false;
                    }
                    else
                    {
                        myMeshCollider.enabled = true;
                    }
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        initial_scaling = objectToBeScaled.transform.localScale;
        sensitivity = 0.003f;
    }

    // Update is called once per frame
    void Update()
    {
        controller_axis = GetComponent<VR_inputs_controller>().controller_axis;
        touchpad_pressed = GetComponent<VR_inputs_controller>().touchpad_pressed;

        //scaling with the touchpad(press-up --> scale_up; press-down --> scale_down; release --> stop)
        if (touchpad_pressed == true)
        {
            if ((controller_axis.y > 0.7f || controller_axis.y < -0.7f))//here we check the input from the controller's touchpad
            {
                scaling_factor = sensitivity * controller_axis.y;
                objectToBeScaled.transform.localScale += new Vector3(scaling_factor, scaling_factor, scaling_factor);

                if (objectToBeScaled.transform.localScale.x <= 0.01) //avoid to have a negative scaling-- > we set a lower limit of 0.01
                {
                    objectToBeScaled.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
            }

            //RESET scaling of the model to its initial scaling by pressing the touchpad in the centre
            else if (controller_axis.y < 0.4f & controller_axis.y > -0.4f & controller_axis.x < 0.4f & controller_axis.x > -0.4f)
            {
                objectToBeScaled.transform.localScale = initial_scaling;
            }
        }

        if (touchpad_pressed == false)
        {
            objectToBeScaled.transform.localScale += new Vector3(0, 0, 0);
        }

        var localScale = System.Math.Round(objectToBeScaled.transform.localScale.x, 2);
        scalingInfo.text = "Scaling : " + localScale.ToString();
    }

    void Display_scaling_image()
    {
        all_canvas = GetComponentsInChildren<Canvas>();
        foreach (Canvas canvas in all_canvas)
        {
            if (canvas.name == "Canvas_upper")
            {
                upper_canvas = canvas;
            }
        }
        scaling_image = upper_canvas.GetComponentInChildren<Image>();
        scaling_image.sprite = scaling_sprite;
    }
}
