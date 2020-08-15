namespace com.faith.camera {

    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(FaithCameraTransationController))]
    public class FaithCameraTransationControllerEditor : BaseEditorClassForFaithCamera
    {
        #region Private Variables

        private FaithCameraTransationController Reference;

        private SerializedProperty SP_CameraSettingPersets;
        private SerializedProperty SP_ListOfCameraTransations;

        #endregion

        #region Editor

        private void OnEnable()
        {
            Reference = (FaithCameraTransationController)target;

            if (Reference != null) {

                SP_CameraSettingPersets     = serializedObject.FindProperty("cameraSettingPresets");
                SP_ListOfCameraTransations  = serializedObject.FindProperty("listOfCameraTransations");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Reference.showDefaultInspector = EditorGUILayout.Toggle(
                    "ShowDefaultInspector",
                    Reference.showDefaultInspector
                );
            if (Reference.showDefaultInspector)
            {

                DrawDefaultInspector();
            }
            else {

                CustomGUI();
            }

            

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region CustomGUI

        private void CustomGUI() {

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(SP_CameraSettingPersets);
                if (GUILayout.Button("+Transation", GUILayout.Width(100)))
                {
                    Reference.listOfCameraTransations.Add(new CameraTransation());
                    Reference.listOfCameraTransations[Reference.listOfCameraTransations.Count - 1].transationClips = new System.Collections.Generic.List<CameraTransationClip>();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            
            DrawHorizontalLine();
            EditorGUILayout.Space();

            int t_NumberOfCameraTransation = Reference.listOfCameraTransations.Count;
            for (int i = 0; i < t_NumberOfCameraTransation; i++)
            {
                SerializedProperty t_SPOnShowOnEditor = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("showOnEditor");

                EditorGUILayout.BeginHorizontal();
                {
                    t_SPOnShowOnEditor.boolValue = EditorGUILayout.Foldout(
                        t_SPOnShowOnEditor.boolValue,
                        "Transation (" + i + ") " + Reference.listOfCameraTransations[i].nameOfTransation,
                        true
                    );
                    if (GUILayout.Button("-Remove",GUILayout.Width(100))) {

                        Reference.listOfCameraTransations.RemoveAt(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (t_SPOnShowOnEditor.boolValue) {

                    SerializedProperty t_SPUseThisTransationSettingsForAllClip = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("useCentralCameraSettings");
                    SerializedProperty t_SPNameOfTransation = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("nameOfTransation");
                    SerializedProperty t_SPTransationType   = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("transationType");
                    SerializedProperty t_SPDefaultCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("defaultCameraSettings");
                    SerializedProperty t_SPTransationClips  = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("transationClips");

                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.PropertyField(t_SPNameOfTransation);
                        EditorGUILayout.PropertyField(t_SPTransationType);

                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(t_SPUseThisTransationSettingsForAllClip);
                        if (t_SPUseThisTransationSettingsForAllClip.boolValue) {
                            EditorGUI.indentLevel += 1;
                            DrawCustomGUIForCameraSettings(t_SPDefaultCameraSettings);
                            EditorGUI.indentLevel -= 1;
                        }

                        int t_NumberOfTransationClip = Reference.listOfCameraTransations[i].transationClips.Count;
                        for (int j = 0; j < t_NumberOfTransationClip; j++) {

                            EditorGUI.indentLevel += 1;
                            EditorGUILayout.BeginVertical();
                            {
                                
                                SerializedProperty t_SPCameraOrigin = t_SPTransationClips.GetArrayElementAtIndex(j).FindPropertyRelative("cameraOrigin");
                                SerializedProperty t_SPCameraFocuses = t_SPTransationClips.GetArrayElementAtIndex(j).FindPropertyRelative("cameraFocuses");
                                
                                if (!t_SPUseThisTransationSettingsForAllClip.boolValue) {

                                    SerializedProperty t_SPCameraSettingsForTransationClip = t_SPTransationClips.GetArrayElementAtIndex(j).FindPropertyRelative("cameraSettings");
                                    EditorGUILayout.PropertyField(t_SPCameraSettingsForTransationClip);
                                }

                                EditorGUILayout.PropertyField(t_SPCameraOrigin);
                                EditorGUILayout.PropertyField(t_SPCameraFocuses);

                            }
                            EditorGUILayout.EndVertical();
                            EditorGUI.indentLevel -= 1;
                        }

                        EditorGUILayout.Space();
                        DrawHorizontalLine();
                        if (GUILayout.Button("+TransationClip")) {

                            Reference.listOfCameraTransations[i].transationClips.Add(new CameraTransationClip());
                        }
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel -= 1;
                }
            }

        }

        #endregion
    }
}

