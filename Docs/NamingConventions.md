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

## Feature Wizard Page Pattern

| Type | Naming |
|---|---|
| Wizard page | `{Feature}WizardPage` (e.g. `DimmingWizardPage`) |
| Data mapper | `{Feature}WizardDataMapper` (e.g. `DimmingWizardDataMapper`) |
| View model | `{Feature}WizardPageViewModel` (e.g. `DimmingWizardPageViewModel`) |

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
