namespace com.faithstudio.Camera {
    using System.Collections.Generic;
    using UnityEngine.Events;
    using UnityEngine;

    public class CameraMovementController : MonoBehaviour {

        #region Public Variables

        public static CameraMovementController Instance;

#if UNITY_EDITOR

        [Header ("Configuretion  :   Editor")]
        public bool showCameraGizmos;
        public bool useFocusOffsetInLocalSpaceInEditor;

        [Space (5.0f)]
        public Transform cameraOriginInEditor;
        public List<Transform> cameraTargetInEditor;

#endif

        [Space (10.0f)]
        [Header ("Configuretion  :   Core Valus & References")]
        public Camera cameraReference;
        [Space (10.0f)]
        public Transform cameraContainerTransformReference;
        public Transform cameraTransformReference;
        [Range (0f, 1f)]
        public float defaultMaxFocusingValue = 0.1f;
        [Range (0f, 1f)]
        public float defaultForwardVelocity = 0.5f;
        [Range (0f, 1f)]
        public float defaultAngulerVelocity = 0.1f;
        public AnimationCurve defaultAngulerVelocityThroughProgression = new AnimationCurve (new Keyframe[] { new Keyframe (0f, 0f), new Keyframe (1f, 1f) });
        public Vector3 defaultCameraFocusOffset;
        public Vector3 defaultCameraPositionOffset;

        [Space (10.0f)]
        [Header ("Configuretion  :   CameraViewOnTargets")]

        [Range (1f, 5f)]
        public float cameraBoundExtend = 1;
        [Range (0f, 1f)]
        public float cameraPositionOnX;
        [Range (0f, 1f)]
        public float cameraPositionOnY;
        [Range (0f, 1f)]
        public float cameraPositionOnZ;

        #endregion

        #region Private Variables


        private bool m_IsCameraOrthographic;
        private bool m_IsFollowingObject;
        private bool m_UseOffsetFocusOnLocalSpace;

        private int m_NumberOfCameraFocuses;

        private float m_InitialCameraFieldOfView;
        private float m_InitialOrthographicSize;
        private float m_MaximumDistanceFromNewCameraOrigin;

        private float m_CameraForwardVelocity;
        private float m_CameraAngulerVelocity;
        private float m_MinZoom;
        private float m_MaxZoom;
        private float m_TargetedZoom;
        private float m_ModifiedZoom;

        private Bounds m_NewCameraBounds;

        private Transform m_TransformReferenceOfCameraOrigin;
        private List<Transform> m_ListOfCameraFocuses;

        private Vector3 m_CameraMovementVelocity;
        private Vector3 m_CameraFocusOffset;
        private Vector3 m_CameraPositionOffset;

        private Vector3 m_FocusPosition;
        private Vector3 m_FocusedEulerAngle;

        private Vector3 m_CurrentPositionOfTheCamera;
        private Vector3 m_NewOriginPositionOfCamera;

        private Vector3 m_ModifiedPosition;
        private Quaternion m_ModifiedRotation;

        private AnimationCurve m_AngulerVelocityThroughProgression;

        private UnityAction OnCameraReachedTargetedPosition;

        #endregion

        #region Mono Behaviour

#if UNITY_EDITOR

        private void OnDrawGizmos () {
            if (showCameraGizmos) {

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube (
                    m_NewCameraBounds.center,
                    m_NewCameraBounds.size
                );

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube (
                    m_NewCameraBounds.center,
                    m_NewCameraBounds.size * cameraBoundExtend
                );
            }
        }

#endif

        private void Awake () {

            if (Instance == null)
                Instance = this;

            m_InitialCameraFieldOfView = cameraReference.fieldOfView;
            m_InitialOrthographicSize = cameraReference.orthographicSize;

            CalculateFOVAndOrthographicSize (defaultMaxFocusingValue);

            EnableAndDisableThisMonoBehaviour (false);
        }

        private void LateUpdate () {
            CalculateCameraBound ();
            MoveCamera ();
            RotateCamera ();
            ZoomCamera ();
        }

        #endregion

        #region Configuretion

        private void EnableAndDisableThisMonoBehaviour (bool t_IsEnable) {

            enabled = t_IsEnable;
        }

