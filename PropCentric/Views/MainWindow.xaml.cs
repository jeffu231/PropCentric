using Microsoft.Extensions.DependencyInjection;
using Orc.Theming;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Setup;

namespace PropCentric.Views;

public partial class MainWindow 
{
    public MainWindow()
    {
        InitializeComponent();
    }

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
        foreach (var propCatalogItem in propCatalog)
        {
            Console.WriteLine($"Prop: {propCatalogItem.Name} ");
            Console.WriteLine($"Prop Features: {propCatalogItem.Features}");

            var propSetup = propSetupFactory.CreateFromCatalogItem(propCatalogItem);
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

                    //We can determine the features of a prop by using the prop feature resolver
                    var propFeatureResolver = provider.GetRequiredService<IPropFeatureResolver>();
                    var features = propFeatureResolver.GetFeatures(prop);
                    Console.WriteLine($"{prop.Name} Features: {features}");

                    //We can see if it has a specific feature
                    var hasDimming = propFeatureResolver.HasFeature(prop, PropFeatureFlags.Dimming);
                    Console.WriteLine($"{prop.Name} Has Dimming: {hasDimming}");
                    if (hasDimming)
                    {
                        var dimmingProp = prop as IHasDimming;
                        Console.WriteLine($"{prop.Name} Dimming: {dimmingProp!.Brightness * 100}%, Gamma: {dimmingProp!.Gamma}");
                    }
                }
            }
        }
    }
}