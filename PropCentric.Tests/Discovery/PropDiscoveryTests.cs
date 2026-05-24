using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Features;
using Props.Abstractions.Setup;
using Props.Abstractions.Visuals;
using Props.Registry;
using Props.Runtime.PolyLine;
using Props.Runtime.PolyLine.Setup;
using Props.Runtime.PolyLine.Visuals;
using Props.Runtime.Tree;
using Props.Runtime.Tree.Setup;
using Props.Runtime.Tree.Visuals;
using Props.Runtime.Wizards.Features.Dimming.Mappers;
using Props.Runtime.Wizards.Features.Dimming.Pages;
using Props.Runtime.Wizards.Features.Rotation.Pages;
using Props.Runtime.Wizards.Features.Segments.Pages;

namespace PropCentric.Tests.Discovery;

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
        Assert.True(flags.HasFlag(PropFeatureFlags.Color));
        Assert.True(typeof(IHasColor).IsAssignableFrom(typeof(TreeProp)));
        Assert.True(flags.HasFlag(PropFeatureFlags.Dimming));
        Assert.True(typeof(IHasDimming).IsAssignableFrom(typeof(TreeProp)));
        Assert.True(flags.HasFlag(PropFeatureFlags.Rotation));
        Assert.True(typeof(ICanAxisRotate).IsAssignableFrom(typeof(TreeProp)));
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
    public void PropFeatureInferrer_InferPolyLineProp_ReturnsExpectedFlags()
    {
        var inferrer = new PropFeatureInferrer();

        var flags = inferrer.Infer(typeof(PolyLineProp));

        Assert.True(flags.HasFlag(PropFeatureFlags.Lights));
        Assert.True(typeof(IHasLights).IsAssignableFrom(typeof(PolyLineProp)));
        Assert.True(flags.HasFlag(PropFeatureFlags.Color));
        Assert.True(typeof(IHasColor).IsAssignableFrom(typeof(PolyLineProp)));
        Assert.True(flags.HasFlag(PropFeatureFlags.Segments));
        Assert.True(typeof(IHasSegments).IsAssignableFrom(typeof(PolyLineProp)));
        Assert.False(flags.HasFlag(PropFeatureFlags.Rotation));
        Assert.False(typeof(ICanAxisRotate).IsAssignableFrom(typeof(PolyLineProp)));
    }

    [Fact]
    public void PropScanner_ScanRuntimeAssembly_FindsPolyLineDescriptor()
    {
        IReadOnlyList<PropDescriptor> descriptors = PropScanner.Scan([typeof(PolyLineProp).Assembly]);

        var descriptor = Assert.Single(descriptors, d => d.PropType == typeof(PolyLineProp));
        Assert.Equal("PolyLine", descriptor.Name);
        Assert.Equal(typeof(PolyLinePropSetup), descriptor.SetupType);
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
    public void FeatureWizardPageScanner_ScanRuntimeAssembly_FindsSegmentsWizardRegistration()
    {
        IReadOnlyList<FeatureWizardPageDescriptor> registrations =
            FeatureWizardPageScanner.Scan([typeof(SegmentsFeatureWizardPage).Assembly]);

        var registration = Assert.Single(registrations, r => r.PageType == typeof(SegmentsFeatureWizardPage));
        Assert.Equal(typeof(IHasSegments), registration.FeatureInterface);
        Assert.Null(registration.MapperType);
        Assert.Equal(150, registration.Priority);
    }

    [Fact]
    public void FeatureWizardPageScanner_ScanRuntimeAssembly_FindsRotationWizardRegistration()
    {
        IReadOnlyList<FeatureWizardPageDescriptor> registrations =
            FeatureWizardPageScanner.Scan([typeof(RotationFeatureWizardPage).Assembly]);

        var registration = Assert.Single(registrations, r => r.PageType == typeof(RotationFeatureWizardPage));
        Assert.Equal(typeof(ICanAxisRotate), registration.FeatureInterface);
        Assert.Null(registration.MapperType);
        Assert.Equal(140, registration.Priority);
    }

    [Fact]
    public void ICanAxisRotate_UsesRotationFeatureFlag()
    {
        var attribute = typeof(ICanAxisRotate).GetCustomAttributes(typeof(PropFeatureAttribute), inherit: false)
            .OfType<PropFeatureAttribute>()
            .Single();

        Assert.Equal(PropFeatureFlags.Rotation, attribute.Flag);
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
        Assert.NotNull(provider.GetService<RotationFeatureWizardPage>());
    }

    [Fact]
    public void AddPropSystem_RegistersDiscoveredPolyLineServicesWithoutManualBootstrap()
    {
        var services = new ServiceCollection();
        var pluginDirectory = Path.GetDirectoryName(typeof(PolyLineProp).Assembly.Location);

        Assert.NotNull(pluginDirectory);

        services.AddPropSystem(pluginDirectory!);
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IVisualInputMapper<PolyLineProp, PolyLineVisualInput>>());
        Assert.NotNull(provider.GetService<IVisualInputMapper<PolyLinePropDraft, PolyLineVisualInput>>());
        Assert.NotNull(provider.GetService<IPropVisualModelBuilder<PolyLineVisualInput, PolyLinePropVisualModel>>());
        Assert.NotNull(provider.GetService<IPropDraftMapper<PolyLinePropDraft, PolyLineProp>>());
        Assert.NotNull(provider.GetService<IWizardPreviewCoordinator<PolyLinePropDraft>>());
        Assert.NotNull(provider.GetService<PolyLineProp>());
        Assert.NotNull(provider.GetService<PolyLinePropSetup>());
        Assert.NotNull(provider.GetService<SegmentsFeatureWizardPage>());
    }

    [Fact]
    public void FeatureWizardPageResolver_ResolvesRotationPageForTreeButNotPolyLine()
    {
        var services = new ServiceCollection();
        var pluginDirectory = Path.GetDirectoryName(typeof(TreeProp).Assembly.Location);

        Assert.NotNull(pluginDirectory);

        services.AddPropSystem(pluginDirectory!);
        using var provider = services.BuildServiceProvider();

        var resolver = provider.GetRequiredService<IFeatureWizardPageResolver>();

        var treePages = resolver.GetPagesFor(typeof(TreeProp));
        var polyLinePages = resolver.GetPagesFor(typeof(PolyLineProp));

        Assert.Contains(treePages, page => page.GetType() == typeof(RotationFeatureWizardPage));
        Assert.DoesNotContain(polyLinePages, page => page.GetType() == typeof(RotationFeatureWizardPage));
    }
}
