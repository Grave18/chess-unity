# AGENTS Guide for Chess3D.Unity

## Project snapshot
- Unity version is **2022.3.62f3** (`ProjectSettings/ProjectVersion.txt`).
- Main game code lives in `Assets/_Project/Develop/Chess3D` split into `Runtime`, `Editor`, and `Tests` asmdefs.
- Dependency stack is Unity + VContainer + UniTask + UniRx + NoesisGUI + Addressables (`Packages/manifest.json`).

## Big-picture architecture (start here)
- App boot is scene-driven: build scenes are `0.Bootstrap -> 1.Logo -> 2.Menu -> 3.Core -> 4.Online -> 5.Blank` (`ProjectSettings/EditorBuildSettings.asset`).
- DI roots are `LifetimeScope` components:
  - `BootstrapScope` registers config/settings/loading and enters `BootstrapFlow`.
  - `MenuScope` wires Noesis menu UI + menu view models and enters `MenuFlow`.
  - `CoreScope` wires board/game/AI/camera/UI/state machines and enters `CoreFlow`.
- Startup logic is orchestrated via `ILoadUnit` + `LoadingService`; flows call `BeginLoading(...)` in explicit order (`Runtime/Utilities/LoadingService.cs`, `BootstrapFlow.cs`, `MenuFlow.cs`, `CoreFlow.cs`).
- Settings/config data flow:
  - defaults from `Assets/_Project/Resources/Settings.json` and `Assets/_Project/Resources/Configs.json`
  - runtime override from `Application.persistentDataPath/Settings.json` (`Runtime/SettingsContainer.cs`)
  - menu tools regenerate defaults (`Editor/SettingsGenerator.cs`, `Editor/ConfigGenerator.cs`).

## Developer workflows that matter
- Scene switching is done from Unity menu `Chess3D/Scenes/*` with hotkeys `Alt+1..6` (`Editor/SceneLoader.cs`).
- Player build is Unity menu `Chess3D/Build Player` (`Alt+B`), implemented in `PlayerBuilder.Build()`.
- Build uses `Assets/Settings/BuildSettings.asset` values, builds Addressables first, then player, then writes `steam_appid.txt` when enabled (`Runtime/Utilities/Common/BuildTool/PlayerBuilder.cs`).
- Build version auto-increments on each build in `PostBuildProcessor` by mutating `PlayerSettings.bundleVersion`.
- Tests are split into EditMode and PlayMode asmdefs under `Assets/_Project/Develop/Chess3D/Tests`.

## Project-specific conventions/patterns
- Prefer DI registration in scope files over `new` in gameplay code; use `builder.Register*` and `RegisterEntryPoint` patterns in scope classes.
- `ServiceLocator` exists as bridge code and is initialized from scope `RegisterBuildCallback(...)`; use only when DI injection is impractical (`Runtime/Utilities/ServiceLocator.cs`).
- Noesis views typically call `XamlUtils.Load(this)` and rely on file-path-derived XAML paths (`Runtime/Utilities/XamlUtils.cs`). Keep `.xaml` and `.xaml.cs` paired and path-stable.
- Runtime logging is centralized via `Log.*` tags (`Runtime/Utilities/Logging/Log.cs`) and many debug features are compiled out under `GRAVEBOX_PROD` (for example `AppDebugView`, `LoadingService` timings).
- Stockfish AI is an external process (`stockfish-windows-x86-64-avx2.exe`) invoked by relative executable name; keep the exe next to the built player or editor working directory (`Runtime/Core/AI/Stockfish.cs`).

## Integration points and caveats
- NoesisGUI is first-class UI in Menu/Core (`Menu/UI/MainViewLoader.cs`, `Core/UI/*`).
- Addressables are required in build pipeline (`AddressableAssetSettings.BuildPlayerContent()` in `PlayerBuilder`).
- Steam app id defaults to `480` in root `steam_appid.txt` and build settings.
- PurrNet settings exist in `ProjectSettings/PurrNetSettings.asset`, but Online runtime wiring is limited in current Runtime folder.
- PlayMode helpers in `Tests/PlayTests/TestUtils.cs` still reference old scene names (`MainMenuScene`, `GameScene`) and stubbed methods; treat PlayMode tests as partially outdated until aligned with current scenes.

## Existing AI-agent conventions discovered
- The requested convention glob found only `Assets/ThirdParty/PlayerPrefsEditor/README.md` (third-party plugin docs); no repository-specific prior agent instruction files were present.

