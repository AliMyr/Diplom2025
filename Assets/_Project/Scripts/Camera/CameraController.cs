using UnityEngine;

namespace ProcessSimulation.Camera
{
    /// <summary>
    /// Контроллер камеры для 3D визуализации
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float smoothTime = 0.1f;

        [Header("Rotation Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float minVerticalAngle = -80f;
        [SerializeField] private float maxVerticalAngle = 80f;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 50f;

        [Header("Focus Settings")]
        [SerializeField] private Transform focusTarget;
        [SerializeField] private float focusDistance = 15f;
        [SerializeField] private float focusHeight = 10f;
        [SerializeField] private float focusAngle = 45f;

        private Vector3 currentVelocity;
        private float verticalRotation;
        private float horizontalRotation;
        private bool isRotating;
        private Vector3 targetPosition;

        public static CameraController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // Инициализация начальных углов
            Vector3 angles = transform.eulerAngles;
            verticalRotation = angles.x;
            horizontalRotation = angles.y;

            targetPosition = transform.position;
        }

        private void Update()
        {
            HandleInput();
            SmoothMove();
        }

        private void HandleInput()
        {
            // Вращение камеры (правая кнопка мыши)
            if (Input.GetMouseButtonDown(1))
            {
                isRotating = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetMouseButtonUp(1))
            {
                isRotating = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (isRotating)
            {
                HandleRotation();
            }

            // Движение камеры (WASD)
            HandleMovement();

            // Зум (колесо мыши)
            HandleZoom();

            // Фокус на цели (F)
            if (Input.GetKeyDown(KeyCode.F))
            {
                FocusOnTarget();
            }

            // Сброс положения (R)
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }
        }

        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            horizontalRotation += mouseX;
            verticalRotation -= mouseY;

            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float upDown = 0f;

            if (Input.GetKey(KeyCode.E))
                upDown = 1f;
            else if (Input.GetKey(KeyCode.Q))
                upDown = -1f;

            Vector3 direction = transform.right * horizontal +
                              transform.forward * vertical +
                              Vector3.up * upDown;

            direction.Normalize();

            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed *= sprintMultiplier;
            }

            if (direction.magnitude > 0.1f)
            {
                targetPosition += direction * speed * Time.deltaTime;
            }
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0f)
            {
                Vector3 zoomDirection = transform.forward;
                targetPosition += zoomDirection * scroll * zoomSpeed;
            }
        }

        private void SmoothMove()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                smoothTime
            );
        }

        public void FocusOnTarget()
        {
            if (focusTarget != null)
            {
                FocusOnPosition(focusTarget.position);
            }
            else
            {
                FocusOnPosition(Vector3.zero);
            }
        }

        public void FocusOnPosition(Vector3 position)
        {
            // Устанавливаем камеру на заданном расстоянии и высоте от цели
            Vector3 direction = new Vector3(0, 0, -1);
            Quaternion rotation = Quaternion.Euler(focusAngle, 0, 0);
            Vector3 offset = rotation * direction * focusDistance;

            targetPosition = position + offset + Vector3.up * focusHeight;
            transform.position = targetPosition;

            // Поворачиваем камеру на цель
            transform.LookAt(position);

            // Обновляем углы
            Vector3 angles = transform.eulerAngles;
            verticalRotation = angles.x;
            horizontalRotation = angles.y;
        }

        public void SetTarget(Transform target)
        {
            focusTarget = target;
        }

        public void ResetCamera()
        {
            targetPosition = new Vector3(0, focusHeight, -focusDistance);
            transform.position = targetPosition;

            verticalRotation = focusAngle;
            horizontalRotation = 0f;

            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }

        public void SetPosition(Vector3 position)
        {
            targetPosition = position;
            transform.position = position;
        }

        public void SetRotation(Vector3 eulerAngles)
        {
            verticalRotation = eulerAngles.x;
            horizontalRotation = eulerAngles.y;
            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }
    }
}
