using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_addComponents : MonoBehaviour {

    public GameObject myController;
    public Transform objectList;
    public GameObject objToggle;
    //public GameObject partButton;
    public Toggle modelToggle;

    public void OnEnable()
    {

        myController = GameObject.Find("Controller (left)");
        objectList = myController.transform.Find("ObjectList_Canvas");
        objToggle = Resources.Load("Prefabs/ObjToggle", typeof(GameObject)) as GameObject;
        //partButton = Resources.Load("Prefabs/ChildButton_v2", typeof(GameObject)) as GameObject;

    }

    // Use this for initialization
    void Start ()
    {

        //transform.parent = GameObject.Find("ContainerOfAllObj").transform;
        //transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        var toggle = Instantiate(objToggle, objectList); //create the new button, with a set position (depending on the number of objects in the scene) and with the *objectsListCanvas* as 'Parent'
        var toggleBoxCollider = toggle.GetComponent<BoxCollider>();
        Text toggleText = toggle.GetComponentInChildren<Text>();
        toggleText.text = transform.name;                                               //give the name of the object to the button
        var rectTransform = toggle.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 100, -2);
        toggleBoxCollider.size = rectTransform.sizeDelta;                               //scale the collider of the button accordingly to its size
        toggle.name = "@" + transform.name;
        modelToggle = toggle.GetComponentInChildren<Toggle>();

        //add to each part (=child of the gameObject) the following components: 'MeshCollider', 'RigidBody' --> they are needed in order to be able to grab each part of the model  
        foreach (Transform part in gameObject.transform)
        {
            var partMeshCollider = part.gameObject.AddComponent<MeshCollider>();
            partMeshCollider.inflateMesh = false;
            partMeshCollider.convex = true;
            partMeshCollider.isTrigger = true;

            var rigidBody = part.gameObject.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
        }
    }
}
