# Feature Wizards

## Overview

All Props can have features that are denoted by feature flags and by implementing the corresponding feature interfaces. Those features are discoverable at runtime.

Features can also provide optional setup wizard pages. Every feature wizard page should follow the shared draft-backed pattern:

- pages initialize once per wizard instance from `FeatureWizardContext`
- pages treat the shared wizard draft as the canonical backing state
- pages can update preview-driving state immediately during the wizard flow through the shared `IWizardPreviewSession`

Feature setup pages are added to a prop's wizard flow when the prop supports the corresponding feature interface.

## Requirements

* Each feature can optionally have a setup wizard page that collects information from the user based on the feature interface. `SegmentsFeatureWizardPage` is the live-preview reference example, and `ColorFeatureWizardPage` is the non-viewer reference example.
* Feature setup pages should be discoverable by attribute metadata rather than naming conventions.
* A feature that did not initially have a wizard can have one added later by following the same discovery pattern.
* Each feature wizard page should declare a priority that determines ordering in the prop setup flow. Higher priority pages are inserted first.
* Each prop setup wrapper should resolve applicable feature pages through the feature-page resolver and insert them before the summary page.
* Each prop should be able to use the same universal feature-page resolution and insertion path.
* Feature wizard pages should implement `IFeatureWizardDraftPage`. The setup wrapper should initialize them once per wizard instance with a shared `FeatureWizardContext`.
* Preview-driving feature data should live in the shared wizard draft, not in page-owned duplicate copies. This allows a feature page to rebuild the OpenGL preview immediately when the user changes a field.
* Feature pages that host the viewer should use the same OpenGL drawing engine path as prop-specific pages. The feature page should rebuild preview by calling `IWizardPreviewSession.BuildPreviewAsync(...)`.
* `SegmentsFeatureWizardPage` is the reference pattern for a live-preview feature page:
  * it targets `IHasSegments`
  * it uses `IHasSegmentsDraft` and `SegmentDraftState`
  * it edits shared draft `PointCount` values directly
  * it hosts the OpenGL preview and reflects edits immediately
* `RotationFeatureWizardPage` is the reference pattern for an axis-rotation feature page:
  * it targets `ICanAxisRotate`
  * it uses `IHasAxisRotationsDraft`
  * it edits shared draft `AxisRotations` values directly
  * those `AxisRotations` define baseline setup-time prop orientation only
  * they are not the same thing as runtime motion/state such as pan, tilt, elevation, or other rendered fixture movement
* `ColorFeatureWizardPage` is the reference pattern for a reusable color-settings page:
  * it targets `IHasColor`
  * it uses `IHasColorSettingsDraft`
  * it edits shared `LightColorConfiguration` state directly
  * it does not host the OpenGL prop preview viewer
* `DimmingFeatureWizardPage` follows the same shared-draft pattern for `IHasDimming` props:
  * it targets `IHasDimming`
  * it uses `IHasDimmingSettingsDraft`
  * it edits shared `Brightness` and `Gamma` state directly
  * it can participate in preview through the shared session without requiring a separate backing model
