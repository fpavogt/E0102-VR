using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_manageMenu : MonoBehaviour
{
    public GameObject toolsMenu;
    //public GameObject keyboard;
    private int menuClickcounter = 0;

    public GameObject objectList;
    private int ObjList_Clickcounter = 0;



    public void OnEnable()
    {
        VR_inputs_controller.OnMenuPressed += ShowToolsMenu;
    }

    public void OnDisable()
    {
        VR_inputs_controller.OnMenuPressed -= ShowToolsMenu;
    }

    public void ShowToolsMenu(string controllerName)
    {
        if (controllerName == "Controller (right)") //you activate or disactivate the ToolsMenu with the RIGHT controller
        {
            menuClickcounter++;
            if (menuClickcounter % 2 == 0)
            {
                toolsMenu.SetActive(true);
                //keyboard.SetActive(true);
            }
            else if (menuClickcounter % 2 != 0)
            {
                toolsMenu.SetActive(false);
                //keyboard.SetActive(false);
            }
        }

        else if (controllerName == "Controller (left)") //you activate or disactivate the ObjectList with the LEFT controller
        {
            ObjList_Clickcounter++;
            GameObject leftController = GameObject.Find("Controller (left)");

            if (ObjList_Clickcounter % 2 == 0)
            {
                objectList.SetActive(true);
                Destroy(leftController.GetComponent<Rigidbody>()); //destroy the component RigidBody if the ObjectList is active --> the user cannot grab objects while the list is visible
            }
            else if (ObjList_Clickcounter % 2 != 0)
            {
                objectList.SetActive(false);
                var leftControllerRigidBody = leftController.AddComponent<Rigidbody>(); //add the component RigidBody to the left controller when the ObjList is not visible
                leftControllerRigidBody.isKinematic = true;
                leftControllerRigidBody.useGravity = false;
            }
        }
    }
}

//remember to insert the public GameObjects 'toolsMenu' & 'objectList'