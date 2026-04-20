using Microsoft.Extensions.DependencyInjection;
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
        }
        
        Console.WriteLine("Goodbye, World!");
    }
}