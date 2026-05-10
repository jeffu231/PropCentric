# AGENTS.md

This file provides guidance to coding agents working with code in this repository. Keep shared guidance in `AGENTS.md` and `CLAUDE.md` aligned unless one file needs tool-specific wording.

## Build & Run Commands

```bash
# Build
dotnet build PropCentric.sln
dotnet build PropCentric.sln --configuration Release

# Run harness app
dotnet run --project PropCentric/PropCentric.csproj

# Run tests
dotnet test PropCentric.Tests/PropCentric.Tests.csproj

# Clean
dotnet clean PropCentric.sln
```

**Stack:** .NET 10 (`net10.0-windows`), C# with nullable reference types and implicit usings, WPF, Catel MVVM, Orc.Wizard, `Microsoft.Extensions.DependencyInjection`.

## Architecture Overview

PropCentric is a plugin-oriented POC for defining, discovering, configuring, and rendering Props.

### Main Projects

| Project | Role |
|---|---|
| `PropCentric` | Harness app; bootstraps DI |
| `Props.Abstractions` | Core contracts, base types, features, setup and visual interfaces |
| `Props.Registry` | Startup discovery, DI registration, registries, factories |
| `Props.Runtime` | Concrete prop implementations and pipelines |
| `Props.OpenGlCommon` | OpenGL drawing engine and viewer support |
| `Props.WPFCommon` | Shared WPF resources and support code |
| `Props.Wizards` | Wizard-related support code |
| `PropCentric.Tests` | Focused tests for discovery, mapping, and visual generation |

## Startup Discovery

Startup begins in `PropSystemBootstrap.Initialize()` and calls:

```csharp
services.AddPropSystem(path);
```

`AddPropSystem(...)` does the following:

1. Loads plugin assemblies from the target directory.
2. Filters to assemblies whose names start with `Props`.
3. Discovers props decorated with `[PropDescriptor]`.
4. Discovers feature wizard pages decorated with `[FeatureWizardPage]`.
5. Registers registry/factory services.
6. Registers discovered prop and setup types.
7. Automatically registers discovered support services for these generic contracts:
   - `IVisualInputMapper<,>`
   - `IPropVisualModelBuilder<,>`
   - `IPropDraftMapper<,>`
   - `IWizardPreviewCoordinator<>`

Implication:

- new prop support pipelines should be picked up by discovery without a prop-specific DI extension method

## Feature System

Props declare supported capabilities by implementing feature interfaces such as:

- `IHasLights`
- `IHasDimming`
- `IHasColor`
- `IHasSegments`

Each feature interface is decorated with `[PropFeature(...)]`. `PropFeatureInferrer` inspects those interfaces and computes the `PropFeatureFlags` for each discovered prop type.

## Prop Setup Pattern

Each prop has a setup wrapper implementing `IPropSetup`.

The setup wrapper is responsible for:

- creating or accepting the prop instance
- creating a draft model
- populating the draft from the prop
- resolving feature wizard pages
- resolving feature data mappers
- showing the wizard
- applying draft and feature data back into the prop
- calling `await prop.CommitAsync()`

Wizard pages should not edit props directly.

## Draft / Mapping / Visual Pattern

Each prop follows the same general pipeline:

```text
Prop / Draft -> VisualInput -> VisualModelBuilder -> PropVisualModel
```

### Draft

The draft is wizard-owned temporary state used during create/edit flows.

Pattern:

- `{Prop}PropDraft`

### Draft Mapper

Moves data between draft and prop.

Pattern:

- `{Prop}PropDraftMapper`
- `IPropDraftMapper<TDraft, TProp>`

### Visual Input

A transfer object containing the rendering-relevant subset of prop or draft state.

Pattern:

- `{Prop}VisualInput`

Important:

- not every prop field must be part of the visual input
- props may contain additional configuration used for patching or other runtime behavior

### Visual Input Mappers

Two mappers usually project onto the same visual input:

- `{Prop}PropToVisualInputMapper`
- `{Prop}DraftToVisualInputMapper`

### Visual Model Builder

The builder owns the geometry/render-model generation logic.

Pattern:

- `{Prop}VisualModelBuilder`
- `IPropVisualModelBuilder<TVisualInput, TVisualModel>`

## Preview Pattern

Wizard preview uses an `IWizardPreviewCoordinator<TDraft>` implementation to:

- map the draft to visual input
- optionally reuse a previously built preview
- return the visual model used by the drawing engine

Pattern:

- `{Prop}WizardPreviewCoordinator`

## Adding a New Prop

1. Create a concrete prop type decorated with `[PropDescriptor(...)]`.
2. Derive from the appropriate base class and implement the needed feature interfaces.
3. Create the setup wrapper implementing `IPropSetup`.
4. Create the draft type.
5. Create the draft mapper.
6. Create the visual input record.
7. Create the prop-to-visual-input mapper.
8. Create the draft-to-visual-input mapper.
9. Create the visual model builder.
10. Create the wizard preview coordinator.
11. Create the prop-specific wizard page(s).
12. Add tests for discovery, mapping, and visual generation.

If these types are placed in a scanned `Props*` assembly and implement the expected contracts, startup discovery should register the pipeline automatically.

## Current Naming

Use the naming rules documented in:

- `Docs/NamingConventions.md`

Important current patterns:

- `{Prop}Prop`
- `{Prop}PropSetup`
- `{Prop}PropDraft`
- `{Prop}PropDraftMapper`
- `{Prop}VisualInput`
- `{Prop}PropToVisualInputMapper`
- `{Prop}DraftToVisualInputMapper`
- `{Prop}PropVisualModel`
- `{Prop}VisualModelBuilder`
- `{Prop}WizardPreviewCoordinator`

## References

- `Docs/Core_Design_Goals.md` is the architecture intent document used for reviews.
- `Docs/NamingConventions.md` is the naming source of truth.

# ExecPlans

When writing complex features or significant refactors, use an ExecPlan (as described in .agents/PLANS.md) from design to implementation.
