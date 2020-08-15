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
        #region Public Variables

        public List<CameraSettingsInfo> cameraSettings;

        #endregion

        #region Configuretion

        private bool IsValidIndexOfCameraSetting(int t_IndexOfCameraSetting) {

            if (t_IndexOfCameraSetting >= 0 && t_IndexOfCameraSetting < cameraSettings.Count) 
                return true;

            Debug.LogError("Invalid 'Index' of CameraSetting");
            return false;
            
        }

        private int IsValidNameOfCameraSettings(string t_NameOfCameraSetting) {

            int t_NumberOfCameraSettings = cameraSettings.Count;
            for (int i = 0; i < t_NumberOfCameraSettings; i++) {

                if (cameraSettings[i].name == t_NameOfCameraSetting) {

                    return i;
                }
            }

            Debug.LogError("Invalid 'Name' of CameraSetting");
            return -1;
        }



        #endregion

        #region Public Callback

        public int GetNumberOfAvailableCameraSettings()
        {

            return cameraSettings.Count;
        }

        public string GetNameOfCameraSettings(int t_IndexOfCameraSetting) {

            if (IsValidIndexOfCameraSetting(t_IndexOfCameraSetting)) {

                return cameraSettings[t_IndexOfCameraSetting].name;
            }

            return "InvalidName";
        }

        public CameraSettings GetCameraSettings(int t_IndexOfCameraSetting) {

            if (IsValidIndexOfCameraSetting(t_IndexOfCameraSetting))
                return cameraSettings[t_IndexOfCameraSetting].cameraSettings;

            
            return null;
        }

        public CameraSettings GetCameraSettings(string t_NameOfCameraSetting)
        {

            int t_IndexOfCameraSetting = IsValidNameOfCameraSettings(t_NameOfCameraSetting);
            if (t_IndexOfCameraSetting != -1) {

                return cameraSettings[t_IndexOfCameraSetting].cameraSettings;
            }

            return null;
        }

        

        #endregion

    }
}

