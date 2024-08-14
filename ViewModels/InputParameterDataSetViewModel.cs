using Microsoft.TemplateEngine.Abstractions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InputParameterDataSet = System.Collections.Generic.KeyValuePair<Microsoft.TemplateEngine.Abstractions.ITemplateParameter, Microsoft.TemplateEngine.Edge.Template.InputParameterData>;

namespace TemplateGenerator.ViewModels
{
    public class InputParameterDataSetViewModel : ViewModelBase
    {
        public InputParameterDataSetViewModel(InputParameterDataSet input)
        {
            parameter = input;
            _value = input.Key.DefaultValue ?? string.Empty;
            _selectedOption = _value;

            if (input.Key.Choices is IReadOnlyDictionary<string, ParameterChoice> choices)
            {
                foreach (KeyValuePair<string, ParameterChoice> choice in choices)
                {
                    options.Add(choice.Key);
                }
            }
        }

        public InputParameterDataSet parameter { get; set; }

        public ObservableCollection<string> options { get; } = new ObservableCollection<string>();

        private string _selectedOption;
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedOption, value);
                Value = value;
            }
        }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
            }
        }
    }
}
