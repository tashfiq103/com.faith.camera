namespace com.faith.camera {
    using UnityEditor;
    using UnityEngine;

    public class BaseEditorClass : Editor {
        #region Editor Module

        public void DrawHorizontalLine () {
            EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
        }

        public void DrawHorizontalLineOnGUI (Rect rect) {
            EditorGUI.LabelField (rect, "", GUI.skin.horizontalSlider);
        }

        public void DrawSettingsEditor (Object settings, System.Action OnSettingsUpdated, ref bool foldout, ref Editor editor) {

            if (settings != null) {

                using (var check = new EditorGUI.ChangeCheckScope ()) {

                    foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);

                    if (foldout) {

                        CreateCachedEditor (settings, null, ref editor);
                        editor.OnInspectorGUI ();

                        if (check.changed) {

                            if (OnSettingsUpdated != null) {

                                OnSettingsUpdated.Invoke ();
                            }
                        }
                    }
                }
            }

        }

        #endregion
    }
}