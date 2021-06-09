using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RecorderPack;
using UnityEngine.EventSystems;

public class RecordButtonInput : MonoBehaviour
{
    public void StartRecording()
    {
        if (Recorder.Instance.IsRecording())
        {
            Recorder.Instance.EndRecording(UtilityFunctions.GetCurrentTimeStampAndPlayerName());
        }
        else
        {
            Recorder.Instance.StartRecording();
        }
    }
}
