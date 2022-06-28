using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;

public class BodyRecordingUIManager : MonoBehaviour
{
	public System.IO.StreamReader file;
	string m_CapturesFolderPath = "/BodyRecording/Captures/";
	public GameObject scrollContent;
	public GameObject scrollPrefab;
	public GameObject playAnimWindow;
	
    [SerializeField]
    [Tooltip("Human body manager reference")]
    ARHumanBodyManager m_HumanBodyManager;

    public ARHumanBodyManager humanBodyManager
    {
        get => m_HumanBodyManager;
        set => m_HumanBodyManager = value;
    }

    [SerializeField]
    [Tooltip("Body tracking UI reference")]
    GameObject m_BodyTrackingUI;

    public GameObject bodyTrackingUI
    {
        get => m_BodyTrackingUI;
        set => m_BodyTrackingUI = value;
    }
    [SerializeField]
    [Tooltip("World tracking UI reference")]
    GameObject m_WorldTrackingUI;

    public GameObject worldTrackingUI
    {
        get => m_WorldTrackingUI;
        set => m_WorldTrackingUI = value;
    }
    [SerializeField]
    [Tooltip("Button for playing back captured animation")]
    Button m_AnimationPlayButton;

    public Button animationPlayButton
    {
        get => m_AnimationPlayButton;
        set => m_AnimationPlayButton = value;
    }
    
    public BodyPlayback m_BodyPlayback;
	public GameObject playPanel;

#if UNITY_EDITOR
#elif UNITY_IOS
    void OnEnable()
    {
        m_HumanBodyManager.humanBodiesChanged += HumanBodyManagerOnhumanHumanBodiesChanged;
        PlaceObjectsOnPlane.onPlacedObject += PlacedObject;
    }

    void OnDisable()
    {
        m_HumanBodyManager.humanBodiesChanged -= HumanBodyManagerOnhumanHumanBodiesChanged;
        PlaceObjectsOnPlane.onPlacedObject -= PlacedObject;
    }
#endif
    void HumanBodyManagerOnhumanHumanBodiesChanged(ARHumanBodiesChangedEventArgs obj)
    {
        
    }
    
    void PlacedObject()
    {
        m_BodyPlayback = FindObjectOfType<BodyPlayback>();
		//m_AnimationPlayButton.gameObject.SetActive(true);
        //m_AnimationPlayButton.onClick.AddListener(m_BodyPlayback.AnimationToggle);
    }
	
	public void ShowAnimPanel()
	{
		if(playAnimWindow.activeSelf)
		{
			foreach (Transform child in scrollContent.transform) {
				GameObject.Destroy(child.gameObject);
			}
			playAnimWindow.SetActive(false);
		}
		else
		{
			playAnimWindow.SetActive(true);
			GetAnimList();
		}
	}
	
	void Start()
	{
	}
	
	public void GetAnimList()
	{
		StartCoroutine(MyCoroutine("https://joinkle.com/anims/beepity2.php"));
        //string line;
		//#if UNITY_IOS
        //file = new System.IO.StreamReader(Application.persistentDataPath + "/AnimationList.txt"); //load text file with data
		//#endif
		//#if UNITY_EDITOR
		//file = new System.IO.StreamReader(Application.dataPath + m_CapturesFolderPath+ "/AnimationList.txt"); //load text file with data
		//#endif
        //while ((line = file.ReadLine()) != null)
        //{ //while text exists.. repeat
//			Debug.Log("LINE: " + line);
			//GameObject gObj = Instantiate(scrollPrefab, scrollContent.transform);
			//gObj.transform.GetChild(0).GetComponent<Text>().text = line.Replace(".txt","");
			//gObj.GetComponent<AnimBtnScript>().bdyManager = this.gameObject.GetComponent<BodyRecordingUIManager>();
			
		//}

	}
	
	public void PlayAnim(string animName)
	{
		ShowAnimPanel();
		if(!m_BodyPlayback){m_BodyPlayback = FindObjectOfType<BodyPlayback>();}
		m_BodyPlayback.DoAnim(animName);
	}
	
	IEnumerator MyCoroutine(string sURL)
{
    WWW wwwdata = new WWW(sURL);
#if UNITY_EDITOR
    while(!wwwdata.isDone){}
#elif UNITY_IOS
    while(!wwwdata.isDone){}
#elif UNITY_WEBGL
	yield return wwwdata;
#endif
	//Debug.Log("WWWDATA: " + wwwdata.text);
    string[] lines = wwwdata.text.Split("\n" [0]);
//        while ((line = file.ReadLine()) != null)
		for (int i = 0; i < lines.Length;i++)
        { //while text exists.. repeat
			//Debug.Log("LINE: " + lines[i]);
			if(lines[i] != ""){
				GameObject gObj = Instantiate(scrollPrefab, scrollContent.transform);
				gObj.transform.GetChild(0).GetComponent<Text>().text = lines[i].Replace(".txt","");
				gObj.GetComponent<AnimBtnScript>().bdyManager = this.gameObject.GetComponent<BodyRecordingUIManager>();
			}
			

		}
    yield return 0;
}
	
//#if UNITY_EDITOR
//#elif UNITY_IOS
    public void ShowWorldTrackingUI()
    {
		//m_AnimationPlayButton.gameObject.SetActive(true);
        m_BodyTrackingUI.SetActive(false);
        m_WorldTrackingUI.SetActive(true);
    }

    public void ShowBodyTrackingUI()
    {
		if(m_BodyPlayback){m_BodyPlayback.playingAnimation = false;}
		playPanel.SetActive(false);
        m_WorldTrackingUI.SetActive(false);
        m_BodyTrackingUI.SetActive(true);
    }
//#endif
}
