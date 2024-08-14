using Avalonia.Platform.Storage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateGenerator.Converters;

namespace TemplateGenerator.ViewModels
{
    public class FolderPickerViewModel : ViewModelBase
    {

        public FolderPickerViewModel()
        {
            _selectFolderInteraction = new Interaction<string?, IStorageFolder?>();
            SelectFolderCommand = ReactiveCommand.CreateFromTask(SelectFolder);
        }

        public FolderPickerViewModel(string watermark) : this()
        {
            Watermark = watermark;
        }

        private IStorageFolder? _selectedFolder;

        public IStorageFolder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFolder, value);
                if (value is null)
                {
                    FolderName = string.Empty;
                }
                else
                {
                    FolderName = FuncConverters.PathConverter(value.Path.ToString()) ?? string.Empty;
                }
            }
        }

        private string _folderName = string.Empty;

        public string Watermark { get; init; } = string.Empty;

        public string FolderName
        {
            get => _folderName;
            set => this.RaiseAndSetIfChanged(ref _folderName, value);
        }

        private readonly Interaction<string?, IStorageFolder?> _selectFolderInteraction;

        public Interaction<string?, IStorageFolder?> SelectFolderInteraction => _selectFolderInteraction;

        public ICommand SelectFolderCommand { get; }

        private async Task SelectFolder()
        {
            SelectedFolder = await _selectFolderInteraction.Handle("Pick Folder");
            InvokeFolderPickedEvent(SelectedFolder);
        }

        public event EventHandler<FolderPickedEventArgs>? FolderPicked;

        public void InvokeFolderPickedEvent(IStorageFolder folder)
        {
            if (FolderPicked != null)
            {
                FolderPicked(this, new FolderPickedEventArgs(folder));
            }
        }
    }
}
