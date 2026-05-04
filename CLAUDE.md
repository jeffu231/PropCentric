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
3. `FeatureWizardPageScanner` discovers types decorated with `[FeatureWizardPage]` via reflection, capturing the feature interface, optional mapper type, and priority into `FeatureWizardPageRegistration` records
4. All discovered prop, wizard, and feature page types are registered as Transient in the DI container
5. `PropFeatureInferrer` (singleton) self-initializes from `Props.Abstractions` — no manual `Initialize()` call required

### Feature System

Props declare capabilities via `PropFeatureFlags` (a `[Flags]` enum: `Lights`, `Color`, `Dimming`, `Segments`, `Orientation`, `Face`, `State`) and implement the corresponding interfaces (`IHasLights`, `IHasColor`, `IHasDimming`, etc.).

Each feature interface is decorated with `[PropFeature(flag)]`. `PropFeatureInferrer.Infer(type)` uses this attribute to automatically compute the combined flags for any prop type. `PropRegistry` indexes props by feature for querying.

### Adding a New Prop

1. Create a class decorated with `[PropDescriptor(id, name, wizardType, icon)]`
2. Implement `IProp` (via the `Prop` base class) plus any feature interfaces (`IHasLights`, `IHasDimming`, etc.)
3. Implement `IPropSetup` for the wizard flow
4. Optionally implement `IPropVisualModelBuilder` for 3D geometry (uses `System.Numerics.Vector3`)
5. The plugin scanner picks it up automatically at runtime — no manual registration needed

### Adding a Feature Wizard Page

Feature wizard pages let any prop that implements a given feature interface automatically gain that page in its wizard, without the prop or its setup class needing to know about it.

1. Create a page class extending `WizardPageBase` decorated with `[FeatureWizardPage(typeof(IHasFeature), mapperType: typeof(YourMapper))]` — the page holds UI state only, no prop references
2. Create a companion mapper `class YourMapper(YourPage page) : IFeatureWizardDataMapper` — this owns all casting to the feature interface and data conversion between UI and prop representations
3. The scanner picks both up automatically; `IFeatureWizardPageResolver` injects the page (via MEDI) and creates the mapper (via `ActivatorUtilities` with the page as a constructor argument)
4. In each `IPropSetup` implementation, call `featurePageResolver.GetPagesFor(propType)` to get page instances and `featurePageResolver.GetMappersFor(pages)` to get mapper instances; iterate mappers directly with `foreach` for populate/apply

### Factory & Catalog Pattern

- `IPropCatalogProvider.GetPropCatalog()` — returns `IPropCatalogItem` entries for all discovered props (use this for discovery, not `IPropFactory`)
- `IPropCatalogProvider.GetPropCatalogByFeature(flags)` — filter catalog by feature flags
- `IPropFactory.Create<TProp>()` / `IPropFactory.Create(Guid)` — creates a prop instance via DI; returns `IProp`
- `IFeatureWizardPageResolver.GetPagesFor(Type propType)` — returns instantiated feature wizard pages for all features the prop implements, ordered by priority
- `IFeatureWizardPageResolver.GetMappersFor(IReadOnlyList<IWizardPage> pages)` — returns instantiated `IFeatureWizardDataMapper` instances paired to the given pages; call `PopulateFrom(prop)` before showing the wizard and `ApplyTo(prop)` after

### Key Patterns

- **Plugin architecture:** dynamic assembly loading + `[PropDescriptor]` attribute scanning; load failures captured in `AssemblyLoadResult` (registered as singleton)
- **Feature discovery:** attribute-driven (`[PropFeature]`) + `PropFeatureInferrer` (injected singleton, self-initializing — replaces the former static `PropFeatureRegistry`)
- **Catalog vs factory separation:** `IPropCatalogProvider` owns discovery; `IPropFactory` and `IWizardFactory` own creation
- **DI-backed factories:** `IServiceProvider` used for all runtime instantiation
- **Feature wizard pages:** `[FeatureWizardPage(featureInterface, mapperType)]` marks a pure-UI page; its companion `IFeatureWizardDataMapper` owns all prop casting and data conversion. Pages are created via MEDI transient registration; mappers via `ActivatorUtilities` with the page as a constructor argument. Reflection runs once at startup (`FeatureWizardPageScanner`); `IPropSetup` classes call `GetPagesFor` / `GetMappersFor` with no runtime reflection.
- **Visual builder:** `IPropVisualModelBuilder.Build()` → `IVisualElement` hierarchy (`PointCloud`, `LineSegment`, `Mesh`)