﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFileReader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Path to folder that contains the BodyCaptureData.txt, must be in the Project's Asset folder")]
    string m_CapturesFolderPath = "/Assets/BodyRecording/Captures/";
    
    public string capturesFolderPath
    {
        get => m_CapturesFolderPath;
        set => m_CapturesFolderPath = value;
    }

    [SerializeField]
    [Tooltip("File name of the captured data")]
    string m_FileName = "BodyCaptureData";
    
    public string fileName
    {
        get => m_FileName;
        set => m_FileName = value;
    }

    [SerializeField]
    [Tooltip("postfix for more easily processing multiple captures")]
    string m_PostFilePath;

    public string postFilePath
    {
        get => m_PostFilePath;
        set => m_PostFilePath = value;
    }

    BodyRuntimeRecorder m_BodyRuntimeRecorder;
    List<Vector3> m_PositionValues = new List<Vector3>();
    List<Quaternion> m_RotationValues = new List<Quaternion>();
    
    public List<Vector3> positionValues => m_PositionValues;
    public List<Quaternion> rotationValues => m_RotationValues;
    
    public void ProcessFile()
    {
        string line;
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + m_CapturesFolderPath+m_FileName+m_PostFilePath+".txt"); //load text file with data
        while ((line = file.ReadLine()) != null)
        { //while text exists.. repeat

            char[] delimiterChar = { ')' };//variable separation
            string[] split = line.Split(delimiterChar, StringSplitOptions.None); //split vector3 and quat into split[0] and split[1]

            // remove first ( char and ,( for quat
            split[0] = split[0].Remove(0, 1);
            split[1] = split[1].Remove(0, 2);

            string[] vecSplit = split[0].Split(','); // split up vector3 into just numbers, 
            string[] quatSplit = split[1].Split(','); // split up quat into just numbers
            
            Vector3 newPOS = new Vector3(float.Parse(vecSplit[0]), float.Parse(vecSplit[1]), float.Parse(vecSplit[2]));
            Quaternion newROT = new Quaternion(float.Parse(quatSplit[0]), float.Parse(quatSplit[1]), float.Parse(quatSplit[2]), float.Parse(quatSplit[3]));

            m_PositionValues.Add(newPOS);
            m_RotationValues.Add(newROT);
        }
        file.Close();
    }
}
