# Color Feature Requirements

## Overview

A Prop may have lights as part of it definition. All lights require some knowledge about their type and how they behave. 
The has color feature will provide the feature of defining the type of the light and capture information about the specific type
configuration. It will also provide a feature setup wizard for all Props that have color to provide a consistent interface 
for the user. The IHasColor interface will be the basis for this feature. IHasLights and IHasColor features are tightly coupled.

## General Requirements

Follow all dotnet-best-practices and csharp-docs rules. Ensure all requirements defined in Docs are adheared to where applicable.
Ask questions if something is not clear or in conflict.
Unit tests should validate as much of the new functionality as possible following existing patterns.

## Light Types

Light types will define the behavior of the lights to 3 specific types. In the current code this is defined as StringType, 
but should be refactored to LightType to better represent the terminology. It is also part of a base prop instead of being 
part of the unimplemented IHasColor feature interface.

- Single Color: This light is a single discrete fixed color like an analog christmas light. The current definition uses a 
  System.Drawing.Color to represent it.
- Multiple Discrete Colors: This type is similar to the Single color, but there can be more than one. It is analagous to having a 
  a light string that has multiple strands of single color lights. It is defined by a color set 
- Full Color: This is a light that supports full mixing.

## Preset Color Naming Convention

- R stands for Red, G stands for Green, B stands for Blue and W Stands for White.
- Example RGB is Red, Green, Blue.
- Red is defined as RGB 255, 0, 0
- Green is defined as RGB 0, 255, 0
- Blue is defined as RGB 0, 0, 255
- White is defined as RGB 255, 255, 255

## Color Sets 

- Color sets are defined by a name and a list of System.Drawing.Color.
- Users can create custom color sets for use to meet their own needs by giving it a name and selecting the colors that make up the list.
- Any custom RGB color that can be defined with System.Drawing.Color can be used for a color in the list.
- The common sets like `RGB`, `RGBW`, and `GRBW` are predefined and selectable by the user.
- The predefined sets follow the preset color naming convention to define the colors in the list.

## Full Color Sets

- Full color sets just use the name. The name follows the preset color naming pattern.
- The common sets like `RGB`, `RBG`, `GBR`, `GRB`, `BRG`, `BGR`, `RGBW`, `GRWB` are predefined and selectable by the user.
- The color sets are restricted to havng the colors Red, Green, Blue, and White (R, G, B, W) at the current time.
- For RGB the color set would be `RGB` and imply the order as Red, Green, Blue. The actual color is not associated with 
  it in the color definition since it is implied from the preset color naming pattern.

## Feature Wizard Page

- The feature wizard page will provide the user with the ability to choose the light type. This should be a combo box entry
  that provides the 3 choices.
- Based on the selection of Light Type, a dynamic section will provide the user with the ability to configure the light type options.
  - Single Color: Shows the Color Picker control for the user to define a single color.
  - Multiple Discrete Colors: 
    
  - Full Color
- The wizard page will behave like other feature wizards and provide summary information for the configration.
- The wizard page will have full navigation like other feature wizards to go to the previous or next step, or cancel. 

## Color Picker

- The picker will provide a reusable color picker control dialog to edit / select the choosen color. The upper left of the picker
  box will have a full color spectrum box to pick a color from.
- Dialog will be a WPF control using Catel and Catel MVVM.
- The picker will have entry boxes along the right side of the full color spectrum box in a group for H S V to enter
  the color in HSV format. Below that will be another group with the ability to enter R G B for RGB values.
- RGB and HSV entries should update the other fields based on any changes. I.E if I change the R value, the HSV value is calculated and
  updated into the H S V fields.
- Below the above controls, will be a row of predefined colors. White, Red, Green, and Blue will be retangle boxes to click with the 
  rectangle being the color. Clicking will update the upper controls to have the correct HSV, RGB and spectrum point to 
  reflect the choosen color.
- A box at the bottom left will be a rectangle divided into an equal top and bottom half that shows the starting color
  as the background with the hex value of the starting color. The bottom half will be the same for the newly editing color so the user
  can see the old and the new color.
- Ok, Cancel controls will be at the bottom of the control in the right in the customary postion.
- Validation will be performed on the RGB and HSV fields to ensure that only valid ranges can be entered. Use a Numeric text entry
  control to ensure only numbers can be entered.
- The control to model from can be found at [Vixen Color Picker](https://github.com/VixenLights/Vixen/tree/feature/VIX-3693/src/Vixen.Common/Controls/ColorPicker) 
  This is a Winforms version of the control we want to create in WPF.

## Multiple Discrete Colors Dialog

- Allows the user to select from the predefined list, or make a new list of their own to select.
- When creating a new list, the user must provide a unique name that is not in the already provided list.
- The user will add N number of colors to the new color set. The default color for newly added ones is White.
- The user can edit any of the colors in the list using the Color Picker defined below. Clicking the color will open the picker dialog
  and seed it with the color to edit.
- Newly created lists become part of the overall list to be selected in the future. The wizard will obtain the predefined list
  and any user defined ones from a service that handles storage. POC will stub this function to provide minimal runtime behavior.
- The Color Picker defined below will allow the user to pick the individual colors that go in the user defined list.
- Dialog will be a reusable WPF control using Catel and Catel MVVM.
- It is highly desirable to have the controls for this edit function to be inline on the wizard page and avoid a pop up that then 
  launches the color picker popup. Best case is the color picker is the only popup from the wizard page.

An example dialog for the Multiple Discrete Colors can be found in the ColorSetsSetupForm found here.  
[Vixen Color Set Form](https://github.com/VixenLights/Vixen/blob/feature/VIX-3693/src/Vixen.Modules/Property/Color)
Use this as the basis for the inline controls on the wizard when the multiple discrete colors is selected.
