using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_manageChildren : MonoBehaviour {

    public GameObject scalingController;
    public GameObject currentChild;
    public Toggle childToggle;
    public string childName;
    //public int OnChildClick_Counter;
    public Material showSelectedObj;
    public Material originalMaterial;
    public float newTransparency;

    public void OnEnable()
    {
        childToggle = transform.GetComponentInChildren<Toggle>();
        //OnChildClick_Counter = 0;
    }

    public void Start()
    {
        childName = transform.name.Remove(0, 1); //the childButton has name @child; here, we remove the '@' to get the name of the child related to the button
        currentChild = GameObject.Find(childName); //find the child gameObject to which the childButton is referred to
        originalMaterial = currentChild.GetComponent<Renderer>().material;
        scalingController = GameObject.Find("Controller (left)");
    }

    public void OnClickChildButton() 
    {
        if (currentChild.tag == "Untagged")
        {
            SelectPart();
        }
        else if (currentChild.tag == "selected_obj")
        {
            DeselectPart();
        }
    }

    public void ActivateDisactivateChildren()
    {
        if (currentChild != null & !childToggle.isOn) //if the currentChild is present and the toggle is off, the MeshRenderer and MeshCollider components of the currentChild are disabled 
        {
            currentChild.GetComponentInChildren<MeshRenderer>().enabled = false;
            currentChild.GetComponentInChildren<MeshCollider>().enabled = false;
        }
        else
        {
            currentChild.GetComponentInChildren<MeshRenderer>().enabled = true; //the MeshRenderer component of the currentChild is enabled --> 3D object visible
            var currentCollider = currentChild.GetComponentInChildren<MeshCollider>();
            if (currentCollider.enabled == false & scalingController.GetComponent<VR_scale_model>().enabled == true) //this is needed to avoid interference with the scaling tool, in which we disable/enable the MeshColliders
            {
                currentCollider.enabled = false; 
            }
            else { currentCollider.enabled = true; }
        }
    }

    public void SelectPart() //change the Material of the child part to show that it has been selected
    {
        currentChild.tag = "selected_obj";
        var currentChildRenderer = currentChild.GetComponent<Renderer>();
        currentChildRenderer.material = showSelectedObj;

        ColorBlock cb = transform.GetComponent<Button>().colors;
        cb.normalColor = new Color(0.0f, 0.47f, 0.74f); //FPAV: adjust color scheme
        transform.GetComponent<Button>().colors = cb;
    }

    public void DeselectPart() //change the Material of the child part back to its original material (R,G,B,transparency)
    {
        currentChild.tag = "Untagged";
        var currentChildRenderer = currentChild.GetComponent<Renderer>();
        currentChildRenderer.material = originalMaterial;
        newTransparency = VR_change_transparency.transparency;
        if (newTransparency > 0.05)
        {
            originalMaterial.color = new Color(originalMaterial.color.r, originalMaterial.color.g, originalMaterial.color.b, newTransparency);
        }

        ColorBlock cb = transform.GetComponent<Button>().colors;
        cb.normalColor = Color.white;
        transform.GetComponent<Button>().colors = cb;
    }
}

//this SCRIPT has to be attached to the childButton
