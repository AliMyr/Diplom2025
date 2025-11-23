using System;
using System.Collections;
using UnityEngine;

namespace ProcessSimulation.Core
{
    /// <summary>
    /// Главный менеджер симуляции процессов
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [SerializeField] private SimulationSpeed defaultSpeed = SimulationSpeed.Normal;
        [SerializeField] private bool autoResetOnComplete = false;

        // Текущее состояние
        private SimulationState currentState = SimulationState.Idle;
        private SimulationSpeed currentSpeed;
        private IProcess currentProcess;
        private Coroutine simulationCoroutine;

        // Статистика
        private int totalSteps;
        private int currentStep;
        private float startTime;
        private float elapsedTime;

        // События
        public event EventHandler<SimulationStateChangedEventArgs> OnStateChanged;
        public event EventHandler<ProcessStepEventArgs> OnProcessStep;
        public event EventHandler OnSimulationCompleted;
        public event EventHandler OnSimulationReset;

        // Singleton
        public static SimulationManager Instance { get; private set; }

        // Свойства
        public SimulationState CurrentState => currentState;
        public SimulationSpeed CurrentSpeed => currentSpeed;
        public IProcess CurrentProcess => currentProcess;
        public float Progress => currentProcess?.GetProgress() ?? 0f;
        public float ElapsedTime => elapsedTime;
        public int CurrentStep => currentStep;
        public int TotalSteps => totalSteps;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            currentSpeed = defaultSpeed;
        }

        /// <summary>
        /// Загрузка нового процесса
        /// </summary>
        public void LoadProcess(IProcess process)
        {
            if (currentState == SimulationState.Running)
            {
                Stop();
            }

            currentProcess = process;

            if (currentProcess != null)
            {
                currentProcess.Initialize();
                currentProcess.OnStepCompleted += OnProcessStepCompleted;
                ChangeState(SimulationState.Idle);
                OnSimulationReset?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Запуск симуляции
        /// </summary>
        public void Play()
        {
            if (currentProcess == null)
            {
                Debug.LogWarning("No process loaded!");
                return;
            }

            if (currentState == SimulationState.Running)
                return;

            if (currentState == SimulationState.Completed)
            {
                ResetSimulation();
            }

            ChangeState(SimulationState.Running);
            startTime = Time.time;

            if (simulationCoroutine != null)
            {
                StopCoroutine(simulationCoroutine);
            }

            simulationCoroutine = StartCoroutine(RunSimulation());
        }

        /// <summary>
        /// Пауза симуляции
        /// </summary>
        public void Pause()
        {
            if (currentState == SimulationState.Running)
            {
                ChangeState(SimulationState.Paused);

                if (simulationCoroutine != null)
                {
                    StopCoroutine(simulationCoroutine);
                    simulationCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Остановка симуляции
        /// </summary>
        public void Stop()
        {
            if (simulationCoroutine != null)
            {
                StopCoroutine(simulationCoroutine);
                simulationCoroutine = null;
            }

            ResetSimulation();
        }

        /// <summary>
        /// Выполнение одного шага
        /// </summary>
        public void StepForward()
        {
            if (currentProcess == null)
                return;

            if (currentState == SimulationState.Completed)
                return;

            if (currentState != SimulationState.StepByStep)
            {
                ChangeState(SimulationState.StepByStep);
            }

            bool canContinue = currentProcess.Step();
            currentStep++;

            if (!canContinue)
            {
                CompleteSimulation();
            }
        }

        /// <summary>
        /// Сброс симуляции
        /// </summary>
        public void ResetSimulation()
        {
            if (currentProcess != null)
            {
                currentProcess.Reset();
            }

            currentStep = 0;
            elapsedTime = 0f;
            ChangeState(SimulationState.Idle);
            OnSimulationReset?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Установка скорости симуляции
        /// </summary>
        public void SetSpeed(SimulationSpeed speed)
        {
            currentSpeed = speed;
        }

        /// <summary>
        /// Увеличение скорости
        /// </summary>
        public void IncreaseSpeed()
        {
            if (currentSpeed < SimulationSpeed.VeryFast)
            {
                currentSpeed++;
            }
        }

        /// <summary>
        /// Уменьшение скорости
        /// </summary>
        public void DecreaseSpeed()
        {
            if (currentSpeed > SimulationSpeed.VerySlow)
            {
                currentSpeed--;
            }
        }

        /// <summary>
        /// Получение множителя скорости
        /// </summary>
        public float GetSpeedMultiplier()
        {
            return currentSpeed switch
            {
                SimulationSpeed.VerySlow => 0.25f,
                SimulationSpeed.Slow => 0.5f,
                SimulationSpeed.Normal => 1.0f,
                SimulationSpeed.Fast => 2.0f,
                SimulationSpeed.VeryFast => 4.0f,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Корутина выполнения симуляции
        /// </summary>
        private IEnumerator RunSimulation()
        {
            while (currentState == SimulationState.Running)
            {
                bool canContinue = currentProcess.Step();
                currentStep++;

                if (!canContinue)
                {
                    CompleteSimulation();
                    yield break;
                }

                // Задержка зависит от скорости
                float delay = 0.5f / GetSpeedMultiplier();
                yield return new WaitForSeconds(delay);
            }
        }

        /// <summary>
        /// Завершение симуляции
        /// </summary>
        private void CompleteSimulation()
        {
            elapsedTime = Time.time - startTime;
            ChangeState(SimulationState.Completed);
            OnSimulationCompleted?.Invoke(this, EventArgs.Empty);

            if (autoResetOnComplete)
            {
                ResetSimulation();
            }
        }

        /// <summary>
        /// Изменение состояния
        /// </summary>
        private void ChangeState(SimulationState newState, string reason = "")
        {
            if (currentState == newState)
                return;

            var previousState = currentState;
            currentState = newState;

            OnStateChanged?.Invoke(this, new SimulationStateChangedEventArgs(previousState, currentState, reason));
        }

        /// <summary>
        /// Обработчик шага процесса
        /// </summary>
        private void OnProcessStepCompleted(object sender, ProcessStepEventArgs e)
        {
            OnProcessStep?.Invoke(sender, e);
        }

        private void Update()
        {
            if (currentState == SimulationState.Running)
            {
                elapsedTime = Time.time - startTime;
            }
        }

        private void OnDestroy()
        {
            if (currentProcess != null)
            {
                currentProcess.OnStepCompleted -= OnProcessStepCompleted;
            }
        }
    }
}
