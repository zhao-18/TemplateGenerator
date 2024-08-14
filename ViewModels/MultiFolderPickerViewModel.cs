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
    public class MultiFolderPickerViewModel : ViewModelBase
    {
        public MultiFolderPickerViewModel()
        {
            _selectFolderInteraction = new Interaction<string?, List<IStorageFolder>?>();
            SelectFolderCommand = ReactiveCommand.CreateFromTask(SelectFolder);
        }

        public MultiFolderPickerViewModel(string watermark) : this()
        {
            Watermark = watermark;
        }

        private List<IStorageFolder>? _selectedFolder;

        public List<IStorageFolder>? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFolder, value);
                if (value is null)
                {
                    FolderList = string.Empty;
                    return;
                }

                foreach(IStorageFolder folder in value)
                {
                    FolderList += FuncConverters.PathConverter(folder.Path.ToString()) + "; ";
                }
            }
        }

        private string _folderList = string.Empty;

        public string Watermark { get; init; } = string.Empty;

        public string FolderList
        {
            get => _folderList;
            set => this.RaiseAndSetIfChanged(ref _folderList, value);
        }

        private readonly Interaction<string?, List<IStorageFolder>?> _selectFolderInteraction;

        public Interaction<string?, List<IStorageFolder>?> SelectFolderInteraction => _selectFolderInteraction;

        public ICommand SelectFolderCommand { get; }

        private async Task SelectFolder()
        {
            SelectedFolder = await _selectFolderInteraction.Handle("Pick Folder");
            InvokeFolderPickedEvent(SelectedFolder);
        }

        public event EventHandler<MultiFolderPickedEventArgs>? FolderPicked;

        public void InvokeFolderPickedEvent(List<IStorageFolder> folders)
        {
            FolderPicked?.Invoke(this, new MultiFolderPickedEventArgs(folders));
        }
    }
}