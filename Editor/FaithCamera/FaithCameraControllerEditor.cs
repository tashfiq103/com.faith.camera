namespace com.faith.camera
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(FaithCameraController))]
    public class FaithCameraControllerEditor : Editor
    {
        #region Private Variables

        private FaithCameraController Reference;

        private SerializedProperty cameraReference;
        private SerializedProperty cameraContainerTransformReference;

        #endregion



        #region OnEditor

        private void OnEnable()
        {
            Reference = (FaithCameraController)target;

            if (Reference != null) {

                cameraReference = serializedObject.FindProperty("cameraReference");
                cameraContainerTransformReference = serializedObject.FindProperty("cameraContainerTransformReference");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Core Reference", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(cameraReference);
            EditorGUILayout.PropertyField(cameraContainerTransformReference);

            if (Reference.cameraReference == null)
            {

                EditorGUILayout.HelpBox("Please add 'Camera' under the 'cameraReference' in order to get access to full settings", MessageType.Error);
            }
            else {

                EditorGUILayout.Space();
                CustomGUI();
            }

            

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region CustomGUI

        private void CustomGUIOnPlayMode()
        {


        }

        private void CustomGUI() {
            
            Reference.showDefaultCameraSettings = EditorGUILayout.Foldout(
                    Reference.showDefaultCameraSettings,
                    "DefaultCameraSettings",
                    true
                );

            if (Reference.showDefaultCameraSettings) {
                DrawCustomGUIForCameraSettings(serializedObject.FindProperty("cameraDefaultSettings"));
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraDefaultSettings"));
            }
        }

        private void DrawCustomGUIForCameraSettings(SerializedProperty t_CameraSettings) {

            SerializedProperty t_ShowParameterForForwardVelocity                    = t_CameraSettings.FindPropertyRelative("showParameterOfForwardVelocity");
            SerializedProperty t_ForwardVelocity                                    = t_CameraSettings.FindPropertyRelative("forwardVelocity");
            SerializedProperty t_CurveForForwardVelocity                            = t_CameraSettings.FindPropertyRelative("curveForForwardVelocity");

            SerializedProperty t_ShowParameterForAngulerVelocity                    = t_CameraSettings.FindPropertyRelative("showParameterOfAngulerVelocity");
            SerializedProperty t_AngulerVelocity                                    = t_CameraSettings.FindPropertyRelative("angulerVelocity");
            SerializedProperty t_CurveForAngulerVelocity                            = t_CameraSettings.FindPropertyRelative("curveForAngulerVelocity");

            SerializedProperty t_ShowParameterForRateOfChangeOnFOV                  = t_CameraSettings.FindPropertyRelative("showParameterOfRateOfChangeOnFOV");
            SerializedProperty t_CameraFOV                                          = t_CameraSettings.FindPropertyRelative("cameraFOV");
            SerializedProperty t_RateOfChangeOnFOV                                  = t_CameraSettings.FindPropertyRelative("rateOfChangeOnFOV");
            SerializedProperty t_CurveForRateOfChangeOnFOV                          = t_CameraSettings.FindPropertyRelative("curveForRateOfChangeOnFOV");

            SerializedProperty t_ShowParameterForRateOfChangeOnOnOrthographicSize   = t_CameraSettings.FindPropertyRelative("showParameterOfRateOfChangeOnOrthographicSize");
            SerializedProperty t_CameraOrthographicSize                             = t_CameraSettings.FindPropertyRelative("cameraOrthographicSize");
            SerializedProperty t_RateOfChangeOnOrthographicSize                     = t_CameraSettings.FindPropertyRelative("rateOfChangeOnOrthographicSize");
            SerializedProperty t_CurveForRateOfChangeOnOrthographicSize             = t_CameraSettings.FindPropertyRelative("curveForRateOfChangeOnOrthographicSize");

            SerializedProperty t_CameraPositionOffset                               = t_CameraSettings.FindPropertyRelative("cameraPositionOffset");

            SerializedProperty t_CameraFocusOffset                                  = t_CameraSettings.FindPropertyRelative("cameraFocusOffset");
            SerializedProperty t_IsUseFocusOffsetOnLocalSpace                       = t_CameraSettings.FindPropertyRelative("isUseFocusOffsetOnLocalSpace");
            SerializedProperty t_FocusConstraint                                    = t_CameraSettings.FindPropertyRelative("focusConstraint");
            SerializedProperty t_FocusType                                          = t_CameraSettings.FindPropertyRelative("focusType");

            EditorGUI.indentLevel += 1;



            t_ShowParameterForForwardVelocity.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForForwardVelocity.boolValue,
                    "ForwardVelocity",
                    true
                );
            if (t_ShowParameterForForwardVelocity.boolValue) {

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.indentLevel += 1;
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

                    EditorGUI.indentLevel -= 1;

                }
                EditorGUILayout.EndVertical();
            }


            t_ShowParameterForAngulerVelocity.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForAngulerVelocity.boolValue,
                    "AngulerVelocity",
                    true
                );
            if (t_ShowParameterForAngulerVelocity.boolValue) {

                EditorGUILayout.LabelField("AngulerVelocity", EditorStyles.boldLabel);
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
            }
            
            if (Reference.cameraReference.orthographic)
            {

                t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue,
                    "Settings : Orthographic",
                    true
                );

                if (t_ShowParameterForRateOfChangeOnOnOrthographicSize.boolValue) {

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
                }
            }
            else
            {

                t_ShowParameterForRateOfChangeOnFOV.boolValue = EditorGUILayout.Foldout(
                    t_ShowParameterForRateOfChangeOnFOV.boolValue,
                    "Settings : Orthographic",
                    true
                );

                if (t_ShowParameterForRateOfChangeOnFOV.boolValue) {

                    

                    EditorGUILayout.BeginHorizontal();
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
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
            t_CameraPositionOffset.vector3Value = EditorGUILayout.Vector3Field(
                    "PositionOffset",
                    t_CameraPositionOffset.vector3Value
                );

            EditorGUI.indentLevel -= 1;
        }

        #endregion
    }
}

