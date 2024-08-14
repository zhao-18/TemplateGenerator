using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using ReactiveUI;
using System.Threading.Tasks;
using System.Collections.Generic;
using Avalonia.ReactiveUI;
using TemplateGenerator.ViewModels;

namespace TemplateGenerator.Views;

public partial class FolderPickerView : ReactiveUserControl<FolderPickerViewModel>
{
    public FolderPickerView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel!.SelectFolderInteraction.RegisterHandler(FolderPickerHandler));
        });
    }

    private async Task FolderPickerHandler(InteractionContext<string?, IStorageFolder?> context)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);

        IReadOnlyList<IStorageFolder>? storageFolder = await topLevel!.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions()
            {
                AllowMultiple = false,
                Title = context.Input
            }
        );

        if (storageFolder is null || storageFolder.Count != 1)
        {
            context.SetOutput(default(IStorageFolder));
        }
        else
        {
            context.SetOutput(storageFolder[0]);
        }
    }
}