using LaTeXTableGenerator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Table = LaTeXTableGenerator.Model.Table;

namespace LaTeXTableGenerator.Data
{
    /// <summary>
    /// Parses a markdown table.
    /// This is a markdown table:
    /// # Heading
    /// | Lorem ipsum | dolor      | sit        |
    /// | ----------- | ---------- | ---------- |
    /// | amet        | consetetur | sadipscing |
    /// | elitr       | sed        | diam       |
    /// | nonumy      | eirmod     | tempor     |
    /// 
    /// </summary>
    public class MarkdownTableReader : ITableReader
    {
        private string Path { get; set; }

        public MarkdownTableReader(string path)
        {
            Path = path;
        }

        public Task<Table> ReadTable()
        {
            var rows = new List<Row>();

            var lines = File.ReadAllLines(Path);

            var columns = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if(string.IsNullOrWhiteSpace(line))
                    continue;
                
                if(!line.Trim().StartsWith("|"))
                    continue;

                if (line.Distinct().All(c => c == ' ' || c == '-' || c == '|'))
                    continue;

                try
                {
                    var row = ReadRowFromLine(line, columns);
                    rows.Add(row);
                }
                catch (Exception e)
                {
                    throw new Exception($"Failed to parse line {i}", e);
                }
            }

            var tableCaption = System.IO.Path.GetFileNameWithoutExtension(Path);

            var table = new Table(rows)
            {
                TableCaption = tableCaption
            };

            return Task.FromResult(table);
        }

        private Row ReadRowFromLine(string line, int expectedColumns = -1)
        {
            // Remove first | and last |
            line = line.Substring(1, line.Length - 2);

            var parts = line.Split('|');

            var columns = new List<Cell>();

            var regex = new Regex(@"<br[ ]*\/>");

            foreach (var part in parts)
            {
                var text = part.Trim();

                text = regex.Replace(text, "\n");

                columns.Add(new Cell(text, false, false));
            }

            if (expectedColumns >= 0 && columns.Count < expectedColumns)
                throw new Exception("Failed to parse row!");

            return new Row(columns);
        }
    }
}
