# Zoo World — тестовое задание

Это небольшая игра: по сцене бегают жертвы и хищники, хищники съедают жертв, над местом смерти показывается текст, а в UI растет счетчик смертей.

Главная идея проекта: игру можно расширять без переписывания основного кода. Новый зверь, новое движение или новый экран добавляются через данные и настройки.

---

## Что использовал

- **VContainer** — чтобы аккуратно собирать зависимости.
- **UniTask** — для асинхронной загрузки.
- **UniRx** — для событий и реактивных значений.
- **Addressables** — для загрузки префабов животных.

---

## Структура

```
Assets/_Project/Scripts/
├── Core/                       базовые примитивы (BaseDisposable, StateMachine)
├── UniRxExtentions/            ReactiveEvent<T>, обёртки над UniRx
├── Infrastructure/             bootstrap, состояния приложения, корневые сервисы
│   ├── RootLifetimeScope.cs    корневой DI-граф + serialized-ссылки сцены
│   ├── GameStateMachine.cs     Bootstrap → Game_Level → Game_CleanUp → Game_AppQuit
│   ├── States/                 сами IState-классы
│   ├── Data/                   слой данных: AnimalDataRow, AnimalDataRegistry, CsvAnimalDataLoader
│   └── Assets/                 слой ассетов: AnimalViewCatalogSO, AddressablesAnimalViewProvider, PooledAnimalViewProvider
├── Gameplay/
│   ├── Level/                  LevelInstaller, LevelScopeInstaller (child-scope)
│   ├── Animals/                MVC: AnimalView (MB) / AnimalModel (POCO) / AnimalControllerBase
│   │   ├── Movement/           IMovementStrategy + LinearMovement, JumpMovement, фабрика
│   │   └── Combat/             IFoodChainResolver
│   ├── Spawning/               AnimalSpawner
│   ├── World/                  IScreenBoundsService
│   ├── Events/                 ReactiveEvent-шины: IAnimalDeathEventBus, ITastyFxBus
│   └── FX/                     FloatingTextView (MB) + DamageTextFactory + пул
└── UI/
    └── DeathCounter/           MVP: View / Model / Presenter
```

---

## Архитектура простыми словами

### 1. Проект разбит на слои

Слои зависят только "вниз":

```
UI ──┐
     ├──► Gameplay ──► Infrastructure ──► Core
FX ──┘                     │
                     Addressables / CSV
```

- `Core` — базовые штуки, без геймплея.
- `Infrastructure` — запуск игры, данные и ассеты.
- `Gameplay` — логика животных и спавна.
- `UI` — только отображение.

Зачем так сделал: если поменяется способ хранения данных или загрузки ассетов, это не ломает геймплей.

### 2. Данные отделены от игрового кода

Сейчас животные читаются из `animals.csv`, но остальной код не привязан к CSV напрямую.

За это отвечают:

- `AnimalDataRow` — одна запись одно животное.
- `IAnimalDataRegistry` — общий доступ к данным животных.
- `CsvAnimalDataLoader` — читает CSV и заполняет реестр.

`AnimalSpawner`, `AnimalFactory` и состояния игры работают с реестром, а не с файлом. Поэтому источник данных потом можно заменить, например на BakingSheet/Google Sheets, без координальных изменений.

### 3. DI разделен на 2 области

- **Root scope** (`RootLifetimeScope`) — живет всю игру.
- **Level scope** (`LevelScopeInstaller`) — создается заново при старте уровня и удаляется при выходе.

Все, что относится к уровню (животные, спавнеры, временные сервисы), гарантированно чисто удаляется при перезапуске уровня.

### 4. Управление игрой через State Machine

Переходы между этапами сделаны через состояния:

`BootstrapState` → `Game_LevelState` → очистка/выход.

Так проще контролировать порядок запуска и порядок остановки. Особенно важно это при выходе из игры, чтобы все сервисы успели корректно закрыться.

### 5. Логика не лежит в MonoBehaviour

- `View` отвечает только за отображение и Unity-события.
- Основная логика находится в обычных C# классах.
- Для обмена событиями используется UniRx.

Почему так: код проще читать, проще тестировать и проще поддерживать.

### 6. Единые правила очистки

Все сервисы, которые создают подписки или держат ресурсы, наследуются от `BaseDisposable`.

Это сделано, чтобы не было утечек: при завершении уровня все подписки и объекты закрываются в одном понятном месте.

### 7. Добавил пулы объектов

Префабы животных и всплывающий текст не создаются каждый раз с нуля. Они переиспользуются из пулов.

Зачем: меньше лагов и меньше мусора для сборщика памяти.

### 8. Животные сделаны по MVC

- `AnimalView` — только отображение.
- `AnimalModel` — данные животного.
- `AnimalController` — поведение.

Обычное добавление нового зверя делается через данные и префаб, без переписывания логики.

### 9. UI сделан по MVP

- `View` показывает данные.
- `Model` хранит данные.
- `Presenter` связывает одно с другим.

Геймплей и UI не завязаны друг на друга напрямую, они общаются через события.

---

## Какие паттерны использовал

| Паттерн | Где | Зачем |
|---------|-----|-------|
| **Dependency Injection** | `RootLifetimeScope`, `LevelScopeInstaller` | чтобы не делать глобальные синглтоны |
| **State Machine** | `GameStateMachine` и `States` | понятный порядок запуска/остановки |
| **MVC** | `AnimalView`, `AnimalModel`, `AnimalController` | разделить картинку, данные и поведение |
| **MVP** | `DeathCounterView`, `Model`, `Presenter` | держать UI простым |
| **Strategy** | `IMovementStrategy` | легко добавлять новые типы движения |
| **Factory** | `AnimalFactory`, `MovementStrategyFactory` | собирать объекты в одном месте |
| **Object Pool** | `PooledAnimalViewProvider`, `FloatingTextPool` | переиспользовать объекты и снизить лаги |
| **Observer / Event Bus** | `ReactiveEvent`, `ReactiveProperty` | отправлять события между системами без жестких связей |

---

## Как запустить

1. Открыть сцену `Main.unity`.
2. Проверить, что на `RootLifetimeScope` проставлены: `animals.csv` (TextAsset), `AnimalViewCatalogSO`, камера, `DeathCounterView`, `FloatingTextPrefab`, `WorldTextParent`.
3. Собрать Addressables (Window → Asset Management → Addressables → Groups → Build → New Build).
4. Play.
