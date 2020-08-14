namespace com.faith.camera {
    
    using UnityEngine;

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

    [System.Serializable]
    public class CameraSettings {


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

        public Vector3 positionOffset = Vector3.zero;

        public FocusType focusType = FocusType.Once;
        public Constraint focusConstraint;
        public bool isUseFocusOffsetOnLocalSpace = false;
        public Vector3  focusOffset = Vector3.zero;
        
    }
}