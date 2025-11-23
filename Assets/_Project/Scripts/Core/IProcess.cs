using System;
using System.Collections;

namespace ProcessSimulation.Core
{
    /// <summary>
    /// Базовый интерфейс для всех процессов/алгоритмов
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Название процесса
        /// </summary>
        string ProcessName { get; }

        /// <summary>
        /// Описание процесса
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Категория процесса
        /// </summary>
        ProcessCategory Category { get; }

        /// <summary>
        /// Временная сложность (нотация Big O)
        /// </summary>
        string TimeComplexity { get; }

        /// <summary>
        /// Пространственная сложность
        /// </summary>
        string SpaceComplexity { get; }

        /// <summary>
        /// Инициализация процесса
        /// </summary>
        void Initialize();

        /// <summary>
        /// Выполнение одного шага процесса
        /// </summary>
        /// <returns>True если процесс продолжается, False если завершён</returns>
        bool Step();

        /// <summary>
        /// Корутина для автоматического выполнения
        /// </summary>
        IEnumerator Execute();

        /// <summary>
        /// Сброс процесса в исходное состояние
        /// </summary>
        void Reset();

        /// <summary>
        /// Получение текущего прогресса (0.0 - 1.0)
        /// </summary>
        float GetProgress();

        /// <summary>
        /// Событие изменения состояния процесса
        /// </summary>
        event EventHandler<ProcessStepEventArgs> OnStepCompleted;
    }

    /// <summary>
    /// Категории процессов
    /// </summary>
    public enum ProcessCategory
    {
        Sorting,            // Сортировка
        Searching,          // Поиск
        GraphTraversal,     // Обход графов
        PathFinding,        // Поиск пути
        DataStructure,      // Структуры данных
        PhysicsSimulation,  // Физическая симуляция
        NetworkSimulation,  // Сетевая симуляция
        Custom              // Пользовательский
    }

    /// <summary>
    /// Событие шага процесса
    /// </summary>
    public class ProcessStepEventArgs : EventArgs
    {
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }
        public string StepDescription { get; set; }
        public object StepData { get; set; }

        public ProcessStepEventArgs(int currentStep, int totalSteps, string description, object data = null)
        {
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
            StepDescription = description;
            StepData = data;
        }
    }
}
