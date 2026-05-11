using Microsoft.Extensions.DependencyInjection;
using Orc.Theming;
using PropCentric.Demo;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;
using Props.Runtime.PolyLine;
using Props.Runtime.Tree;

namespace PropCentric.Views;

/// <summary>
/// The application's main window, which bootstraps the prop system and exercises the catalog on startup.
/// </summary>
public partial class MainWindow
{
    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Console.WriteLine("Theme Synchronized");
        var provider = PropSystemBootstrap.Initialize();
        Console.WriteLine("PropSystemBootstrap initialized");
        var catalogProvider = provider.GetRequiredService<IPropCatalogProvider>();
        Console.WriteLine("Catalog Provider obtained");
        var propSetupFactory = provider.GetRequiredService<IPropSetupFactory>();
        Console.WriteLine("PropSetupFactory obtained");

        var propCatalog = catalogProvider.GetPropCatalog();
        Console.WriteLine("PropCatalog obtained");

        foreach (var pci in propCatalog)
        {
            Console.WriteLine($"Prop: {pci.Name} ");
            Console.WriteLine($"Prop Features: {pci.Features}");
        }

        var propCatalogItem = propCatalog.First(x => x.PropType == typeof(PolyLineProp));

        Console.WriteLine($"Loading setup for: {propCatalogItem.Name} ");

        var propSetup = propSetupFactory.CreateFromCatalogItem(propCatalogItem);
        Console.WriteLine($"Prop Setup created: {propSetup.GetType().Name}");

        var captureContext = FixedSegmentCaptureSource.CreateInitialCaptureContext();
        Console.WriteLine($"Using fixed capture stub with {captureContext.Segments.Count} captured segments.");

        try
        {
            var propGroup = propSetup.CreateAsync(captureContext).Result;
            if (propGroup != null)
            {
                Console.WriteLine($"Prop Group created: {propGroup.GroupName}");
                Console.WriteLine($"Prop Group Props: {propGroup.Props.Count}");
                if (propGroup.Props.Count > 0)
                {
                    var prop = propGroup.Props.First();
                    Console.WriteLine($"First Prop Type: {prop.GetType().Name}");

                    // We can determine the features of a prop by using the prop feature resolver.
                    var propFeatureResolver = provider.GetRequiredService<IPropFeatureResolver>();
                    var features = propFeatureResolver.GetFeatures(prop);
                    Console.WriteLine($"{prop.Name} Features: {features}");

                    // We can see if it has a specific feature.
                    var hasDimming = propFeatureResolver.HasFeature(prop, PropFeatureFlags.Dimming);
                    Console.WriteLine($"{prop.Name} Has Dimming: {hasDimming}");
                    if (hasDimming)
                    {
                        var dimmingProp = prop as IHasDimming;
                        Console.WriteLine($"{prop.Name} Dimming: {dimmingProp!.Brightness}%, Gamma: {dimmingProp!.Gamma}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        try
        {
            //Now test the tree
            propCatalogItem = propCatalog.First(x => x.PropType == typeof(TreeProp));

            Console.WriteLine($"Loading setup for: {propCatalogItem.Name} ");

            propSetup = propSetupFactory.CreateFromCatalogItem(propCatalogItem);
            Console.WriteLine($"Prop Setup created: {propSetup.GetType().Name}");
            var propGroup = propSetup.CreateAsync().Result;
            if (propGroup != null)
            {
                Console.WriteLine($"Prop Group created: {propGroup.GroupName}");
                Console.WriteLine($"Prop Group Props: {propGroup.Props.Count}");
                if (propGroup.Props.Count > 0)
                {
                    var prop = propGroup.Props.First();
                    Console.WriteLine($"First Prop Type: {prop.GetType().Name}");

                    // We can determine the features of a prop by using the prop feature resolver.
                    var propFeatureResolver = provider.GetRequiredService<IPropFeatureResolver>();
                    var features = propFeatureResolver.GetFeatures(prop);
                    Console.WriteLine($"{prop.Name} Features: {features}");

                    // We can see if it has a specific feature.
                    var hasDimming = propFeatureResolver.HasFeature(prop, PropFeatureFlags.Dimming);
                    Console.WriteLine($"{prop.Name} Has Dimming: {hasDimming}");
                    if (hasDimming)
                    {
                        var dimmingProp = prop as IHasDimming;
                        Console.WriteLine($"{prop.Name} Dimming: {dimmingProp!.Brightness}%, Gamma: {dimmingProp!.Gamma}");
                    }
                }
            }
        }catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
