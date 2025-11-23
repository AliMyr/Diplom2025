# Архитектура системы

## Обзор

Программный комплекс построен на модульной архитектуре, которая разделяет ответственность между различными компонентами системы.

## Принципы проектирования

### 1. Разделение ответственности (Separation of Concerns)
- **Core** - логика симуляции и управления
- **Visualization** - отображение и анимации
- **UI** - пользовательский интерфейс
- **Processes** - реализации алгоритмов

### 2. Инверсия зависимостей (Dependency Inversion)
- Использование интерфейсов (`IProcess`)
- Абстрактные базовые классы (`SortingAlgorithm`)
- Слабая связанность компонентов

### 3. Событийно-ориентированная архитектура
- События жизненного цикла симуляции
- События шагов процесса
- Pub/Sub паттерн для связи компонентов

## Диаграмма компонентов

```
┌─────────────────────────────────────────────────┐
│          ApplicationController                   │
│  (Главный контроллер приложения)                 │
└───────────────┬─────────────────────────────────┘
                │
    ┌───────────┴───────────┐
    │                       │
┌───▼──────────────┐   ┌───▼──────────────────┐
│ SimulationManager│   │VisualizationController│
│ (Управление)     │   │ (Отображение)        │
└───┬──────────────┘   └───┬──────────────────┘
    │                      │
    │  ┌───────────────────┘
    │  │
┌───▼──▼────────────┐
│    IProcess       │
│  (Интерфейс)      │
└───┬───────────────┘
    │
    │ Реализации:
    ├─► BubbleSort
    ├─► QuickSort
    ├─► InsertionSort
    └─► SelectionSort
```

## Основные компоненты

### Core Layer (Ядро системы)

#### SimulationManager
**Назначение**: Центральный менеджер симуляции

**Ответственность**:
- Управление состоянием симуляции (Idle, Running, Paused, etc.)
- Загрузка и выполнение процессов
- Контроль скорости выполнения
- Генерация событий изменения состояния

**Публичный API**:
```csharp
void LoadProcess(IProcess process)
void Play()
void Pause()
void Stop()
void StepForward()
void ResetSimulation()
void SetSpeed(SimulationSpeed speed)
float GetSpeedMultiplier()
```

**События**:
```csharp
event EventHandler<SimulationStateChangedEventArgs> OnStateChanged
event EventHandler<ProcessStepEventArgs> OnProcessStep
event EventHandler OnSimulationCompleted
event EventHandler OnSimulationReset
```

#### VisualizationController
**Назначение**: Управление визуальными элементами

**Ответственность**:
- Создание и уничтожение визуальных элементов
- Управление расположением (layout)
- Анимации перемещения и обмена
- Цветовая индикация состояний

**Публичный API**:
```csharp
List<BarVisualization> CreateBars(int[] values)
void SwapElements(int indexA, int indexB, float duration)
void HighlightElement(int index)
void MarkForComparison(int index)
void MarkAsComplete(int index)
void ResetElement(int index)
void ClearElements()
```

#### ApplicationController
**Назначение**: Главный контроллер приложения

**Ответственность**:
- Генерация тестовых данных
- Загрузка и переключение алгоритмов
- Координация между менеджерами
- Обработка глобальных команд

### Process Layer (Слой процессов)

#### IProcess Interface
**Назначение**: Унифицированный интерфейс для всех процессов

**Контракт**:
```csharp
public interface IProcess
{
    // Метаданные
    string ProcessName { get; }
    string Description { get; }
    ProcessCategory Category { get; }
    string TimeComplexity { get; }
    string SpaceComplexity { get; }

    // Жизненный цикл
    void Initialize()
    bool Step()
    IEnumerator Execute()
    void Reset()

    // Состояние
    float GetProgress()

    // События
    event EventHandler<ProcessStepEventArgs> OnStepCompleted
}
```

#### SortingAlgorithm (Abstract Base Class)
**Назначение**: Базовая реализация для алгоритмов сортировки

**Предоставляет**:
- Общую логику работы с массивом
- Методы сравнения и обмена с подсчётом статистики
- Интеграцию с визуализацией
- Шаблонные методы для наследников

**Защищённые методы**:
```csharp
protected bool Compare(int indexA, int indexB)
protected void Swap(int indexA, int indexB)
protected void MarkSorted(int index)
protected void ResetVisual(int index)
protected void RaiseStepEvent(string description, object data)
```

### Visualization Layer (Слой визуализации)

