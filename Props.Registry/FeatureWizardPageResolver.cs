using Microsoft.Extensions.DependencyInjection;
using Orc.Wizard;
using Props.Abstractions.Features;

namespace Props.Registry;

public class FeatureWizardPageResolver(
    IReadOnlyList<FeatureWizardPageDescriptor> registrations,
    IServiceProvider serviceProvider) : IFeatureWizardPageResolver
{
    public IReadOnlyList<IWizardPage> GetPagesFor(Type propType)
        => registrations
            .Where(r => r.FeatureInterface.IsAssignableFrom(propType))
            .OrderByDescending(r => r.Priority)
            .Select(r => (IWizardPage)serviceProvider.GetRequiredService(r.PageType))
            .ToList();

    public IReadOnlyList<IFeatureWizardDataMapper> GetMappersFor(IReadOnlyList<IWizardPage> pages)
    {
        var result = new List<IFeatureWizardDataMapper>();
        foreach (var page in pages)
        {
            var reg = registrations.FirstOrDefault(r => r.PageType == page.GetType());
            if (reg?.MapperType is { } mapperType)
                result.Add((IFeatureWizardDataMapper)ActivatorUtilities.CreateInstance(
                    serviceProvider, mapperType, page));
        }
        return result;
    }
}
