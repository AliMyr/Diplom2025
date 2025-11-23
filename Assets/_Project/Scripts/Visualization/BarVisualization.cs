using UnityEngine;

namespace ProcessSimulation.Visualization
{
    /// <summary>
    /// Столбчатая визуализация для сортировки
    /// </summary>
    public class BarVisualization : VisualizationElement
    {
        [Header("Bar Settings")]
        [SerializeField] private float maxHeight = 10f;
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float barWidth = 0.8f;
        [SerializeField] private float barDepth = 0.8f;

        [Header("Color Gradient")]
        [SerializeField] private Gradient valueGradient;
        [SerializeField] private bool useGradient = true;

        private int maxValue = 100;

        /// <summary>
        /// Установка максимального значения для нормализации
        /// </summary>
        public void SetMaxValue(int max)
        {
            maxValue = max;
            UpdateVisualization();
        }

        /// <summary>
        /// Установка значения с автоматическим обновлением высоты
        /// </summary>
        public override void SetValue(int newValue)
        {
            base.SetValue(newValue);
            UpdateVisualization();
        }

        protected override void UpdateVisualization()
        {
            // Вычисляем высоту столбца
            float normalizedValue = (float)value / maxValue;
            float height = Mathf.Lerp(minHeight, maxHeight, normalizedValue);

            // Устанавливаем размер
            Vector3 newScale = new Vector3(barWidth, height, barDepth);
            transform.localScale = newScale;

            // Корректируем позицию (столбец растёт вверх от основания)
            Vector3 pos = transform.localPosition;
            pos.y = height / 2f;
            transform.localPosition = pos;

            // Применяем градиент цвета если включён
            if (useGradient && valueGradient != null && meshRenderer != null)
            {
                Color color = valueGradient.Evaluate(normalizedValue);
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);
                props.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(props);
                currentColor = color;
            }
        }

        /// <summary>
        /// Анимация изменения высоты
        /// </summary>
        public void AnimateHeight(int newValue, float duration = -1)
        {
            if (duration < 0) duration = animationDuration;

            value = newValue;
            float normalizedValue = (float)value / maxValue;
            float targetHeight = Mathf.Lerp(minHeight, maxHeight, normalizedValue);

            Vector3 targetScale = new Vector3(barWidth, targetHeight, barDepth);
            AnimateScale(targetScale, duration);

            // Анимация позиции
            Vector3 targetPos = transform.localPosition;
            targetPos.y = targetHeight / 2f;
            AnimateLocalMove(targetPos, duration);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateVisualization();
            }
        }
    }
}