#### VisualizationElement
**Назначение**: Базовый класс для визуальных элементов

**Возможности**:
- Анимации движения
- Анимации масштабирования
- Анимации цвета
- Состояния (highlight, comparison, complete)

#### BarVisualization
**Назначение**: Столбчатая визуализация для сортировки

**Особенности**:
- Автоматическое масштабирование высоты по значению
- Градиентная раскраска
- Корректное позиционирование (рост от основания вверх)

### UI Layer (Слой интерфейса)

#### UIManager
**Назначение**: Управление пользовательским интерфейсом

**Ответственность**:
- Отображение текущего состояния
- Обработка пользовательского ввода
- Обновление статистики
- Управление доступностью элементов

### Camera Layer (Слой камеры)

#### CameraController
**Назначение**: Управление камерой для просмотра

**Возможности**:
- Вращение (orbit)
- Перемещение (pan)
- Приближение (zoom)
- Фокусировка на объектах
- Сброс в начальное положение

## Потоки данных

### Инициализация

```
ApplicationController
    ↓
    1. GenerateRandomArray()
    ↓
    2. CreateVisualization() → VisualizationController.CreateBars()
    ↓
    3. LoadAlgorithm() → Creates Algorithm Instance
    ↓
    4. Algorithm.SetData(array, visuals)
    ↓
    5. SimulationManager.LoadProcess(algorithm)
```

### Выполнение симуляции

```
User clicks Play
    ↓
UIManager.OnPlayClicked()
    ↓
SimulationManager.Play()
    ↓
    ┌─────────────────────┐
    │ Coroutine Loop      │
    │ while (Running)     │
    │   Process.Step()    │
    │   ↓                 │
    │   Visual updates    │
    │   ↓                 │
    │   Events fired      │
    │   ↓                 │
    │   Wait delay        │
    └─────────────────────┘
    ↓
OnStepCompleted event
    ↓
UIManager updates statistics
```

### Пошаговое выполнение

```
User clicks Step
    ↓
UIManager.OnStepClicked()
    ↓
SimulationManager.StepForward()
    ↓
State = StepByStep
    ↓
Process.Step()
    ↓
VisualizationController updates visuals
    ↓
Statistics updated
```

## Паттерны проектирования

### Singleton
Используется для глобально доступных менеджеров:
- `SimulationManager.Instance`
- `VisualizationController.Instance`
- `UIManager.Instance`
- `ApplicationController.Instance`
- `CameraController.Instance`

### Strategy Pattern
Различные алгоритмы реализуют общий интерфейс `IProcess`, позволяя заменять их во время выполнения.

### Observer Pattern
Событийная система для связи компонентов:
```csharp
SimulationManager → OnStateChanged → UIManager
Process → OnStepCompleted → Statistics Update
```

### Template Method
`SortingAlgorithm` предоставляет шаблон с общей логикой, специфичные части реализуются в наследниках.

### Command Pattern
Методы управления симуляцией (Play, Pause, Step, Reset) инкапсулируют операции.

## Расширяемость

### Добавление нового типа процесса

1. Реализуйте интерфейс `IProcess`
2. Определите новую категорию в `ProcessCategory` (если нужно)
3. Создайте соответствующую визуализацию
4. Зарегистрируйте в `ApplicationController`

### Добавление нового типа визуализации

1. Наследуйтесь от `VisualizationElement`
2. Переопределите `UpdateVisualization()`
3. Добавьте специфичные методы
4. Создайте префаб
5. Добавьте в `VisualizationController`

## Оптимизация

### Производительность

- Object pooling для визуальных элементов (можно добавить)
- Кэширование компонентов
- Batch операции для схожих обновлений

### Память

- Переиспользование массивов
- Своевременная очистка событий
- Уничтожение неиспользуемых объектов

## Тестирование

### Модульные тесты
Каждый алгоритм можно тестировать отдельно:
```csharp
[Test]
public void BubbleSort_SortsCorrectly()
{
    var algorithm = new BubbleSort();
    var array = new int[] { 5, 2, 8, 1, 9 };
    algorithm.SetData(array, null);

    while (algorithm.Step()) { }

    Assert.IsTrue(IsSorted(array));
}
```

### Интеграционные тесты
Проверка взаимодействия компонентов в Unity Test Framework.

## Документация кода

Все публичные API документированы с помощью XML комментариев:
```csharp
/// <summary>
/// Описание метода
/// </summary>
/// <param name="paramName">Описание параметра</param>
/// <returns>Описание возвращаемого значения</returns>
```
