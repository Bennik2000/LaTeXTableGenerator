using LaTeXTableGenerator.Model;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LaTeXTableGenerator.Data
{
    public class ExcelTableReader : ITableReader
    {
        private string Path { get; }

        public ExcelTableReader(string path)
        {
            Path = path;
        }

        public Task<Table> ReadTable()
        {
            using (var package = new ExcelPackage(new FileInfo(Path)))
            {
                var worksheet = package.Workbook.Worksheets
                    .First(w => w.GetType() == typeof(ExcelWorksheet));

                var rows = ReadRows(worksheet);

                var table = new Table(rows);

                return Task.FromResult(table);
            }
        }

        private static List<Row> ReadRows(ExcelWorksheet worksheet)
        {
            var rows = new List<Row>();
            var dimension = worksheet.Dimension;

            for (int i = dimension.Start.Row; i < dimension.End.Row + 1; i++)
            {
                var cells = ReadRow(worksheet, i);

                rows.Add(new Row(cells));
            }

            return rows;
        }

        private static List<Cell> ReadRow(ExcelWorksheet worksheet, int row)
        {
            var dimension = worksheet.Dimension;

            var cells = new List<Cell>();

            for (int j = dimension.Start.Column; j < dimension.End.Column + 1; j++)
            {
                var cell = worksheet.Cells[row, j];

                cells.Add(new Cell()
                {
                    Text = cell.Text,
                    IsBold = cell.Style.Font.Bold,
                    IsItalic = cell.Style.Font.Italic
                });
            }

            return cells;
        }
    }
}
