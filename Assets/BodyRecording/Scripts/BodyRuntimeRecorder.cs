﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class BodyRuntimeRecorder : MonoBehaviour
{
	public GameObject recordingBtn;
    bool m_IsRecording = false;
	
    List<Vector3> m_JointPositions;

    public List<Vector3> JointPositions
    {
        get => m_JointPositions;
        set => m_JointPositions = value;
    }
    
    List<Quaternion> m_JointRotations;

    public List<Quaternion> JointRotations
    {
        get => m_JointRotations;
        set => m_JointRotations = value;
    }

    public event Action dataRecorded;

    [SerializeField]
    [Tooltip("Human body manager reference")]
    ARHumanBodyManager m_HumanBodyManager;
    
    public ARHumanBodyManager humanBodyManager
    {
        get => m_HumanBodyManager;
        set => m_HumanBodyManager = value;
    }

    JointHandler m_ActiveTrackedBodyJoints;
    const int k_TrackedBodyJointCount = 92; // 91 + root parent
#if UNITY_EDITOR
#elif UNITY_IOS
    void OnEnable()
    {
        m_HumanBodyManager.humanBodiesChanged += HumanBodyManagerOnhumanBodiesChanged;
    }

    void OnDisable()
    {
        m_HumanBodyManager.humanBodiesChanged -= HumanBodyManagerOnhumanBodiesChanged;
    }
#endif
    void HumanBodyManagerOnhumanBodiesChanged(ARHumanBodiesChangedEventArgs bodyChangeEventArgs)
    {
        if (bodyChangeEventArgs.added.Count > 0)
        {
            m_ActiveTrackedBodyJoints = bodyChangeEventArgs.added[0].transform.GetChild(0).GetComponent<JointHandler>();
        }

        if (bodyChangeEventArgs.updated.Count > 0)
        {
            if (m_ActiveTrackedBodyJoints == null)
            {
                m_ActiveTrackedBodyJoints = bodyChangeEventArgs.updated[0].transform.GetChild(0).GetComponent<JointHandler>();
            }
        }
        
        // TODO handle no longer tracking
    }

    void Start()
    {
        m_JointPositions = new List<Vector3>();
        m_JointRotations = new List<Quaternion>();
    }

    void Update()
    {
        if (m_IsRecording)
        {
            for (int i = 0; i < k_TrackedBodyJointCount; i++)
            {
                if (i == 0)
                {
                    // store root parent / anchor in world space
                    m_JointPositions.Add(m_ActiveTrackedBodyJoints.Joints[i].position);
                    m_JointRotations.Add(m_ActiveTrackedBodyJoints.Joints[i].rotation);
                }
                else
                {
                    // store rig joints in local space
                    m_JointPositions.Add(m_ActiveTrackedBodyJoints.Joints[i].localPosition);
                    m_JointRotations.Add(m_ActiveTrackedBodyJoints.Joints[i].localRotation);
                }
            }
        }
        
        //m_RecordingIndicator.SetActive(m_IsRecording);
    }

    public void RecordingToggle()
    {
        m_IsRecording = !m_IsRecording;

        if (!m_IsRecording && m_JointPositions.Count > 0)
        {
            if (dataRecorded != null)
            {
                dataRecorded();
            }
        }
		if(m_IsRecording)
		{
			recordingBtn.GetComponent<Image>().color = new Color32(255,0,0,255);
		}
		else
		{
			recordingBtn.GetComponent<Image>().color = new Color32(255,255,255,255);
		}
    }

    public void ForceStopRecording()
    {
        m_IsRecording = false;
		recordingBtn.GetComponent<Image>().color = new Color32(255,255,255,255);
    }
}




