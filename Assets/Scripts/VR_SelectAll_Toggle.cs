using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_SelectAll_Toggle : MonoBehaviour
{
    public GameObject selectedParentToggle;

    public void SelectAllParts()
    {
        selectedParentToggle = GameObject.FindGameObjectWithTag("selected_ParentToggle");
        if (selectedParentToggle != null)
        {
            var listOfAllChildrenButtons = selectedParentToggle.GetComponent<VR_manageSelection>().allChildrenButtons;
            if (transform.GetComponent<Toggle>().isOn) //if the SelectAll_Toggle is On, we select all the childrenButtons --> all the parts belonging to the SelectedParent_Toggle will be selected
            {
                foreach (Button childB in listOfAllChildrenButtons)
                {
                    childB.GetComponent<VR_manageChildren>().SelectPart();
                }
            }
            else //on the other hand, if the SelectAll_Toggle is Off, we deselect all the childrenButtons --> all the parts belonging to the SelectedParent_Toggle will be deselected
            {
                foreach (Button childB in listOfAllChildrenButtons)
                {
                    childB.GetComponent<VR_manageChildren>().DeselectPart();
                }
            }
        }
    }
}
