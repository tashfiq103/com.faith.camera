namespace com.faith.camera {
    
    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public struct Constraint
    {
        public bool x;
        public bool y;
        public bool z;
    }

    public enum UpdateType
    {
        LateUpdate,
        Update,
        FixedUpdate,
        SmartUpdate
    }

    public enum FocusType
    {
        Once,
        Follow
    }

    public enum TransationType
    {
        Cinematic,
        Dynamic
    }

    [System.Serializable]
    public class CameraSettings {


#if UNITY_EDITOR

        public bool showParameterOfForwardVelocity;
        public bool showParameterOfAngulerVelocity;
        public bool showParameterOfRateOfChangeOnFOV;
        public bool showParameterOfRateOfChangeOnOrthographicSize;
        public bool showParameterOfOffsets;
        public bool showParameterForAdvancedSettings;


#endif

        [Range (0f, 1f)]
        public float forwardVelocity = 0.2f;
        public AnimationCurve curveForForwardVelocity = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

        [Range (0f, 1f)]
        public float angulerVelocity = 0.1f;
        public AnimationCurve curveForAngulerVelocity = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });

        //Configuretion :   PerspectiveCamera
        [Range (0f, 180f)]
        public float cameraFOV = 60;
        [Range(0f,1f)]
        public float rateOfChangeOnFOV = 0.1f;
        public AnimationCurve curveForRateOfChangeOnFOV = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
        //---------------

        //Configuretion :   OrthographicCamera
        [Range(0f, 180f)]
        public float cameraOrthographicSize = 6;
        [Range(0f, 1f)]
        public float rateOfChangeOnOrthographicSize = 0.1f;
        public AnimationCurve curveForRateOfChangeOnOrthographicSize = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
        //---------------

        public Vector3      cameraPositionOffset = Vector3.zero;
        public Vector3      cameraFocusOffset = Vector3.zero;
        public bool         isUseFocusOffsetOnLocalSpace = false;
        public Constraint   focusConstraint;
        public FocusType    focusType = FocusType.Once;
        
    }

    [System.Serializable]
    public class CameraTransationClip
    {
#if UNITY_EDITOR

        public bool showOnEditor;
        public bool showCameraSettings;
        public int indexOfSelectedCameraSettings = 0;
#endif

        public Transform        cameraOrigin;
        public List<Transform>  cameraFocuses;

        public CameraSettings cameraSettings;

        [Range(0.1f,50f)]
        public float durationOfTransation = 0.1f;

        [Range(0f, 180f)]
        public float overrideCameraFOV = 60;
        [Range(0f, 180f)]
        public float overrideCameraOrthographicSize = 6;
    }

    [System.Serializable]
    public class CameraTransation
    {

#if UNITY_EDITOR

        public bool showOnEditor;

        public int indexOfSelectedCameraSettings = 0;

#endif

        public bool useCentralCameraSettings = true;
        public string nameOfTransation;
        public TransationType transationType;
        public CameraSettings defaultCameraSettings;

        public List<CameraTransationClip> transationClips;
        
    }
}