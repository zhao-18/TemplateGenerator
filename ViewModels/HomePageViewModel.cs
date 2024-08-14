using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using TemplateGenerator.Controls;
using TemplateGenerator.Views;

namespace TemplateGenerator.ViewModels
{
    public class HomePageViewModel : PageViewModelBase
    {
        

        public HomePageViewModel() : base()
        {
            InstantiateTemplateWindowCommand = ReactiveCommand.Create(InstantiateTemplateWindow);
            InstantiateInstallWindowCommand = ReactiveCommand.Create(InstantiateInstallWindow);
        }
        public HomePageViewModel(TemplateEngineWrapper templateEngineWrapper) : base(templateEngineWrapper)
        {
            InstantiateTemplateWindowCommand = ReactiveCommand.Create(InstantiateTemplateWindow);
            InstantiateInstallWindowCommand = ReactiveCommand.Create(InstantiateInstallWindow);
        }

        public ICommand InstantiateTemplateWindowCommand { get; }
        public ICommand InstantiateInstallWindowCommand { get; }

        private void InstantiateTemplateWindow()
        {
            InvokeNavigateNextEvent(1);
        }
        private void InstantiateInstallWindow()
        {
            InvokeNavigateNextEvent(4);
        }
    }
}
