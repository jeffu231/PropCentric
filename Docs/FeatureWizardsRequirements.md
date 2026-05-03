# Feature Wizards

## Overview

All Props can have features that are denoted by the feature flags and the implementation of a corresponding interface. These are discoverable at runtime and already implemented.
It is also desireable for each feature to provide an optional setup wizard page that can capture user input to satisfy the feature interface properties. These feature setup wizards will be added to each Props setup wizard flow based
on whether the Prop supports the feature. Configuration from each feature wizard setup can be read or written to the Prop based on the feature interface.

## Requirements

* Each feature can optionally have a setup wizard page that collects information from the user based on the feature interface. DimmingWizardPage in the project Props.Runtime is an example of a wizard for the Dimming feature.
* Feature setup pages should be discoverable by some means based on the feature or interface definition. There should be a clear pattern for adding the Wizard and associating it to the Feature. It should be decaritive and not rely on some naming pattern.
* A Feature that did not initially have a Wizard can have a Wizard added in the future by a simple pattern.
* Each Feature wizard should have a priority that will be used to determine the order they are added to the Prop setup page flow. Features with a higher priority will be added first.
* Each Prop in its setup will pass the Prop or PRop type to a builder that can assemble the feature pages in order and add them to the Props core setup pages. This should utiliaze the feature registry.
* TreePropSetup creates a TreePropWizard that has the base interfaces that allows additional pages to be added. A TODO marker is in the CreateTreeWizard method of TreePropSetup to denote the likely place for this new dynamic page add logic.
* These feature pages will be inserted before the SummaryWizardPage.
* Each Prop should be able to leverage this universal function to add the feature pages.
* There should be universal logic that can add the data collected from the Feature Wizard page to the Prop in the Create flow based on the feature interface.
* There should be universal logic that can extract the data from a Prop and populate the feature wizard page in the Edit flow based on the feature interface.