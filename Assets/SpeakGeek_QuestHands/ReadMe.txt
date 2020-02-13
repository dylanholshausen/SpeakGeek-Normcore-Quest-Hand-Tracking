1) Drag OVRCameraRig into your scene.
2) On OVRManager set Hand Tracking Support to hands only. (OVRManager is on OVRCameraRig).
3) Drag OVRHandPrefab into OVRCameraRig>TrackingSpace>LeftHandAnchor, rename OVRHandPrefab to LeftHand.
4) Drag OVRHandPrefab into OVRCameraRig>TrackingSpace>RightHandAnchor, rename OVRHandPrefab to RightHand.
5) On RightHand GameObject - Ensure OVRHand, OVRSkeleton and OVRMesh are set to Hand Right.
6) Drag SpeakGeek_LeftHand into scene, ensure RightHand bool is unchecked.
7) Drag SpeakGeek_RightHand into scene, ensure RightHand bool is checked.