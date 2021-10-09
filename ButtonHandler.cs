using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public Button btn;
    public InputField songF;
    public InputField melF;
    public InputField chordF;
    public InputField JSONF;
    public InputField BPMF;
    public InputField BPBF;

    public static string songPath = "";
    public static string melPath = "";
    public static string chordPath = "";
    public static string JSONPath = "";
    public static int BPM;
    public static int BPB;


    void Start()
    {

        btn.onClick.AddListener(TaskOnClick);

    }


    public void TaskOnClick()
    {

        //Check if all fields are filled
        if (songF.text.Trim().Length > 0 && 
            melF.text.Trim('\"').Length > 0 && 
            chordF.text.Trim().Length > 0 &&
            JSONF.text.Trim().Length > 0 &&
            BPMF.text.Trim().Length > 0 &&
            BPBF.text.Trim().Length > 0)
        {
            char[] trimArr = {'\"', ' '}; //Characters to trim

            //Set public fields
            songPath = songF.text.Trim(trimArr);
            melPath = melF.text.Trim(trimArr);
            chordPath = chordF.text.Trim(trimArr);
            JSONPath = JSONF.text.Trim(trimArr);
            BPM = int.Parse(BPMF.text);
            BPB = int.Parse(BPBF.text);

            if (gameObject.GetComponentInChildren<Text>().text == "Data") SceneManager.LoadScene("TimeSyncScene");
            if (gameObject.GetComponentInChildren<Text>().text == "Level") SceneManager.LoadScene("Levelv2Scene");

        }
        else
        {
            Debug.Log("Not all paths are given");
        }

    }
}
