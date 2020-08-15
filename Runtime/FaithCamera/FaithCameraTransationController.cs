namespace com.faith.camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FaithCameraTransationController : MonoBehaviour
    {
        #region Public Variables

#if UNITY_EDITOR

        public bool showDefaultInspector;

#endif

        public FaithCameraSettings      cameraSettingPresets;
        public List<CameraTransation>   listOfCameraTransations;

        #endregion
    }
}