        private void CalculateFOVAndOrthographicSize (float t_MaxFocusingValue) {

            if (cameraReference.orthographic) {
                m_IsCameraOrthographic = true;

                m_MinZoom = m_InitialOrthographicSize;
                m_MaxZoom = m_MinZoom * (1f - t_MaxFocusingValue);
            } else {
                m_IsCameraOrthographic = false;

                m_MinZoom = m_InitialCameraFieldOfView;
                m_MaxZoom = m_MinZoom * (1f - t_MaxFocusingValue);
            }
        }

        private void CalculateCameraBound () {

            m_NewCameraBounds = new Bounds (m_ListOfCameraFocuses[0].position, Vector3.zero);
            for (int i = 0; i < m_NumberOfCameraFocuses; i++) {
                m_NewCameraBounds.Encapsulate (m_ListOfCameraFocuses[i].position);
            }

            Vector3 t_TargetPosition = m_NewCameraBounds.center;
            Vector3 t_AbsoluteCameraFocus = t_TargetPosition;

            if (m_UseOffsetFocusOnLocalSpace) {
                Vector3 t_OriginPosition = cameraTransformReference.position;
                t_OriginPosition.y = t_TargetPosition.y;

                Vector3 t_NormalizedDirection = Vector3.Normalize (t_TargetPosition - t_OriginPosition);
                t_AbsoluteCameraFocus = t_TargetPosition;
                t_AbsoluteCameraFocus += (t_NormalizedDirection * m_CameraFocusOffset.z);
                t_AbsoluteCameraFocus += new Vector3 (
                    t_NormalizedDirection.z,
                    0,
                    t_NormalizedDirection.x
                ) * m_CameraFocusOffset.x;
                t_AbsoluteCameraFocus += Vector3.up * m_CameraFocusOffset.y;
            } else {

                t_AbsoluteCameraFocus += m_CameraFocusOffset;
            }

            m_FocusPosition = t_AbsoluteCameraFocus;
            m_FocusedEulerAngle = Quaternion.LookRotation (m_FocusPosition - cameraTransformReference.position).eulerAngles;

        }

        private void MoveCamera () {

            m_CurrentPositionOfTheCamera = cameraContainerTransformReference.position;

            if (m_TransformReferenceOfCameraOrigin != null)
                m_NewOriginPositionOfCamera = m_TransformReferenceOfCameraOrigin.position;

            m_ModifiedPosition = Vector3.SmoothDamp (
                m_CurrentPositionOfTheCamera,
                m_NewOriginPositionOfCamera + m_CameraPositionOffset,
                ref m_CameraMovementVelocity,
                m_CameraForwardVelocity
            );
            cameraContainerTransformReference.position = m_ModifiedPosition;

            if (!m_IsFollowingObject && (Vector3.Distance (m_ModifiedPosition, m_NewOriginPositionOfCamera) <= 0.1f && Vector3.Distance (cameraTransformReference.eulerAngles, m_FocusedEulerAngle) <= 0.1)) {
                
                OnCameraReachedTargetedPosition?.Invoke ();
                EnableAndDisableThisMonoBehaviour (false);
            }

            
        }

        private void RotateCamera () {
            float t_AbsoluteAngulerVelocity = m_TransformReferenceOfCameraOrigin == null ? m_CameraAngulerVelocity : m_CameraAngulerVelocity * (1 - m_AngulerVelocityThroughProgression.Evaluate (Vector3.Distance (m_TransformReferenceOfCameraOrigin.position, cameraContainerTransformReference.position) / m_MaximumDistanceFromNewCameraOrigin));
            m_ModifiedRotation = Quaternion.Slerp (
                cameraTransformReference.rotation,
                Quaternion.LookRotation (m_FocusPosition - cameraTransformReference.position),
                t_AbsoluteAngulerVelocity
            );
            cameraTransformReference.rotation = m_ModifiedRotation;
        }

