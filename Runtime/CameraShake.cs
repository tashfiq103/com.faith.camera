namespace com.faithstudio.Camera {

    using UnityEngine;

    public class CameraShake : MonoBehaviour
    {
        #region Public Variables

        public Transform cameraContainerTransformReference;

        [Space(5.0f)]
        [Range(0f, 100f)]
        public float cameraShakeSpeed = 1f;
        [Range(0f, 10f)]
        public float defaultDurationOfCameraShake = 2.5f;

        [Space(5.0f)]
        [Header("Shaking Points")]
        [Range(0f, 10f)]
        public float cameraShakeOnXAxis = 1f;
        [Range(0f, 10f)]
        public float cameraShakeOnYAxis = 1f;
        [Range(0f, 10f)]
        public float cameraShakeOnZAxis = 1f;

        [Space(5.0f)]
        [Header("Rotation Points")]
        [Range(0, 180f)]
        public float cameraRotateOnXAxis = 0f;
        [Range(0, 180f)]
        public float cameraRotateOnYAxis = 0f;
        [Range(0, 180f)]
        public float cameraRotateOnZAxis = 0f;


        #endregion

        #region Private Variables

        private Vector3 m_VarientInitialPositionOfCameraContainer;
        private Vector3 m_VarientInitialEulerAngleOfCameraContainer;
        private Vector3 m_InitialPositionOfCameraContainer;
        private Vector3 m_InitialLocalEulerAngleIfCameraContainer;

        private bool m_IsCameraShakeControllerRunning;
        private bool m_IsRepetativeShakeEnabled;

        private float m_AbsoluteCameraShakeDuration;
        private float m_RemainingTimeForCameraShake;
        private float m_AbsluteCameraShakingSpeed;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Initialization();
        }

        private void Start()
        {
        }

        private void Update() {

            float t_DetalTime = Time.deltaTime;
            if (m_RemainingTimeForCameraShake > 0)
            {
                Vector3 m_ModifiedPosition = Vector3.Lerp(
                        m_VarientInitialPositionOfCameraContainer,
                        m_VarientInitialPositionOfCameraContainer + new Vector3(
                            Random.Range(-cameraShakeOnXAxis, cameraShakeOnXAxis),
                            Random.Range(-cameraShakeOnYAxis, cameraShakeOnYAxis),
                            Random.Range(-cameraShakeOnZAxis, cameraShakeOnZAxis)
                        ),
                        m_AbsluteCameraShakingSpeed * t_DetalTime
                    );
                cameraContainerTransformReference.localPosition = m_ModifiedPosition;

                Vector3 m_ModifiedEulerAngle = Vector3.Lerp(
                    m_VarientInitialEulerAngleOfCameraContainer,
                    m_VarientInitialEulerAngleOfCameraContainer + new Vector3(
                            Random.Range(-cameraRotateOnXAxis, cameraRotateOnXAxis),
                            Random.Range(-cameraRotateOnYAxis, cameraRotateOnYAxis),
                            Random.Range(-cameraRotateOnZAxis, cameraRotateOnZAxis)
                        ),
                        m_AbsluteCameraShakingSpeed * t_DetalTime
                );
                cameraContainerTransformReference.localEulerAngles = m_ModifiedEulerAngle;

                if (!m_IsRepetativeShakeEnabled)
                {
                    m_RemainingTimeForCameraShake -= t_DetalTime;
                }
            }
            else if (
                Vector3.Distance(cameraContainerTransformReference.localPosition, m_InitialPositionOfCameraContainer) >= 0.1f) {

                Vector3 m_ModifiedPosition = Vector3.Lerp(
                        cameraContainerTransformReference.localPosition,
                        m_InitialPositionOfCameraContainer,
                        m_AbsluteCameraShakingSpeed * t_DetalTime
                    );
                cameraContainerTransformReference.localPosition = m_ModifiedPosition;

                Vector3 m_ModifiedEulerAngle = Vector3.Lerp(
                        cameraContainerTransformReference.localEulerAngles,
                        m_InitialLocalEulerAngleIfCameraContainer,
                        m_AbsluteCameraShakingSpeed * t_DetalTime
                    );
                cameraContainerTransformReference.localEulerAngles = m_ModifiedEulerAngle;
            }
            else
            {
                cameraContainerTransformReference.localPosition= m_InitialPositionOfCameraContainer;
                cameraContainerTransformReference.localEulerAngles = m_InitialLocalEulerAngleIfCameraContainer;
                m_IsCameraShakeControllerRunning = false;
                enabled = false;
            }
        }

        #endregion

        #region Configuretion

        private void Initialization()
        {

            enabled = false;

            if (cameraContainerTransformReference == null)
                cameraContainerTransformReference = transform;

            m_InitialPositionOfCameraContainer = cameraContainerTransformReference.localPosition;
            m_InitialLocalEulerAngleIfCameraContainer = cameraContainerTransformReference.localEulerAngles;

        }

       

        #endregion

        #region Public Callback

        public void ShowCameraShake()
        {

            ShowCameraShake(defaultDurationOfCameraShake, cameraShakeSpeed, false);
        }

        public void ShowCameraShake(float t_Duration, bool t_RepetativeShake)
        {

            ShowCameraShake(t_Duration, cameraShakeSpeed, t_RepetativeShake);
        }

        public void ShowCameraShake(float t_Duration, float t_CameraShakeSpeed, bool t_RepetativeShake)
        {

            if (!m_IsCameraShakeControllerRunning)
            {
                m_AbsluteCameraShakingSpeed = t_CameraShakeSpeed;
                m_AbsoluteCameraShakeDuration = t_Duration;
                m_RemainingTimeForCameraShake = t_Duration;
                m_IsRepetativeShakeEnabled = t_RepetativeShake;

                m_VarientInitialPositionOfCameraContainer = m_InitialPositionOfCameraContainer;
                m_VarientInitialEulerAngleOfCameraContainer = m_InitialLocalEulerAngleIfCameraContainer;

                m_IsCameraShakeControllerRunning = true;
                enabled = true;
            }
            else
            {

                Debug.LogWarning("CameraShake Controller already running");
            }
        }

        public void StopCameraShake()
        {

            m_RemainingTimeForCameraShake = 0f;
        }

        public void ChangeShakingValue(float t_ShakeSpeed)
        {
            m_AbsluteCameraShakingSpeed = t_ShakeSpeed;
        }

        #endregion
    }
}
