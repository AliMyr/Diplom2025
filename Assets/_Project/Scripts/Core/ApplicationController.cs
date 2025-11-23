using UnityEngine;
using ProcessSimulation.Algorithms;
using ProcessSimulation.Visualization;
using System.Collections.Generic;

namespace ProcessSimulation.Core
{
    /// <summary>
    /// Главный контроллер приложения
    /// </summary>
    public class ApplicationController : MonoBehaviour
    {
        [Header("Data Generation")]
        [SerializeField] private int arraySize = 20;
        [SerializeField] private int minValue = 1;
        [SerializeField] private int maxValue = 100;
        [SerializeField] private bool randomizeOnStart = true;

        [Header("Algorithm Selection")]
        [SerializeField] private AlgorithmType initialAlgorithm = AlgorithmType.BubbleSort;

        private int[] currentArray;
        private List<BarVisualization> currentBars;
        private Processes.SortingAlgorithm currentAlgorithm;

        public enum AlgorithmType
        {
            BubbleSort,
            QuickSort,
            InsertionSort,
            SelectionSort
        }

        public static ApplicationController Instance { get; private set; }

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
        }

        private void Start()
        {
            if (randomizeOnStart)
            {
                GenerateRandomArray();
                CreateVisualization();
                LoadAlgorithm(initialAlgorithm);
            }
        }

        /// <summary>
        /// Генерация случайного массива
        /// </summary>
        public void GenerateRandomArray()
        {
            currentArray = new int[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                currentArray[i] = Random.Range(minValue, maxValue + 1);
            }

            Debug.Log($"Generated array: [{string.Join(", ", currentArray)}]");
        }

        /// <summary>
        /// Генерация почти отсортированного массива
        /// </summary>
        public void GenerateNearlySortedArray()
        {
            currentArray = new int[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                currentArray[i] = minValue + i * (maxValue - minValue) / arraySize;
            }

            // Делаем несколько случайных обменов
            int swaps = arraySize / 5;
            for (int i = 0; i < swaps; i++)
            {
                int idx1 = Random.Range(0, arraySize);
                int idx2 = Random.Range(0, arraySize);

                int temp = currentArray[idx1];
                currentArray[idx1] = currentArray[idx2];
                currentArray[idx2] = temp;
            }
        }

        /// <summary>
        /// Генерация обратно отсортированного массива
        /// </summary>
        public void GenerateReverseSortedArray()
        {
            currentArray = new int[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                currentArray[i] = maxValue - i * (maxValue - minValue) / arraySize;
            }
        }

        /// <summary>
        /// Создание визуализации
        /// </summary>
        public void CreateVisualization()
        {
            if (VisualizationController.Instance != null && currentArray != null)
            {
                currentBars = VisualizationController.Instance.CreateBars(currentArray);
            }
        }

        /// <summary>
        /// Загрузка алгоритма
        /// </summary>
        public void LoadAlgorithm(AlgorithmType algorithmType)
        {
            // Создаём экземпляр алгоритма
            currentAlgorithm = CreateAlgorithmInstance(algorithmType);

            if (currentAlgorithm != null && currentArray != null && currentBars != null)
            {
                // Устанавливаем данные
                currentAlgorithm.SetData(currentArray, currentBars);

                // Загружаем в SimulationManager
                if (SimulationManager.Instance != null)
                {
                    SimulationManager.Instance.LoadProcess(currentAlgorithm);
                }

                // Обновляем UI
                if (UI.UIManager.Instance != null)
                {
                    UI.UIManager.Instance.SetAlgorithmInfo(
                        currentAlgorithm.ProcessName,
                        currentAlgorithm.Description
                    );
                }

                Debug.Log($"Loaded algorithm: {currentAlgorithm.ProcessName}");
            }
        }

        /// <summary>
        /// Создание экземпляра алгоритма
        /// </summary>
        private Processes.SortingAlgorithm CreateAlgorithmInstance(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.BubbleSort => new BubbleSort(),
                AlgorithmType.QuickSort => new QuickSort(),
                AlgorithmType.InsertionSort => new InsertionSort(),
                AlgorithmType.SelectionSort => new SelectionSort(),
                _ => new BubbleSort()
            };
        }

        /// <summary>
        /// Установка размера массива
        /// </summary>
        public void SetArraySize(int size)
        {
            arraySize = Mathf.Clamp(size, 5, 100);
        }

        /// <summary>
        /// Получение текущего размера массива
        /// </summary>
        public int GetArraySize()
        {
            return arraySize;
        }

        /// <summary>
        /// Перезагрузка с новыми данными
        /// </summary>
        public void Reload()
        {
            // Останавливаем симуляцию
            if (SimulationManager.Instance != null)
            {
                SimulationManager.Instance.Stop();
            }

            // Генерируем новый массив
            GenerateRandomArray();

            // Пересоздаём визуализацию
            CreateVisualization();

            // Перезагружаем алгоритм
            if (currentAlgorithm != null)
            {
                currentAlgorithm.SetData(currentArray, currentBars);
                currentAlgorithm.Initialize();
            }
        }

        private void Update()
        {
            // Обновляем статистику UI
            if (currentAlgorithm != null && UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.UpdateStatistics(
                    currentAlgorithm.Comparisons,
                    currentAlgorithm.Swaps
                );
            }

            // Горячие клавиши
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (SimulationManager.Instance != null)
                {
                    if (SimulationManager.Instance.CurrentState == SimulationState.Running)
                    {
                        SimulationManager.Instance.Pause();
                    }
                    else
                    {
                        SimulationManager.Instance.Play();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                Reload();
            }
        }
    }
}
