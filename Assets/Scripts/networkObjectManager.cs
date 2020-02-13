/************************************************************************************
Copyright : Copyright 2019 (c) Speak Geek (PTY), LTD and its affiliates. All rights reserved.

Developer : Dylan Holshausen

Script Description : Enable/Disable Objects Based on Remote/Local Player

************************************************************************************/

using UnityEngine;
using Normal.Realtime;

public class networkObjectManager : MonoBehaviour
{
    public Camera localCamera;
    public AudioListener localAudioListener;

    public OVRCameraRig localOVRCameraRig;

    public GameObject[] localOVRHands;

    public RealtimeView rtView;

    private bool _Init = false;

    private void Update()
    {
        if (!_Init)
            Init();
    }

    private void Init()
    {
        if (rtView == null)
            return;

        transform.name = rtView.ownerID.ToString();

        //SEARCH FOR GAMEOBJECTS WITH THE TAG 'SPAWN' AND SET TRANSFORM THE SAME
        //BASED ON OWNER ID
        foreach(GameObject myGO in GameObject.FindGameObjectsWithTag("spawn"))
        {
            if (myGO.name == transform.name)
            {
                transform.position = myGO.transform.position;
                transform.rotation = myGO.transform.rotation;
            }
        }

        //IF THIS IS NOT OUR REALTIME VIEW
        if (!rtView.isOwnedLocally)
        {
            Destroy(localAudioListener);

            Destroy(localOVRCameraRig);

            Destroy(localCamera);

            //LOOP THROUGH HAND COMPONENTS AND DISABLE OVR COMPONENTS
            foreach (GameObject ovrHand in localOVRHands)
            {
                //OVR SKELETON
                if (ovrHand.GetComponent<OVRSkeleton>())
                {
                    Destroy(ovrHand.GetComponent<OVRSkeleton>());
                }

                //OVR HAND
                if (ovrHand.GetComponent<OVRHand>())
                {
                    Destroy(ovrHand.GetComponent<OVRHand>());
                }
            }
        }
        else
        {
            //TAG LOCAL CAMERA
            localCamera.gameObject.tag = "MainCamera";

            //REQUEST OWNERSHIP OF EACH CHILD REALTIMEVIEW
            foreach (RealtimeView childRTView in GetComponentsInChildren<RealtimeView>())
            {
                childRTView.RequestOwnership();
            }

            //REQUEST OWNERSHIP OF EACH CHILD REALTIMETRANSFORM
            foreach (RealtimeTransform childRTTransform in GetComponentsInChildren<RealtimeTransform>())
            {
                childRTTransform.RequestOwnership();
            }

            if (Application.isEditor)
            {
                //LOOP THROUGH HAND COMPONENTS AND DISABLE OVR COMPONENTS
                foreach (GameObject ovrHand in localOVRHands)
                {
                    //OVR SKELETON
                    if (ovrHand.GetComponent<OVRSkeleton>())
                    {
                        Destroy(ovrHand.GetComponent<OVRSkeleton>());
                    }

                    //OVR HAND
                    if (ovrHand.GetComponent<OVRHand>())
                    {
                        Destroy(ovrHand.GetComponent<OVRHand>());
                    }
                }
            }
        }

        //LOOP THROUGH HAND COMPONENTS AND READY OUR LOCAL HANDS
        foreach (GameObject ovrHand in localOVRHands)
        {
            //Speak Geek Quest Hand
            if (ovrHand.GetComponentInChildren<SpeakGeekOculusQuestHand>())
            {
                ovrHand.GetComponentInChildren<SpeakGeekOculusQuestHand>().readyHand();
            }
        }

        _Init = true;
    }
}
