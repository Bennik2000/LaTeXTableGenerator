using LaTeXTableGenerator.Model;
using LaTeXTableGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;

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
        public ICommand AddColumnLeftCommand { get; set; }
        public ICommand AddColumnRightCommand { get; set; }
        public ICommand DeleteColumnCommand { get; set; }

        public TableViewModel()
        {
            SelectedCells = new ObservableCollection<object>();
            AddRowCommand = new RelayCommand(OnAddRowCommand);
            AddColumnLeftCommand = new RelayCommand(OnAddColumnLeftCommand);
            AddColumnRightCommand = new RelayCommand(OnAddColumnRightCommand);
            DeleteRowCommand = new RelayCommand(OnDeleteRowCommand);
            DeleteColumnCommand = new RelayCommand(OnDeleteColumnCommand);

            TableItemsSource = new DataTable();
            Rows = new List<RowViewModel>();

            InitializeTable(10, 5);
        }

        private void OnDeleteColumnCommand(object obj)
        {
            var range = GetRangeOfSelection();

            var dataTable = TableItemsSource;
            TableItemsSource = null;

            for (int i = range.maxColumn; i >= range.minColumn; i--)
            {
                DeleteColumn(i, dataTable);
            }

            TableItemsSource = dataTable;
        }

        private void OnDeleteRowCommand(object obj)
        {
            var range = GetRangeOfSelection();

            var dataTable = TableItemsSource;
            TableItemsSource = null;

            for (int i = range.maxRow; i >= range.minRow; i--)
            {
                DeleteRow(i, dataTable);
            }

            TableItemsSource = dataTable;
        }

        private void OnAddColumnLeftCommand(object obj)
        {
            var range = GetRangeOfSelection();
            AddColumn(range.minColumn);
        }

        private void OnAddColumnRightCommand(object obj)
        {
            var range = GetRangeOfSelection();
            AddColumn(range.maxColumn + 1);
        }

        private void OnAddRowCommand(object obj)
        {

        }

        private void AddColumn(int index)
        {
            foreach (var row in Rows)
            {
                row.Cells.Insert(index, new CellViewModel());
            }

            var newColumnCount = TableItemsSource.Columns.Count + 1;

            var newTable = CreateDataTable(Rows, newColumnCount);

            TableItemsSource = newTable;
        }

        private void DeleteColumn(int index, DataTable dataTable)
        {
            foreach (var row in Rows)
            {
                row.Cells.RemoveAt(index);
            }

            dataTable.Columns.RemoveAt(index);
        }

        private void DeleteRow(int index, DataTable dataTable)
        {
            Rows.RemoveAt(index);
            dataTable.Rows.RemoveAt(index);
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

            return (minRow, minColumn, maxRow, maxColumn);
        }

        public void InitializeTable(int rows, int columns)
        {
            Rows.Clear();

            for (int i = 0; i < rows; i++)
            {
                var cells = new List<CellViewModel>();

                for (int j = 0; j < columns; j++)
                {
                    cells.Add(new CellViewModel()
                    {
                        Text = $"Lorem Row: {i} Col: {j}"
                    });
                }

                Rows.Add(new RowViewModel(cells));
            }

            TableItemsSource = CreateDataTable(Rows, columns);
        }

        public DataTable CreateDataTable(List<RowViewModel> rows, int columns)
        {
            var dataTable = new DataTable();

            for (int i = 0; i < columns; i++)
            {
                dataTable.Columns.Add($"Column{i}", typeof(CellViewModel));
            }

            foreach (var row in rows)
            {
                var parameters = row.Cells.ToArray<object>();
                dataTable.Rows.Add(parameters);
            }

            return dataTable;
        }

        public Table ToTable()
        {
            var rows = Rows.Select(r => r.ToRow()).ToList();

            var table = new Table(rows);

            return table;
        }
    }
}
