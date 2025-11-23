# Руководство по установке и настройке

## Требования

### Системные требования

- **Операционная система**: Windows 10/11, macOS 10.15+, или Linux (Ubuntu 20.04+)
- **Unity**: версия 6.0 или выше
- **Процессор**: Intel Core i5 или эквивалент
- **Оперативная память**: минимум 8 ГБ
- **Видеокарта**: Поддержка DirectX 11/12 или OpenGL 4.5

### Программное обеспечение

1. **Unity Hub** - для управления версиями Unity
2. **Unity Editor 6.0+**
3. **Visual Studio 2022** или **Rider** (рекомендуется)
4. **Git** - для клонирования репозитория

## Установка

### Шаг 1: Клонирование проекта

```bash
git clone <repository-url>
cd Diplom2025
```

### Шаг 2: Открытие проекта в Unity

1. Запустите Unity Hub
2. Нажмите "Add" → "Add project from disk"
3. Выберите папку с проектом
4. Убедитесь, что выбрана Unity 6.0+
5. Откройте проект

### Шаг 3: Установка зависимостей

#### Обязательные пакеты

Проект автоматически загрузит следующие пакеты через Package Manager:

1. **TextMeshPro**
   ```
   Window → Package Manager → TextMeshPro → Install
   ```
   При первом запуске Unity предложит импортировать TMP Essentials - согласитесь.

#### Опциональные пакеты

2. **DOTween** (для плавных анимаций)

   **Вариант A: Asset Store**
   - Откройте Unity Asset Store
   - Найдите "DOTween (HOTween v2)"
   - Загрузите и импортируйте

   **Вариант B: Package Manager (если есть в реестре)**
   ```
   Window → Package Manager → Add package from git URL
   https://github.com/Demigiant/dotween.git
   ```

   **Вариант C: Без DOTween**
   Если вы не хотите использовать DOTween, можно заменить анимации на стандартные:
   - Замените вызовы `DOTween` на `Coroutine` с ручной интерполяцией
   - Или используйте `Unity.Animation`

### Шаг 4: Настройка проекта

#### 1. Настройки качества (Quality Settings)

```
Edit → Project Settings → Quality
```

Рекомендуемые настройки:
- **V Sync Count**: Don't Sync (для контроля FPS)
- **Anti Aliasing**: 4x Multi Sampling
- **Shadow Resolution**: High Resolution

#### 2. Настройки рендеринга (Graphics Settings)

```
Edit → Project Settings → Graphics
```

- **Scriptable Render Pipeline**: Built-in Render Pipeline (по умолчанию)
- Или URP для лучшей производительности

#### 3. Настройки ввода (Input Settings)

Проект использует старую систему Input Manager (по умолчанию):
- Horizontal: A/D, ←/→
- Vertical: W/S, ↑/↓
- Mouse X/Y: движение мыши

### Шаг 5: Создание префабов

#### Префаб для Bar Element

1. Создайте 3D объект Cube:
   ```
   Hierarchy → 3D Object → Cube
   ```

2. Настройте:
   - Переименуйте в "BarElement"
   - Scale: (0.8, 1, 0.8)
   - Добавьте компонент `BarVisualization`

3. Создайте материалы:
   - **DefaultMaterial** - базовый цвет (серый)
   - **HighlightMaterial** - подсветка (жёлтый)
   - **ComparisonMaterial** - сравнение (оранжевый)
   - **CompleteMaterial** - завершено (зелёный)

4. Перетащите в папку Prefabs

### Шаг 6: Настройка сцены

#### Создание менеджеров

1. **Создайте пустой GameObject "SimulationManager"**
   ```
   Hierarchy → Create Empty
   ```
   - Добавьте компонент `SimulationManager`

2. **Создайте пустой GameObject "VisualizationController"**
   - Добавьте компонент `VisualizationController`
   - Назначьте префаб BarElement в поле "Bar Element Prefab"

3. **Создайте пустой GameObject "ApplicationController"**
   - Добавьте компонент `ApplicationController`
   - Настройте параметры:
     - Array Size: 20
     - Min Value: 1
     - Max Value: 100
     - Initial Algorithm: BubbleSort

4. **Настройте Main Camera**
   - Добавьте компонент `CameraController`
   - Position: (0, 10, -15)
   - Rotation: (45, 0, 0)

#### Создание UI

