using System;
using System.Collections;
using UnityEngine;
using ProcessSimulation.Processes;

namespace ProcessSimulation.Algorithms
{
    /// <summary>
    /// Пузырьковая сортировка
    /// </summary>
    public class BubbleSort : SortingAlgorithm
    {
        private int currentPass;
        private int currentIndex;
        private int maxUnsortedIndex;

        public override string ProcessName => "Пузырьковая сортировка";
        public override string Description => "Простой алгоритм сортировки, который многократно проходит по списку, сравнивая соседние элементы и меняя их местами, если они находятся в неправильном порядке.";
        public override string TimeComplexity => "O(n²)";
        public override string SpaceComplexity => "O(1)";

        public override void Initialize()
        {
            base.Initialize();
            currentPass = 0;
            currentIndex = 0;
            maxUnsortedIndex = array != null ? array.Length - 1 : 0;
            totalOperations = array != null ? (array.Length * (array.Length - 1)) / 2 : 0;
        }

        public override bool Step()
        {
            if (isCompleted || array == null || array.Length <= 1)
            {
                isCompleted = true;
                return false;
            }

            // Если текущий проход завершён
            if (currentIndex >= maxUnsortedIndex)
            {
                // Отмечаем последний элемент как отсортированный
                MarkSorted(maxUnsortedIndex);

                maxUnsortedIndex--;
                currentIndex = 0;
                currentPass++;

                // Если массив полностью отсортирован
                if (maxUnsortedIndex <= 0)
                {
                    if (maxUnsortedIndex == 0)
                        MarkSorted(0);

                    isCompleted = true;
                    RaiseStepEvent($"Сортировка завершена! Проходов: {currentPass}, Сравнений: {comparisons}, Обменов: {swaps}");
                    return false;
                }

                RaiseStepEvent($"Проход {currentPass} завершён");
                return true;
            }

            // Сравниваем и меняем местами если нужно
            ResetVisual(currentIndex);
            ResetVisual(currentIndex + 1);

            if (Compare(currentIndex, currentIndex + 1))
            {
                Swap(currentIndex, currentIndex + 1);
                RaiseStepEvent($"Обмен элементов на позициях {currentIndex} и {currentIndex + 1}");
            }
            else
            {
                RaiseStepEvent($"Сравнение элементов {currentIndex} и {currentIndex + 1} - обмен не требуется");
            }

            currentIndex++;
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
    }
}
