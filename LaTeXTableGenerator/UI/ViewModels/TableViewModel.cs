using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Input;
using LaTeXTableGenerator.Model;
using LaTeXTableGenerator.Utils;
using Microsoft.Xaml.Behaviors.Core;

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

        private List<RowViewModel> Rows { get; set; }

        public ObservableCollection<object> SelectedCells { get; set; }

        private IEnumerable<CellViewModel> SelectedCellsViewModels =>
            SelectedCells?
                .Select(i => i as CellViewModel)
                .Where(i => i != null) ?? Enumerable.Empty<CellViewModel>();

        public bool ContextMenuItalicChecked
        {
            get => SelectedCellsViewModels.Any(c => c.IsItalic);
            set => SelectedCellsViewModels.ForEach(c => c.IsItalic = value);
        }

        public bool ContextMenuBoldChecked
        {
            get => SelectedCellsViewModels.Any(c => c.IsBold);
            set => SelectedCellsViewModels.ForEach(c => c.IsBold = value);
        }

        public ICommand AddRowCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand AddColumnCommand { get; set; }
        public ICommand DeleteColumnCommand { get; set; }

        public Table Table { get; set; }

        public TableViewModel()
        {
            SelectedCells = new ObservableCollection<object>();
            AddRowCommand = new RelayCommand(OnAddRowCommand);
            AddColumnCommand = new RelayCommand(OnAddColumnCommand);
            DeleteRowCommand = new RelayCommand(OnDeleteRowCommand);
            DeleteColumnCommand = new RelayCommand(OnDeleteColumnCommand);

            TableItemsSource = new DataTable();
            Rows = new List<RowViewModel>();

            InitializeTable(10, 5);
        }

        public TableViewModel(Table table)
        {
            Table = table;
        }

        private void OnDeleteColumnCommand(object obj)
        {
            var range = GetRangeOfSelection();

            var dataTable = TableItemsSource;
            TableItemsSource = null;

            SelectedCells.Clear();

            for (int i = range.maxColumn; i >= range.minColumn; i--)
            {
                DeleteColumn(i, dataTable);
            }

            TableItemsSource = dataTable;
        }

        private void DeleteColumn(int index, DataTable dataTable)
        {
            foreach (var row in Rows)
            {
                row.Cells.RemoveAt(index);
            }

            dataTable.Columns.RemoveAt(index);
        }

        private void OnDeleteRowCommand(object obj)
        {
            var range = GetRangeOfSelection();

            var dataTable = TableItemsSource;
            TableItemsSource = null;

            SelectedCells.Clear();

            for (int i = range.maxRow; i >= range.minRow; i--)
            {
                DeleteRow(i, dataTable);
            }

            TableItemsSource = dataTable;
        }

        private void DeleteRow(int index, DataTable dataTable)
        {
            Rows.RemoveAt(index);
            dataTable.Rows.RemoveAt(index);
        }

        private void OnAddColumnCommand(object obj)
        {
            
        }

        private void OnAddRowCommand(object obj)
        {

        }

        private (
            int minRow, 
            int minColumn, 
            int maxRow, 
            int maxColumn) GetRangeOfSelection()
        {
            var selection = SelectedCellsViewModels.ToList();

            var minRow = int.MaxValue;
            var maxRow = 0;
            var minColumn = int.MaxValue;
            var maxColumn = 0;

            foreach (var cell in selection)
            {
                bool breakLoop = false;
                int rowCounter = 0;
                foreach (var row in TableItemsSource.Rows.Cast<DataRow>())
                {
                    int columnCounter = 0;
                    foreach (var rowCell in row.ItemArray.Cast<CellViewModel>())
                    {
                        if (rowCell == cell)
                        {
                            minRow = Math.Min(minRow, rowCounter);
                            maxRow = Math.Max(maxRow, rowCounter);
                            minColumn = Math.Min(minColumn, columnCounter);
                            maxColumn = Math.Max(maxColumn, columnCounter);

                            breakLoop = true;
                            break;
                        }

                        columnCounter++;
                    }

                    if (breakLoop)
                        break;

                    rowCounter++;
                }
            }

            return (
                minRow: minRow, 
                minColumn: minColumn, 
                maxRow: maxRow, 
                maxColumn: maxColumn);
        }

        public void InitializeTable(int rows, int columns)
        {
            Rows.Clear();

            for (int i = 0; i < rows; i++)
            {
                var cells = new List<CellViewModel>();

                for (int j = 0; j < columns; j++)
                {
                    cells.Add(new CellViewModel());
                }

                Rows.Add(new RowViewModel(cells));
            }

            TableItemsSource.Clear();

            for (int i = 0; i < columns; i++)
            {
                TableItemsSource.Columns.Add($"Column{i}", typeof(CellViewModel));
            }

            for (int i = 0; i < rows; i++)
            {
                var parameters = Rows[i].Cells.ToArray<object>();

                TableItemsSource.Rows.Add(parameters);
            }

        }

        public Table ToTable()
        {
            var rows = Rows.Select(r => r.ToRow()).ToList();

            var table = new Table(rows);

            return table;
        }
    }
}
