using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimBtnScript : MonoBehaviour
{
	public BodyRecordingUIManager bdyManager;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(DoIt);
    }
	
	public void DoIt()
	{
		bdyManager.PlayAnim(this.gameObject.transform.GetChild(0).GetComponent<Text>().text);
		foreach (Transform child in bdyManager.scrollContent.transform) {
			GameObject.Destroy(child.gameObject);
		}
		bdyManager.playAnimWindow.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
