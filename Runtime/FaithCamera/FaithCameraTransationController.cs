namespace com.faith.camera {
    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;

    public class FaithCameraTransationController : MonoBehaviour {
        #region Public Variables

#if UNITY_EDITOR

        public bool showDefaultInspector;
        public bool showPresetsOfCameraSettings;

#endif

        public FaithCameraSettings cameraSettingPresets;
        public List<CameraTransation> listOfCameraTransations;

        #endregion

        #region Private Variables

        //Variables :   CinematicTransition
        private bool m_IsCinematicTransitionRunning = false;
        private bool m_IsExecuteForceQuitFromCameraTransition = false;

        #endregion

        #region Configuretion

        private bool IsValidIndexForCameraTransition (int t_TransitionIndex) {

            if (t_TransitionIndex >= 0 && t_TransitionIndex < listOfCameraTransations.Count)
                return true;

            return false;
        }

        private int GetTransationIndexFromName (string t_Name) {

            int t_NumberOfAvailableTransation = listOfCameraTransations.Count;
            for (int i = 0; i < t_NumberOfAvailableTransation; i++) {

                if (t_Name == listOfCameraTransations[i].nameOfTransation)
                    return i;
            }

            Debug.LogError ("No such transation found with the following name : " + t_Name);
            return -1;
        }

        private IEnumerator ControllerForOverridingCinematicCameraTransition (int t_TransitionIndex) {

            WaitUntil t_WaitUntilTheCycleEnd = new WaitUntil (() => {

                if (m_IsCinematicTransitionRunning)
                    return false;

                return true;
            });

            m_IsExecuteForceQuitFromCameraTransition = true;
            yield return t_WaitUntilTheCycleEnd;
            m_IsExecuteForceQuitFromCameraTransition = false;

            StartCoroutine (ControllerForCinematicCameraTransition (t_TransitionIndex));
            StopCoroutine (ControllerForOverridingCinematicCameraTransition (-1));

        }

        private IEnumerator ControllerForCinematicCameraTransition (int t_TransitionIndex) {
            //-------------
            //Variable Declaration
            
            Transform t_NewCameraPointer    = new GameObject().transform;
#if UNITY_EDITOR
            t_NewCameraPointer.name = "#CameraPointer";
#endif

            int t_NumberOfAvailableClip = listOfCameraTransations[t_TransitionIndex].transationClips.Count;
            int t_CurrentClipIndex = 0;
            int t_NextClipDirectionValue = 1;
            float t_CycleLength = 0.0167f;
            WaitForSeconds t_CycleDelay = new WaitForSeconds(t_CycleLength);

            //-------------
            //TransationLoop
            while (t_CurrentClipIndex >= 0 && t_CurrentClipIndex < t_NumberOfAvailableClip) {



                //Starting : Camera transition with the new origin and clip respective settings.
                FaithCameraController.Instance.FocusCamera (
                    t_CameraFocuses: listOfCameraTransations[t_TransitionIndex].transationClips[t_CurrentClipIndex].cameraFocuses,
                    t_CameraOriginPosition: t_NewCameraPointer,
                    t_CameraSettings : listOfCameraTransations[t_TransitionIndex].transationClips[t_CurrentClipIndex].cameraSettings
                );

                //Initialization    :   For Camera
                float t_DurationOfTransation = listOfCameraTransations[t_TransitionIndex].transationClips[t_CurrentClipIndex].durationOfTransation;
                float t_RemainingTimeForTransation = t_DurationOfTransation;
                float t_TimeProgression = 0;
                Vector3 t_InitialPositionOfCamera = FaithCameraController.Instance.cameraContainerTransformReference.position;
                
                //Timer :   For transition clip
                while(!m_IsExecuteForceQuitFromCameraTransition && t_RemainingTimeForTransation > 0){

                    t_TimeProgression = 1f - (t_RemainingTimeForTransation / t_DurationOfTransation);
                    
                    Vector3 t_ModifiedPosition = Vector3.Lerp(
                        t_InitialPositionOfCamera,
                        listOfCameraTransations[t_TransitionIndex].transationClips[t_CurrentClipIndex].cameraOrigin.position,
                        t_TimeProgression
                    );
                    t_NewCameraPointer.position = t_ModifiedPosition;
                    
                    yield return t_CycleDelay;
                    
                    t_RemainingTimeForTransation -= t_CycleLength;
                }

                //if : ForceQuitExecute
                if(m_IsExecuteForceQuitFromCameraTransition)
                    break;

                //Go To Next Transition
                t_CurrentClipIndex += t_NextClipDirectionValue;

                //Take Action Based On TransitionMode
                switch(listOfCameraTransations[t_TransitionIndex].transitionMode){
                    case TransitionMode.Once:
                        if(t_CurrentClipIndex >= t_NumberOfAvailableClip)
                            t_CurrentClipIndex = -1;
                    break;
                    case TransitionMode.Loop:
                        if(t_CurrentClipIndex >= t_NumberOfAvailableClip)
                            t_CurrentClipIndex = 0;
                    break;
                    case TransitionMode.PingPong:
                        
                        if(t_CurrentClipIndex >= t_NumberOfAvailableClip){
                            
                            t_CurrentClipIndex = t_NumberOfAvailableClip - 1;
                            t_NextClipDirectionValue = -1;
                        }else if(t_CurrentClipIndex <= 0){

                            t_CurrentClipIndex = 1;
                            t_NextClipDirectionValue = 1;
                        }
                    break;
                }
            }

            Destroy(t_NewCameraPointer.gameObject);
            StopCoroutine(ControllerForCinematicCameraTransition(-1));
        }

        #endregion

        #region Public Callback

        public void PlayTransition (string t_TransitionName) {

            PlayTransition (GetTransationIndexFromName (t_TransitionName));
        }

        public void PlayTransition (int t_TransitionIndex, bool t_OverrideCurrentTransitionIfAny = false) {

            if (!IsValidIndexForCameraTransition (t_TransitionIndex))
                return;
            
            if (m_IsCinematicTransitionRunning) {
                if (t_OverrideCurrentTransitionIfAny)
                    StartCoroutine (ControllerForOverridingCinematicCameraTransition (t_TransitionIndex));
                else
                    StartCoroutine (ControllerForCinematicCameraTransition (t_TransitionIndex));
            } else 
                StartCoroutine (ControllerForCinematicCameraTransition (t_TransitionIndex));
            
        }

        #endregion
    }
}