using System;
using System.Collections;
using UnityEngine;
using ProcessSimulation.Processes;

namespace ProcessSimulation.Algorithms
{
    /// <summary>
    /// Сортировка выбором
    /// </summary>
    public class SelectionSort : SortingAlgorithm
    {
        private int currentIndex;
        private int minIndex;
        private int searchIndex;
        private enum State { FindMin, Swap }
        private State currentState;

        public override string ProcessName => "Сортировка выбором";
        public override string Description => "Алгоритм сортировки, который находит минимальный элемент в неотсортированной части массива и помещает его в начало.";
        public override string TimeComplexity => "O(n²)";
        public override string SpaceComplexity => "O(1)";

        public override void Initialize()
        {
            base.Initialize();
            currentIndex = 0;
            minIndex = 0;
            searchIndex = 0;
            currentState = State.FindMin;
            totalOperations = array != null ? (array.Length * (array.Length - 1)) / 2 : 0;
        }

        public override bool Step()
        {
            if (isCompleted || array == null || array.Length <= 1)
            {
                isCompleted = true;
                return false;
            }

            if (currentIndex >= array.Length - 1)
            {
                // Отмечаем последний элемент как отсортированный
                MarkSorted(array.Length - 1);

                isCompleted = true;
                RaiseStepEvent($"Сортировка выбором завершена! Сравнений: {comparisons}, Обменов: {swaps}");
                return false;
            }

            switch (currentState)
            {
                case State.FindMin:
                    if (searchIndex == currentIndex)
                    {
                        // Начинаем новый поиск минимума
                        minIndex = currentIndex;
                        searchIndex = currentIndex + 1;

                        HighlightElement(currentIndex);
                        RaiseStepEvent($"Начинаем поиск минимума от позиции {currentIndex}");
                    }
                    else if (searchIndex < array.Length)
                    {
                        // Сравниваем с текущим минимумом
                        ResetVisual(searchIndex);

                        if (Compare(minIndex, searchIndex)) // array[searchIndex] < array[minIndex]
                        {
                            ResetVisual(minIndex);
                            minIndex = searchIndex;
                            HighlightElement(minIndex);
                            RaiseStepEvent($"Найден новый минимум на позиции {minIndex}");
                        }
                        else
                        {
                            RaiseStepEvent($"Элемент на позиции {searchIndex} больше текущего минимума");
                        }

                        searchIndex++;
                    }
                    else
                    {
                        // Поиск завершён, переходим к обмену
                        currentState = State.Swap;
                    }
                    break;

                case State.Swap:
                    if (minIndex != currentIndex)
                    {
                        Swap(currentIndex, minIndex);
                        RaiseStepEvent($"Обмен минимального элемента: позиции {currentIndex} и {minIndex}");
                    }

                    // Отмечаем элемент как отсортированный
                    ResetVisual(currentIndex);
                    MarkSorted(currentIndex);

                    currentIndex++;
                    currentState = State.FindMin;
                    searchIndex = currentIndex;
                    break;
            }

            currentStep++;
            return true;
        }

        public override IEnumerator Execute()
        {
            Initialize();

            while (!isCompleted)
            {
                Step();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void HighlightElement(int index)
        {
            if (VisualizationController.Instance != null)
            {
                VisualizationController.Instance.HighlightElement(index);
            }
        }
    }
}
