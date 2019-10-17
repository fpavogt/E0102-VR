using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VR_LoadFile : MonoBehaviour {

    public InputField inputField;
    public Button finish;
    public Text feedback;
    public List<string> filePaths = new List<string>(); //list of all the uploaded file paths
    public static List<string> infoInFiles = new List<string>();  //list with all the text inside the uploaded files

    public delegate void ChangeScene();
    public static event ChangeScene LoadModelsClicked; //create an event to initialize the VR_ReadFile script

    public void Start()
    {
        finish.gameObject.SetActive(false);
    }

    public void GetFilePath() //get the .txt file path from the user, who uses the VR keyboard to write the path and ENTER
    {
        string file_path = inputField.text;
        StartCoroutine("LoadTxtFile", file_path); //after the user presses ENTER, the LoadTxtFile function is called
    }

    IEnumerator LoadTxtFile(string file_path) //The LoadTxtFile function allows to take the .txt file path written by the user and load the text of the file 
    {
        using (WWW file = new WWW(file_path))
        {
            yield return file;
            if (!string.IsNullOrEmpty(file.error)) //return an error message for invalid file_path
            {
                feedback.text = "Invalid path: " + file.error;
                feedback.color = Color.red;
            }
            else
            { 
                feedback.text = "Valid path: " + file_path + "\n";
                feedback.color = Color.green;
                filePaths.Add(file_path);
                infoInFiles.Add(file.text);

                if(filePaths.Count >= 1) //you can only load the models if you have at least one valid file path
                {
                    finish.gameObject.SetActive(true); //activate the "Load Models" button
                }
            }   
        }
    }

    public void LoadModels() //load the models
    {
        if (LoadModelsClicked != null)
            LoadModelsClicked();

        infoInFiles.Clear();
    }
}
