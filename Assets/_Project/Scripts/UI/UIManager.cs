using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProcessSimulation.Core;
using System;

namespace ProcessSimulation.UI
{
    /// <summary>
    /// Главный менеджер пользовательского интерфейса
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Control Panels")]
        [SerializeField] private GameObject controlPanel;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private GameObject statisticsPanel;

        [Header("Control Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button stepButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button speedUpButton;
        [SerializeField] private Button speedDownButton;

        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI elapsedTimeText;
        [SerializeField] private TextMeshProUGUI algorithmNameText;
        [SerializeField] private TextMeshProUGUI algorithmDescriptionText;

        [Header("Statistics")]
        [SerializeField] private TextMeshProUGUI comparisonsText;
        [SerializeField] private TextMeshProUGUI swapsText;
        [SerializeField] private TextMeshProUGUI currentStepText;

        [Header("Progress Bar")]
        [SerializeField] private Slider progressSlider;

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeButtons();
        }

        private void Start()
        {
            // Подписываемся на события SimulationManager
            if (SimulationManager.Instance != null)
            {
                SimulationManager.Instance.OnStateChanged += OnSimulationStateChanged;
                SimulationManager.Instance.OnProcessStep += OnProcessStep;
                SimulationManager.Instance.OnSimulationCompleted += OnSimulationCompleted;
                SimulationManager.Instance.OnSimulationReset += OnSimulationReset;
            }

            UpdateUI();
        }

        private void InitializeButtons()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);

            if (stepButton != null)
                stepButton.onClick.AddListener(OnStepClicked);

            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetClicked);

            if (speedUpButton != null)
                speedUpButton.onClick.AddListener(OnSpeedUpClicked);

            if (speedDownButton != null)
                speedDownButton.onClick.AddListener(OnSpeedDownClicked);
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (SimulationManager.Instance == null)
                return;

            // Обновляем состояние
            if (stateText != null)
            {
                stateText.text = $"Состояние: {GetStateText(SimulationManager.Instance.CurrentState)}";
            }

            // Обновляем скорость
            if (speedText != null)
            {
                speedText.text = $"Скорость: x{SimulationManager.Instance.GetSpeedMultiplier():F2}";
            }

            // Обновляем прогресс
            float progress = SimulationManager.Instance.Progress;
            if (progressText != null)
            {
                progressText.text = $"Прогресс: {progress * 100:F1}%";
            }

            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            // Обновляем время
            if (elapsedTimeText != null)
            {
                elapsedTimeText.text = $"Время: {SimulationManager.Instance.ElapsedTime:F2}с";
            }

            // Обновляем шаги
            if (currentStepText != null)
            {
                currentStepText.text = $"Шаг: {SimulationManager.Instance.CurrentStep}";
            }

            // Обновляем доступность кнопок
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            var state = SimulationManager.Instance.CurrentState;

            if (playButton != null)
            {
                playButton.interactable = state == SimulationState.Idle ||
                                          state == SimulationState.Paused ||
                                          state == SimulationState.StepByStep;
            }

            if (pauseButton != null)
            {
                pauseButton.interactable = state == SimulationState.Running;
            }

            if (stepButton != null)
            {
                stepButton.interactable = state != SimulationState.Running &&
                                          state != SimulationState.Completed;
            }

            if (resetButton != null)
            {
                resetButton.interactable = state != SimulationState.Idle;
            }
        }

        public void SetAlgorithmInfo(string name, string description)
        {
            if (algorithmNameText != null)
            {
                algorithmNameText.text = name;
            }

            if (algorithmDescriptionText != null)
            {
                algorithmDescriptionText.text = description;
            }
        }

        public void UpdateStatistics(int comparisons, int swaps)
        {
            if (comparisonsText != null)
            {
                comparisonsText.text = $"Сравнений: {comparisons}";
            }

            if (swapsText != null)
            {
                swapsText.text = $"Обменов: {swaps}";
            }
        }

        private string GetStateText(SimulationState state)
        {
            return state switch
            {
                SimulationState.Idle => "Ожидание",
                SimulationState.Running => "Выполнение",
                SimulationState.Paused => "Пауза",
                SimulationState.StepByStep => "Пошаговый режим",
                SimulationState.Completed => "Завершено",
                SimulationState.Error => "Ошибка",
                _ => "Неизвестно"
            };
        }

        // Обработчики кнопок
        private void OnPlayClicked()
        {
            SimulationManager.Instance?.Play();
        }

        private void OnPauseClicked()
        {
            SimulationManager.Instance?.Pause();
        }

        private void OnStepClicked()
        {
            SimulationManager.Instance?.StepForward();
        }

        private void OnResetClicked()
        {
            SimulationManager.Instance?.ResetSimulation();
        }

        private void OnSpeedUpClicked()
        {
            SimulationManager.Instance?.IncreaseSpeed();
        }

        private void OnSpeedDownClicked()
        {
            SimulationManager.Instance?.DecreaseSpeed();
        }

        // Обработчики событий симуляции
        private void OnSimulationStateChanged(object sender, SimulationStateChangedEventArgs e)
        {
            Debug.Log($"State changed: {e.PreviousState} -> {e.CurrentState}");
        }

        private void OnProcessStep(object sender, ProcessStepEventArgs e)
        {
            // Обновляется в Update()
        }

        private void OnSimulationCompleted(object sender, EventArgs e)
        {
            Debug.Log("Simulation completed!");
        }

        private void OnSimulationReset(object sender, EventArgs e)
        {
            Debug.Log("Simulation reset!");
        }

        private void OnDestroy()
        {
            if (SimulationManager.Instance != null)
            {
                SimulationManager.Instance.OnStateChanged -= OnSimulationStateChanged;
                SimulationManager.Instance.OnProcessStep -= OnProcessStep;
                SimulationManager.Instance.OnSimulationCompleted -= OnSimulationCompleted;
                SimulationManager.Instance.OnSimulationReset -= OnSimulationReset;
            }
        }
    }
}