        private void ZoomCamera () {

            if (m_ListOfCameraFocuses != null) {

                m_TargetedZoom = Mathf.Lerp (
                    m_MaxZoom,
                    m_MinZoom,
                    (((m_NewCameraBounds.size.x + m_NewCameraBounds.size.y + m_NewCameraBounds.size.z) / 3.0f) / m_MinZoom));

                if (m_IsCameraOrthographic) {
                    m_ModifiedZoom = Mathf.Lerp (
                        cameraReference.orthographicSize,
                        m_TargetedZoom,
                        Time.deltaTime
                    );

                    cameraReference.orthographicSize = m_ModifiedZoom;
                } else {

                    m_ModifiedZoom = Mathf.Lerp (
                        cameraReference.fieldOfView,
                        m_TargetedZoom,
                        Time.deltaTime
                    );

                    cameraReference.fieldOfView = m_ModifiedZoom;
                }
            }
        }

        private void ConfigureCamera (
            bool t_IsFollowingObject,
            List<Transform> t_ListOfCameraFocuses,
            bool t_UseOffsetFocusOnLocalSpace = false,
            Vector3 t_CameraFocusOffset = new Vector3 (),
            Vector3 t_CameraPositionOffset = new Vector3 (),
            float t_CameraFocusValue = 0,
            float t_CameraForwardVelocity = 0,
            float t_CameraAngulerVelocity = 0,
            AnimationCurve t_AngulerVelocityThroughProgression = null,
            UnityAction OnCameraReachedTargetedPosition = null,
            Transform t_CameraOriginWhileTarget = null) {
            
            m_IsFollowingObject = t_IsFollowingObject;

            m_TransformReferenceOfCameraOrigin = t_CameraOriginWhileTarget;

            m_UseOffsetFocusOnLocalSpace = t_UseOffsetFocusOnLocalSpace;

            m_NumberOfCameraFocuses = t_ListOfCameraFocuses.Count;
            m_ListOfCameraFocuses = t_ListOfCameraFocuses;

            if (t_CameraOriginWhileTarget != null)
                m_MaximumDistanceFromNewCameraOrigin = Vector3.Distance (m_TransformReferenceOfCameraOrigin.position, cameraContainerTransformReference.position);
            else
                m_MaximumDistanceFromNewCameraOrigin = 0;

            if (t_CameraFocusOffset == Vector3.zero)
                m_CameraFocusOffset = defaultCameraFocusOffset;
            else
                m_CameraFocusOffset = t_CameraFocusOffset;

            if (t_CameraPositionOffset == Vector3.zero)
                m_CameraPositionOffset = defaultCameraPositionOffset;
            else
                m_CameraPositionOffset = t_CameraPositionOffset;

            if (t_CameraFocusValue == 0)
                CalculateFOVAndOrthographicSize (defaultMaxFocusingValue);
            else {
                //Debug.Log("cameraFocusValue : " + t_CameraFocusValue);
                CalculateFOVAndOrthographicSize (t_CameraFocusValue);
            }

            if (t_CameraForwardVelocity == 0)
                m_CameraForwardVelocity = 1f - defaultForwardVelocity;
            else
                m_CameraForwardVelocity = 1f - Mathf.Clamp01 (t_CameraForwardVelocity);

            if (t_CameraAngulerVelocity == 0)
                m_CameraAngulerVelocity = defaultAngulerVelocity;
            else
                m_CameraAngulerVelocity = Mathf.SmoothStep (0f, 1f, t_CameraAngulerVelocity);

            if (t_AngulerVelocityThroughProgression == null)
                m_AngulerVelocityThroughProgression = defaultAngulerVelocityThroughProgression;
            else
                m_AngulerVelocityThroughProgression = t_AngulerVelocityThroughProgression;

            this.OnCameraReachedTargetedPosition = OnCameraReachedTargetedPosition;

            //Calculating Bound
            CalculateCameraBound ();

            Vector3 t_NewBoundSize = m_NewCameraBounds.size;
            if (t_CameraOriginWhileTarget == null) {
                t_NewBoundSize *= cameraBoundExtend;
            } else {

                float t_MaxLengthOfSize = Mathf.Max (t_NewBoundSize.x, t_NewBoundSize.z);

                t_NewBoundSize = new Vector3 (
                    t_MaxLengthOfSize,
                    t_NewBoundSize.y,
                    t_MaxLengthOfSize
                ) * cameraBoundExtend;
            }

            m_NewOriginPositionOfCamera =
                m_NewCameraBounds.center +
                new Vector3 (
                    Mathf.SmoothStep (-(t_NewBoundSize.x / 2.0f), t_NewBoundSize.x / 2.0f, cameraPositionOnX),
                    Mathf.SmoothStep (-(t_NewBoundSize.y / 2.0f), t_NewBoundSize.y / 2.0f, cameraPositionOnY),
                    Mathf.SmoothStep (-(t_NewBoundSize.z / 2.0f), t_NewBoundSize.z / 2.0f, cameraPositionOnZ)
                ) +
                defaultCameraPositionOffset;

            EnableAndDisableThisMonoBehaviour (true);
        }

