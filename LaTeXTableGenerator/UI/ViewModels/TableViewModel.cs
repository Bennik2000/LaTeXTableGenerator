using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using LaTeXTableGenerator.Model;

namespace LaTeXTableGenerator.UI.ViewModels
{
    public class TableViewModel : ViewModelBase
    {

        public DataTable TableItemsSource
        {
            get => _tableItemsSource;
            private set
            {
                if (Equals(value, _tableItemsSource)) return;
                _tableItemsSource = value;
                OnPropertyChanged();
            }
        }
        private DataTable _tableItemsSource;

        private List<List<CellViewModel>> Cells { get; set; }

        public ObservableCollection<object> SelectedCells { 
            get; 
            set;
        }

        private IEnumerable<CellViewModel> SelectedCellsViewModels =>
            SelectedCells?
                .Select(i => i as CellViewModel)
                .Where(i => i != null) ?? Enumerable.Empty<CellViewModel>();

        public bool ContextMenuItalicChecked
        {
            get => SelectedCellsViewModels.Any(c => c.IsItalic);
            set
            {
                foreach (var selectedCellsViewModel in SelectedCellsViewModels)
                {
                    selectedCellsViewModel.IsItalic = value;
                }
            }
        }

        public bool ContextMenuBoldChecked
        {
            get => SelectedCellsViewModels.Any(c => c.IsBold);
            set
            {
                foreach (var selectedCellsViewModel in SelectedCellsViewModels)
                {
                    selectedCellsViewModel.IsBold = value;
                }
            }
        }

        public TableViewModel()
        {
            TableItemsSource = new DataTable();
            Cells = new List<List<CellViewModel>>();

            InitializeTable(10, 5);
        }

        public void InitializeTable(int rows, int columns)
        {
            Cells.Clear();

            for (int i = 0; i < rows; i++)
            {
                var row = new List<CellViewModel>();

                for (int j = 0; j < columns; j++)
                {
                    row.Add(new CellViewModel());
                }

                Cells.Add(row);
            }

            TableItemsSource.Clear();


            for (int i = 0; i < columns; i++)
            {
                TableItemsSource.Columns.Add($"Column{i}", typeof(CellViewModel));
            }

            for (int i = 0; i < rows; i++)
            {
                var parameters = new object[columns];

                for (int j = 0; j < columns; j++)
                {
                    parameters[j] = Cells[i][j];
                }

                TableItemsSource.Rows.Add(parameters);
            }

        }
    }
}
