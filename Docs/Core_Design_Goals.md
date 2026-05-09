# Core Design Goals

## Overview

This is a POC for the Vixen application to allow for creating Props and the setup wizards to allow the user to create and edit them.

## Vixen Baseline

This POC is to improve the design of the current Vixen Prop Centric feature branch. The target code is located here. [Vixen Prop Centric Feature](https://github.com/VixenLights/Vixen/tree/feature/VIX-3693).
If successful this POC will be used as a pattern to refactor the Vixen feature branch of code into a better design for handling Props.
This will be limited to Props and their Visual Models.

IProp/Prop in this POC is a direct mapping to the IProp/Prop in the feature branch.
PropVisualModel in this POC is a direct mapping to the PropModel in the feature branch. PropModel will be renamed to PropVisualModel.

## Key Features

* Props are the core data object that describes the state and features of a Prop.
* Each overall Prop type will extend from the IProp interface and add feature interfaces to define its behaviors.
* Props can have several features like color, dimming, segments, face, states, fixture, etc.
* There will be many implementations of Prop that model things like a tree, light strings, candy canes, arches, DMX fixtures, etc.
* Props can be rotated around any axis to align with the users real prop as part of the core Prop. This will allow Props like a candy cane to be rotated 90 degress from the standard view.
* Props have visual models that allow thier visual to be drawn in an OpenGL based viewer.
* There are two main types of viewers. 
  * A setup viewer that shows in some wizard pages to allow the user to see a sample of what the Prop will look like.
  * A preview that allows the user to model a scene of all the Props in one view. This is will give the world view of the complete setup.
* An individual Prop creation flow will allow for the user to create a group of Props that are all the same except for the name. The grouping wizard and creation of clones is outside the scope of this POC.

## Key Design Considerations

### Prop

* Props will own all their state / configuration data.
* Props should be able to declare features by implementing standard interfaces for the features they provide.
* Props should have some base classes that can provide core implmentations for things like light based props or fixture based props.
* The Prop Setup wrapper will be responsible for orchestrating the create and edit of a Prop. This will accept / return an IProp interface for edit and return a PropGroup for create.
* The Prop Setup wrapper will map the Prop specific data from the Prop Wizard page into the Prop and orchestrate the Feature Data Mappers.

### Features

* Features should have a Feature Flag and a cooresponding interface that should be implemented by the Prop.
* Features may have Wizard setup pages to handle thier data described in the feature interfaces.

### Discovery
 
* The discovery mechanism should infer the features a Prop has from the feature interfaces and build a Feature Flag set for that Prop Type the registry can provide.
* Prop types should be discoverable and itemized in a registry. Avoid looking through assemblies that are not part of this solution. For this POC, that will be assemblies starting with Props.
* Features should be discoverable and itemized in a registry.
* Features that a Prop provides will be inferred during discovery and be available via a registry.

### Prop Visual Models

* Each Prop will have a visual model that knows how to draw what a Prop looks like. It will contain all drawing logic and provide standard vector of mesh type geometry that can be drawn in OpenGl.
* Visual models should just contain drawing logic and resulting vector or mesh information that the viewer can draw.
* Simple light points can be modeled using a LightPoint object and a LightPointCloud to associate groups of LightPoints in a structural way.
* Prop data that is necessary for the drawing will be passed into the Visual Models via a simple record type transfer object. Visual models should not be used as a data model for the Prop.
* Visual elements like LightPoint need to have mapping to the ElementNode element to obtain intent state to know if they are visible and what color they are for each render frame in the full world view preview. A element guid with a look up table is currently used in the Vixen system and a similar approach should be maintained.

### Rotations

* Props can have rotations applied at the Prop level as part of thier setup design to rotate the basic prop orientation.
* Rotations can be in any plane for 3D use. One rotation in each plane will be allowed. 
* Rotations will have an order in which they are applied to allow the user to get the state they desire. Example rotate X then Y. Or Y then X, then Z.
* Visual Models are built using standard input and then rotations are applied as a transform to the completed model.
* Viewers can apply their own rotations on top of the Prop based ones. The will likely only be a world view option initially and is to facilitate making the prop more accurate in the world space.

### Wizards

* Each Prop should have a setup process that provides for a Wizard flow that can collect information about the Prop from the user.
* The Prop implementation should provide its core Wizard pages that are specific to the Prop.
* Features will have Wizard pages that can be used to setup each feature. A Wizard page for a feature is optional.
* Feature Wizard pages will be discoverable alongside the Feature itself and provide some type of registry look them up.
* Feature Wizard pages will also be able to declare a data mapper to map the data they collect into a feature interface on a Prop.
* Wizard pages of any type should not edit or have any awareness of a Prop directly. They should use their own model and then use a mapping flow and the feature data mappers to pull data from the Wizard and populate the Prop.
* Wizards should have base interfaces and classes that can provide for many of the common functions that a Wizard may have. These include things like the ability to have a viewer that can draw the Prop Visual Model during setup.
* The Wizards should be able to display a visual of the Prop as they are collecting data without modifying the Prop using the Drawing Engine. The wizard will pass the updated transfer record into the Visual Model to redraw.

### Drawing Engine

* A core drawing engine should be able to draw any Prop on a background image using the Prop Visual Model. 
* The drawing engine should be able to render a single Prop in a simple view, or translate it into a world view with many other Props.
* The drawing engine will support features like pan and zoom. The wizards will not initially utilize pan and zoom and will used a fixed setting that shows the Prop filling most of the viewer window.
* The drawing engine in the wizard preview viewer does not need mapping from its Visual Elements to discover intent state for drawing. The wizards will generally use a default color like white for drawing color on points. The Element mapping should be ignored by the viewer in the wizards.
* The drawing engine should be able to draw on a simple color background, or an image. 

### World View Previews

* The world view preview will not be specifically modeled in this POC, but it needs to be represented in the design.
* The user can define which Props appear in the world view preview.
* The user can define the placement location in the world view preview.
* The same core rendering engine should be used for the world view preview as the wizard viewer.
* Pan and Zoom will be supported.
* Reset to standard camera views will be supported to reset any pan and zoom operations to something normal.
* The user can further rotate a Prop when placing in the world view. This is applied as a transform on the finished prop visual model.

## Core Logic and Library Requirements

* Use .NET 10 with C# as the language.
* Use Catel MVVM with WPF for all UI flows. Catel version 6.2. [Catel](https://github.com/Catel/Catel/tree/master)
* Orc.Wizard will be used for all Wizard flows. Version 5.2. [Orc.Wizard](https://github.com/WildGums/Orc.Wizard)
* Orchestra.Core will be used to support the Wizards. No expansion beyond Wizard support. Version 7.3. [Orchestra](https://github.com/WildGums/Orchestra)
* Reflection should be limited to the one time startup discovery process of Props, Features, Feature Wizard Pages, and Feature Data Mappers. In all other cases it is strongly discouraged. 
* Coding to interfaces should be the preferred option.
* Feature Flags should be used to determine if casting to a feature interface can be done. Do not use casting to determine features outside of the discovery process. Is or as casting can be used once it is verified a Prop supports a feature.
* Use of dependency injection is strongly recommended. Use Catel DI for UI flows and Microsoft DI for any other areas.
* Usage of the registries and factories should be initiated via DI.
* OpenGL under WPF will be the technology used for the drawing engine. OpenTK is the OpenGL implementation library.
* OpenTK.GLWpfControl will be the library used to create any viewers to render Props, or the world view of all Props.
* All UI screens will be WPF.

## Future Considerations

* Move ElementNode management in the Prop to an external adapter or integration service. The hooks need to be in BaseProp, but the implementation should be extracted. This is outside the current POC scope.

## Areas To Ignore

* The PropCentric entry point is just a test harness to drive the POC. None of that code will be used in the real solution.
* Vixen.Library is a placeholder for core structure in the target solution. It is just here to enable the POC without copying large amounts of the Vixen code over.
* Vixen.Shim is also a placeholder for some bits of Vixen code needed to enable the POC.
* Props.WPFCommon is more code shim from Vixen to enable POC.