        #endregion

        #region Public Callback

        public void FocusCameraWithOrigin (
            Transform t_CameraOriginWhileTarget,
            List<Transform> t_ListOfCameraFocuses,
            UnityAction OnCameraReachedTargetedPosition) {
                
            FocusCameraWithOrigin (
                t_CameraOriginWhileTarget,
                t_ListOfCameraFocuses,
                false,
                Vector3.zero,
                Vector3.zero,
                0,
                0,
                0,
                null,
                OnCameraReachedTargetedPosition
            );
        }
        public void FocusCameraWithOrigin (
            Transform t_CameraOriginWhileTarget,
            List<Transform> t_ListOfCameraFocuses,
            bool t_UseFocusOffsetOnLocalSpace = false,
            Vector3 t_CameraFocusOffset = new Vector3 (),
            Vector3 t_CameraPositionOffset = new Vector3 (),
            float t_CameraFocusValue = 0,
            float t_CameraForwardVelocity = 0,
            float t_CameraAngulerVelocity = 0,
            AnimationCurve t_AngulerVelocityThroughProgression = null,
            UnityAction OnCameraReachedTargetedPosition = null
        ) {

            ConfigureCamera (
                false,
                t_ListOfCameraFocuses,
                t_UseFocusOffsetOnLocalSpace,
                t_CameraFocusOffset,
                t_CameraPositionOffset,
                t_CameraFocusValue,
                t_CameraForwardVelocity,
                t_CameraAngulerVelocity,
                t_AngulerVelocityThroughProgression,
                OnCameraReachedTargetedPosition,
                t_CameraOriginWhileTarget
            );
        }

        public void FocusCameraAsArea (
            List<Transform> t_ListOfCameraFocuses,
            bool t_UseOffsetFocusOnLocalSpace = false,
            Vector3 t_CameraFocusOffset = new Vector3 (),
            Vector3 t_CameraPositionOffset = new Vector3 (),
            float t_CameraFocusValue = 0,
            float t_CameraForwardVelocity = 0,
            float t_CameraAngulerVelocity = 0,
            AnimationCurve t_AngulerVelocityThroughProgression = null,
            UnityAction OnCameraReachedTargetedPosition = null
        ) {

            ConfigureCamera (
                false,
                t_ListOfCameraFocuses,
                t_UseOffsetFocusOnLocalSpace,
                t_CameraFocusOffset,
                t_CameraPositionOffset,
                t_CameraFocusValue,
                t_CameraForwardVelocity,
                t_CameraAngulerVelocity,
                t_AngulerVelocityThroughProgression,
                OnCameraReachedTargetedPosition,
                null
            );
        }

        public void FollowCamera (
            Transform t_CameraOriginWhileTarget,
            List<Transform> t_ListOfCameraFocuses,
            bool t_UseFocusOffsetOnLocalSpace = false,
            Vector3 t_CameraFocusOffset = new Vector3 (),
            Vector3 t_CameraPositionOffset = new Vector3 (),
            float t_CameraFocusValue = 0,
            float t_CameraForwardVelocity = 0,
            float t_CameraAngulerVelocity = 0,
            AnimationCurve t_AngulerVelocityThroughProgression = null
        ) {

            ConfigureCamera (
                true,
                t_ListOfCameraFocuses,
                t_UseFocusOffsetOnLocalSpace,
                t_CameraFocusOffset,
                t_CameraPositionOffset,
                t_CameraFocusValue,
                t_CameraForwardVelocity,
                t_CameraAngulerVelocity,
                t_AngulerVelocityThroughProgression,
                OnCameraReachedTargetedPosition,
                t_CameraOriginWhileTarget
            );
        }

        public void UnfollowCamera(){

            m_IsFollowingObject = false;
        }     

        #endregion
    }
}