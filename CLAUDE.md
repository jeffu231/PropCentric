# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Build
dotnet build PropCentric.sln
dotnet build PropCentric.sln --configuration Release

# Run console app
dotnet run --project PropCentric/PropCentric.csproj

# Clean
dotnet clean PropCentric.sln
```

**Stack:** .NET 10.0 (`net10.0`), C# with nullable reference types and implicit usings, `Microsoft.Extensions.DependencyInjection` for IoC.

## Architecture Overview

PropCentric is a **plugin-based system for managing stage/lighting Props** (physical equipment with configurable capabilities). It has 7 projects:

| Project | Role |
|---|---|
| `PropCentric` | Console app entry point; bootstraps DI container |
| `Props.Abstractions` | Core interfaces, types, attributes, feature flags |
| `Props.Registry` | Plugin discovery (reflection), DI registration, factories |
| `Props.Runtime` | Example `TreeProp` implementation |
| `Props.Adapter` | Adapter layer (currently empty, reserved for external system integration) |
| `Props.Wizards` | Wizard UI page components |
| `Vixen.Core` | Domain primitives (`ElementNode`, `Segment`) |

### Startup Flow

`Program.cs` → `PropSystemBootstrap.Initialize()` → `IServiceCollection.AddPropSystem(pluginDirectory)`:
1. `AssemblyLoader.LoadAll()` loads `.dll` files from the plugin directory, returning an `AssemblyLoadResult` (captures failures without crashing; register `throwOnAssemblyLoadFailure: true` in dev to fail fast)
2. `PropScanner` discovers types decorated with `[PropDescriptor]` via reflection
3. All discovered prop and wizard types are registered as Transient in the DI container
4. `PropFeatureInferrer` (singleton) self-initializes from `Props.Abstractions` — no manual `Initialize()` call required

### Feature System

Props declare capabilities via `PropFeatureFlags` (a `[Flags]` enum: `Lights`, `Color`, `Dimming`, `Segments`, `Orientation`, `Face`, `State`) and implement the corresponding interfaces (`IHasLights`, `IHasColor`, `IHasDimming`, etc.).

Each feature interface is decorated with `[PropFeature(flag)]`. `PropFeatureInferrer.Infer(type)` uses this attribute to automatically compute the combined flags for any prop type. `PropRegistry` indexes props by feature for querying.

### Adding a New Prop

1. Create a class decorated with `[PropDescriptor(id, name, wizardType, visualModelType, icon)]`
2. Implement `IProp` (via the `Prop` base class) plus any feature interfaces (`IHasLights`, etc.)
3. Implement `IPropSetupWizard<TYourProp>` for the wizard
4. Optionally implement `IPropVisualModelBuilder` for 3D geometry (uses `System.Numerics.Vector3`)
5. The plugin scanner picks it up automatically at runtime — no manual registration needed

### Factory & Catalog Pattern

- `IPropCatalogProvider.GetPropCatalog()` — returns `IPropCatalogItem` entries for all discovered props (use this for discovery, not `IPropFactory`)
- `IPropCatalogProvider.GetPropCatalogByFeature(flags)` — filter catalog by feature flags
- `IPropFactory.Create<TProp>()` / `IPropFactory.Create(Guid)` — creates a prop instance via DI; returns `IProp`
- `IWizardFactory.CreateWizard<TProp>()` — creates a wizard; call `.CreateAsync()` / `.EditAsync()` to configure a prop
- `IPropSetupStepWizardPage` — individual pages within a wizard; `FeatureWizardResolver` is the extension point for feature-specific pages

### Key Patterns

- **Plugin architecture:** dynamic assembly loading + `[PropDescriptor]` attribute scanning; load failures captured in `AssemblyLoadResult` (registered as singleton)
- **Feature discovery:** attribute-driven (`[PropFeature]`) + `PropFeatureInferrer` (injected singleton, self-initializing — replaces the former static `PropFeatureRegistry`)
- **Catalog vs factory separation:** `IPropCatalogProvider` owns discovery; `IPropFactory` and `IWizardFactory` own creation
- **DI-backed factories:** `IServiceProvider` used for all runtime instantiation
- **Visual builder:** `IPropVisualModelBuilder.Build()` → `IVisualElement` hierarchy (`PointCloud`, `LineSegment`, `Mesh`)