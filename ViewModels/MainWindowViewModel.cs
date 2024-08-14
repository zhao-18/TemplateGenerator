using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateGenerator.Controls;

namespace TemplateGenerator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public MainWindowViewModel ()
    {
        TemplateEngineNewLocation = templateEngine.TemplatesCachedLocation;
        defaultTemplatePaths = templateEngine.defaultTemplatePaths;

        // Initialize template engine
        templateEngine.CacheTemplates( TemplateEnginePackageLocation, TemplateEngineNewLocation );

        bool InstallResult = Task.Run(
            async () => await templateEngine.InstallTemplatesAsync(defaultTemplatePaths.ToList())
        ).Result;
        // --


        Pages = new PageViewModelBase[]
        {
            new HomePageViewModel(templateEngine),
            new PickNewTemplatePageViewModel(templateEngine),
            new ParameterSelectionPageViewModel(templateEngine),
            new DonePageViewModel(templateEngine),
            new InstallNewTemplatePageViewModel(templateEngine)
        };

        _CurrentPage = Pages[0];
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious);

        Pages[0].NavigateNextEvent += OnNavigateNext;
    }

    private string TemplateEnginePackageLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".templateengine", "packages");
    private string TemplateEngineNewLocation;

    private ObservableCollection<string> defaultTemplatePaths;

    private TemplateEngineWrapper templateEngine = new TemplateEngineWrapper("TestIdentifier", "TestVar");

    // Hold on which page is available
    private readonly PageViewModelBase[] Pages;

    private PageViewModelBase _CurrentPage;

    public PageViewModelBase CurrentPage
    {
        get => _CurrentPage;
        set
        {
            CurrentPage.NavigateNextEvent -= OnNavigateNext;
            this.RaiseAndSetIfChanged(ref _CurrentPage, value);
            CurrentPage.NavigateNextEvent += OnNavigateNext;
        }
    }

    public ICommand NavigatePreviousCommand { get; }

    private void NavigatePrevious()
    {
        CurrentPage = Pages[0];
    }


    protected void OnNavigateNext(object? sender, NavigateNextEventArgs e)
    {
        if (e.pageNumber == 2 && !((ParameterSelectionPageViewModel)Pages[2]).UpdateParameterList())
        {
            CurrentPage = Pages[3];
            return;
        }
        if (e.pageNumber == 1)
        {
            ((PickNewTemplatePageViewModel)Pages[1]).RefreshTemplateList();
        }
        CurrentPage = Pages[e.pageNumber];
    }
}
