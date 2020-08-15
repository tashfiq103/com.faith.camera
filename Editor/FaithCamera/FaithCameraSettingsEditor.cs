namespace com.faith.camera
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(FaithCameraSettings))]
    public class FaithCameraSettingsEditor : BaseEditorClassForFaithCamera
    {
        private FaithCameraSettings Reference;
        private SerializedProperty SP_ListOfCameraSettings;

        private void OnEnable()
        {
            Reference = (FaithCameraSettings)target;

            if (Reference != null) {
                if (Reference.cameraSettings == null)
                    Reference.cameraSettings = new System.Collections.Generic.List<CameraSettingsInfo>();

                SP_ListOfCameraSettings = serializedObject.FindProperty("cameraSettings");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int t_NumberOfCameraSettings = Reference.cameraSettings.Count;
            for (int i = 0; i < t_NumberOfCameraSettings; i++) {

                SerializedProperty t_ShowInEditor = SP_ListOfCameraSettings.GetArrayElementAtIndex(i).FindPropertyRelative("showOnEditor");
                SerializedProperty t_NameOfCameraSettings = SP_ListOfCameraSettings.GetArrayElementAtIndex(i).FindPropertyRelative("name");

                EditorGUILayout.BeginHorizontal();
                {
                    t_ShowInEditor.boolValue = EditorGUILayout.Foldout(
                        t_ShowInEditor.boolValue,
                        "CamSettings (" + i + ") : " + t_NameOfCameraSettings.stringValue,
                        true
                    );

                    if (GUILayout.Button("-Remove")) {

                        Reference.cameraSettings.RemoveAt(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (t_ShowInEditor.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    t_NameOfCameraSettings.stringValue = EditorGUILayout.TextField(
                            "Name",
                            t_NameOfCameraSettings.stringValue
                        );
                    DrawCustomGUIForCameraSettings(SP_ListOfCameraSettings.GetArrayElementAtIndex(i).FindPropertyRelative("cameraSettings"));
                    DrawHorizontalLine();
                    EditorGUI.indentLevel -= 1;
                }
            }

            if (!EditorApplication.isPlaying) {

                EditorGUILayout.Space();
                if (GUILayout.Button("+CameraSettings")) {

                    Reference.cameraSettings.Add(new CameraSettingsInfo());
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

