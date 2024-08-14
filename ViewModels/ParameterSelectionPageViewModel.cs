using System.Collections.ObjectModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateGenerator.Controls;

using InputParameterDataSet = System.Collections.Generic.KeyValuePair<Microsoft.TemplateEngine.Abstractions.ITemplateParameter, Microsoft.TemplateEngine.Edge.Template.InputParameterData>;
using Microsoft.TemplateEngine.Edge.Template;
using Microsoft.TemplateEngine.Abstractions;
using System.Diagnostics;
using System.Windows.Input;
using TemplateGenerator.Converters;
using System.IO;

namespace TemplateGenerator.ViewModels
{
    public class ParameterSelectionPageViewModel : PageViewModelBase
    {
        public ParameterSelectionPageViewModel() : base()
        {
            System.IObservable<bool> canNavNext = this.WhenAnyValue(
                o => o.CanNavigateNext
            );
            NextPageCommand = ReactiveCommand.Create(NextPage, canNavNext);
        }
        public ParameterSelectionPageViewModel(TemplateEngineWrapper templateEngineWrapper) : base(templateEngineWrapper)
        {
            System.IObservable<bool> canNavNext = this.WhenAnyValue(
                o => o.CanNavigateNext
            );
            NextPageCommand = ReactiveCommand.Create(NextPage, canNavNext);
        }

        public ObservableCollection<InputParameterDataSetViewModel> ParameterList { get; set; } = new ObservableCollection<InputParameterDataSetViewModel>();

        public bool UpdateParameterList()
        {

            ParameterList.Clear();
            if (TemplateEngineWrapper.SelectedTemplate is not ITemplateInfo template)
            {
                throw new InvalidOperationException("TemplateEngineWrapper.SelectedTemplate is not ITemplateInfo template evaluated to true");
            }

            InputDataSet param = new InputDataSet(template);
            foreach(InputParameterDataSet paramData in param)
            {
                if (paramData.Key.Name == "name" || paramData.Key.Name == "language" || paramData.Key.Name == "type")
                {
                    // Things that user will not be able to change.
                    continue;
                }
                InputParameterDataSetViewModel inputParam = new InputParameterDataSetViewModel(paramData);
                ParameterList.Add(inputParam);
            }

            if (ParameterList.Count == 0)
            {
                // Create Template here
                InstantiateTemplateAsyncWrapper();
                return false;
            }

            return true;
        }

        public ICommand NextPageCommand { get; }

        private void NextPage()
        {
            // Create Template here
            InstantiateTemplateAsyncWrapper();
            InvokeNavigateNextEvent(3);
        }

        public void InstantiateTemplateAsyncWrapper()
        {
            Dictionary<string, string?> ParameterIn = new Dictionary<string, string?>();
            foreach (InputParameterDataSetViewModel param in ParameterList)
            {
                ParameterIn.Add(param.parameter.Key.Name, param.Value);
            }

            if (TemplateEngineWrapper.SelectedTemplate is null)
            {
                TemplateEngineWrapper.LastRunResult = false;
                return;
            }

            TemplateEngineWrapper.LastRunResult = Task.Run(
                async () => await TemplateEngineWrapper.CreateTemplateAsync(
                    TemplateEngineWrapper.SelectedTemplate,
                    FuncConverters.PathConverter(TemplateEngineWrapper.InstantiationLocation?.Path.ToString()) ?? TemplateEngineWrapper.DefaultTemplateDeployLocation,
                    TemplateEngineWrapper.TemplateName,
                    ParameterIn)
            ).Result;
        }


        private bool _canNavigateNext = true;

        public bool CanNavigateNext
        {
            get => _canNavigateNext;
            set => this.RaiseAndSetIfChanged(ref _canNavigateNext, value);
        }



        /// <TODO>
        /// Implement the method to verify all the choice and bool 
        /// </TODO>
        private void UpdateCanNavigateNext()
        {
            CanNavigateNext = true;
        }
    }
}
