using Avalonia.Platform.Storage;
using Microsoft.TemplateEngine.Abstractions.Installer;
using NuGet.Protocol.Plugins;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateGenerator.Controls;
using TemplateGenerator.Converters;

namespace TemplateGenerator.ViewModels
{
    public class InstallNewTemplatePageViewModel : PageViewModelBase
    {
        public InstallNewTemplatePageViewModel() : base()
        {
            MultiFilePicker.FilePicked += ((sender, e) =>
            {
                filesPicked = e.Entries;
            });

            MultiFolderPicker.FolderPicked += ((sender, e) =>
            {
                foldersPicked = e.Entries;
            });

            InstallTemplateAsyncCommand = ReactiveCommand.CreateFromTask(InstallTemplatesAsync);
        }
        public InstallNewTemplatePageViewModel(TemplateEngineWrapper templateEngineWrapper) : base(templateEngineWrapper)
        {
            MultiFilePicker.FilePicked += ((sender, e) =>
            {
                filesPicked = e.Entries;
            });

            MultiFolderPicker.FolderPicked += ((sender, e) =>
            {
                foldersPicked = e.Entries;
            });

            InstallTemplateAsyncCommand = ReactiveCommand.CreateFromTask(InstallTemplatesAsync);
        }


        public MultiFilePickerViewModel MultiFilePicker { get; set; } = new MultiFilePickerViewModel("NuGet template files");
        public MultiFolderPickerViewModel MultiFolderPicker { get; set; } = new MultiFolderPickerViewModel("Folder containing templates");

        private List<IStorageFolder> foldersPicked = new();
        private List<IStorageFile> filesPicked = new();

        public ICommand InstallTemplateAsyncCommand { get; }
        private async Task InstallTemplatesAsync()
        {

            List<InstallRequest> installRequests = new List<InstallRequest>();

            foreach (IStorageFolder folder in foldersPicked)
            {
                TemplateEngineWrapper.CacheTemplates(FuncConverters.PathConverter(folder.Path.ToString()) ?? string.Empty, Path.Combine(TemplateEngineWrapper.TemplatesCachedLocation, folder.Name));
                installRequests.Add(new InstallRequest(FuncConverters.PathConverter(folder.Path.ToString()) ?? string.Empty));
            }

            foreach (IStorageFile file in filesPicked)
            {
                TemplateEngineWrapper.CacheTemplates(FuncConverters.PathConverter(file.Path.ToString()) ?? string.Empty, TemplateEngineWrapper.TemplatesCachedLocation);
                installRequests.Add(new InstallRequest(FuncConverters.PathConverter(file.Path.ToString()) ?? string.Empty));
            }

            TemplateEngineWrapper.LastRunResult = await TemplateEngineWrapper.InstallTemplatesAsync(installRequests);

            InvokeNavigateNextEvent(3);
        }
    }
}
