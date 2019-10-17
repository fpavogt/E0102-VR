using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VR_showDebugText : MonoBehaviour {

    Text debugText;

    // Use this for initialization
    void Start()
    {
        debugText = gameObject.GetComponentInChildren<Text>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (debugText.text.Length > 250 & debugText!=null)
        {
            debugText.text = message + "\n";
            debugText.color = Color.black;
        }
        else
        {
            debugText.text += message + "\n";
            debugText.color = Color.black;
        }
    }
}
