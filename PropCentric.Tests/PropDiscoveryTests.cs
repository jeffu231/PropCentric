using Props.Abstractions.Features;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Registry;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;
using Microsoft.Extensions.DependencyInjection;
using Props.Runtime.Wizards.Features.Dimming.Mappers;
using Props.Runtime.Wizards.Features.Dimming.Pages;

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
            FeatureWizardPageScanner.Scan([typeof(DimmingFeatureWizardPage).Assembly]);

        var registration = Assert.Single(registrations, r => r.PageType == typeof(DimmingFeatureWizardPage));
        Assert.Equal(typeof(IHasDimming), registration.FeatureInterface);
        Assert.Equal(typeof(DimmingFeatureWizardDataMapper), registration.MapperType);
        Assert.Equal(100, registration.Priority);
    }

    [Fact]
    public void AddPropSystem_RegistersDiscoveredSupportServicesWithoutManualTreeBootstrap()
    {
        var services = new ServiceCollection();
        var pluginDirectory = Path.GetDirectoryName(typeof(TreeProp).Assembly.Location);

        Assert.NotNull(pluginDirectory);

        services.AddPropSystem(pluginDirectory!);
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IVisualInputMapper<TreeProp, TreeVisualInput>>());
        Assert.NotNull(provider.GetService<IVisualInputMapper<TreePropDraft, TreeVisualInput>>());
        Assert.NotNull(provider.GetService<IPropVisualModelBuilder<TreeVisualInput, TreePropVisualModel>>());
        Assert.NotNull(provider.GetService<IPropDraftMapper<TreePropDraft, TreeProp>>());
        Assert.NotNull(provider.GetService<IWizardPreviewCoordinator<TreePropDraft>>());
        Assert.NotNull(provider.GetService<ISegmentCaptureNormalizer>());
        Assert.NotNull(provider.GetService<TreeProp>());
        Assert.NotNull(provider.GetService<TreePropSetup>());
    }
}
