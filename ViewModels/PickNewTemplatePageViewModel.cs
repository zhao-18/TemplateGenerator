using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using TemplateGenerator.Controls;
using Microsoft.TemplateEngine.Abstractions;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace TemplateGenerator.ViewModels
{
    public class PickNewTemplatePageViewModel : PageViewModelBase
    {

        public PickNewTemplatePageViewModel() : base()
        {
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(700))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(SearchTemplate);

            templateLists = new ObservableCollection<ITemplateInfo>(TemplateEngineWrapper.templateList);

            FolderPicker.FolderPicked += ((sender, e) =>
            {
                TemplateEngineWrapper.InstantiationLocation = e.storageFolder;
                UpdateCanNavigateNext();
            });

            System.IObservable<bool> canNavNext = this.WhenAnyValue(
                o => o.CanNavigateNext
            );
            NextPageCommand = ReactiveCommand.Create(NextPage, canNavNext);
        }
        public PickNewTemplatePageViewModel(TemplateEngineWrapper templateEngineWrapper) : base(templateEngineWrapper)
        {
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(700))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(SearchTemplate);

            templateLists = new ObservableCollection<ITemplateInfo>(TemplateEngineWrapper.templateList);

            FolderPicker.FolderPicked += ((sender, e) =>
            {
                TemplateEngineWrapper.InstantiationLocation = e.storageFolder;
                UpdateCanNavigateNext();
            });

            System.IObservable<bool> canNavNext = this.WhenAnyValue(
                o => o.CanNavigateNext
            );
            NextPageCommand = ReactiveCommand.Create(NextPage, canNavNext);
        }

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        [Required]
        public string Name
        {
            get => TemplateEngineWrapper.TemplateName;
            set
            {
                this.RaiseAndSetIfChanged(ref TemplateEngineWrapper.TemplateName, value);
                UpdateCanNavigateNext();
            }

        }


        public ObservableCollection<ITemplateInfo> templateLists { get; set; }


        [Required]
        private ITemplateInfo? _selectedTemplate;
        public ITemplateInfo? SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedTemplate, value);
                UpdateCanNavigateNext();
                TemplateEngineWrapper.SelectedTemplate = value;
            }
        }

        private bool AttributeContains(ITemplateInfo template, string s)
        {
            IReadOnlyList<string> attributes = template.Classifications;
            return attributes.Any(attr => attr.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        private void SearchTemplate(string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {

                IEnumerable<ITemplateInfo>? temp = TemplateEngineWrapper.templateList.Where(
                    template => (template.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                                 AttributeContains(template, s) ||
                                 TemplateEngineWrapper.shortNameGetter(template).Contains(s, StringComparison.OrdinalIgnoreCase) ||
                                 TemplateEngineWrapper.languageGetter(template).Contains(s, StringComparison.OrdinalIgnoreCase))
                );

                templateLists.Clear();
                if (temp is not null && temp.Count() > 0)
                {
                    foreach (ITemplateInfo template in temp)
                    {
                        templateLists.Add(template);
                    }
                }
            } 
            else
            {
                templateLists.Clear();
                foreach (ITemplateInfo template in TemplateEngineWrapper.templateList)
                {
                    templateLists.Add(template);
                }
            }
        }

        public ICommand NextPageCommand { get; }

        private void NextPage()
        {
            InvokeNavigateNextEvent(2);
        }

        public FolderPickerViewModel FolderPicker { get; set; } = new FolderPickerViewModel("Location");

        private bool _canNavigateNext;

        public bool CanNavigateNext
        {
            get => _canNavigateNext;
            set => this.RaiseAndSetIfChanged(ref _canNavigateNext, value);
        }

        private void UpdateCanNavigateNext()
        {
            CanNavigateNext = (SelectedTemplate is not null) &&
                              (TemplateEngineWrapper.TemplateName != string.Empty) &&
                              (TemplateEngineWrapper.InstantiationLocation is not null);
        }

        public void RefreshTemplateList()
        {
            templateLists = new ObservableCollection<ITemplateInfo>(TemplateEngineWrapper.templateList);
        }
    }
}
