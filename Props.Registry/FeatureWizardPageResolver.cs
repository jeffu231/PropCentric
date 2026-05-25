using Microsoft.Extensions.DependencyInjection;
using Orc.Wizard;
using Props.Abstractions.Features;

namespace Props.Registry;

/// <summary>
/// Resolves feature wizard pages for a given prop type using registrations discovered by
/// <see cref="FeatureWizardPageScanner"/>.
/// </summary>
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

    public void InitializePages(IReadOnlyList<IWizardPage> pages, FeatureWizardContext context)
    {
        foreach (var page in pages)
        {
            if (page is IFeatureWizardDraftPage draftPage)
            {
                draftPage.Initialize(context);
            }
        }
    }
}
