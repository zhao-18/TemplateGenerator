using ReactiveUI;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemplateGenerator.Controls;

namespace TemplateGenerator.ViewModels
{
    public class PageViewModelBase : ViewModelBase
    {
        public PageViewModelBase() {
            TemplateEngineWrapper = new TemplateEngineWrapper("TemplateGeneratoor", "Version");
        }

        public PageViewModelBase(TemplateEngineWrapper templateEngineWrapper)
        {
            TemplateEngineWrapper = templateEngineWrapper;
        }

        public TemplateEngineWrapper TemplateEngineWrapper { get; set; }

        public event EventHandler<NavigateNextEventArgs>? NavigateNextEvent;

        public void InvokeNavigateNextEvent(int pageNumber)
        {
            if (NavigateNextEvent != null)
            {
                NavigateNextEvent(this, new NavigateNextEventArgs(pageNumber));
            }
        }
    }
}
