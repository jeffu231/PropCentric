using System.Windows;
using Props.Runtime.Wizards.Core.Views;
using Props.Runtime.Wizards.Features.Color.Pages;
using Props.Runtime.Wizards.Features.Color.ViewModels;

namespace Props.Runtime.Wizards.Features.Color.Views;

/// <summary>
/// Code-behind for the reusable color feature wizard page.
/// </summary>
public partial class ColorFeatureWizardPageView : WizardPageViewBase
{
    public ColorFeatureWizardPageView()
    {
        InitializeComponent();

        OpenTkCntrl = OpenTkControl;
        Initialize();
    }

    private void PickSingleColorButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ColorFeatureWizardPageViewModel viewModel)
        {
            return;
        }
        var page = viewModel.Page;

        var picker = new ColorPickerDialogView
        {
            Owner = Window.GetWindow(this),
            DataContext = new ColorPickerDialogViewModel(page.SingleColor)
        };

        if (picker.ShowDialog() == true && picker.DataContext is ColorPickerDialogViewModel pickerViewModel)
        {
            page.SetSingleColor(pickerViewModel.SelectedColor);
        }
    }

    private void EditWorkingColorButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: EditableDiscreteColorItem item }
            || DataContext is not ColorFeatureWizardPageViewModel viewModel)
        {
            return;
        }
        var page = viewModel.Page;

        var picker = new ColorPickerDialogView
        {
            Owner = Window.GetWindow(this),
            DataContext = new ColorPickerDialogViewModel(item.Color)
        };

        if (picker.ShowDialog() == true && picker.DataContext is ColorPickerDialogViewModel pickerViewModel)
        {
            page.SetWorkingDiscreteColor(item, pickerViewModel.SelectedColor);
        }
    }

    private void AddWorkingColorButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is ColorFeatureWizardPageViewModel viewModel)
        {
            viewModel.Page.AddWorkingDiscreteColor();
        }
    }

    private void RemoveWorkingColorButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is ColorFeatureWizardPageViewModel viewModel)
        {
            viewModel.Page.RemoveSelectedWorkingDiscreteColor();
        }
    }

    private void SaveCustomSetButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ColorFeatureWizardPageViewModel viewModel)
        {
            return;
        }

        try
        {
            viewModel.Page.SaveCustomDiscreteColorSet();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Color Set", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
