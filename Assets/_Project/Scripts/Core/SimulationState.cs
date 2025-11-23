using System;

namespace ProcessSimulation.Core
{
    /// <summary>
    /// Состояния симуляции
    /// </summary>
    public enum SimulationState
    {
        Idle,           // Ожидание
        Running,        // Выполняется
        Paused,         // На паузе
        StepByStep,     // Пошаговое выполнение
        Completed,      // Завершена
        Error           // Ошибка
    }

    /// <summary>
    /// Скорость симуляции
    /// </summary>
    public enum SimulationSpeed
    {
        VerySlow = 0,   // x0.25
        Slow = 1,       // x0.5
        Normal = 2,     // x1.0
        Fast = 3,       // x2.0
        VeryFast = 4    // x4.0
    }

    /// <summary>
    /// Событие изменения состояния симуляции
    /// </summary>
    public class SimulationStateChangedEventArgs : EventArgs
    {
        public SimulationState PreviousState { get; set; }
        public SimulationState CurrentState { get; set; }
        public string Reason { get; set; }

        public SimulationStateChangedEventArgs(SimulationState previous, SimulationState current, string reason = "")
        {
            PreviousState = previous;
            CurrentState = current;
            Reason = reason;
        }
    }
}
