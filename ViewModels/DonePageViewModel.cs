using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateGenerator.Controls;

namespace TemplateGenerator.ViewModels
{
    public class DonePageViewModel : PageViewModelBase
    {
        public DonePageViewModel() : base()
        {
            NextPageCommand = ReactiveCommand.Create(NextPage);
        }
        public DonePageViewModel(TemplateEngineWrapper templateEngineWrapper) : base(templateEngineWrapper)
        {
            NextPageCommand = ReactiveCommand.Create(NextPage);
        }

        public ICommand NextPageCommand { get; }

        private void NextPage()
        {
            InvokeNavigateNextEvent(0);
        }
    }
}
