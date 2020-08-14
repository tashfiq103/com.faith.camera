namespace com.faith.camera
{
    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public struct CameraSettingsInfo
    {

#if UNITY_EDITOR
        public bool showOnEditor;
#endif

        public string name;
        public CameraSettings cameraSettings;
    }

    [CreateAssetMenu(fileName = "CameraSettings", menuName = "FAITH/New CameraSettings")]
    public class FaithCameraSettings : ScriptableObject
    {
        public List<CameraSettingsInfo> cameraSettings;
    }
}

