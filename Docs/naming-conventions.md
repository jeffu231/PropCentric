# Naming Conventions

This document defines the go-forward naming patterns for PropCentric. Apply these consistently as new props, features, and wizard pages are added.

## Type Roles

### Scanner
Discovers metadata from assemblies at startup. Returns a collection of `*Descriptor` types.

```
FeatureWizardPageScanner.Scan(assemblies) → IReadOnlyList<FeatureWizardPageDescriptor>
PropScanner.Scan(assemblies)              → IReadOnlyList<PropDescriptor>
```

### Descriptor
An immutable sealed record capturing discovered metadata about a type. Named `{Subject}Descriptor`.

```
PropDescriptor              — metadata for a discovered prop type
FeatureWizardPageDescriptor — metadata for a discovered feature wizard page type
```

### Registry
Indexes descriptors for fast lookup. Method naming:

```
GetDescriptorById(Guid id)         — lookup by primary key
GetDescriptorForProp(IProp prop)   — lookup from a live instance
GetAllDescriptors()                — unfiltered collection
GetDescriptorsByFeature(PropFeatureFlags flags) — filtered by feature flags
```

Parameter name for feature flag arguments is always `flags` (plural, because `PropFeatureFlags` is a `[Flags]` enum).

### Factory
Creates instances. Method naming follows a source-oriented convention:

```
Create(Guid id)                            — create by primary key
CreateFromCatalogItem(IPropCatalogItem)    — create from a catalog entry
CreateFor*(target)                         — create for a specific target (reserved for future use)
```

Do not include the noun the factory produces in its own method names (e.g. avoid `CreateSetup` on `IPropSetupFactory`).

### Builder
Builds a derived model from input data. Use `Builder` when the type owns geometry or composition logic rather than simple instance construction.

Method naming:

```
Create(input)        — build a derived model from a transfer input
Apply*(target, ...)  — apply secondary state or transforms to an existing target (reserved for future use)
```

Examples:

```
TreeVisualModelBuilder
IPropVisualModelBuilder<TVisualInput, TVisualModel>
```

### Resolver
Maps an input type to resolved instances. Method naming uses `Get*For`:

```
IFeatureWizardPageResolver.GetPagesFor(Type propType)
IFeatureWizardPageResolver.GetMappersFor(IReadOnlyList<IWizardPage> pages)
```

### Provider
Returns collections for UI or external consumption. Method naming uses `Get*`:

```
IPropCatalogProvider.GetPropCatalog()
IPropCatalogProvider.GetPropCatalogByFeature(PropFeatureFlags flags)
```

## Feature Wizard Pattern

Feature wizard components must include `Feature` in the type name to distinguish them from prop-specific wizard flow types.

### Feature Wizard Folder Structure

Within `Props.Runtime`, organize reusable wizard infrastructure under `Wizards/Core` and feature-specific wizard components under `Wizards/Features/{Feature}`.

Pattern:

```text
Wizards/
  Core/
    Pages/
    ViewModels/
    Views/

  Features/
    {Feature}/
      Mappers/
      Pages/
      ViewModels/
      Views/
```

### Feature Wizard Namespace Structure

Namespaces should match folders exactly.

Core namespaces:

```
Props.Runtime.Wizards.Core
Props.Runtime.Wizards.Core.Pages
Props.Runtime.Wizards.Core.ViewModels
Props.Runtime.Wizards.Core.Views
```

Feature namespaces:

```
Props.Runtime.Wizards.Features.{Feature}.Mappers
Props.Runtime.Wizards.Features.{Feature}.Pages
Props.Runtime.Wizards.Features.{Feature}.ViewModels
Props.Runtime.Wizards.Features.{Feature}.Views
```

### Feature Wizard Type Naming

| Type | Naming |
|---|---|
| Wizard page | `{Feature}FeatureWizardPage` (e.g. `DimmingFeatureWizardPage`) |
| Data mapper | `{Feature}FeatureWizardDataMapper` (e.g. `DimmingFeatureWizardDataMapper`) |
| View model | `{Feature}FeatureWizardPageViewModel` (e.g. `DimmingFeatureWizardPageViewModel`) |
| View | `{Feature}FeatureWizardPageView` (e.g. `DimmingFeatureWizardPageView`) |

## Prop Pipeline Pattern

