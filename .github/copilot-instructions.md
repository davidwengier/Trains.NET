# Trains.NET repository instructions

## Build, test, and run

Run commands from the repository root. The solution targets .NET 10, uses preview C# features, and includes WPF and Windows Forms hosts, so use a recent .NET 10 SDK on Windows for solution-level builds.

```powershell
# Required before the first solution build, and after workload changes
dotnet workload restore Trains.NET.sln

# CI-equivalent build and code-style/analyser check
dotnet build Trains.NET.sln -p:CI=true -c Debug

# CI release build; writes msbuild.binlog
dotnet build Trains.NET.sln -p:CI=true -c Release -bl

# Full test suite
dotnet test Trains.NET.sln --no-restore

# Main unit-test project
dotnet test tests\Trains.NET.Tests\Trains.NET.Tests.csproj --no-restore

# One test
dotnet test tests\Trains.NET.Tests\Trains.NET.Tests.csproj --no-restore --filter "FullyQualifiedName=Trains.NET.Tests.ImageCacheTests.Set"

# Primary WPF application
dotnet run --project src\Trains\Trains.csproj

# Blazor WebAssembly host
dotnet run --project src\BlazingTrains\BlazingTrains.csproj
```

There is no separate lint target. `-p:CI=true` enables warnings as errors, Roslyn analysers, and EditorConfig code-style enforcement.

A push to `main` runs `.github/workflows/dotnetcore.yml` unless every changed path matches its `*.md` ignore pattern. The workflow builds and tests, publishes the WPF executable as a GitHub release, and deploys the Blazor host to `gh-pages`.

## Architecture

- `Trains.NET.Engine` is the platform-neutral simulation and persistence layer: terrain, layouts, tracks, trains, tools, the game loop, and save/load state.
- `Trains.NET.Rendering` is platform-neutral rendering and interaction code. It renders through `ICanvas`/`IImageFactory`, maps coordinates through `IPixelMapper`, and composes ordered board layers and screens.
- `Trains.NET.Rendering.Skia` adapts the rendering abstractions to SkiaSharp and owns SVG-backed drawing assets.
- `Trains.NET.Instrumentation` supplies the lightweight counters and timing stats used by the engine, renderer, and hosts.
- `Trains`, `WinTrains`, `BlazingTrains`, and `SilkTrains` are WPF, Windows Forms, Blazor WebAssembly, and Silk.NET hosts. They resolve the shared `IGame` and `IInteractionManager`, translate native input/render events, and provide platform-specific storage or timing where needed. `Trains.Emoji` is a console image generator using the same renderers.
- `Trains.NET.SourceGenerator` supplies the generated `DI.ServiceLocator` and converts SVG `AdditionalFiles` into Skia picture classes. Host projects reference it as an analyser rather than a runtime assembly.

The runtime flow is: host events -> `IInteractionManager`/`IGame` -> engine state and ordered renderers -> `ICanvas` -> the host's Skia surface.

## Repository conventions

### Generated dependency injection

- There is no hand-written registration table. Calls to `DI.ServiceLocator.GetService<T>()` are roots for `DISourceGenerator`, which finds implementations and recursively constructs their constructor dependencies.
- Services are singleton by default. Put `[Transient]` on an interface when every resolution needs a new instance.
- `IEnumerable<T>` injection includes every implementation, sorted by `[Order]`. A normal interface resolution selects the first ordered implementation. Keep injectable types to one intended constructor.
- Ordering is behaviour: render layers and initializers run in ascending order; `InteractionManager` reverses its ordered handlers so higher-order UI handlers receive input first. `GameStateManager` deliberately uses `[Order(999999)]` so persisted state loads after other initialization.
- `ILayout<T>` is special-cased by the generator and resolves to a `FilteredLayout<T>` over the shared `ILayout`.
- Do not edit generated `ServiceLocator.cs` files under `obj`; change interfaces, implementations, constructors, `[Order]`, or `[Transient]` instead.

### Engine lifecycle and state

- Implement `IInitializeAsync` for services that must initialize when `IGame.InitializeAsync` runs.
- Implement `IGameStep` for services updated by the roughly 16 ms game loop.
- Implement `IGameState` for state included in all-or-nothing load/reset/save processing by `GameStateManager`.
- Mutate static entities through `ILayout.Add`, `Set`, and `Remove`. These methods set coordinates, invoke entity lifecycle hooks (`Stored`, `Created`, `Replaced`, `Removed`), and raise `CollectionChanged`.
- `IStaticEntity.Identifier` represents visual/state identity, not position. Static renderers use it as an image-cache key.

### Rendering and interaction

- Keep shared rendering against abstractions in `Trains.NET.Rendering`; platform Skia types belong in `Trains.NET.Rendering.Skia` or host glue.
- New layer renderers, screens, interaction handlers, tools, factories, initializers, and game steps are discovered through their interfaces. Use `[Order]` deliberately rather than adding manual lists.
- `ICachableLayerRenderer` and `IScreen` implementations must raise `Changed` whenever their rendered output changes. `Game` listens to those events and invalidates `ImageCache`; missing notifications produce stale frames.
- `StaticEntityCollectionRenderer<T>` already forwards layout changes and caches each visual entity state. Derive from it for normal static board layers instead of rebuilding that mechanism.

### Projects, packages, generated assets, and tests

- Package versions are centralised in `Directory.Packages.props`; project files normally use versionless `PackageReference` entries.
- Shared defaults in `Directory.Build.props` enable nullable reference types, implicit usings, preview language features, strict features, and .NET analysers. Tests inherit them through `tests\Directory.Build.props`, which also adds global xUnit usings.
- Follow `.editorconfig`: C# files are UTF-8 with BOM, trailing whitespace is trimmed, and CI enforces its style diagnostics.
- SVGs in `src\Trains.NET.Rendering.Skia\svg` are `AdditionalFiles`. `SvgSourceGenerator` generates `Svg_<file_name>` classes in `Trains.NET.Rendering.Skia.Assets`; edit the SVG or project metadata, never generated output.
- `src\Trains\Storage\FileSystemStorage.cs` is linked into other desktop hosts from their project files. A change there affects more than the WPF project.
- Unit tests commonly derive from `TestBase`, which builds a deterministic engine graph with `TestTimer` instead of using generated DI. Update that explicit graph when constructor dependencies change.
