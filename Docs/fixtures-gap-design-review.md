# Fixture Design / Gap Discovery

## Overview

The purpose of this task is to discover any design gaps or impediments to incorporating Fixures as implemented in
the Prop Centric Feature branch in the Vixen source tree with the current POC. The current POC has developed patterns 
and practices to model the core lighting Props. The key gap remaining before this pattern can be considered for incorporation
into the main feature is to understand what is needed for Fixtures to work. This POC would likley need to clone a larger
of the existing Vixen code to try and model a Fixture as an example. So this research will be key to discover any gaps 
that need to be addressed to apply the pattern to Fixtures. Considerable effort has been put into the Fixture setup flow
in the Vixen feature branch and the goal is to be able to apply these patterns with minimal effort while staying true to
the design goals in this POC.

## References

Target Vixen branch for incorporating this POC pattern into [Vixen Feature](https://github.com/VixenLights/Vixen/tree/feature/VIX-3693)

Core Prop that models a Fixture in the Vixen feature branch. [Intelligent Fixture Prop](https://github.com/VixenLights/Vixen/tree/feature/VIX-3693/src/Vixen.Modules/App/Props/Models/IntelligentFixture)

Setup Wizard flows in the Vixen source that are used for setup of a fixture. [Intelligent Fixture Wizard](https://github.com/VixenLights/Vixen/tree/feature/VIX-3693/src/Vixen.Modules/Editor/FixtureWizard)

IPropModel in the Vixen code is equivelent to IVisualPropModel in the POC.

## Guidance

You must use the documentation in Docs to ensure you understand the current POC architecture, goals and design.
You must read the code source in the References section to be sure you understand how Fixtures work today outside of the POC.
Use dotnet-design-pattern-review, dotnet-best-practices and catel-mvvm skills where needed in the review.
Use plans.md to format the steps needed to understand the gaps and challenges.
Ask questions to be sure you have a full understanding of the task.





