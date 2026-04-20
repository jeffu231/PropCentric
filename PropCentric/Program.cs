using Microsoft.Extensions.DependencyInjection;
using Props.Abstractions.Features;
using Props.Abstractions.Props;
using Props.Abstractions.Wizards;

namespace PropCentric;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var provider = PropSystemBootstrap.Initialize();
        
        // Resolve prop factory
        var propFactory = provider.GetRequiredService<IPropFactory>();
        var wizardFactory = provider.GetRequiredService<IWizardFactory>();
        
        var propCatalog = propFactory.GetPropCatalog();
        foreach (var propCatalogItem in propCatalog)
        {
            Console.WriteLine($"Prop: {propCatalogItem.Name} ");
            Console.WriteLine($"Prop Features: {propCatalogItem.Features}");

            var propSetupWizard = wizardFactory.CreateWizard(propCatalogItem);
            Console.WriteLine($"Prop Wizard created: {propSetupWizard.GetType().Name}");
            var prop = propSetupWizard.CreateAsync().Result;
            Console.WriteLine($"Prop created: {prop.GetType().Name}");
            
            //We can determine the features of a prop by using the prop feature resolver
            var propFeatureResolver = provider.GetRequiredService<IPropFeatureResolver>();
            var features = propFeatureResolver.GetFeatures(prop);
            Console.WriteLine($"{prop.GetType().Name} Features: {features}");
            
            //We can see if it has a specific feature
            var hasDimming = propFeatureResolver.HasFeature(prop, PropFeatureFlags.Dimming);
            Console.WriteLine($"{prop.GetType().Name} Has Dimming: {hasDimming}");
            if (hasDimming)
            {
                var dimmingProp = prop as IHasDimming;
                Console.WriteLine($"{prop.GetType().Name} Dimming: {dimmingProp?.Brightness}");
            }
        }
        
        Console.WriteLine("Goodbye, World!");
    }
}