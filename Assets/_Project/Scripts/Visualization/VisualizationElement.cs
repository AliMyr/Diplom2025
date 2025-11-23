using UnityEngine;
using DG.Tweening;

namespace ProcessSimulation.Visualization
{
    /// <summary>
    /// Базовый класс для визуальных элементов
    /// </summary>
    public class VisualizationElement : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected Material defaultMaterial;
        [SerializeField] protected Material highlightMaterial;
        [SerializeField] protected Material comparisonMaterial;
        [SerializeField] protected Material completeMaterial;

        [Header("Animation Settings")]
        [SerializeField] protected float animationDuration = 0.3f;
        [SerializeField] protected Ease animationEase = Ease.OutQuad;

        protected int value;
        protected Color currentColor;
        protected Vector3 originalScale;
        protected Sequence currentAnimation;

        public int Value => value;
        public Transform Transform => transform;

        protected virtual void Awake()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            originalScale = transform.localScale;
        }

        /// <summary>
        /// Установка значения элемента
        /// </summary>
        public virtual void SetValue(int newValue)
        {
            value = newValue;
            UpdateVisualization();
        }

        /// <summary>
        /// Обновление визуализации
        /// </summary>
        protected virtual void UpdateVisualization()
        {
            // Переопределяется в наследниках
        }

        /// <summary>
        /// Анимация перемещения
        /// </summary>
        public virtual Tween AnimateMove(Vector3 targetPosition, float duration = -1)
        {
            if (duration < 0) duration = animationDuration;

            KillCurrentAnimation();
            return transform.DOMove(targetPosition, duration).SetEase(animationEase);
        }

        /// <summary>
        /// Анимация локального перемещения
        /// </summary>
        public virtual Tween AnimateLocalMove(Vector3 targetPosition, float duration = -1)
        {
            if (duration < 0) duration = animationDuration;

            KillCurrentAnimation();
            return transform.DOLocalMove(targetPosition, duration).SetEase(animationEase);
        }

        /// <summary>
        /// Анимация масштабирования
        /// </summary>
        public virtual Tween AnimateScale(Vector3 targetScale, float duration = -1)
        {
            if (duration < 0) duration = animationDuration;

            return transform.DOScale(targetScale, duration).SetEase(animationEase);
        }

        /// <summary>
        /// Анимация изменения цвета
        /// </summary>
        public virtual Tween AnimateColor(Color targetColor, float duration = -1)
        {
            if (duration < 0) duration = animationDuration;

            if (meshRenderer != null)
            {
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(props);

                return DOTween.To(
                    () => currentColor,
                    x => {
                        currentColor = x;
                        props.SetColor("_Color", x);
                        meshRenderer.SetPropertyBlock(props);
                    },
                    targetColor,
                    duration
                ).SetEase(animationEase);
            }

            return null;
        }

        /// <summary>
        /// Подсветка элемента
        /// </summary>
        public virtual void Highlight()
        {
            if (highlightMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = highlightMaterial;
            }

            AnimateScale(originalScale * 1.2f, animationDuration);
        }

        /// <summary>
        /// Отметка элемента для сравнения
        /// </summary>
        public virtual void MarkForComparison()
        {
            if (comparisonMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = comparisonMaterial;
            }
        }

        /// <summary>
        /// Отметка элемента как завершённого
        /// </summary>
        public virtual void MarkAsComplete()
        {
            if (completeMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = completeMaterial;
            }
        }

        /// <summary>
        /// Сброс к стандартному виду
        /// </summary>
        public virtual void ResetVisual()
        {
            if (defaultMaterial != null && meshRenderer != null)
            {
                meshRenderer.material = defaultMaterial;
            }

            KillCurrentAnimation();
            transform.localScale = originalScale;
        }

        /// <summary>
        /// Остановка текущей анимации
        /// </summary>
        protected void KillCurrentAnimation()
        {
            if (currentAnimation != null && currentAnimation.IsActive())
            {
                currentAnimation.Kill();
            }
        }

        /// <summary>
        /// Эффект появления
        /// </summary>
        public virtual Sequence AnimateSpawn()
        {
            transform.localScale = Vector3.zero;

            currentAnimation = DOTween.Sequence();
            currentAnimation.Append(transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack));

            return currentAnimation;
        }

        /// <summary>
        /// Эффект исчезновения
        /// </summary>
        public virtual Sequence AnimateDespawn()
        {
            currentAnimation = DOTween.Sequence();
            currentAnimation.Append(transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack));

            return currentAnimation;
        }

        protected virtual void OnDestroy()
        {
            KillCurrentAnimation();
            DOTween.Kill(transform);
        }
    }
}
