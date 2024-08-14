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

public partial class MultiFolderPickerView : ReactiveUserControl<MultiFolderPickerViewModel>
{
    public MultiFolderPickerView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            d(ViewModel!.SelectFolderInteraction.RegisterHandler(MultiFolderPickerHandler));
        });
    }

    private async Task MultiFolderPickerHandler(InteractionContext<string?, List<IStorageFolder>?> context)
    {
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        IReadOnlyList<IStorageFolder>? storageFolders = await topLevel!.StorageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions()
            {
                AllowMultiple = true,
                Title = context.Input
            }
        );

        context.SetOutput(new List<IStorageFolder>(storageFolders));
    }
}