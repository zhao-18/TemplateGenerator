using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateGenerator.ViewModels;

namespace TemplateGenerator.Views;

public partial class MultiFilePickerView : ReactiveUserControl<MultiFilePickerViewModel>
{
    public MultiFilePickerView()
    {
        InitializeComponent();

        this.WhenActivated(d => {
            d(ViewModel!.SelectFileInteraction.RegisterHandler(MultiFilePickerHandler));
        });
    }

    private async Task MultiFilePickerHandler(InteractionContext<string?, List<IStorageFile>?> context)
    {
        TopLevel? toplevel = TopLevel.GetTopLevel(this);

        IReadOnlyList<IStorageFile>? storageFiles = await toplevel!.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions()
            {
                AllowMultiple = true,
                Title = context.Input
            }
        );

        context.SetOutput(new List<IStorageFile>(storageFiles));
    }
}