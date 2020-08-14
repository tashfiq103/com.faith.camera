
namespace com.faithstudio.camera
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(CameraMovementController))]
    public class CameraMovementControllerEditor : Editor
    {
        private CameraMovementController Reference;

        private void OnEnable()
        {
            Reference = (CameraMovementController)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (EditorApplication.isPlaying)
            {

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Focus Target"))
                    {

                        Reference.FocusCameraWithOrigin(
                                Reference.cameraOriginInEditor,
                                Reference.cameraTargetInEditor,
                                Reference.useFocusOffsetInLocalSpaceInEditor
                            );
                    }

                    if (GUILayout.Button("Focus Area"))
                    {

                        Reference.FocusCameraAsArea(
                                Reference.cameraTargetInEditor,
                                Reference.useFocusOffsetInLocalSpaceInEditor
                            );
                    }
                }
                EditorGUILayout.EndHorizontal();


            }

            EditorGUILayout.Space();
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}


