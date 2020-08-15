namespace com.faith.camera
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(FaithCameraController))]
    public class FaithCameraControllerEditor : BaseEditorClassForFaithCamera
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
            }
        }

        #endregion
    }
}

