using System;
using System.Collections.Generic;

namespace LaTeXTableGenerator.Model
{
    public class Table
    {
        public List<Row> Rows { get; }
        
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public string TableCaption { get; set; }

        public bool VerticalTableLines { get; set; } = true;

        public bool HorizontalTableLines { get; set; } = true;

        public Table(List<Row> rows)
        {
            TableCaption = string.Empty;

            ValidateRows(rows);

            Rows = rows;
        }

        public Table(int rows, int columns)
        {
            TableCaption = string.Empty;
            Rows = new List<Row>();

            InitializeTable(rows, columns);
        }


        private void ValidateRows(List<Row> rows)
        {
            if (rows.Count <= 0)
                throw new ArgumentException("There must be at least one row!");

            var columnCount = rows[0].Cells.Count;

            if (columnCount == 0)
                throw new ArgumentException("There must be at least one column!");


            for (int i = 1; i < rows.Count; i++)
            {
                if (rows[i].Cells.Count != columnCount)
                    throw new ArgumentException("All rows must have the same count of columns!");
            }

            ColumnCount = columnCount;
            RowCount = rows.Count;
        }

        private void InitializeTable(int rows, int columns)
        {
            RowCount = rows;
            ColumnCount = columns;

            Rows.Clear();

            for (int i = 0; i < rows; i++)
            {
                Rows.Add(GenerateRow(columns));
            }
        }

        private static Row GenerateRow(int columns)
        {
            var cells = new List<Cell>();

            for (int j = 0; j < columns; j++)
            {
                cells.Add(new Cell());
            }

            var row = new Row(cells);
            return row;
        }
    }
}