1. **Создайте Canvas**
   ```
   Hierarchy → UI → Canvas
   ```
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080

2. **Добавьте пустой GameObject "UIManager"**
   - Добавьте компонент `UIManager`

3. **Создайте панели управления**

   **Control Panel** (левый нижний угол):
   ```
   Canvas → UI → Panel (ControlPanel)
   ```
   - Добавьте кнопки:
     - Play Button
     - Pause Button
     - Step Button
     - Reset Button
     - Speed Up Button (+)
     - Speed Down Button (-)

   **Info Panel** (правый верхний угол):
   ```
   Canvas → UI → Panel (InfoPanel)
   ```
   - Добавьте TextMeshPro элементы:
     - Algorithm Name (заголовок)
     - Algorithm Description (описание)
     - Time Complexity
     - Space Complexity

   **Statistics Panel** (левый верхний угол):
   ```
   Canvas → UI → Panel (StatisticsPanel)
   ```
   - Добавьте TextMeshPro элементы:
     - State Text (состояние)
     - Speed Text (скорость)
     - Progress Text (прогресс)
     - Elapsed Time (время)
     - Comparisons (сравнения)
     - Swaps (обмены)
     - Current Step (текущий шаг)

4. **Добавьте Progress Bar**
   ```
   Canvas → UI → Slider (ProgressSlider)
   ```
   - Interactable: false
   - Min Value: 0
   - Max Value: 1

5. **Привяжите UI элементы к UIManager**
   - Перетащите все созданные элементы в соответствующие поля UIManager

### Шаг 7: Создание градиента для визуализации

1. В папке `_Project` создайте папку `Gradients`
2. Создайте Gradient:
   ```
   Assets → Create → Gradient
   ```
3. Настройте цвета от синего (низкие значения) к красному (высокие)
4. Назначьте в компоненте `BarVisualization`

## Проверка установки

### Тестовый запуск

1. Нажмите Play в Unity Editor
2. Проверьте, что:
   - Генерируются случайные столбцы
   - Кнопка Play запускает анимацию
   - Кнопка Pause приостанавливает
   - Кнопка Step выполняет один шаг
   - Статистика обновляется
   - Камера управляется мышью и клавиатурой

### Типичные проблемы

#### Проблема: Столбцы не отображаются
**Решение**:
- Проверьте, что префаб BarElement назначен в VisualizationController
- Убедитесь, что материалы настроены

#### Проблема: UI не реагирует
**Решение**:
- Убедитесь, что все кнопки привязаны к UIManager
- Проверьте EventSystem в сцене

#### Проблема: Ошибки компиляции
**Решение**:
- Убедитесь, что установлен TextMeshPro
- Проверьте версию Unity (должна быть 6.0+)
- Если используете DOTween, убедитесь что он импортирован

#### Проблема: Анимации не плавные
**Решение**:
- Установите DOTween
- Или реализуйте альтернативную систему анимаций

## Сборка проекта

### Windows Build

```
File → Build Settings
- Platform: Windows
- Architecture: x86_64
- Build
```

### macOS Build

```
File → Build Settings
- Platform: macOS
- Architecture: Apple Silicon или Intel
- Build
```

### WebGL Build

```
File → Build Settings
- Platform: WebGL
- Compression Format: Gzip
- Build
```

## Дополнительная настройка

### Производительность

Для улучшения производительности:

1. **Batching**
   ```
   Edit → Project Settings → Player → Other Settings
   - Dynamic Batching: On
   - GPU Skinning: On
   ```

2. **Occlusion Culling**
   ```
   Window → Rendering → Occlusion Culling
   ```

3. **Quality Settings**
   - Уменьшите тени на слабых системах
   - Отключите V-Sync для тестирования

### Отладка

Включите логирование в SimulationManager:
```csharp
Debug.Log($"Step {currentStep}: {description}");
```

## Следующие шаги

После успешной установки:

1. Изучите [README.md](README.md) для понимания функционала
2. Прочитайте [ARCHITECTURE.md](ARCHITECTURE.md) для понимания структуры
3. Экспериментируйте с разными алгоритмами
4. Попробуйте добавить свой алгоритм

## Поддержка

При возникновении проблем:
1. Проверьте логи Unity Console
2. Убедитесь в правильной версии Unity
3. Проверьте, что все зависимости установлены
