# Feature Wizards

## Overview

All Props can have features that are denoted by feature flags and by implementing the corresponding feature interfaces. Those features are discoverable at runtime.

Features can also provide optional setup wizard pages. A feature wizard page can either follow the legacy prop-mapper pattern or the newer draft-backed pattern:

- legacy pages read from the prop before the wizard opens and write back to the prop when the wizard is accepted
- draft-backed pages are initialized with the shared wizard draft and can update preview-driving state immediately during the wizard flow

Feature setup pages are added to a prop's wizard flow when the prop supports the corresponding feature interface.

## Requirements

* Each feature can optionally have a setup wizard page that collects information from the user based on the feature interface. `DimmingFeatureWizardPage` in `Props.Runtime` is the legacy mapper-backed example. `SegmentsFeatureWizardPage` is the draft-backed live-preview example.
* Feature setup pages should be discoverable by attribute metadata rather than naming conventions.
* A feature that did not initially have a wizard can have one added later by following the same discovery pattern.
* Each feature wizard page should declare a priority that determines ordering in the prop setup flow. Higher priority pages are inserted first.
* Each prop setup wrapper should resolve applicable feature pages through the feature-page resolver and insert them before the summary page.
* Each prop should be able to use the same universal feature-page resolution and insertion path.
* Legacy feature pages may declare a mapper type that can populate page state from the prop before the wizard opens and apply page state back to the prop after the user confirms.
* Draft-backed feature pages should implement `IFeatureWizardDraftPage`. The setup wrapper should initialize them with the shared `IPropDraft` and `IWizardPreviewSession` for that wizard instance.
* Preview-driving feature data should live in the shared wizard draft, not in page-owned duplicate copies. This allows a feature page to rebuild the OpenGL preview immediately when the user changes a field.
* Feature pages that host the viewer should use the same OpenGL drawing engine path as prop-specific pages. The feature page should rebuild preview by calling `IWizardPreviewSession.BuildPreview()`.
* `SegmentsFeatureWizardPage` is the reference pattern for a draft-backed live-preview feature page:
  * it targets `IHasSegments`
  * it uses `IHasSegmentsDraft` and `SegmentDraftState`
  * it edits shared draft `PointCount` values directly
  * it hosts the OpenGL preview and reflects edits immediately
