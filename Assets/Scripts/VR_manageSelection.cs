using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_manageSelection : MonoBehaviour {

    public GameObject containerOfAllObj;
    public GameObject currentObj;
    public Toggle myToggle;
    public Toggle parentToggle;
    public Button childButton;
    public string objName;
    public float manageObjCounter;
    public List<Button> allChildrenButtons = new List<Button>();
    //private string childRelatedInfo;
    public Shader standardShader;
    public Shader diffuseZWrite;

    private void Awake()
    {
        manageObjCounter = 0;
    }

    void Start()
    {
        myToggle = transform.GetComponent<Toggle>();
        myToggle.group = GameObject.Find("ToggleGroup").GetComponent<ToggleGroup>();
        containerOfAllObj = GameObject.Find("ContainerOfAllObj");
        standardShader = Shader.Find("Standard");

        currentObj = containerOfAllObj.transform.GetChild(1).gameObject; //get the second child of 'containerOfAllObjects' --> the first child will be the Reference System
        currentObj.tag = "mainObject";
        objName = currentObj.transform.name; //the toggle has name @gameObject (from ImportObj script); here, we remove the @ to get the name of the object related to the button


        //here you create all the buttons for each child in the currentObj (= each layer of the supernova)
        int childCounter = 0; //it is just needed to position the button in space, so that it does not collide with other child buttons 
        foreach (Transform child in currentObj.transform)
        {
            //create the child button for each child + position the button in space
            string childName = child.name;
            var buttonChild = Instantiate(childButton, transform) as Button; //create the button for each subpart of the model
            buttonChild.name = "@" + child.name;
            buttonChild.GetComponentInChildren<Text>().text = child.name.Replace("_"," "); // FPAV: Replace underscores with gaps, for a better look!
            buttonChild.transform.localPosition = new Vector3(320, childCounter * -30, 0);
            allChildrenButtons.Add(buttonChild);

            ////add to each child the following components: 'MeshCollider', 'RigidBody' --> they are needed in order to be able to grab each part of the model imported
            //var childMeshCollider = child.GetComponentInChildren<MeshCollider>();
            //var rigidBody = childMeshCollider.transform.gameObject.AddComponent<Rigidbody>();
            //rigidBody.useGravity = false;

            ////SET STANDARD SHADER WITH TRANSPARENT RENDERING MODE
            //var childMaterial = child.GetComponent<Renderer>().material;
            //childMaterial.shader = standardShader;
            //childMaterial.SetFloat("_Mode", 3f);
            //childMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //childMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //childMaterial.SetInt("_ZWrite", 1);
            //childMaterial.DisableKeyword("_ALPHATEST_ON");
            //childMaterial.EnableKeyword("_ALPHABLEND_ON");
            //childMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //childMaterial.renderQueue = 3000 + childCounter;

            var childMaterial = child.GetComponent<Renderer>().material;
            childMaterial.shader = diffuseZWrite;
            childMaterial.renderQueue = 3000 + childCounter;

            //add the script 'GenericThreePlanesCuttingControl' to be able to use 3 Clip Planes
            child.gameObject.AddComponent<GenericThreePlanesCuttingController>().enabled = false;

            //add the script 'GenericThreePlanesCuttingControl' to be able to use the Full Freedom Clip Plane
            child.gameObject.AddComponent<OnePlaneCuttingController>().enabled = false;

            ////add the 'Text' component to child and assign the childRelatedInfo
            //var childText = child.gameObject.AddComponent<Text>();
            //childText.text = childRelatedInfo;
            //childRelatedInfo = null; //reset for the next child
            childCounter++;
        }
    }

    public void ManageObj()
    {
        manageObjCounter++; //every time that you press the button to show the hierarchy of the currentObj (=supernova), this counter increases
                            //it starts from 0, so the first time that you will press it, it will go to 1

        if (currentObj != null)
        {
            //decide if you want to show or not the entire list of 'allChildrenButtons' when pressing the 'parentButton'
            if (manageObjCounter > 0 & manageObjCounter % 2 != 0) //check how many times you pressed the parentButton and disactivate 'allChildrenButtons'
            {
                //hide (--> disactivate) children buttons
                foreach (Button b in allChildrenButtons)
                {
                    b.gameObject.SetActive(false);
                }
                myToggle.tag = "Untagged";

                ColorBlock myToggleCB = myToggle.colors;
                myToggleCB.normalColor = Color.white;
                myToggle.colors = myToggleCB;
                myToggle.transform.GetChild(1).GetComponent<Text>().color = Color.black; //FPAV: adjust color scheme


            }

            else if (manageObjCounter > 0 & manageObjCounter % 2 == 0) //check how many times you pressed the parentButton and activate 'allChildrenButtons'
            {
                //show (--> activate) children buttons
                foreach (Button b in allChildrenButtons)
                {
                    b.gameObject.SetActive(true);
                }
                myToggle.tag = "selected_ParentToggle";

                ColorBlock myToggleCB = myToggle.colors;
                myToggleCB.normalColor = myToggleCB.pressedColor; //FPAV: adjust color scheme
                myToggle.colors = myToggleCB;
                myToggle.transform.GetChild(1).GetComponent<Text>().color = Color.white; //FPAV: adjust color scheme
            }
        }
    }

    public void ActivateDisactivateObj()
    {
        objName = transform.name.Remove(0, 1); //the toggle has name @gameObject (from ImportObj script); here, we remove the @ to get the name of the object related to the toggle
        currentObj = GameObject.Find(objName); //find the gameObject to which the button is referred to

        if (currentObj != null & !parentToggle.isOn) //if the currentObj is present and the toggle is off, the currentObj disappears (it disactivates all the childrenToggles; NOTE: it does not work if you disactivate the currentObj itself, because you need it in the scripts)
        {
            foreach (Button cB in allChildrenButtons)
            {
                cB.GetComponentInChildren<Toggle>().isOn = false;
            }
        }
        else
        {
            foreach (Button cB in allChildrenButtons)
            {
                cB.GetComponentInChildren<Toggle>().isOn = true;
            }
        }
    }
}

//REMEMBER to add the childButton Prefab in 'public Button childButton'
//this SCRIPT has to be attached to the parentToggle
