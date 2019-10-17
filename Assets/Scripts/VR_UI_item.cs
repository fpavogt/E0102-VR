using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class VR_UI_item : MonoBehaviour {

    //Note: in order to be able to interact with a button, the latter needs to have a collider attached to it.
    //With this script we ensure that colliders size & shape match our button.  
    //We could do this manually, but to avoid having to resize the collider 
    //whenever the button changes, we can use this simple script.

    private BoxCollider boxCollider;
    private RectTransform rectTransform;
    private int translation_counter = 0;
    private int rotation_counter = 0;
    private int scaling_counter = 0;
    private int measurement_counter = 0;
    private int clipPlanes_counter = 0;
    private int FFclipPlane_counter = 0;
    private int transparency_counter = 0;
    private int referenceSyst_counter = 0;

    public GameObject controller;
    public GameObject toolsInfo;
    private GameObject containerOfAllObj;
    public Shader crossSectionShader3planes;
    public Shader crossSectionShader1plane;
    public Shader standardShader;
    public Shader diffuseZWrite;
    private GameObject plane1;
    private GameObject plane2;
    private GameObject plane3;
    private GameObject FFplane;
    private GameObject[] selectedObj;
    private GameObject vis_inst_canvas;
    public static bool measuring;

    private void OnEnable()
    {
        ValidateCollider();
        containerOfAllObj = GameObject.Find("ContainerOfAllObj");
        plane1 = GameObject.Find("ClipPlane_XY");
        plane2 = GameObject.Find("ClipPlane_XZ");
        plane3 = GameObject.Find("ClipPlane_YZ");
        FFplane = GameObject.Find("ClipPlane_FullFreedom");

        vis_inst_canvas = GameObject.Find("vis-inst_canvas");

    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    private void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        boxCollider.size = rectTransform.sizeDelta;
    }

    public void Start()
    {
        //clipping planes invisible
        //FPAV: add "handles to the clip planes -> moved the actual clipping planes to a Child level. 
        // It seems easier to just disable or enable as required. The only problem is: Once the plan is disabled, I may not be able to access it.
        // I suppose this might cause some trouble at time ... for now, a dirty work around, checking if I can access the planes.

        if (plane1 != null)
        {
            plane1.SetActive(false);
        }
        if (plane2 != null)
        {
            plane2.SetActive(false);
        }
        if (plane3 != null)
        {
            plane3.SetActive(false);
        }

        if (FFplane != null)
        {
            FFplane.SetActive(false);
        }

        // EBa: oginal code
        //plane1.GetComponent<MeshCollider>().enabled = false; //plane #1
        //plane1.GetComponent<MeshRenderer>().enabled = false;
        //plane2.GetComponent<MeshCollider>().enabled = false; //plane #2
        //plane2.GetComponent<MeshRenderer>().enabled = false;
        //plane3.GetComponent<MeshCollider>().enabled = false; //plane#3
        //plane3.GetComponent<MeshRenderer>().enabled = false;
        //FFplane.GetComponent<MeshCollider>().enabled = false; //FullFreedom plane
        //FFplane.GetComponent<MeshRenderer>().enabled = false;
    }

    //////////////////////////  LIST OF ALL THE FUNCTIONS THAT CAN BE ENABLED THROUGH THE BUTTONS ON THE TOOLS MENU  /////////////////////////   

    ////////////////////////////////
    //      RIGHT CONTROLLER      //
    ////////////////////////////////

    public void EnableMotionTranslation() //RIGHT CONTROLLER
    {
        translation_counter++;
        if (translation_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Translation mode");
            VR_model_motion_translation.upper_canvas.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Unity");
            controller.GetComponent<VR_model_motion_translation>().enabled = false;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/no_selected_controllers");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(1).GetComponent<Image>().enabled = false;


        }
        else
        {
            ChangeButtonColor_Enable("Translation mode");
            controller.GetComponent<VR_model_motion_translation>().enabled = true;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/right_controller");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(1).GetComponent<Image>().enabled =  true;


        }

        //tell the user about the possible interference between Rotation and Translation modes if both enabled at the same time
        if (controller.GetComponent<VR_model_motion_rotation>().enabled == true & controller.GetComponent<VR_model_motion_translation>().enabled == true)
        {
            toolsInfo.GetComponent<Text>().text = "WARNING:" + "\n" + "Both the Rotation and Translation modes are enabled.";
        }
        else
        {
            toolsInfo.GetComponent<Text>().text = "";
        }
    }

    public void EnableMotionRotation() //RIGHT CONTROLLER
    {
        rotation_counter++;
        if (rotation_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Rotation mode");
            VR_model_motion_rotation.upper_canvas.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Unity");
            controller.GetComponent<VR_model_motion_rotation>().enabled = false;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/no_selected_controllers");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(2).GetComponent<Image>().enabled = false;
        }
        else
        {
            ChangeButtonColor_Enable("Rotation mode");
            controller.GetComponent<VR_model_motion_rotation>().enabled = true;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/right_controller");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(2).GetComponent<Image>().enabled = true;
        }

        //tell the user about the possible interference between Rotation and Translation modes if both enabled at the same time
        if (controller.GetComponent<VR_model_motion_rotation>().enabled == true & controller.GetComponent<VR_model_motion_translation>().enabled == true)
        {
            toolsInfo.GetComponent<Text>().text = "WARNING:" + "\n" + "Both the Rotation and Translation modes are enabled.";
        }
        else
        {
            toolsInfo.GetComponent<Text>().text = "";
        }
    }

    public void EnableClippingPlanes() //RIGHT CONTROLLER
    {

        clipPlanes_counter++;
        if (clipPlanes_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("X-Y-Z clip planes");
            standardShader = Shader.Find("Standard");

            var parentCount = containerOfAllObj.transform.childCount;
            for (int k = 1; k < parentCount; k++) //skip the first child of containerOfAllObjs because it is the ReferenceSystem
            {
                AddDesiredShader(containerOfAllObj.transform.GetChild(k), diffuseZWrite); //back to transparent shader
            }
            //clipping planes invisible

            //FPAV: add "handles to the clip planbes -> just enable or diable them.
            plane1.gameObject.SetActive(false);
            plane2.gameObject.SetActive(false);
            plane3.gameObject.SetActive(false);

            //plane1.GetComponent<MeshCollider>().enabled = false; //plane #1
            //plane1.GetComponent<MeshRenderer>().enabled = false;
            //plane2.GetComponent<MeshCollider>().enabled = false; //plane #2
            //plane2.GetComponent<MeshRenderer>().enabled = false;
            //plane3.GetComponent<MeshCollider>().enabled = false; //plane#3
            //plane3.GetComponent<MeshRenderer>().enabled = false;

            //FPAV: also, disable the help panel!
            vis_inst_canvas.transform.GetChild(3).GetComponent<Image>().enabled = false;

        }
        else
        {
            ChangeButtonColor_Enable("X-Y-Z clip planes");
            var parentCount = containerOfAllObj.transform.childCount;

            //FPAV: add "handles to the clip planes -> just enable or diable them.
            plane1.gameObject.SetActive(true);
            plane2.gameObject.SetActive(true);
            plane3.gameObject.SetActive(true);

            for (int k = 1; k < parentCount; k++) //skip the first child of containerOfAllObjs because it is the ReferenceSystem
            {
                AddDesiredShader(containerOfAllObj.transform.GetChild(k), crossSectionShader3planes); //add the CrossSection shader to each child in 'containerOfAllObj'
            }
            //clipping planes visible

            //plane1.GetComponent<MeshCollider>().enabled = true; //plane #1
            //plane1.GetComponent<MeshRenderer>().enabled = true;
            //plane2.GetComponent<MeshCollider>().enabled = true; //plane #2
            //plane2.GetComponent<MeshRenderer>().enabled = true;
            //plane3.GetComponent<MeshCollider>().enabled = true; //plane#3
            //plane3.GetComponent<MeshRenderer>().enabled = true;

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(3).GetComponent<Image>().enabled = true;

        }
    }

    public void EnableFullFreedomClipPlane()
    {
        FFclipPlane_counter++;
        if (FFclipPlane_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Full-freedom clip plane");
            standardShader = Shader.Find("Standard");

            var parentCount = containerOfAllObj.transform.childCount;
            for (int m = 1; m < parentCount; m++) //skip the first child of containerOfAllObjs because it is the ReferenceSystem
            {
                AddDesiredShader(containerOfAllObj.transform.GetChild(m), diffuseZWrite); //back to transparent shader
            }
            //Full Freedom Clip Plane invisible
            //FPAV: update required, after adding handles
            FFplane.gameObject.SetActive(false);

            //FFplane.GetComponent<MeshCollider>().enabled = false; 
            //FFplane.GetComponent<MeshRenderer>().enabled = false;

            //FPAV: also, disable the help panel!
            vis_inst_canvas.transform.GetChild(3).GetComponent<Image>().enabled = false;
        }
        else
        {
            ChangeButtonColor_Enable("Full-freedom clip plane");

            var parentCount = containerOfAllObj.transform.childCount;
            for (int m = 1; m < parentCount; m++) //skip the first child of containerOfAllObjs because it is the ReferenceSystem
            {
                AddDesiredShader(containerOfAllObj.transform.GetChild(m), crossSectionShader1plane); //add the CrossSection shader to each child in 'containerOfAllObj'
            }
            //Full Freedom Clip Plane visible
            //FPAV: update required, after adding handles
            FFplane.gameObject.SetActive(true);
            //FFplane.GetComponent<MeshCollider>().enabled = true;
            //FFplane.GetComponent<MeshRenderer>().enabled = true;

            //FPAV: also, disable the help panel!
            vis_inst_canvas.transform.GetChild(3).GetComponent<Image>().enabled = true;

        }
    }

    ////////////////////////////////
    //      LEFT CONTROLLER       //
    ////////////////////////////////

    public void EnableScaling() //LEFT CONTROLLER
    {
        scaling_counter++;
        if (scaling_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Scaling mode");
            VR_scale_model.upper_canvas.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Unity");
            controller.GetComponent<VR_scale_model>().enabled = false;
            toolsInfo.GetComponent<Text>().text = "";
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/no_selected_controllers");

            //FPAV: also, disable the help panel!
            vis_inst_canvas.transform.GetChild(4).GetComponent<Image>().enabled = false;
        }
        else
        {
            ChangeButtonColor_Enable("Scaling mode");
            controller.GetComponent<VR_scale_model>().enabled = true;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/left_controller");
            toolsInfo.GetComponent<Text>().text = "NOTE" + "\n" + "When Scaling Mode is enabled you cannot grab the model's parts";

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(4).GetComponent<Image>().enabled = true;

        }
    }

    public void EnableMeasurements() //LEFT CONTROLLER
    {
        measurement_counter++;
        Transform pointerL = controller.transform.Find("pointerL");
        Transform sphereColl = controller.transform.Find("SphericalPointer_L");
        Transform selectingPlane = controller.transform.Find("Selecting_plane");
        Transform unselectingPlane = controller.transform.Find("Unselecting_plane");

        if (measurement_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Measurement mode");
            measuring = false;
            //pointerL.GetComponentInChildren<SphereCollider>().enabled = false;   //controller should not grab points if the 'Measurement Tool' is disabled --> we disable the colliders to grab the points
            //pointerL.GetComponentInChildren<MeshRenderer>().enabled = false;
            //FPAV
            sphereColl.GetComponent<SphereCollider>().enabled = false;   //controller should not grab points if the 'Measurement Tool' is disabled --> we disable the colliders to grab the points
            sphereColl.GetComponent<MeshRenderer>().enabled = false;

            //FPAV: hide the entire "planes when not in measurement mode, to avoid confusing the user
            selectingPlane.gameObject.SetActive(false);
            unselectingPlane.gameObject.SetActive(false);

            selectingPlane.GetComponent<MeshCollider>().enabled = false;
            unselectingPlane.GetComponent<MeshCollider>().enabled = false;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/no_selected_controllers");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(5).GetComponent<Image>().enabled = false;

        }
        else
        {
            ChangeButtonColor_Enable("Measurement mode");
            controller.GetComponent<VR_Measurement_tool>().enabled = true;
            measuring = true;
            //pointerL.GetComponentInChildren<SphereCollider>().enabled = true;   //enable the colliders to grab the points
            //pointerL.GetComponentInChildren<MeshRenderer>().enabled = true;
            //FPAV
            sphereColl.GetComponent<SphereCollider>().enabled = true;   //controller should not grab points if the 'Measurement Tool' is disabled --> we disable the colliders to grab the points
            sphereColl.GetComponent<MeshRenderer>().enabled = true;

            //FPAB: hide the entire "planes when not in measurement mode, to avoid consuding the user
            selectingPlane.gameObject.SetActive(true);
            unselectingPlane.gameObject.SetActive(true);

            selectingPlane.GetComponent<MeshCollider>().enabled = true;
            unselectingPlane.GetComponent<MeshCollider>().enabled = true;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/left_controller");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(5).GetComponent<Image>().enabled = true;
        }
    }

    public void ChangeTransparency() //LEFT CONTROLLER
    {
        transparency_counter++;
        if (transparency_counter % 2 == 0)
        {
            ChangeButtonColor_Disable("Transparency mode");
            VR_change_transparency.upper_canvas.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/Unity");
            controller.GetComponent<VR_change_transparency>().enabled = false;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/no_selected_controllers");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(6).GetComponent<Image>().enabled = false;
        }
        else
        {
            ChangeButtonColor_Enable("Transparency mode");
            controller.GetComponent<VR_change_transparency>().enabled = true;
            transform.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Sprites/left_controller");

            //FPAV: also, enable the help panel!
            vis_inst_canvas.transform.GetChild(6).GetComponent<Image>().enabled = true;
        }
    }

    public void ResetObjPosition()
    {
        selectedObj = GameObject.FindGameObjectsWithTag("selected_obj");
        foreach (GameObject objResetPosition in selectedObj)
        {
            objResetPosition.transform.localPosition = new Vector3(0, 0, 0);
            objResetPosition.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
    }

    public void Show_Hide_ReferenceSystem()
    {
        referenceSyst_counter++;
        GameObject containerOfallObjs = GameObject.Find("ContainerOfAllObj");
        Transform referenceSystem = containerOfallObjs.transform.Find("ReferenceSystem");
        if (referenceSyst_counter % 2 == 0)
        {
            transform.GetComponentInChildren<Text>().text = "Hide R=3pc sphere";
            transform.GetComponentInChildren<Text>().color = Color.white;

            // FPAV: make the button change color ... could not get ChangeButtonColor_Disable to work, so do it by hand ...
            Button myButton = transform.GetComponent<Button>();
            ColorBlock myButtonColor = myButton.colors;
            myButtonColor.normalColor = myButtonColor.pressedColor;
            myButton.colors = myButtonColor;

            referenceSystem.transform.Find("Center").gameObject.SetActive(true);
            referenceSystem.transform.Find("x").gameObject.SetActive(true);
            referenceSystem.transform.Find("y").gameObject.SetActive(true);
            referenceSystem.transform.Find("z").gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponentInChildren<Text>().text = "Show R=3pc sphere";
            transform.GetComponentInChildren<Text>().color = Color.black;

            // FPAV: make the button change color ... could not get ChangeButtonColor_Disable to work, so do it by hand ...
            Button myButton = transform.GetComponent<Button>();
            ColorBlock myButtonColor = myButton.colors;
            myButtonColor.normalColor = Color.white;
            myButton.colors = myButtonColor;


            referenceSystem.transform.Find("Center").gameObject.SetActive(false);
            referenceSystem.transform.Find("x").gameObject.SetActive(false);
            referenceSystem.transform.Find("y").gameObject.SetActive(false);
            referenceSystem.transform.Find("z").gameObject.SetActive(false);
        }
    }

    public void ChangeButtonColor_Disable(string buttonName) 
    {
        Toggle myButton = GetComponent<Toggle>();

        Text myButton_text = myButton.GetComponentInChildren<Text>();
        ColorBlock myButtonColor = myButton.colors;

        myButton_text.text = buttonName;// + " Disabled";
        myButton_text.color = Color.black;

        myButtonColor.normalColor = Color.white;
        myButton.colors = myButtonColor;
    }

    public void ChangeButtonColor_Enable(string buttonName) 
    {
        Toggle myButton = GetComponent<Toggle>();

        Text myButton_text = myButton.GetComponentInChildren<Text>();
        ColorBlock myButtonColor = myButton.colors;

        myButton_text.text = buttonName;// + " Enabled";
        myButton_text.color = Color.white; //color of the text on the Button

        myButtonColor.normalColor = myButtonColor.pressedColor; //color of the Button itself
        myButton.colors = myButtonColor;
    }

    public void AddDesiredShader(Transform parentChangeShader, Shader desiredShader)
    {
        var childCount = parentChangeShader.childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (desiredShader == standardShader) //add the Standard Shader (if the clipping planes are not enabled)
            {
                Material childMat = parentChangeShader.GetChild(i).GetComponent<Renderer>().material;
                childMat.shader = standardShader;
                childMat.SetFloat("_Mode", 3f);
                childMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                childMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                childMat.SetInt("_ZWrite", 0);
                childMat.DisableKeyword("_ALPHATEST_ON");
                childMat.EnableKeyword("_ALPHABLEND_ON");
                childMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                childMat.renderQueue = 3000 + i;

                parentChangeShader.GetChild(i).GetComponent<OnePlaneCuttingController>().enabled = false;
                parentChangeShader.GetChild(i).GetComponent<GenericThreePlanesCuttingController>().enabled = false;
            }

            else if (desiredShader == diffuseZWrite) //add the Transparent/DiffuseZWrite Shader 
            {
                parentChangeShader.GetChild(i).GetComponent<OnePlaneCuttingController>().enabled = false;
                parentChangeShader.GetChild(i).GetComponent<GenericThreePlanesCuttingController>().enabled = false;
                Material childMat = parentChangeShader.GetChild(i).GetComponent<Renderer>().material;
                childMat.shader = diffuseZWrite;
                childMat.renderQueue = 3000 + i; 
            }

            else if (desiredShader == crossSectionShader1plane) //add the 1 Full Freedom Clip Plane Shader & enable the corresponding script ('OnePlaneCuttingController')
            {
                parentChangeShader.GetChild(i).GetComponent<OnePlaneCuttingController>().enabled = true;
                parentChangeShader.GetChild(i).GetComponent<GenericThreePlanesCuttingController>().enabled = false;
                Material childMat1 = parentChangeShader.GetChild(i).GetComponent<Renderer>().material;
                childMat1.shader = crossSectionShader1plane;
            }

            else if (desiredShader == crossSectionShader3planes) //add the 3 Clip Planes Shader & enable the corresponding script ('GenericThreePlanesCuttingController')
            {
                parentChangeShader.GetChild(i).GetComponent<GenericThreePlanesCuttingController>().enabled = true;
                parentChangeShader.GetChild(i).GetComponent<OnePlaneCuttingController>().enabled = false; ;
                Material childMat3 = parentChangeShader.GetChild(i).GetComponent<Renderer>().material;
                childMat3.shader = crossSectionShader3planes;
            }
        }
    }
}