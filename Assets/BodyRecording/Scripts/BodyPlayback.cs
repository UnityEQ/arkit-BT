﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BodyPlayback : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Is this playback happening in AR or In the Editor")]
    bool m_InEditor;

    public bool inEditor
    {
        get => m_InEditor;
        set => m_InEditor = value;
    }

    [SerializeField]
    [Tooltip("True if the animation is currently playing")]
    bool m_PlayingAnimation = false;
    
    public bool playingAnimation
    {
        get => m_PlayingAnimation;
        set => m_PlayingAnimation = value;
    }

    [SerializeField]
    [Tooltip("True if you want to save the captured animation to a Unity animation clip")]
    bool m_RecordToAnimationClip;

    public bool recordToAnimationClip
    {
        get => m_RecordToAnimationClip;
        set => m_RecordToAnimationClip = value;
    }
    
    JointHandler m_JointHandler;
    BodyRuntimeRecorder m_BodyRuntimeRecorder;
    BodyEditorRecorder m_BodyEditorRecorder;

    int m_JointIndex = 0;

    List<Vector3> m_JointPositions;
    List<Quaternion> m_JointRotations;
    
    void OnEnable()
    {
        m_JointHandler = GetComponent<JointHandler>();
        m_JointHandler.GetJoints();

		#if UNITY_IOS
        //m_BodyRuntimeRecorder = FindObjectOfType<BodyRuntimeRecorder>();
        //m_JointPositions = m_BodyRuntimeRecorder.JointPositions;
        //m_JointRotations = m_BodyRuntimeRecorder.JointRotations;
		#endif
        
		#if UNITY_EDITOR
        //m_BodyEditorRecorder = GetComponent<BodyEditorRecorder>();
//        BodyFileReader reader = GetComponent<BodyFileReader>();
//        reader.ProcessFile("BodyCaptureData");
//        m_JointPositions = reader.positionValues;
//        m_JointRotations = reader.rotationValues;
//		SetBodyStartPose();
		#endif
        
        //SetBodyStartPose();
    }

	public void DoAnim(string animName)
	{
		m_PlayingAnimation = false;
		m_JointIndex = 0;
		BodyFileReader reader = GetComponent<BodyFileReader>();
		reader.ProcessFile(animName);
		m_JointPositions = reader.positionValues;
		m_JointRotations = reader.rotationValues;
		//m_PlayingAnimation = true;
	}
	
	public void DoAnim2()
	{
		m_PlayingAnimation = false;
	}
	
    void Update()
    {
        LoopPlayback();
    }

    void LoopPlayback()
    {
        if (m_PlayingAnimation)
        {
            for (int i = 0; i < m_JointHandler.Joints.Count; i++)
            {
                if (i == 0)
                {
                    // hip joint position adjustment to center around placed object
                    Vector3 m_JointPositionWorld = m_JointPositions[m_JointIndex] - m_JointPositions[0];
					Quaternion m_JointRotationWorld = m_JointRotations[m_JointIndex] * Quaternion.Inverse(m_JointRotations[0]);
					
                    m_JointHandler.Joints[0].transform.localPosition = m_JointPositionWorld;
                    m_JointHandler.Joints[0].transform.localRotation = m_JointRotationWorld;
                }
                else
                {
                    m_JointHandler.Joints[i].transform.localPosition = m_JointPositions[m_JointIndex];
                    m_JointHandler.Joints[i].transform.localRotation = m_JointRotations[m_JointIndex];
                }
                
                
                m_JointIndex++;
            }

            // resets animation
            if (m_JointIndex == m_JointPositions.Count)
            {
                m_JointIndex = 0;
                if (m_InEditor && m_RecordToAnimationClip)
                {
                    //m_PlayingAnimation = false;
                    //m_BodyEditorRecorder.RecordToggle();
                }
            }
        }
    }
    
    void SetBodyStartPose()
    {
        for (int i = 0; i < m_JointHandler.Joints.Count; i++)
        {
            m_JointHandler.Joints[i].transform.localRotation = m_JointRotations[i];
			m_JointHandler.Joints[i].transform.localPosition = m_JointPositions[i];
        }
    }

    public void AnimationToggle()
    {
        m_PlayingAnimation = !m_PlayingAnimation;
    }

    Vector3 CalculateAveragePosition(int rootIndex)
    {
        int numberOfCapturedPositions = m_JointPositions.Count / 92;
        Vector3 retVal = Vector3.zero;
        
        for (int i = 0; i < numberOfCapturedPositions; i++)
        {
            retVal += m_JointPositions[i * 92];
        }

        return retVal / numberOfCapturedPositions;
    }
}
