namespace com.faith.camera
{
    using UnityEngine;
    using UnityEditor;

    public class BaseEditorClassForFaithCamera : Editor
    {
        #region CustomGUI

        public void DrawCustomGUIForCameraSettings(SerializedProperty t_CameraSettings)
        {

            SerializedProperty t_ShowParameterForForwardVelocity = t_CameraSettings.FindPropertyRelative("showParameterOfForwardVelocity");
            SerializedProperty t_ForwardVelocity = t_CameraSettings.FindPropertyRelative("forwardVelocity");
            SerializedProperty t_CurveForForwardVelocity = t_CameraSettings.FindPropertyRelative("curveForForwardVelocity");

            SerializedProperty t_ShowParameterForAngulerVelocity = t_CameraSettings.FindPropertyRelative("showParameterOfAngulerVelocity");
            SerializedProperty t_AngulerVelocity = t_CameraSettings.FindPropertyRelative("angulerVelocity");
            SerializedProperty t_CurveForAngulerVelocity = t_CameraSettings.FindPropertyRelative("curveForAngulerVelocity");

            SerializedProperty t_ShowParameterForRateOfChangeOnFOV = t_CameraSettings.FindPropertyRelative("showParameterOfRateOfChangeOnFOV");
            SerializedProperty t_CameraFOV = t_CameraSettings.FindPropertyRelative("cameraFOV");
            SerializedProperty t_RateOfChangeOnFOV = t_CameraSettings.FindPropertyRelative("rateOfChangeOnFOV");
            SerializedProperty t_CurveForRateOfChangeOnFOV = t_CameraSettings.FindPropertyRelative("curveForRateOfChangeOnFOV");

            SerializedProperty t_ShowParameterForRateOfChangeOnOnOrthographicSize = t_CameraSettings.FindPropertyRelative("showParameterOfRateOfChangeOnOrthographicSize");
            SerializedProperty t_CameraOrthographicSize = t_CameraSettings.FindPropertyRelative("cameraOrthographicSize");
            SerializedProperty t_RateOfChangeOnOrthographicSize = t_CameraSettings.FindPropertyRelative("rateOfChangeOnOrthographicSize");
            SerializedProperty t_CurveForRateOfChangeOnOrthographicSize = t_CameraSettings.FindPropertyRelative("curveForRateOfChangeOnOrthographicSize");

            SerializedProperty t_ShowParameterForCameraOffset = t_CameraSettings.FindPropertyRelative("showParameterOfOffsets");
            SerializedProperty t_CameraPositionOffset = t_CameraSettings.FindPropertyRelative("cameraPositionOffset");
            SerializedProperty t_CameraFocusOffset = t_CameraSettings.FindPropertyRelative("cameraFocusOffset");

            SerializedProperty t_ShowParameterForAdvanceSettings = t_CameraSettings.FindPropertyRelative("showParameterForAdvancedSettings");
            SerializedProperty t_IsUseFocusOffsetOnLocalSpace = t_CameraSettings.FindPropertyRelative("isUseFocusOffsetOnLocalSpace");
            SerializedProperty t_FocusConstraint = t_CameraSettings.FindPropertyRelative("focusConstraint");
            SerializedProperty t_FocusType = t_CameraSettings.FindPropertyRelative("focusType");

            EditorGUI.indentLevel += 1;

            //Section   :   Orthographic
            t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue = EditorGUILayout.Foldout(
                   t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue,
                   "Orthographic",
                   true
               );

            if (t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_CameraOrthographicSize.floatValue = EditorGUILayout.Slider(
                        "OrthograpicSize",
                        t_CameraOrthographicSize.floatValue,
                        1,
                        60
                    );

                    t_RateOfChangeOnOrthographicSize.floatValue = EditorGUILayout.Slider(
                            "rateOfChange",
                            t_RateOfChangeOnOrthographicSize.floatValue,
                            0f,
                            1f
                        );

                    t_CurveForRateOfChangeOnOrthographicSize.animationCurveValue = EditorGUILayout.CurveField(
                            "rateOfChangeOverTime",
                            t_CurveForRateOfChangeOnOrthographicSize.animationCurveValue
                        );

                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }

            //Section   :   Perspective
            t_ShowParameterForRateOfChangeOnFOV.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForRateOfChangeOnFOV.boolValue,
                    "Perspective",
                    true
                );

