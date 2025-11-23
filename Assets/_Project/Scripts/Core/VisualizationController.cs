using System.Collections.Generic;
using UnityEngine;
using ProcessSimulation.Visualization;
using DG.Tweening;

namespace ProcessSimulation.Core
{
    /// <summary>
    /// Контроллер визуализации процессов
    /// </summary>
    public class VisualizationController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject barElementPrefab;
        [SerializeField] private GameObject nodeElementPrefab;
        [SerializeField] private GameObject particleElementPrefab;

        [Header("Layout Settings")]
        [SerializeField] private float elementSpacing = 1.5f;
        [SerializeField] private Vector3 startPosition = Vector3.zero;
        [SerializeField] private bool centerLayout = true;

        [Header("Animation Settings")]
        [SerializeField] private float spawnDelay = 0.05f;
        [SerializeField] private bool animateOnSpawn = true;

        private List<VisualizationElement> activeElements = new List<VisualizationElement>();
        private Transform elementsContainer;

        public static VisualizationController Instance { get; private set; }

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

            // Создаём контейнер для элементов
            elementsContainer = new GameObject("Elements Container").transform;
            elementsContainer.SetParent(transform);
            elementsContainer.localPosition = startPosition;
        }

        /// <summary>
        /// Создание массива столбцов
        /// </summary>
        public List<BarVisualization> CreateBars(int[] values)
        {
            ClearElements();

            List<BarVisualization> bars = new List<BarVisualization>();
            int maxValue = GetMaxValue(values);

            float totalWidth = values.Length * elementSpacing;
            float startX = centerLayout ? -totalWidth / 2f : 0f;

            for (int i = 0; i < values.Length; i++)
            {
                Vector3 position = new Vector3(startX + i * elementSpacing, 0f, 0f);
                BarVisualization bar = CreateBar(values[i], position, maxValue);

                if (animateOnSpawn)
                {
                    bar.AnimateSpawn().SetDelay(i * spawnDelay);
                }

                bars.Add(bar);
                activeElements.Add(bar);
            }

            return bars;
        }

        /// <summary>
        /// Создание одного столбца
        /// </summary>
        private BarVisualization CreateBar(int value, Vector3 position, int maxValue)
        {
            GameObject barObj = Instantiate(barElementPrefab, elementsContainer);
            barObj.transform.localPosition = position;

            BarVisualization bar = barObj.GetComponent<BarVisualization>();
            if (bar == null)
            {
                bar = barObj.AddComponent<BarVisualization>();
            }

            bar.SetMaxValue(maxValue);
            bar.SetValue(value);

            return bar;
        }

        /// <summary>
        /// Обмен позициями двух элементов
        /// </summary>
        public void SwapElements(int indexA, int indexB, float duration = 0.5f)
        {
            if (indexA < 0 || indexA >= activeElements.Count ||
                indexB < 0 || indexB >= activeElements.Count)
            {
                return;
            }

            var elementA = activeElements[indexA];
            var elementB = activeElements[indexB];

            Vector3 posA = elementA.Transform.localPosition;
            Vector3 posB = elementB.Transform.localPosition;

            // Анимация обмена с дугой
            Sequence swapSequence = DOTween.Sequence();

            // Поднимаем первый элемент
            swapSequence.Append(elementA.Transform.DOLocalMoveY(posA.y + 2f, duration / 3f));
            swapSequence.Join(elementB.Transform.DOLocalMoveY(posB.y + 2f, duration / 3f));

            // Меняем X позиции
            swapSequence.Append(elementA.Transform.DOLocalMoveX(posB.x, duration / 3f));
            swapSequence.Join(elementB.Transform.DOLocalMoveX(posA.x, duration / 3f));

            // Опускаем обратно
            swapSequence.Append(elementA.Transform.DOLocalMoveY(posA.y, duration / 3f));
            swapSequence.Join(elementB.Transform.DOLocalMoveY(posB.y, duration / 3f));

            // Обновляем список
            activeElements[indexA] = elementB;
            activeElements[indexB] = elementA;
        }

        /// <summary>
        /// Подсветка элемента
        /// </summary>
        public void HighlightElement(int index)
        {
            if (index >= 0 && index < activeElements.Count)
            {
                activeElements[index].Highlight();
            }
        }

        /// <summary>
        /// Отметка элемента для сравнения
        /// </summary>
        public void MarkForComparison(int index)
        {
            if (index >= 0 && index < activeElements.Count)
            {
                activeElements[index].MarkForComparison();
            }
        }

        /// <summary>
        /// Отметка элемента как завершённого
        /// </summary>
        public void MarkAsComplete(int index)
        {
            if (index >= 0 && index < activeElements.Count)
            {
                activeElements[index].MarkAsComplete();
            }
        }

        /// <summary>
        /// Сброс визуального состояния элемента
        /// </summary>
        public void ResetElement(int index)
        {
            if (index >= 0 && index < activeElements.Count)
            {
                activeElements[index].ResetVisual();
            }
        }

        /// <summary>
        /// Сброс всех элементов
        /// </summary>
        public void ResetAllElements()
        {
            foreach (var element in activeElements)
            {
                element.ResetVisual();
            }
        }

        /// <summary>
        /// Получение элемента по индексу
        /// </summary>
        public VisualizationElement GetElement(int index)
        {
            if (index >= 0 && index < activeElements.Count)
            {
                return activeElements[index];
            }
            return null;
        }

        /// <summary>
        /// Получение всех элементов
        /// </summary>
        public List<VisualizationElement> GetAllElements()
        {
            return new List<VisualizationElement>(activeElements);
        }

        /// <summary>
        /// Очистка всех элементов
        /// </summary>
        public void ClearElements()
        {
            foreach (var element in activeElements)
            {
                if (element != null)
                {
                    Destroy(element.gameObject);
                }
            }

            activeElements.Clear();
        }

        /// <summary>
        /// Получение максимального значения в массиве
        /// </summary>
        private int GetMaxValue(int[] values)
        {
            int max = values[0];
            foreach (int value in values)
            {
                if (value > max) max = value;
            }
            return max;
        }

        /// <summary>
        /// Установка позиции начала
        /// </summary>
        public void SetStartPosition(Vector3 position)
        {
            startPosition = position;
            if (elementsContainer != null)
            {
                elementsContainer.localPosition = position;
            }
        }

        private void OnDestroy()
        {
            ClearElements();
        }
    }
}
