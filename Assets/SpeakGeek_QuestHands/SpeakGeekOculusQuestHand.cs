using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using static OVRSkeleton;

public class SpeakGeekOculusQuestHand : MonoBehaviour
{
    public bool RightHand = false;
    
    [Header("These two fields need to match the OVRSkelton fields")]
    [SerializeField]
    private bool _updateRootPose = false;
    [SerializeField]
    private bool _updateRootScale = false;

    //This hands bones
    private List<Transform> _bones = new List<Transform>();

    //This list is used to get all children recursively
    private List<Transform> listOfChildren = new List<Transform>();

    //This hands mesh renderer
    private SkinnedMeshRenderer _mySkinMeshRenderer;

    //References to Oculus Objects
    public OVRSkeleton _myOVRSkeleton;

    //private SkinnedMeshRenderer _ovrSkinMeshRenderer;
    private IOVRSkeletonDataProvider _dataProvider;

    private handPoseModelSync _SGHandSync;
    public Normal.Realtime.RealtimeView rtView;

    public bool handReady = false;

    private void Awake()
    {
        transform.eulerAngles = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void readyHand()
    {
        _SGHandSync = GetComponent<handPoseModelSync>();

        //Check the two children of this object, namely 'Bones' and 'BindPoses'
        //And get all bones
        foreach (Transform child in transform)
        {
            _bones = new List<Transform>();
            if (child.name.ToLower() == "bones")
            {
                listOfChildren = new List<Transform>();
                GetChildRecursive(child.transform);

                //We need bones to be in the same order as oculus
                //So we add all the bones and keep a reference to 5 finger tips. (OVRSkeleton sets these bone id's last)
                //We then add finger tips back to bones to they are last.
                List<Transform> fingerTips = new List<Transform>();
                foreach (var bone in listOfChildren)
                {
                    if (bone.name.Contains("Tip"))
                    {
                        fingerTips.Add(bone);
                    }
                    else
                    {
                        _bones.Add(bone);
                    }
                }
                //And finger tips back to bones
                foreach (var bone in fingerTips)
                {
                    _bones.Add(bone);
                }
            }
        }

        InitializeHand();

        //IOVRSkeletonDataProvider holds the hand rotation data. So we get reference to the same DataProvider as the oculus hand we are copying.
        if (_dataProvider == null && _myOVRSkeleton != null)
        {
            _dataProvider = _myOVRSkeleton?.GetComponent<IOVRSkeletonDataProvider>();
        }
    }

    private void Update()
    {
        if (!handReady)
            return;

        if (rtView != null)
        {
            if (!rtView.isOwnedLocally)
            {
                return;
            }
        }

        //Ensure we still have the DataProvider otherwise attempt to find it again.
        //If we do then update our hand from the data provider.
        if (_dataProvider == null && _myOVRSkeleton != null)
        {
            _dataProvider = _myOVRSkeleton?.GetComponent<IOVRSkeletonDataProvider>();
        }
        else
        {
            var data = _dataProvider.GetSkeletonPoseData();
            string dataToSend = "";

            if (data.IsDataValid && data.IsDataHighConfidence)
            {
                _mySkinMeshRenderer.enabled = true;

                dataToSend += "1|";

                if (_updateRootPose)
                {
                    transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
                    transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();

                    dataToSend += "1|";
                    dataToSend += transform.localPosition.x + "|" + transform.localPosition.y + "|" + transform.localPosition.z + "|";
                    dataToSend += transform.localEulerAngles.x + "|" + transform.localEulerAngles.y + "|" + transform.localEulerAngles.z + "|";
                }
                else
                {
                    dataToSend += "0|";
                    dataToSend += "0|0|0|";
                    dataToSend += "0|0|0|";
                }

                if (_updateRootScale)
                {
                    transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);

                    dataToSend += "1|";
                    dataToSend += transform.localScale.x + "|" + transform.localScale.y + "|" + transform.localScale.z + "|";
                }
                else
                {
                    dataToSend += "0|";
                    dataToSend += "0|0|0|";
                }

                for (var i = 0; i < _bones.Count; ++i)
                {
                    _bones[i].transform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();

                    dataToSend += _bones[i].transform.localEulerAngles.x + "|" + _bones[i].transform.localEulerAngles.y + "|" + _bones[i].transform.localEulerAngles.z + "|";
                }
            }
            else
            {
                _mySkinMeshRenderer.enabled = false;

                dataToSend = "0|";
            }

            if (!Application.isEditor)
            {
                if (rtView != null)
                {
                    if (rtView.isOwnedLocally)
                    {
                        _SGHandSync.SetTrackedData(dataToSend);
                    }
                }
            }
        }
    }

    public void updateFromNormCore(string netHandData)
    {
        if (!handReady)
            return;

        if (netHandData == "")
            return;

        if (rtView != null)
        {
            if (rtView.isOwnedLocally)
            {
                return;
            }
        }

        string[] netHandDataArr = netHandData.Split('|');

        if (netHandDataArr[0] == "0")
        {
            _mySkinMeshRenderer.enabled = false;

            return;
        }
        else if (netHandDataArr[0] == "1")
        {
            _mySkinMeshRenderer.enabled = true;
        }


        if (netHandDataArr[1] == "1")
        {
            transform.localPosition = new Vector3(float.Parse(netHandDataArr[2], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[3], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[4], CultureInfo.InvariantCulture));
            transform.localEulerAngles = new Vector3(float.Parse(netHandDataArr[5], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[6], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[7], CultureInfo.InvariantCulture));
        }

        if (netHandDataArr[8] == "1")
        {
            transform.localScale = new Vector3(float.Parse(netHandDataArr[9], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[10], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[11], CultureInfo.InvariantCulture));
        }

        for (var i = 0; i < _bones.Count; ++i)
        {
            int tmpBoneCount = i * 3;

            _bones[i].transform.localEulerAngles = new Vector3(float.Parse(netHandDataArr[12 + tmpBoneCount], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[13 + tmpBoneCount], CultureInfo.InvariantCulture), float.Parse(netHandDataArr[14 + tmpBoneCount], CultureInfo.InvariantCulture));
        }
    }

    private void InitializeHand()
    {
        _mySkinMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _mySkinMeshRenderer.enabled = true;
        _mySkinMeshRenderer.bones = _bones.ToArray();

        handReady = true;
    }

    private void GetChildRecursive(Transform obj)
    {
        if (null == obj)
            return;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;

            if (child != obj)
            {
                listOfChildren.Add(child);
            }
            GetChildRecursive(child);
        }
    }
}
