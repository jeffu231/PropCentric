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
4. Follow the three-layer visual model pattern (see below) for geometry and live wizard preview
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

### Three-Layer Visual Model Pattern

Every prop's geometry pipeline uses three layers so the wizard preview, the runtime prop, and the geometry algorithm stay fully decoupled:

```
Draft / Prop  ──►  VisualInput (record)  ──►  VisualFactory  ──►  IPropVisualModel
```

**Layer 1 — Draft (`IPropDraft`)**: wizard-owned POCO holding only the user-entered fields. Excludes feature state (Brightness, Gamma — those stay in feature wizard pages). `TreePropDraft` is the canonical example.

**Layer 2 — Visual Input**: a `sealed record` (e.g., `TreeVisualInput`) whose positional parameters are exactly the fields that affect geometry. Using a `record` gives free structural equality; `IWizardPreviewCoordinator` caches the last input and skips factory calls when nothing changed.

**Layer 3 — Visual Factory (`IPropVisualModelFactory<TVisualInput>`)**: a pure function that owns the geometry algorithm and produces a fresh `IPropVisualModel` from an input record. The factory is the single owner of the geometry logic — it lives here and nowhere else.

**Two input mappers** project onto the same record:
- `IVisualInputMapper<TDraft, TVisualInput>` — used by the wizard preview coordinator
- `IVisualInputMapper<TProp, TVisualInput>` — used by the prop's `BuildVisualModel()` at runtime

**Draft mapper (`IPropDraftMapper<TDraft, TProp>`)**: `PopulateDraft(draft, prop)` copies prop → draft before the wizard opens; `ApplyDraft(draft, prop)` copies draft → prop after the user confirms. The mapper is a pure field copier — no side effects.

**Element-node generation**: After `ApplyDraft` and all `mapper.ApplyTo(prop)` calls, the setup orchestrator calls `await prop.CommitAsync()`. `IProp.CommitAsync()` is a lifecycle hook — `BaseProp` returns `Task.CompletedTask`; `BaseLightProp` overrides it to call the protected `GenerateElementsAsync()`. Element nodes are built once at commit time, never during wizard editing.

**Wizard preview wiring**:
1. `IPropSetup` creates the draft, constructs `TreePropWizardPage(draft, coordinator)`, and passes the page to the wizard
2. Page properties forward directly to draft fields (`get => _draft.X; set { _draft.X = value; RaisePropertyChanged(...); }`)
3. Parent-class Catel properties (`Name`, `LightSize`) sync to the draft via a `PropertyChanged` subscription on the page
4. `GraphicsWizardPageViewModelBase` debounces all `PropertyChanged` events (150 ms) and calls `TriggerPreviewRebuild()`
5. The concrete ViewModel sets `PreviewBuilder` — a closure that syncs rotation state to the draft then calls `coordinator.BuildPreview(draft)`
6. `IWizardPreviewCoordinator` maps draft → `TVisualInput`, skips factory if unchanged, otherwise calls the factory and caches the result
7. `InitializeAsync` calls `TriggerPreviewRebuild()` so the preview is populated when the wizard first opens

**DI registration per prop** (add to your `*ServicesExtensions.cs`):
```csharp
services.AddTransient<IVisualInputMapper<TProp, TVisualInput>, TPropToVisualInputMapper>();
services.AddTransient<IVisualInputMapper<TDraft, TVisualInput>, TDraftToVisualInputMapper>();
services.AddTransient<IPropVisualModelFactory<TVisualInput>, TVisualModelFactory>();
services.AddTransient<IPropDraftMapper<TDraft, TProp>, TPropDraftMapper>();
services.AddTransient<IWizardPreviewCoordinator<TDraft>, TWizardPreviewCoordinator>();
```

**Drawing engine (`IPropDrawingEngine`)**: defined in `Props.Abstractions.Drawing`. Accepts `IReadOnlyList<IPropVisualModel>` via `SetModels`. The wizard passes a single-element list; the world view passes all active models. The OpenTK/WPF implementation is deferred until the wizard preview control is connected.

### Key Patterns

- **Plugin architecture:** dynamic assembly loading + `[PropDescriptor]` attribute scanning; load failures captured in `AssemblyLoadResult` (registered as singleton)
- **Feature discovery:** attribute-driven (`[PropFeature]`) + `PropFeatureInferrer` (injected singleton, self-initializing — replaces the former static `PropFeatureRegistry`)
- **Catalog vs factory separation:** `IPropCatalogProvider` owns discovery; `IPropFactory` and `IWizardFactory` own creation
- **DI-backed factories:** `IServiceProvider` used for all runtime instantiation
- **Feature wizard pages:** `[FeatureWizardPage(featureInterface, mapperType)]` marks a pure-UI page; its companion `IFeatureWizardDataMapper` owns all prop casting and data conversion. Pages are created via MEDI transient registration; mappers via `ActivatorUtilities` with the page as a constructor argument. Reflection runs once at startup (`FeatureWizardPageScanner`); `IPropSetup` classes call `GetPagesFor` / `GetMappersFor` with no runtime reflection.
- **Visual model:** see Three-Layer Visual Model Pattern above