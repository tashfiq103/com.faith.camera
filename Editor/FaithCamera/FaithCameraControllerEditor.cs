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
                DrawCustomGUIForCameraSettings(Reference.cameraDefaultSettings);
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraDefaultSettings"));
            }
        }

        private void DrawCustomGUIForCameraSettings(CameraSettings t_CameraSettings) {

            EditorGUI.indentLevel += 1;

            EditorGUILayout.LabelField("ForwardVelocity", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                t_CameraSettings.forwardVelocity = EditorGUILayout.Slider(
                        t_CameraSettings.forwardVelocity,
                        0f,
                        1f,
                        GUILayout.Width(Screen.width * 0.4f)
                    );

                t_CameraSettings.curveForForwardVelocity = EditorGUILayout.CurveField(
                        t_CameraSettings.curveForForwardVelocity,
                        GUILayout.Width(Screen.width * 0.6f)
                    );
                
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("AngulerVelocity", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                t_CameraSettings.angulerVelocity = EditorGUILayout.Slider(
                        t_CameraSettings.angulerVelocity,
                        0f,
                        1f,
                        GUILayout.Width(Screen.width * 0.4f)
                    );

                t_CameraSettings.curveForAngulerVelocity = EditorGUILayout.CurveField(
                        t_CameraSettings.curveForAngulerVelocity,
                        GUILayout.Width(Screen.width * 0.6f)
                    );

            }
            EditorGUILayout.EndHorizontal();

            if (Reference.cameraReference.orthographic)
            {

                EditorGUILayout.LabelField("OrthographicSize", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                {
                    t_CameraSettings.rateOfChangeOnOrthographicSize = EditorGUILayout.Slider(
                            t_CameraSettings.rateOfChangeOnOrthographicSize,
                            0f,
                            1f,
                            GUILayout.Width(Screen.width * 0.4f)
                        );

                    t_CameraSettings.curveForRateOfChangeOnOrthographicSize = EditorGUILayout.CurveField(
                            t_CameraSettings.curveForRateOfChangeOnOrthographicSize,
                            GUILayout.Width(Screen.width * 0.6f)
                        );

                }
                EditorGUILayout.EndHorizontal();
            }
            else {

                EditorGUILayout.LabelField("FOV", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                {
                    t_CameraSettings.cameraFOV = EditorGUILayout.Slider(
                            t_CameraSettings.cameraFOV,
                            0f,
                            1f,
                            GUILayout.Width(Screen.width * 0.4f)
                        );

                    t_CameraSettings.curveForRateOfChangeOnFOV = EditorGUILayout.CurveField(
                            t_CameraSettings.curveForRateOfChangeOnFOV,
                            GUILayout.Width(Screen.width * 0.6f)
                        );

                }
                EditorGUILayout.EndHorizontal();
            }

            

            EditorGUI.indentLevel -= 1;
        }

        #endregion
    }
}

