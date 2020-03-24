using System.Windows.Input;
using LaTeXTableGenerator.Data;
using LaTeXTableGenerator.LaTeX;
using LaTeXTableGenerator.Utils;
using Microsoft.Win32;

namespace LaTeXTableGenerator.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public TableViewModel TableViewModel
        {
            get => _tableViewModel;
            set
            {
                if (Equals(value, _tableViewModel)) return;
                _tableViewModel = value;
                OnPropertyChanged();
            }
        }
        private TableViewModel _tableViewModel;

        public OutputViewModel OutputViewModel
        {
            get => _outputViewModel;
            set
            {
                if (Equals(value, _outputViewModel)) return;
                _outputViewModel = value;
                OnPropertyChanged();
            }
        }
        private OutputViewModel _outputViewModel;

        public ICommand LoadTableCommand { get; }
        public ICommand GenerateLaTeXCommand { get; }

        public MainWindowViewModel()
        {
            LoadTableCommand = new RelayCommand(OnLoadTableCommand);
            GenerateLaTeXCommand = new RelayCommand(OnGenerateLaTeXCommand);
            TableViewModel = new TableViewModel();
        }

        private void OnGenerateLaTeXCommand(object obj)
        {
            var laTeX = 
                new LaTeXOutputGenerator().GenerateTable(TableViewModel.ToTable());

            OutputViewModel = new OutputViewModel()
            {
                Code = laTeX
            };
        }

        private async void OnLoadTableCommand(object obj)
        {
            var dialog = new OpenFileDialog {Multiselect = false};

            var result = dialog.ShowDialog();

            if (result ?? false)
            {
                var table = await new TableLoader().LoadTable(dialog.FileName);
                TableViewModel = new TableViewModel(table);
            }
        }
    }
}