For prop-specific setup, mapping, and rendering types, use the prop name as the prefix and the pipeline role as the suffix.

| Type | Naming |
|---|---|
| Prop type | `{Prop}Prop` |
| Setup wrapper | `{Prop}PropSetup` |
| Draft model | `{Prop}PropDraft` |
| Draft mapper | `{Prop}PropDraftMapper` |
| Visual input record | `{Prop}VisualInput` |
| Prop -> visual input mapper | `{Prop}PropToVisualInputMapper` |
| Draft -> visual input mapper | `{Prop}DraftToVisualInputMapper` |
| Visual model | `{Prop}PropVisualModel` |
| Visual model builder | `{Prop}VisualModelBuilder` |
| Preview coordinator | `{Prop}WizardPreviewCoordinator` |
| Prop wizard flow | `{Prop}PropWizard` |
| Core prop wizard page | `{Prop}PropWizardPage` |
| Core prop wizard page view model | `{Prop}PropWizardPageViewModel` |

Examples:

```
TreeProp
TreePropSetup
TreePropDraft
TreePropDraftMapper
TreeVisualInput
TreePropToVisualInputMapper
TreeDraftToVisualInputMapper
TreePropVisualModel
TreeVisualModelBuilder
TreeWizardPreviewCoordinator
TreePropWizard
TreePropWizardPage
TreePropWizardPageViewModel

PolyLineProp
PolyLinePropSetup
PolyLinePropDraft
PolyLinePropDraftMapper
PolyLineVisualInput
PolyLinePropToVisualInputMapper
PolyLineDraftToVisualInputMapper
PolyLinePropVisualModel
PolyLineVisualModelBuilder
PolyLineWizardPreviewCoordinator
PolyLinePropWizard
PolyLinePropWizardPage
PolyLinePropWizardPageViewModel
```

## Mapper Naming

Use `Mapper` for translation-only types that project state from one shape into another. Name the type from source to destination.

Pattern:

```
{Source}To{Destination}Mapper
```

Examples:

```
TreePropToVisualInputMapper
TreeDraftToVisualInputMapper
```

Use `{Prop}PropDraftMapper` specifically for the paired draft/prop copier role because it represents a bidirectional setup contract rather than a one-way projection.

## Draft Naming

Use `Draft` for wizard-owned temporary state that exists during create/edit flows.

Pattern:

```
{Prop}PropDraft
IPropDraft
IPropDraftMapper<TDraft, TProp>
```

`Draft` should imply:

- temporary setup/edit state
- not the live prop
- safe to discard if the wizard is cancelled

Feature-specific draft capability interfaces should live under `Props.Abstractions.Setup.Drafts` and be named from the state they expose. Examples:

```
IHasSegmentsDraft
IHasAxisRotationsDraft
```

These are setup-only contracts. They are not runtime prop feature interfaces.

## Visual Input Naming

Use `VisualInput` for transfer objects that carry only the subset of data required to build a visual model.

Pattern:

```
{Prop}VisualInput
```

`VisualInput` should imply:

- rendering-focused transfer state
- not the full prop state
- suitable for both runtime prop rendering and wizard preview rendering

## Base Classes and Interfaces

Use the `*Base` **suffix**, never the `Base*` prefix:

```
PropWizardPageBase    ✓
BasePropWizardPage    ✗

WizardPageBase        ✓
BaseWizardPage        ✗
```

## Local Variables

In metadata-heavy code (registries, factories, scanners), use the full descriptor name rather than single-letter abbreviations:

```csharp
// ✓
var descriptor = registry.GetDescriptorById(id);

// ✗
var d = registry.GetDescriptorById(id);
```

Single-letter loop variables (`i`, `flag`) remain acceptable in short, obvious contexts.

## AxisRotation Naming

When the abstraction is specifically about baseline prop-definition axis rotations, prefer `AxisRotation` in the name instead of generic `Rotation`.

Examples:

```
AxisRotationModel
AxisRotationCollectionFactory
AxisRotations
ICanAxisRotate
IHasAxisRotationsDraft
```

Use this naming for setup-time baseline prop orientation. Do not reuse `AxisRotation` naming for runtime rendered state such as fixture pan, tilt, elevation, or other animated motion. Those are separate concepts and should be named from their rendered-state behavior.
