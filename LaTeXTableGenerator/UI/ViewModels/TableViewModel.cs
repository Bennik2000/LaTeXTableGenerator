using LaTeXTableGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Table = LaTeXTableGenerator.Model.Table;

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

        public string TableCaption
        {
            get => _tableCaption;
            set
            {
                if (value == _tableCaption) return;
                _tableCaption = value;
                OnPropertyChanged();
                TableChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private string _tableCaption;

        public bool VerticalTableLines
        {
            get => _verticalTableLines;
            set
            {
                if (value == _verticalTableLines) return;
                _verticalTableLines = value;
                OnPropertyChanged();
                TableChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool _verticalTableLines;

        public bool HorizontalTableLines
        {
            get => _horizontalTableLines;
            set
            {
                if (value == _horizontalTableLines) return;
                _horizontalTableLines = value;
                OnPropertyChanged();
                TableChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool _horizontalTableLines;
        
        public ObservableCollection<object> SelectedCells
        {
            get => _selectedCells;
            set
            {
                if (Equals(value, _selectedCells)) return;

                if (_selectedCells != null) _selectedCells.CollectionChanged -= OnSelectedCellsChanged;
                _selectedCells = value;
                if (_selectedCells != null) _selectedCells.CollectionChanged += OnSelectedCellsChanged;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<object> _selectedCells;

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

        public ICommand AddRowAboveCommand { get; private set; }
        public ICommand AddRowBelowCommand { get; private set; }
        public ICommand DeleteRowCommand { get; private set; }
        public ICommand AddColumnLeftCommand { get; private set; }
        public ICommand AddColumnRightCommand { get; private set; }
        public ICommand DeleteColumnCommand { get; private set; }

        public event TableChangedEventHandler TableChanged;


        public TableViewModel(Table table)
        {
            InitializeViewModel();
            FromTable(table);
        }


        void InitializeViewModel()
        {
            SelectedCells = new ObservableCollection<object>();
            AddRowAboveCommand = new RelayCommand(OnAddRowAboveCommand);
            AddRowBelowCommand = new RelayCommand(OnAddRowBelowCommand);
            AddColumnLeftCommand = new RelayCommand(OnAddColumnLeftCommand);
            AddColumnRightCommand = new RelayCommand(OnAddColumnRightCommand);
            DeleteRowCommand = new RelayCommand(OnDeleteRowCommand);
            DeleteColumnCommand = new RelayCommand(OnDeleteColumnCommand);

            TableItemsSource = new DataTable();
            Rows = new List<RowViewModel>();
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

            TableChanged?.Invoke(this, EventArgs.Empty);
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

            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddColumnLeftCommand(object obj)
        {
            var range = GetRangeOfSelection();

            AddColumn(range.hasSelection 
                ? range.minColumn 
                : 0);

            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddColumnRightCommand(object obj)
        {
            var range = GetRangeOfSelection();

            AddColumn(range.hasSelection
                ? range.maxColumn + 1
                : TableItemsSource.Columns.Count);


            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddRowAboveCommand(object obj)
        {
            var range = GetRangeOfSelection();

            AddRow(range.hasSelection ? range.minRow : 0);

            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddRowBelowCommand(object obj)
        {
            var range = GetRangeOfSelection();

            AddRow(range.hasSelection ? range.maxRow + 1 : Rows.Count);

            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectedCellsChanged(object sender, EventArgs args)
        {
            OnPropertyChanged(nameof(ContextMenuItalicChecked));
            OnPropertyChanged(nameof(ContextMenuBoldChecked));
        }

        private void AddColumn(int index)
        {
            foreach (var row in Rows)
            {
                var cell = new CellViewModel();
                cell.PropertyChanged += CellPropertyChanged;

                row.Cells.Insert(index, cell);
            }

            var newColumnCount = TableItemsSource.Columns.Count + 1;

            var newTable = CreateDataTable(Rows, newColumnCount);

            TableItemsSource = newTable;
        }

        private void AddRow(int index)
        {
            var cells = new List<CellViewModel>();

            for (int i = 0; i < TableItemsSource.Columns.Count; i++)
            {
                var cell = new CellViewModel();
                cell.PropertyChanged += CellPropertyChanged;

                cells.Add(cell);
            }

            Rows.Insert(index, new RowViewModel(cells));

            var table = TableItemsSource;
            TableItemsSource = null;

            var row = table.NewRow();

            row.ItemArray = cells.ToArray<object>();

            table.Rows.InsertAt(row, index);

            TableItemsSource = table;
        }

        private void DeleteColumn(int index, DataTable dataTable)
        {
            if (dataTable.Columns.Count <= 1) return;

            foreach (var row in Rows)
            {
                if(row.Cells.Count == 0) continue;
                
                var cell = row.Cells[index];
                cell.PropertyChanged -= CellPropertyChanged;

                row.Cells.RemoveAt(index);
            }

            dataTable.Columns.RemoveAt(index);
        }

        private void DeleteRow(int index, DataTable dataTable)
        {
            if (Rows.Count <= 1) return;

            var row = Rows[index];
            foreach (var cell in row.Cells)
            {
                cell.PropertyChanged -= CellPropertyChanged;
            }

            Rows.RemoveAt(index);
            dataTable.Rows.RemoveAt(index);
        }

        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TableChanged?.Invoke(this, EventArgs.Empty);
        }

        private (
            int minRow,
            int minColumn,
            int maxRow,
            int maxColumn,
            bool hasSelection) GetRangeOfSelection()
        {
            var selection = SelectedCellsViewModels.ToList();

            if (!selection.Any())
                return (0, 0, 0, 0, false);

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

            return (minRow, minColumn, maxRow, maxColumn, true);
        }

        private DataTable CreateDataTable(IEnumerable<RowViewModel> rows, int columns)
        {
            var dataTable = new DataTable();

            for (int i = 0; i < columns; i++)
            {
                dataTable.Columns.Add($"Column {i + 1}", typeof(CellViewModel));
            }

            foreach (var row in rows)
            {
                var parameters = row.Cells.ToArray<object>();
                dataTable.Rows.Add(parameters);
            }

            return dataTable;
        }

        public void FromTable(Table table)
        {
            TableCaption = table.TableCaption;
            VerticalTableLines = table.VerticalTableLines;
            HorizontalTableLines = table.HorizontalTableLines;

            SelectedCells.Clear();
            Rows.Clear();
            TableItemsSource.Clear();

            Rows = table.Rows.Select(t =>
                new RowViewModel(t.Cells.Select((c) =>
                {
                    var cell = new CellViewModel(c);
                    cell.PropertyChanged += CellPropertyChanged;
                    return cell;
                }))).ToList();

            TableItemsSource = CreateDataTable(Rows, table.ColumnCount);
        }

        public Table ToTable()
        {
            var rows = Rows.Select(r => r.ToRow()).ToList();

            var table = new Table(rows)
            {
                TableCaption = TableCaption,
                VerticalTableLines = VerticalTableLines,
                HorizontalTableLines = HorizontalTableLines
            };

            return table;
        }
    }

    public delegate void TableChangedEventHandler(object sender, EventArgs args);
}
