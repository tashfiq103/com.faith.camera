namespace com.faith.camera {

    using UnityEditor;
    using UnityEngine;

    public class CameraEditorWindow : EditorWindow {

        #region Private Variables
        private static CameraEditorWindow m_CameraEditorWindowReference;

        private static string m_NameOfThePackage = "com.faith.camera";
        private static string m_NameOfCameraPreset = "Container - MainCamera";
        #endregion

        #region EditorWindow

        #endregion

        #region Configuretion

        private static bool IsRepositoryInAssetFolder (string t_PackageName) {

            string[] t_Directories = AssetDatabase.FindAssets (t_PackageName, new string[] { "Assets" });
            if (t_Directories.Length > 0) {
                return true;
            }
            return false;
        }

        #endregion

        #region Public Callback

        [MenuItem("FAITH/Camera/Preset/Main Camera", false, 97)]
        public static void CreateCameraInstance(MenuCommand t_MenuCommand){

            string t_SubFolder = "";
            if(IsRepositoryInAssetFolder(m_NameOfThePackage)){

                t_SubFolder = "Assets/" + m_NameOfThePackage;
            }else{

                t_SubFolder = "Packages/" + m_NameOfThePackage;
            }

            GameObject t_CameraPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets(
                        m_NameOfCameraPreset,
                        new string[]{t_SubFolder})[0]
                ),
                typeof (GameObject)
            );

            GameObject t_NewCameraInstance = Instantiate(t_CameraPrefab);
            GameObjectUtility.SetParentAndAlign(t_NewCameraInstance, t_MenuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(t_NewCameraInstance, "Create " + t_NewCameraInstance.name);
            Selection.activeObject = t_NewCameraInstance;
        }

        #endregion

    }
}