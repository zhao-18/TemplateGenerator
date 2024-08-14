using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.TemplateEngine.IDE;
using Microsoft.TemplateEngine.Edge;
using Microsoft.TemplateEngine.Edge.Template;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Abstractions.Installer;
using Avalonia.Platform.Storage;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace TemplateGenerator.Controls
{
    public class TemplateEngineWrapper
    {
        public TemplateEngineWrapper(string hostIdentifier, string version) {
            Host = new DefaultTemplateEngineHost(hostIdentifier, version);
            Bootstrapper = new Bootstrapper(Host, true);
            ObservableCollection<string> tempCollection = new ObservableCollection<string> {
                TemplatesCachedLocation,
                "C:/Program Files/dotnet/templates/8.0.3",
                "C:/Program Files/dotnet/template-packs",
                "/usr/lib/dotnet/templates/8.0.7"
            };

            IEnumerable<string> paths = tempCollection.Where(path => Directory.Exists(path));

            foreach(string path in paths)
            {
                defaultTemplatePaths.Add(path);
            }
        }

        public string TemplateName = string.Empty;
        public IStorageFolder? InstantiationLocation { get; set; }
        public ITemplateInfo? SelectedTemplate { get; set; }
        public bool LastRunResult { get; set; } = false;
        public string DefaultTemplateDeployLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public string TemplatesCachedLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".templatemanager");
        public ObservableCollection<string> defaultTemplatePaths = new ObservableCollection<string>();

        private ITemplateEngineHost Host;
        private Bootstrapper Bootstrapper;
        public List<ITemplateInfo> templateList = new();

        public void CacheTemplates(string source, string destination)
        {
            string sourceFullPath = Path.GetFullPath(source);
            string destinationFullPath = Path.GetFullPath(destination);

            if (!Directory.Exists(destinationFullPath))
            {
                Directory.CreateDirectory(destinationFullPath);
            }

            IEnumerable<string> Files = Directory.EnumerateFileSystemEntries(sourceFullPath);

            foreach (string file in Files)
            {
                string FileName = file.Substring(sourceFullPath.Length + 1);
                string destinationFilePath = Path.Combine(destinationFullPath, FileName);

                if (File.Exists(file))
                {
                    if (File.Exists(destinationFilePath))
                    {
                        if (File.GetLastWriteTime(destinationFilePath) != File.GetLastWriteTime(file))
                        {
                            File.Copy(file, destinationFilePath, true);
                        }
                    }
                    else
                    {
                        File.Copy(file, destinationFilePath);
                    }
                } 
                else if (Directory.Exists(file))
                {
                    if (Directory.Exists(destinationFilePath))
                    {
                        if (Directory.GetLastWriteTime(destinationFilePath) != Directory.GetLastWriteTime(file))
                        {
                            DirectoryCopy(file, destinationFilePath);
                        }
                    } 
                    else
                    {
                        DirectoryCopy(file, destinationFilePath);
                    }
                }
            }
        }

        public static void DirectoryCopy(string source, string destination)
        {
            DirectoryInfo dir = new DirectoryInfo(source);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(source);

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destination);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destination, file.Name);
                file.CopyTo(targetFilePath);
            }

            foreach(DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destination, subDir.Name);
                DirectoryCopy(subDir.FullName, newDestinationDir);
            }
        }

        public async Task<bool> InstallTemplatesAsync(List<string> templatePaths)
        {
            List<InstallRequest> installRequests = new List<InstallRequest>();

            foreach (string path in templatePaths)
            {
                IEnumerable<string> TemplatePackages = Directory.EnumerateFileSystemEntries(path);

                foreach (string Package in TemplatePackages)
                {
                    installRequests.Add(new InstallRequest(Package));
                }
            }

            return await InstallTemplatesAsync(installRequests);
        }

        public async Task<bool> InstallTemplatesAsync(List<InstallRequest> templates)
        {
            IReadOnlyList<InstallResult> installResult = await Bootstrapper.InstallTemplatePackagesAsync(templates);

            if (installResult.Any(result => !result.Success))
            {
                Debug.WriteLine("Loading Failed");
                return false;
            }

            templateList = new List<ITemplateInfo>(await GetTemplatesAsync());
            return true;
        }


        public string languageGetter(ITemplateInfo? template)
        {
            string? language = string.Empty;
            template?.TagsCollection.TryGetValue("language", out language);
            return language ?? string.Empty;
        }

        public string shortNameGetter(ITemplateInfo? template)
        {
            return template?.ShortNameList.Count > 0 ? template.ShortNameList[0] : string.Empty;
        }

        public ITemplateInfo? getTemplate(string shortName, string language)
        {
            return templateList.FirstOrDefault(template => (shortNameGetter(template) == shortName) && (languageGetter(template) == language));
        }

        public string? PathConverter(string? AvaloniaPath)
        {
            if (AvaloniaPath?.StartsWith("file:///") ?? false)
            {
                return AvaloniaPath.Substring(8);
            }
            return AvaloniaPath;
        }

        public async Task<bool> CreateTemplateAsync(string templateName, string language, string outputPath, string name, IDictionary<string, string?> inputParameters)
        {
            ITemplateInfo? targetTemplate = getTemplate(templateName, language);

            if (targetTemplate is null)
                return false;

            return await CreateTemplateAsync(targetTemplate, outputPath, name, inputParameters);
        }

        public async Task<bool> CreateTemplateAsync(ITemplateInfo template, string outputPath, string name, IDictionary<string, string?> inputParameters)
        {
            InputDataSet parameters = new InputDataSet(template, inputParameters.AsReadOnly());

            ITemplateCreationResult creationResult = await Bootstrapper.CreateAsync(
                template,
                name,
                outputPath,
                parameters);

            Debug.WriteLine(creationResult.Status);
            Debug.WriteLine(creationResult.ErrorMessage);

            return creationResult.Status == CreationResultStatus.Success;
        }

        public async Task<IReadOnlyList<ITemplateInfo>> GetTemplatesAsync()
            => await Bootstrapper.GetTemplatesAsync(default);

        public void Dispose()
        {
            Bootstrapper.Dispose();
        }

    }
}
