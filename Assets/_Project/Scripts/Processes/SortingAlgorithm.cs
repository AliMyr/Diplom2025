using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcessSimulation.Core;
using ProcessSimulation.Visualization;

namespace ProcessSimulation.Processes
{
    /// <summary>
    /// Базовый класс для алгоритмов сортировки
    /// </summary>
    public abstract class SortingAlgorithm : IProcess
    {
        protected int[] array;
        protected List<BarVisualization> visualElements;
        protected int currentStep;
        protected int totalOperations;
        protected bool isCompleted;

        // Статистика
        protected int comparisons;
        protected int swaps;
        protected int arrayAccesses;

        public abstract string ProcessName { get; }
        public abstract string Description { get; }
        public abstract string TimeComplexity { get; }
        public abstract string SpaceComplexity { get; }
        public virtual ProcessCategory Category => ProcessCategory.Sorting;

        public event EventHandler<ProcessStepEventArgs> OnStepCompleted;

        public int Comparisons => comparisons;
        public int Swaps => swaps;
        public int ArrayAccesses => arrayAccesses;

        /// <summary>
        /// Инициализация алгоритма
        /// </summary>
        public virtual void Initialize()
        {
            currentStep = 0;
            totalOperations = 0;
            isCompleted = false;
            comparisons = 0;
            swaps = 0;
            arrayAccesses = 0;
        }

        /// <summary>
        /// Установка данных для сортировки
        /// </summary>
        public void SetData(int[] inputArray, List<BarVisualization> visuals)
        {
            array = (int[])inputArray.Clone();
            visualElements = new List<BarVisualization>(visuals);
            Initialize();
        }

        /// <summary>
        /// Выполнение одного шага
        /// </summary>
        public abstract bool Step();

        /// <summary>
        /// Автоматическое выполнение
        /// </summary>
        public abstract IEnumerator Execute();

        /// <summary>
        /// Сброс
        /// </summary>
        public virtual void Reset()
        {
            Initialize();

            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.ResetAllElements();
            }
        }

        /// <summary>
        /// Получение прогресса
        /// </summary>
        public virtual float GetProgress()
        {
            if (totalOperations == 0) return 0f;
            return Mathf.Clamp01((float)currentStep / totalOperations);
        }

        /// <summary>
        /// Сравнение двух элементов
        /// </summary>
        protected bool Compare(int indexA, int indexB)
        {
            comparisons++;
            arrayAccesses += 2;

            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.MarkForComparison(indexA);
                VisualizationController.Instance.MarkForComparison(indexB);
            }

            return array[indexA] > array[indexB];
        }

        /// <summary>
        /// Обмен элементов
        /// </summary>
        protected void Swap(int indexA, int indexB)
        {
            if (indexA == indexB) return;

            swaps++;
            arrayAccesses += 4;

            // Обмен значений
            int temp = array[indexA];
            array[indexA] = array[indexB];
            array[indexB] = temp;

            // Визуальный обмен
            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.SwapElements(indexA, indexB);
            }
        }

        /// <summary>
        /// Отметка элемента как отсортированного
        /// </summary>
        protected void MarkSorted(int index)
        {
            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.MarkAsComplete(index);
            }
        }

        /// <summary>
        /// Сброс визуального состояния элемента
        /// </summary>
        protected void ResetVisual(int index)
        {
            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.ResetElement(index);
            }
        }

        /// <summary>
        /// Вызов события шага
        /// </summary>
        protected void RaiseStepEvent(string description, object data = null)
        {
            OnStepCompleted?.Invoke(this, new ProcessStepEventArgs(
                currentStep,
                totalOperations,
                description,
                data
            ));
        }

        /// <summary>
        /// Проверка отсортированности массива
        /// </summary>
        protected bool IsSorted()
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1])
                    return false;
            }
            return true;
        }
    }
}
