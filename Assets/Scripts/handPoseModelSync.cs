using Normal.Realtime;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public class handPoseModelSync : RealtimeComponent
{
    //private OculusSampleFramework.HandSkeleton _handSkeleton;
    private handPoseModel _model;

    public SpeakGeekOculusQuestHand _SGHand;
    
    private void Start()
    {
        _SGHand = GetComponent<SpeakGeekOculusQuestHand>();
    }

    private handPoseModel model
    {
        set
        {
            if (_model != null)
            {
                _model.skeletonTrackedDataDidChange -= TrackedDataDidChange;
            }

            _model = value;

            if (_model != null)
            {
                if (_model.skeletonTrackedData != null)
                {
                    UpdateTrackedData();

                    _model.skeletonTrackedDataDidChange += TrackedDataDidChange;
                }
            }
        }
    }

    private void TrackedDataDidChange(handPoseModel model, string value)
    {
        UpdateTrackedData();
    }

    private void UpdateTrackedData()
    {
        if (_model == null)
            return;

        if (_model.skeletonTrackedData == "")
            return;

        _SGHand.updateFromNormCore(_model.skeletonTrackedData);
    }

    public void SetTrackedData(string trackedData)
    {
        _model.skeletonTrackedData = trackedData;
    }
}
