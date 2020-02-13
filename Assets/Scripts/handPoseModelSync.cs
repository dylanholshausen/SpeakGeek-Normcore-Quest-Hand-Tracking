/************************************************************************************
Copyright : Copyright 2019 (c) Speak Geek (PTY), LTD and its affiliates. All rights reserved.

Developer : Dylan Holshausen

Script Description : Sync Hand Pose Model Data Through Normcore

************************************************************************************/

using Normal.Realtime;

public class handPoseModelSync : RealtimeComponent
{
    //Private Variables
    private handPoseModel _model;

    //Public Variables
    public SpeakGeekOculusQuestHand sgHand;
    
    private void Start()
    {
        //Reference Our Oculus Hand Script That Gets/Applies Bone Data to the Hands
        sgHand = GetComponent<SpeakGeekOculusQuestHand>();
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

        //Send Received Hand/Bone Data to Update Function in SG Quest Hand Script
        sgHand.updateFromNormCore(_model.skeletonTrackedData);
    }

    public void SetTrackedData(string trackedData)
    {
        _model.skeletonTrackedData = trackedData;
    }
}
