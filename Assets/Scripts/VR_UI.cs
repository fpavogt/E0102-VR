using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VR_UI : MonoBehaviour
{
    private void OnEnable()  //unity built-in function needed for enabling the events
    {
        //(1)script where the event is created.(2)name of the event += (3) function that we want to subscribe to the created event.
        //we subscribe a function (or method) to an event --> whenever the event will occur, the function that is subscribed to the event will be called

        VR_laser_pointer.PointerIn += HandlePointerIn;
        VR_laser_pointer.PointerOut += HandlePointerOut;
        VR_inputs_controller.OnTriggerPressed += HandleTriggerPressed;
    }

    private void OnDisable() //unity built-in function needed for disabling the events
    {
        VR_laser_pointer.PointerIn -= HandlePointerIn;
        VR_laser_pointer.PointerOut -= HandlePointerOut;
        VR_inputs_controller.OnTriggerPressed -= HandleTriggerPressed;
    }

    private void HandleTriggerPressed(string controllerName)
    {
        if (EventSystem.current.currentSelectedGameObject != null & controllerName == "Controller (right)")
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    //NOTE: Selecting a button is different from clicking on it 
    private void HandlePointerIn(object sender, PointerEventArguments e) //when the laser is on the button (PointerIn), you Select the button (button.Select()) --> it will change color
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
                button.Select();
                //Debug.Log("HandlePointerIn " + e.target.name);
        }

        var toggle = e.target.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.Select();
        }
    }

    private void HandlePointerOut(object sender, PointerEventArguments e) //when the laser is outside the button (PointerOut), you Deselect the button (button.SetSelectedGameObject(null)) --> it will return to white color
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            //Debug.Log("HandlePointerOut " + e.target.name);
        }

        var toggle = e.target.GetComponent<Toggle>();
        if (toggle != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}

//https://unity3d.college/2017/06/17/steamvr-laser-pointer-menus/
