namespace com.faith.camera {

    using System.Collections.Generic;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class FaithCameraController : MonoBehaviour {

        #region Public Variables

#if UNITY_EDITOR

        public bool showDefaultCameraSettings;

#endif

        public static FaithCameraController Instance;

        public Camera cameraReference;
        public Transform cameraContainerTransformReference;

        public CameraSettings cameraDefaultSettings;

        #endregion  

        #region Private Variables

        private CameraSettings  m_CurrentCameraSettings;
        private Vector3         m_ModifiedForwardVelocity;
        private bool            m_IsOrthographicCamera = false;
        private float           m_DeltaTime;


        private Transform       m_CameraTransformReference;

        //Configuretion :   Transation
        private Transform       m_CameraOriginPosition;
        private float           m_MaximumDistanceFromNewCameraOrigin;

        //Configuretion :   Focus Parameter
        private Bounds          m_NewCameraBounds;
        private int             m_NumberOfCameraFocuses;
        private List<Transform> m_CameraFocuses;
        private Vector3         m_FocusPosition;
        private Vector3         m_FocusedEulerAngle;


        //Configuretion :   Event
        private UnityAction OnCameraReachedTargetedPosition;

        #endregion

        #region Mono Behaviour

        private void Awake () {

            Instance = this;

            m_CameraTransformReference  = cameraReference.transform;
            m_IsOrthographicCamera      = cameraReference.orthographic;

            EnableAndDisableThisMonoBehaviour(false);
        }

        private void LateUpdate()
        {
            CalculateCameraBound();
            MoveCamera();
            RotateCamera();
            CameraZoom();
        }

        #endregion

        #region Configuretion

        private void EnableAndDisableThisMonoBehaviour(bool t_IsEnable)
        {

            enabled = t_IsEnable;
        }

        private void CalculateCameraBound()
        {

            m_NewCameraBounds = new Bounds(m_CameraFocuses[0].position, Vector3.zero);
            for (int i = 0; i < m_NumberOfCameraFocuses; i++)
            {
                m_NewCameraBounds.Encapsulate(m_CameraFocuses[i].position);
            }

            Vector3 t_TargetPosition = m_NewCameraBounds.center;
            Vector3 t_AbsoluteCameraFocus = t_TargetPosition;

            if (m_CurrentCameraSettings.isUseFocusOffsetOnLocalSpace)
            {
                Vector3 t_OriginPosition = m_CameraTransformReference.position;
                t_OriginPosition.y = t_TargetPosition.y;

                Vector3 t_NormalizedDirection = Vector3.Normalize(t_TargetPosition - t_OriginPosition);
                t_AbsoluteCameraFocus = t_TargetPosition;
                t_AbsoluteCameraFocus += (t_NormalizedDirection * m_CurrentCameraSettings.cameraFocusOffset.z);
                t_AbsoluteCameraFocus += new Vector3(
                    t_NormalizedDirection.z,
                    0,
                    t_NormalizedDirection.x
                ) * m_CurrentCameraSettings.cameraFocusOffset.x;
                t_AbsoluteCameraFocus += Vector3.up * m_CurrentCameraSettings.cameraFocusOffset.y;
            }
            else
            {

                t_AbsoluteCameraFocus += m_CurrentCameraSettings.cameraFocusOffset;
            }

            m_FocusPosition = new Vector3(
                m_CurrentCameraSettings.focusConstraint.x ? m_CurrentCameraSettings.cameraFocusOffset.x : t_AbsoluteCameraFocus.x,
                m_CurrentCameraSettings.focusConstraint.y ? m_CurrentCameraSettings.cameraFocusOffset.y : t_AbsoluteCameraFocus.y,
                m_CurrentCameraSettings.focusConstraint.z ? m_CurrentCameraSettings.cameraFocusOffset.z : t_AbsoluteCameraFocus.z
            );
            m_FocusedEulerAngle = Quaternion.LookRotation(m_FocusPosition - m_CameraTransformReference.position).eulerAngles;

        }

        private void MoveCamera()
        {

            Vector3 t_CurrentPositionOfTheCamera = cameraContainerTransformReference.position;
            Vector3 t_CameraOrigin               = m_CameraOriginPosition == null ? m_FocusPosition : m_CameraOriginPosition.position;
            t_CameraOrigin                      += m_CurrentCameraSettings.cameraPositionOffset;

            float t_ForwardVelocityAsSmoothTime = m_CameraOriginPosition == null
                ? m_CurrentCameraSettings.forwardVelocity
                : Mathf.Clamp(0.0167f, 1, m_CurrentCameraSettings.forwardVelocity * (1 - m_CurrentCameraSettings.curveForForwardVelocity.Evaluate(Vector3.Distance(m_CameraOriginPosition.position, cameraContainerTransformReference.position) / m_MaximumDistanceFromNewCameraOrigin))); ;


            Vector3 t_ModifiedPosition = Vector3.SmoothDamp(
                t_CurrentPositionOfTheCamera,
                t_CameraOrigin,
                ref m_ModifiedForwardVelocity,
                t_ForwardVelocityAsSmoothTime
            );
            cameraContainerTransformReference.position = t_ModifiedPosition;

            if (Vector3.Distance(t_ModifiedPosition, t_CameraOrigin) <= 0.1f && Vector3.Distance(m_CameraTransformReference.eulerAngles, m_FocusedEulerAngle) <= 0.1) {

                switch (m_CurrentCameraSettings.focusType) {

                    case FocusType.Once:

                        OnCameraReachedTargetedPosition?.Invoke();
                        EnableAndDisableThisMonoBehaviour(false);
                        break;
                    case FocusType.Follow:

                        break;
                }
            }
        }

        private void RotateCamera()
        {
            float t_AbsoluteAngulerVelocity = m_CameraOriginPosition == null
                ? m_CurrentCameraSettings.angulerVelocity
                : Mathf.Clamp(0.0167f,1,m_CurrentCameraSettings.angulerVelocity * (1 - m_CurrentCameraSettings.curveForAngulerVelocity.Evaluate(Vector3.Distance(m_CameraOriginPosition.position, cameraContainerTransformReference.position) / m_MaximumDistanceFromNewCameraOrigin)));

            Quaternion t_ModifiedRotation = Quaternion.Slerp(
                m_CameraTransformReference.rotation,
                Quaternion.LookRotation(m_FocusPosition - m_CameraTransformReference.position),
                t_AbsoluteAngulerVelocity
            );

            m_CameraTransformReference.rotation = t_ModifiedRotation;
        }

        private void CameraZoom() {

            if (m_IsOrthographicCamera)
            {

                float t_RateOfChange = m_CameraOriginPosition == null
                    ? m_CurrentCameraSettings.rateOfChangeOnOrthographicSize
                    : Mathf.Clamp(0.0167f, 1, m_CurrentCameraSettings.rateOfChangeOnOrthographicSize * (1 - m_CurrentCameraSettings.curveForRateOfChangeOnOrthographicSize.Evaluate(Vector3.Distance(m_CameraOriginPosition.position, cameraContainerTransformReference.position) / m_MaximumDistanceFromNewCameraOrigin)));

                float t_ModifiedFOV = Mathf.Lerp(
                        cameraReference.orthographicSize,
                        m_CurrentCameraSettings.cameraOrthographicSize,
                        t_RateOfChange * Time.deltaTime
                    );

                cameraReference.orthographicSize = t_ModifiedFOV;
            }
            else {

                float t_RateOfChange = m_CameraOriginPosition == null
                    ? m_CurrentCameraSettings.rateOfChangeOnFOV
                    : Mathf.Clamp(0.0167f, 1, m_CurrentCameraSettings.rateOfChangeOnFOV * (1 - m_CurrentCameraSettings.curveForRateOfChangeOnFOV.Evaluate(Vector3.Distance(m_CameraOriginPosition.position, cameraContainerTransformReference.position) / m_MaximumDistanceFromNewCameraOrigin)));

                float t_ModifiedFOV = Mathf.Lerp(
                        cameraReference.fieldOfView,
                        m_CurrentCameraSettings.cameraFOV,
                        t_RateOfChange * Time.deltaTime
                    );

                cameraReference.fieldOfView = t_ModifiedFOV;
            }
        }

        private void ConfigureCamera(
            List<Transform> t_CameraFocuses,
            Transform t_CameraOriginPosition = null,
            CameraSettings t_CameraSettings = null,
            UnityAction t_OnCameraReachedTargetedPosition = null) {

            m_CameraFocuses = t_CameraFocuses;
            m_CameraOriginPosition = t_CameraOriginPosition;

            if (t_CameraSettings == null)
            {
                m_CurrentCameraSettings = cameraDefaultSettings;
            }
            else {

                m_CurrentCameraSettings = t_CameraSettings;
            }

            OnCameraReachedTargetedPosition = t_OnCameraReachedTargetedPosition;
            
            Vector3 t_TargetedPosition = Vector3.zero;
            if(t_CameraOriginPosition == null) t_TargetedPosition = new Bounds(t_CameraFocuses[0].position, Vector3.zero).center;
            else t_TargetedPosition = t_CameraOriginPosition.position;
            
            m_MaximumDistanceFromNewCameraOrigin = Vector3.Distance(
                cameraContainerTransformReference.position,
                t_TargetedPosition
            );

            EnableAndDisableThisMonoBehaviour(true);
        }

        #endregion

        #region Public Callback

        public void FocusCamera(
            List<Transform> t_CameraFocuses,
            Transform t_CameraOriginPosition = null,
            CameraSettings t_CameraSettings = null,
            UnityAction t_OnCameraReachedTargetedPosition = null) {

            ConfigureCamera(t_CameraFocuses, t_CameraOriginPosition, t_CameraSettings, t_OnCameraReachedTargetedPosition);
        }

        #endregion
    }

}