namespace com.faith.camera {

    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomEditor(typeof(FaithCameraTransationController))]
    public class FaithCameraTransationControllerEditor : BaseEditorClassForFaithCamera
    {
        #region Private Variables

        private FaithCameraTransationController Reference;

        private Editor cameraPresetEditor;

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

        #region Configuretion

        private void CopyCameraSettingsFromOneToAnother(SerializedProperty t_SerializedCameraSettings, CameraSettings t_NewValue) {

            t_SerializedCameraSettings.FindPropertyRelative("forwardVelocity").floatValue = t_NewValue.forwardVelocity;
            t_SerializedCameraSettings.FindPropertyRelative("curveForForwardVelocity").animationCurveValue = t_NewValue.curveForForwardVelocity;

            t_SerializedCameraSettings.FindPropertyRelative("angulerVelocity").floatValue = t_NewValue.angulerVelocity;
            t_SerializedCameraSettings.FindPropertyRelative("curveForAngulerVelocity").animationCurveValue = t_NewValue.curveForAngulerVelocity;

            t_SerializedCameraSettings.FindPropertyRelative("cameraPositionOffset").vector3Value = t_NewValue.cameraPositionOffset;
            t_SerializedCameraSettings.FindPropertyRelative("cameraFocusOffset").vector3Value = t_NewValue.cameraFocusOffset;

            t_SerializedCameraSettings.FindPropertyRelative("isUseFocusOffsetOnLocalSpace").boolValue = t_NewValue.isUseFocusOffsetOnLocalSpace;
            t_SerializedCameraSettings.FindPropertyRelative("focusConstraint").FindPropertyRelative("x").boolValue = t_NewValue.focusConstraint.x;
            t_SerializedCameraSettings.FindPropertyRelative("focusConstraint").FindPropertyRelative("y").boolValue = t_NewValue.focusConstraint.y;
            t_SerializedCameraSettings.FindPropertyRelative("focusConstraint").FindPropertyRelative("z").boolValue = t_NewValue.focusConstraint.z;
            t_SerializedCameraSettings.FindPropertyRelative("focusType").enumValueIndex = (int) t_NewValue.focusType;

        }

        #endregion

        #region CustomGUI

        private void CustomGUI() {

            List<string> t_OptionsForCameraSettings = new List<string>();
            if (Reference.cameraSettingPresets != null)
            {
                int t_NumberOfPresetOfCameraSettings = Reference.cameraSettingPresets.GetNumberOfAvailableCameraSettings();
                for (int i = 0; i < t_NumberOfPresetOfCameraSettings; i++)
                {

                    t_OptionsForCameraSettings.Add(Reference.cameraSettingPresets.GetNameOfCameraSettings(i));
                }
            }
            t_OptionsForCameraSettings.Add("Custom");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(SP_CameraSettingPersets);
                if (EditorGUI.EndChangeCheck())
                {
                    if (SP_CameraSettingPersets.objectReferenceValue == null) {

                        int t_NumberOfTransation = Reference.listOfCameraTransations.Count;
                        for (int i = 0; i < t_NumberOfTransation; i++) {

                            SerializedProperty t_IndexOfSelectedCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("indexOfSelectedCameraSettings");
                            t_IndexOfSelectedCameraSettings.intValue = 0;
                        }
                    }
                }
                if (Reference.cameraSettingPresets != null && GUILayout.Button("Remove", GUILayout.Width(100))) {

                    SP_CameraSettingPersets.objectReferenceValue = null;
                    return;
                }

                if (GUILayout.Button("+Transation", GUILayout.Width(100)))
                {
                    Reference.listOfCameraTransations.Add(new CameraTransation());
                    Reference.listOfCameraTransations[Reference.listOfCameraTransations.Count - 1].transationClips = new System.Collections.Generic.List<CameraTransationClip>();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (Reference.cameraSettingPresets != null)
                DrawSettingsEditor(Reference.cameraSettingPresets, null, ref Reference.showPresetsOfCameraSettings, ref cameraPresetEditor);


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
                    if (GUILayout.Button("+Clip", GUILayout.Width(100)))
                    {

                        Reference.listOfCameraTransations[i].transationClips.Add(new CameraTransationClip());
                        return;
                    }
                    if (GUILayout.Button("-Remove",GUILayout.Width(100))) {

                        Reference.listOfCameraTransations.RemoveAt(i);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (t_SPOnShowOnEditor.boolValue) {

                    SerializedProperty t_SPUseThisTransationSettingsForAllClip = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("useCentralCameraSettings");
                    SerializedProperty t_SPNameOfTransation = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("nameOfTransation");
                    SerializedProperty t_SPTransationType = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("transationType");
                    SerializedProperty t_SPDefaultCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("defaultCameraSettings");
                    SerializedProperty t_SPTransationClips = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("transationClips");

                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.PropertyField(t_SPNameOfTransation);
                        EditorGUILayout.PropertyField(t_SPTransationType);



                        EditorGUILayout.Space();
                        DrawHorizontalLine();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.PropertyField(t_SPUseThisTransationSettingsForAllClip);
                            if (t_SPUseThisTransationSettingsForAllClip.boolValue) {

                                SerializedProperty t_IndexOfSelectedCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("indexOfSelectedCameraSettings");

                                EditorGUI.BeginChangeCheck();
                                t_IndexOfSelectedCameraSettings.intValue = EditorGUILayout.Popup(
                                    t_IndexOfSelectedCameraSettings.intValue,
                                    t_OptionsForCameraSettings.ToArray());
                                if (EditorGUI.EndChangeCheck()) {

                                    if (Reference.cameraSettingPresets != null) {

                                        if (t_IndexOfSelectedCameraSettings.intValue < Reference.cameraSettingPresets.GetNumberOfAvailableCameraSettings())
                                        {
                                            CopyCameraSettingsFromOneToAnother(
                                                t_SPDefaultCameraSettings,
                                                Reference.cameraSettingPresets.cameraSettings[t_IndexOfSelectedCameraSettings.intValue].cameraSettings);

                                        }
                                    }

                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        if (t_SPUseThisTransationSettingsForAllClip.boolValue
                            && ((Reference.cameraSettingPresets != null && Reference.listOfCameraTransations[i].indexOfSelectedCameraSettings == Reference.cameraSettingPresets.GetNumberOfAvailableCameraSettings()) || Reference.cameraSettingPresets == null)) {
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

                                if (SP_CameraSettingPersets.objectReferenceValue != null) {

                                }

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

                        
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel -= 1;
                }
            }

        }

        #endregion
    }
}