            if (t_ShowParameterForRateOfChangeOnFOV.boolValue)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_CameraFOV.floatValue = EditorGUILayout.Slider(
                        "FieldOfView",
                        t_CameraFOV.floatValue,
                        1,
                        60
                    );

                    t_RateOfChangeOnFOV.floatValue = EditorGUILayout.Slider(
                            "rateOfChange",
                            t_RateOfChangeOnFOV.floatValue,
                            0f,
                            1f
                        );

                    t_CurveForRateOfChangeOnFOV.animationCurveValue = EditorGUILayout.CurveField(
                        "rateOfChangeOverTime",
                        t_CurveForRateOfChangeOnFOV.animationCurveValue
                        );

                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }

            //Section   :   ForwardVelocity
            t_ShowParameterForForwardVelocity.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForForwardVelocity.boolValue,
                    "ForwardVelocity",
                    true
                );
            if (t_ShowParameterForForwardVelocity.boolValue)
            {

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_ForwardVelocity.floatValue = EditorGUILayout.Slider(
                            "rateOfChange",
                            t_ForwardVelocity.floatValue,
                            0f,
                            1f
                        );

                    t_CurveForForwardVelocity.animationCurveValue = EditorGUILayout.CurveField(
                            "rateOfChangeOverTime",
                            t_CurveForForwardVelocity.animationCurveValue
                        );
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }

            //Section   :   AngulerVelocity
            t_ShowParameterForAngulerVelocity.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForAngulerVelocity.boolValue,
                    "AngulerVelocity",
                    true
                );
            if (t_ShowParameterForAngulerVelocity.boolValue)
            {

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_AngulerVelocity.floatValue = EditorGUILayout.Slider(
                            "rateOfChange",
                            t_AngulerVelocity.floatValue,
                            0f,
                            1f
                        );

                    t_CurveForAngulerVelocity.animationCurveValue = EditorGUILayout.CurveField(
                            "rateOfChangeOverTime",
                            t_CurveForAngulerVelocity.animationCurveValue
                        );

                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }


            //Section   :   Offset
            t_ShowParameterForCameraOffset.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForCameraOffset.boolValue,
                    "Offsets",
                    true
                );
            if (t_ShowParameterForCameraOffset.boolValue)
            {

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_CameraPositionOffset.vector3Value = EditorGUILayout.Vector3Field(
                        "PositionOffset",
                        t_CameraPositionOffset.vector3Value
                    );

                    t_CameraFocusOffset.vector3Value = EditorGUILayout.Vector3Field(
                        "FocusOffset",
                        t_CameraFocusOffset.vector3Value
                    );
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }

            //Section   :   Advanced Settings
            t_ShowParameterForAdvanceSettings.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForAdvanceSettings.boolValue,
                    "Advanced Settings",
                    true
                );
            if (t_ShowParameterForAdvanceSettings.boolValue)
            {

                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginVertical();
                {
                    t_IsUseFocusOffsetOnLocalSpace.boolValue = EditorGUILayout.Toggle(
                        "UseFocusOffsetOnLocalSpace",
                        t_IsUseFocusOffsetOnLocalSpace.boolValue
                    );

                    EditorGUILayout.PropertyField(t_FocusConstraint);
                    EditorGUILayout.PropertyField(t_FocusType);
                }
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel -= 1;
            }


            EditorGUI.indentLevel -= 1;
        }

        #endregion

        #region Editor Module

        public void DrawHorizontalLine()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        public void DrawHorizontalLineOnGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);
        }

        public void DrawSettingsEditor(Object settings, System.Action OnSettingsUpdated, ref bool foldout, ref Editor editor)
        {

            if (settings != null)
            {

                using (var check = new EditorGUI.ChangeCheckScope())
                {

                    foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

                    if (foldout)
                    {

                        CreateCachedEditor(settings, null, ref editor);
                        editor.OnInspectorGUI();

                        if (check.changed)
                        {

                            if (OnSettingsUpdated != null)
                            {

                                OnSettingsUpdated.Invoke();
                            }
                        }
                    }
                }
            }

        }
        #endregion
    }
}

