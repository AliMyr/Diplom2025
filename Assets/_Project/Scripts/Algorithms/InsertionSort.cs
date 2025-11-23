using System;
using System.Collections;
using UnityEngine;
using ProcessSimulation.Processes;

namespace ProcessSimulation.Algorithms
{
    /// <summary>
    /// Сортировка вставками
    /// </summary>
    public class InsertionSort : SortingAlgorithm
    {
        private int currentIndex;
        private int compareIndex;
        private int keyValue;
        private bool isInserting;

        public override string ProcessName => "Сортировка вставками";
        public override string Description => "Алгоритм сортировки, который строит отсортированный массив по одному элементу за раз, вставляя каждый новый элемент в правильную позицию.";
        public override string TimeComplexity => "O(n²)";
        public override string SpaceComplexity => "O(1)";

        public override void Initialize()
        {
            base.Initialize();
            currentIndex = 1;
            compareIndex = 0;
            isInserting = false;
            totalOperations = array != null ? (array.Length * (array.Length - 1)) / 2 : 0;

            // Первый элемент уже "отсортирован"
            if (array != null && array.Length > 0)
            {
                MarkSorted(0);
            }
        }

        public override bool Step()
        {
            if (isCompleted || array == null || array.Length <= 1)
            {
                isCompleted = true;
                return false;
            }

            // Если обработали все элементы
            if (currentIndex >= array.Length)
            {
                isCompleted = true;
                RaiseStepEvent($"Сортировка вставками завершена! Сравнений: {comparisons}, Обменов: {swaps}");
                return false;
            }

            if (!isInserting)
            {
                // Начинаем вставку нового элемента
                keyValue = array[currentIndex];
                compareIndex = currentIndex - 1;
                isInserting = true;

                HighlightElement(currentIndex);
                RaiseStepEvent($"Вставка элемента {keyValue} с позиции {currentIndex}");
            }
            else
            {
                // Процесс вставки
                if (compareIndex >= 0 && array[compareIndex] > keyValue)
                {
                    // Сдвигаем элемент вправо
                    array[compareIndex + 1] = array[compareIndex];

                    // Визуализация сдвига
                    if (visualElements != null && compareIndex + 1 < visualElements.Count)
                    {
                        visualElements[compareIndex + 1].SetValue(array[compareIndex]);
                    }

                    comparisons++;
                    arrayAccesses += 2;

                    RaiseStepEvent($"Сдвиг элемента {array[compareIndex]} вправо");
                    compareIndex--;
                }
                else
                {
                    // Нашли правильную позицию для вставки
                    array[compareIndex + 1] = keyValue;

                    if (visualElements != null && compareIndex + 1 < visualElements.Count)
                    {
                        visualElements[compareIndex + 1].SetValue(keyValue);
                    }

                    arrayAccesses++;

                    // Отмечаем элементы от 0 до currentIndex как отсортированные
                    for (int i = 0; i <= currentIndex; i++)
                    {
                        ResetVisual(i);
                        MarkSorted(i);
                    }

                    RaiseStepEvent($"Элемент {keyValue} вставлен на позицию {compareIndex + 1}");

                    currentIndex++;
                    isInserting = false;
                }
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
