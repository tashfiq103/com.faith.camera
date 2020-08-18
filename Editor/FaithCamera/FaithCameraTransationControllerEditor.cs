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

            CustomGUIForPresetCameraSettings();

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

            

            int t_NumberOfCameraTransation = Reference.listOfCameraTransations.Count;

            if (t_NumberOfCameraTransation > 0)
                DrawHorizontalLine();

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
                        Reference.listOfCameraTransations.TrimExcess();
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (t_SPOnShowOnEditor.boolValue) {

                    SerializedProperty t_SPUseThisTransationSettingsForAllClip = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("useCentralCameraSettings");
                    SerializedProperty t_SPNameOfTransation = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("nameOfTransation");
                    SerializedProperty t_SPTransationType = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("transationType");
                    SerializedProperty t_SPDefaultCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("defaultCameraSettings");
                    

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

                        if (t_NumberOfTransationClip > 0)
                            DrawHorizontalLine();

                        for (int j = 0; j < t_NumberOfTransationClip; j++) {

                            CustomGUIForTransationClip(i, j, t_SPUseThisTransationSettingsForAllClip.boolValue, t_OptionsForCameraSettings);
                            
                        }

                        
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel -= 1;
                }
            }

            DrawHorizontalLine();
            if (GUILayout.Button("+Transation"))
            {
                Reference.listOfCameraTransations.Add(new CameraTransation());
                Reference.listOfCameraTransations[Reference.listOfCameraTransations.Count - 1].transationClips = new List<CameraTransationClip>();
                return;
            }

        }

        private void CustomGUIForPresetCameraSettings() {

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(SP_CameraSettingPersets);
                if (EditorGUI.EndChangeCheck())
                {
                    if (SP_CameraSettingPersets.objectReferenceValue == null)
                    {

                        int t_NumberOfTransation = Reference.listOfCameraTransations.Count;
                        for (int i = 0; i < t_NumberOfTransation; i++)
                        {

                            SerializedProperty t_IndexOfSelectedCameraSettings = SP_ListOfCameraTransations.GetArrayElementAtIndex(i).FindPropertyRelative("indexOfSelectedCameraSettings");
                            t_IndexOfSelectedCameraSettings.intValue = 0;
                        }
                    }
                }

                if (Reference.cameraSettingPresets != null)
                {
                    if (GUILayout.Button("-Remove", GUILayout.Width(100)))
                    {
                        SP_CameraSettingPersets.objectReferenceValue = null;
                        return;
                    }
                }
                else
                {

                    if (GUILayout.Button("+New", GUILayout.Width(100)))
                    {

                        ScriptableObject t_NewCameraSettings = ScriptableObject.CreateInstance(typeof(FaithCameraSettings)) as FaithCameraSettings;
                        ((FaithCameraSettings)t_NewCameraSettings).cameraSettings = new List<CameraSettingsInfo>();
                        AssetDatabase.CreateAsset(t_NewCameraSettings, "Assets/CameraSettings.asset");
                        SP_CameraSettingPersets.objectReferenceValue = t_NewCameraSettings;

                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (Reference.cameraSettingPresets != null)
            {
                DrawSettingsEditor(Reference.cameraSettingPresets, null, ref Reference.showPresetsOfCameraSettings, ref cameraPresetEditor);
            }
        }

        private void CustomGUIForTransationClip(
            int t_TransationIndex,
            int t_ClipIndex,
            bool t_UseThisTransationSettingsForAllClip,
            List<string> t_OptionsForCameraSettings) {

            SerializedProperty t_SPTransationClips = SP_ListOfCameraTransations.GetArrayElementAtIndex(t_TransationIndex).FindPropertyRelative("transationClips");

            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginVertical();
            {

                SerializedProperty t_SPShowOnEditor = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("showOnEditor");
                EditorGUILayout.BeginHorizontal();
                {
                    t_SPShowOnEditor.boolValue = EditorGUILayout.Foldout(
                            t_SPShowOnEditor.boolValue,
                            "Clip (" + t_ClipIndex + ")",
                            true
                        );
                    if (GUILayout.Button("-Remove", GUILayout.Width(100f))) {

                        Reference.listOfCameraTransations[t_TransationIndex].transationClips.RemoveAt(t_ClipIndex);
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
                

                if (t_SPShowOnEditor.boolValue) {

                    EditorGUI.indentLevel += 1;

                    SerializedProperty t_SPCameraOrigin = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("cameraOrigin");
                    SerializedProperty t_SPCameraFocuses = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("cameraFocuses");
                    SerializedProperty t_SPOverrideCameraFOV = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("overrideCameraFOV");
                    SerializedProperty t_SPOverrideCameraOrthograpicSize = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("overrideCameraOrthographicSize");
                    SerializedProperty t_SPDurationOfTransation = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("durationOfTransation");

                    EditorGUILayout.PropertyField(t_SPCameraOrigin);
                    EditorGUILayout.PropertyField(t_SPCameraFocuses);
                    if (Reference.listOfCameraTransations[t_TransationIndex].transationType == TransationType.Cinematic)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(t_SPDurationOfTransation);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("OverrideValue", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(t_SPOverrideCameraFOV);
                    EditorGUILayout.PropertyField(t_SPOverrideCameraOrthograpicSize);

                    EditorGUILayout.Space();
                    if (!t_UseThisTransationSettingsForAllClip)
                    {
                        SerializedProperty t_SPCameraSettingsForTransationClip  = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("cameraSettings");
                        SerializedProperty t_SPShowCameraSettings               = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("showCameraSettings");

                        

                        if (SP_CameraSettingPersets.objectReferenceValue == null)
                        {
                            //if : No CameraSettings Preset aren't assigned
                            t_SPShowCameraSettings.boolValue = EditorGUILayout.Foldout(
                                t_SPShowCameraSettings.boolValue,
                                "CameraSettings",
                                true
                            );

                            if (t_SPShowCameraSettings.boolValue) {

                                DrawCustomGUIForCameraSettings(t_SPCameraSettingsForTransationClip);
                            }
                        }
                        else if(Reference.cameraSettingPresets != null)
                        {

                            //if : Assigned, Then it will shown from the drop down option
                            //if : No settings are made in the option, it will switch to custom.

                            SerializedProperty t_SPIndexOfSelectedCameraSettings = t_SPTransationClips.GetArrayElementAtIndex(t_ClipIndex).FindPropertyRelative("indexOfSelectedCameraSettings");

                            EditorGUI.BeginChangeCheck();
                            {
                                t_SPIndexOfSelectedCameraSettings.intValue = EditorGUILayout.Popup(
                                    "CameraSettings",
                                    t_SPIndexOfSelectedCameraSettings.intValue,
                                    t_OptionsForCameraSettings.ToArray());
                            }
                            if (EditorGUI.EndChangeCheck()) {

                                if (t_SPIndexOfSelectedCameraSettings.intValue < Reference.cameraSettingPresets.cameraSettings.Count) {

                                    CopyCameraSettingsFromOneToAnother(t_SPCameraSettingsForTransationClip, Reference.cameraSettingPresets.cameraSettings[t_SPIndexOfSelectedCameraSettings.intValue].cameraSettings);
                                }
                            }
                            

                            if (t_SPIndexOfSelectedCameraSettings.intValue == Reference.cameraSettingPresets.cameraSettings.Count) {

                                EditorGUI.indentLevel += 1;
                                DrawCustomGUIForCameraSettings(t_SPCameraSettingsForTransationClip);
                                EditorGUI.indentLevel -= 1;
                            }
                        }
                    }

                    EditorGUI.indentLevel -= 1;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;
        }

        #endregion
    }
}

