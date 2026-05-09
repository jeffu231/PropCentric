using Props.Abstractions.Features;
using Props.Registry;
using Props.Runtime.Tree;
using Props.Runtime.Wizards.Mappers;
using Props.Runtime.Wizards.Pages;

namespace PropCentric.Tests;

/// <summary>
/// Verifies discovery-oriented behavior.
/// </summary>
public class PropDiscoveryTests
{
    [Fact]
    public void PropFeatureInferrer_InferTreeProp_ReturnsExpectedFlags()
    {
        var inferrer = new PropFeatureInferrer();

        var flags = inferrer.Infer(typeof(TreeProp));

        Assert.True(flags.HasFlag(PropFeatureFlags.Lights));
        Assert.True(typeof(IHasLights).IsAssignableFrom(typeof(TreeProp)));
        Assert.True(flags.HasFlag(PropFeatureFlags.Dimming));
        Assert.True(typeof(IHasDimming).IsAssignableFrom(typeof(TreeProp)));
    }

    [Fact]
    public void PropScanner_ScanRuntimeAssembly_FindsTreeDescriptor()
    {
        IReadOnlyList<PropDescriptor> descriptors = PropScanner.Scan([typeof(TreeProp).Assembly]);

        var descriptor = Assert.Single(descriptors, d => d.PropType == typeof(TreeProp));
        Assert.Equal("Tree", descriptor.Name);
        Assert.Equal(typeof(TreePropSetup), descriptor.SetupType);
    }

    [Fact]
    public void FeatureWizardPageScanner_ScanRuntimeAssembly_FindsDimmingWizardRegistration()
    {
        IReadOnlyList<FeatureWizardPageDescriptor> registrations =
            FeatureWizardPageScanner.Scan([typeof(DimmingWizardPage).Assembly]);

        var registration = Assert.Single(registrations, r => r.PageType == typeof(DimmingWizardPage));
        Assert.Equal(typeof(IHasDimming), registration.FeatureInterface);
        Assert.Equal(typeof(DimmingWizardDataMapper), registration.MapperType);
        Assert.Equal(100, registration.Priority);
    }
}
