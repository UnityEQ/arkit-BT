using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class BodyFileWriter : MonoBehaviour
{
	
    string m_FilePath; 
    BodyRuntimeRecorder m_BodyRuntimeRecorder;
    string m_CapturesFolderPath = "/BodyRecording/Captures/";
    static string s_TextFileName = "BodyCaptureData.txt";
	public string phpUrl;
	public InputField btnText;
	public GameObject animPanel;
	public string pickles;
	public GameObject savePanel;
	public GameObject waitPanel;
    void OnEnable()
    {
        m_BodyRuntimeRecorder = GetComponent<BodyRuntimeRecorder>();
    }

    public void Share()
    {
		savePanel.SetActive(false);
		waitPanel.SetActive(true);
		if(btnText.text == ""){btnText.text = "dummy";}
		pickles = "";

        for (int i = 0; i < m_BodyRuntimeRecorder.JointPositions.Count; i++)
        {
			if((m_BodyRuntimeRecorder.JointPositions.Count >= i) && (m_BodyRuntimeRecorder.JointRotations.Count >= i))
			{
				pickles += m_BodyRuntimeRecorder.JointPositions[i].ToString("F7") + "," + m_BodyRuntimeRecorder.JointRotations[i].ToString("F7")+"\n";
			}
        }
		StartCoroutine(UpLoadUserData(pickles));
    }

IEnumerator UpLoadUserData(string result)
{ 
	if(result == ""){result = new Vector3(0,0,0)+","+new Quaternion(0,0,0,0);}
	Debug.Log("RESULT: " + result);
	Debug.Log("NAME: " + btnText.text);
	
    WWWForm form = new WWWForm();
	form.AddField("name", btnText.text);
    form.AddField("data", result);
    UnityWebRequest www = UnityWebRequest.Post(phpUrl, form);
    yield return www.SendWebRequest();
	btnText.text = "";
	animPanel.SetActive(false);
    if (www.isNetworkError)
        Debug.Log(www.error);
    else
        Debug.Log("Uploaded");
}



}