namespace com.faith.camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FaithCameraTransationController : MonoBehaviour
    {
        #region Public Variables

#if UNITY_EDITOR

        public bool showDefaultInspector;
        public bool showPresetsOfCameraSettings;

#endif

        public FaithCameraSettings      cameraSettingPresets;
        public List<CameraTransation>   listOfCameraTransations;

        #endregion

        #region Private Variables

        //Variables :   CinematicTransition
        private bool m_IsCinematicTransitionRunning = false;
        private bool m_IsExecuteForceQuitFromCameraTransition = false;

        #endregion

        #region Configuretion

        private bool IsValidIndexForCameraTransition(int t_TransitionIndex){

            if(t_TransitionIndex >= 0 && t_TransitionIndex < listOfCameraTransations.Count)
                return true;
            
            return false;
        }

        private int GetTransationIndexFromName(string t_Name){

            int t_NumberOfAvailableTransation = listOfCameraTransations.Count;
            for(int i = 0; i < t_NumberOfAvailableTransation; i++){

                if(t_Name == listOfCameraTransations[i].nameOfTransation)
                    return i;
            }

            Debug.LogError("No such transation found with the following name : " + t_Name);
            return -1;
        }

        
        private IEnumerator ControllerForOverridingCinematicCameraTransition(int t_TransitionIndex){

            WaitUntil t_WaitUntilTheCycleEnd = new WaitUntil(() => {

                if(m_IsCinematicTransitionRunning)
                    return false;

                return true;
            });

            m_IsExecuteForceQuitFromCameraTransition = true;
            yield return t_WaitUntilTheCycleEnd;
            m_IsExecuteForceQuitFromCameraTransition = false;

            StartCoroutine(ControllerForCinematicCameraTransition(t_TransitionIndex));
            StopCoroutine(ControllerForOverridingCinematicCameraTransition(-1));

        }

        private IEnumerator ControllerForCinematicCameraTransition(int t_TransitionIndex){

            return null;

        }

        #endregion

        #region Public Callback

        public void PlayTransition(string t_TransitionName){

            PlayTransition(GetTransationIndexFromName(t_TransitionName));
        }

        public void PlayTransition(int t_TransitionIndex, bool t_OverrideCurrentTransitionIfAny = false){

            if(!IsValidIndexForCameraTransition(t_TransitionIndex))
                return;
            
            if(m_IsCinematicTransitionRunning){

                if(t_OverrideCurrentTransitionIfAny){

                    StartCoroutine(ControllerForOverridingCinematicCameraTransition(t_TransitionIndex));
                }
            }else{

            }
        }

        #endregion
    }
}

