using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcessSimulation.Processes;

namespace ProcessSimulation.Algorithms
{
    /// <summary>
    /// Быстрая сортировка
    /// </summary>
    public class QuickSort : SortingAlgorithm
    {
        private Stack<(int low, int high)> stack;
        private int currentLow;
        private int currentHigh;
        private int pivotIndex;
        private int i, j;
        private enum State { SelectPivot, Partition, Complete }
        private State currentState;

        public override string ProcessName => "Быстрая сортировка";
        public override string Description => "Эффективный алгоритм сортировки, использующий принцип 'разделяй и властвуй'. Выбирает опорный элемент и разбивает массив на две части.";
        public override string TimeComplexity => "O(n log n) средний, O(n²) худший";
        public override string SpaceComplexity => "O(log n)";

        public override void Initialize()
        {
            base.Initialize();
            stack = new Stack<(int, int)>();

            if (array != null && array.Length > 1)
            {
                stack.Push((0, array.Length - 1));
                totalOperations = array.Length * (int)Math.Log(array.Length, 2);
            }

            currentState = State.SelectPivot;
        }

        public override bool Step()
        {
            if (isCompleted || array == null || array.Length <= 1)
            {
                isCompleted = true;
                return false;
            }

            if (stack.Count == 0)
            {
                // Сортировка завершена, отмечаем все элементы
                for (int idx = 0; idx < array.Length; idx++)
                {
                    MarkSorted(idx);
                }

                isCompleted = true;
                RaiseStepEvent($"Быстрая сортировка завершена! Сравнений: {comparisons}, Обменов: {swaps}");
                return false;
            }

            switch (currentState)
            {
                case State.SelectPivot:
                    (currentLow, currentHigh) = stack.Pop();

                    if (currentLow < currentHigh)
                    {
                        // Выбираем опорный элемент (последний элемент в диапазоне)
                        pivotIndex = currentHigh;
                        i = currentLow - 1;
                        j = currentLow;

                        HighlightElement(pivotIndex);
                        RaiseStepEvent($"Выбран опорный элемент: индекс {pivotIndex}, значение {array[pivotIndex]}");

                        currentState = State.Partition;
                    }
                    else if (currentLow == currentHigh)
                    {
                        MarkSorted(currentLow);
                    }
                    break;

                case State.Partition:
                    if (j < currentHigh)
                    {
                        ResetVisual(j);

                        if (Compare(pivotIndex, j)) // Если array[j] <= array[pivot]
                        {
                            i++;
                            if (i != j)
                            {
                                Swap(i, j);
                                RaiseStepEvent($"Элемент {array[i]} меньше опорного, перемещён на позицию {i}");
                            }
                        }

                        j++;
                    }
                    else
                    {
                        // Помещаем опорный элемент на правильную позицию
                        if (i + 1 != pivotIndex)
                        {
                            Swap(i + 1, pivotIndex);
                        }

                        pivotIndex = i + 1;
                        MarkSorted(pivotIndex);

                        RaiseStepEvent($"Опорный элемент размещён на позиции {pivotIndex}");

                        // Добавляем подмассивы в стек
                        if (pivotIndex - 1 > currentLow)
                            stack.Push((currentLow, pivotIndex - 1));

                        if (pivotIndex + 1 < currentHigh)
                            stack.Push((pivotIndex + 1, currentHigh));

                        currentState = State.SelectPivot;
                    }
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
