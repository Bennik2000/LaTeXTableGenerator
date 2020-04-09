using System;
using System.Windows;
using System.Windows.Input;
using LaTeXTableGenerator.Data;
using LaTeXTableGenerator.LaTeX;
using LaTeXTableGenerator.Model;
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

                if(_tableViewModel != null) _tableViewModel.TableChanged -= TableChangedEvent;
                if (value != null) value.TableChanged += TableChangedEvent;

                _tableViewModel = value;

                if(_tableViewModel != null) GenerateLaTeXCommand?.Execute(null);

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
            OutputViewModel = new OutputViewModel();
            TableViewModel = new TableViewModel(new Table(4, 3));
        }

        private void OnGenerateLaTeXCommand(object obj)
        {
            try
            {
                var laTeX =
                    new LaTeXOutputGenerator().GenerateTable(TableViewModel.ToTable());
                OutputViewModel.Code = laTeX;
            }
            catch (Exception)
            {
                OutputViewModel.Code = string.Empty;
            }
        }

        private async void OnLoadTableCommand(object obj)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Markdown files (*.md)|*.md|Excel files|*.xls;*.xlsx|LaTeX files (*.tex)|*.tex|All files (*.*)|*.*"
            };

            var result = dialog.ShowDialog();

            if (result ?? false)
            {
                try
                {
                    var table = await new TableLoader().LoadTable(dialog.FileName);
                    TableViewModel = new TableViewModel(table);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Failed to load table!\n{e.Message}", "Failed to load table");
                }
            }
        }

        private void TableChangedEvent(object sender, EventArgs args)
        {
            GenerateLaTeXCommand?.Execute(null);
        }
    }
}
