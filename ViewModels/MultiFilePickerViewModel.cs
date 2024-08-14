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
    public class MultiFilePickerViewModel : ViewModelBase
    {
        public MultiFilePickerViewModel()
        {
            _selectFileInteraction = new Interaction<string?, List<IStorageFile>?>();
            SelectFileCommand = ReactiveCommand.CreateFromTask(SelectFiles);
        }

        public MultiFilePickerViewModel(string watermark) : this()
        {
            Watermark = watermark;
        }

        private List<IStorageFile>? _selectedFiles;

        public List<IStorageFile>? SelectedFiles
        {
            get => _selectedFiles;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFiles, value);

                if (value is null)
                {
                    FileList = string.Empty;
                    return;
                }

                foreach (IStorageFile file in value)
                {
                    FileList += FuncConverters.PathConverter(file.Path.ToString()) + "; " ;
                }
            }
        }

        private string _fileList = string.Empty;

        public string Watermark { get; init; } = string.Empty;

        public string FileList
        {
            get => _fileList;
            set => this.RaiseAndSetIfChanged(ref _fileList, value);
        }

        private readonly Interaction<string?, List<IStorageFile>?> _selectFileInteraction;

        public Interaction<string?, List<IStorageFile>?> SelectFileInteraction => _selectFileInteraction;

        public ICommand SelectFileCommand { get; }

        private async Task SelectFiles()
        {
            SelectedFiles = await _selectFileInteraction.Handle("Pick Files");
            InvokeFilePickedEvent(SelectedFiles);
        }

        public event EventHandler<MultiFilePickedEventArgs>? FilePicked;

        public void InvokeFilePickedEvent(List<IStorageFile> files)
        {
            FilePicked?.Invoke(this, new MultiFilePickedEventArgs(files));
        }
    }
}
