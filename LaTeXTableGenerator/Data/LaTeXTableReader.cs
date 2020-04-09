using System.CodeDom;
using LaTeXTableGenerator.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LaTeXTableGenerator.Data
{
    public class LaTeXTableReader : ITableReader
    {
        private const string TableBeginRegex =
            @"\\begin{(table|longtable|tabley|tablex|tabular)}[ \r\n\t]*(\[(l|c|r)\])?[ \r\n\t]*";

        private const string TableEndRegex =
            @"\\end{(table|longtable|tabley|tablex|tabular)}";

        private const string TableColumnDefinitionRegex = @"{[lcr|\n\r\t ]+}";

        private const string TableNewLineRegex = @"(\\tabularnewline|\\\\)";

        private const string TableNewColumnRegex = @"(?<!\\)(&)";

        private const string HlineRegex = @"(\\hline)";

        private const string TextbfRegex = @"\\textbf{([^}.]*)}";

        private const string TextitRegex = @"\\textit{([^}.]*)}";


        public string Path { get; set; }


        public LaTeXTableReader(string path)
        {
            Path = path;
        }

        public Task<Table> ReadTable()
        {
            var fileContent = File.ReadAllText(Path);

            var tableString = ExtractTableLaTeXFromFile(fileContent);

            if (string.IsNullOrWhiteSpace(tableString)) return null;

            var rows = SplitIntoRows(tableString);

            var tableRows = new List<Row>();

            foreach (var row in rows)
            {
                var columns = SplitIntoColumns(ExtractRow(row));

                if(row == rows.Last() && columns.Length <= 1) continue;

                var tableColumns = columns.Select(ExtractCell).ToList();

                tableRows.Add(new Row(tableColumns));
            }

            return Task.FromResult(new Table(tableRows));
        }

        private string ExtractTableLaTeXFromFile(string fileContent)
        {
            var tableBeginMatcher = new Regex(TableBeginRegex);
            var tableBeginMatch = tableBeginMatcher.Match(fileContent);

            if (!tableBeginMatch.Success) return null;


            var tableEndMatcher = new Regex(TableEndRegex);
            var tableEndMatch = tableEndMatcher.Match(fileContent, tableBeginMatch.Index);

            if (!tableEndMatch.Success) return null;


            // Search the matching \end{...} statement
            while (tableBeginMatch.Groups[1].Value != tableEndMatch.Groups[1].Value)
            {
                tableEndMatch = tableEndMatcher.Match(fileContent, tableEndMatch.Index + tableEndMatch.Length);

                if (!tableEndMatch.Success) return null;
            }


            var tableContent = fileContent.Substring(tableBeginMatch.Index + tableBeginMatch.Length, 
                tableEndMatch.Index - tableBeginMatch.Index - tableBeginMatch.Length);

            // Remove the {r|l|c|c} part of the table definition
            var tableColumnDefinitionMatcher = new Regex(TableColumnDefinitionRegex);
            tableContent = tableColumnDefinitionMatcher.Replace(tableContent, "", 1);


            // Continue to extract table content until we are at the most inner table
            var subContent = ExtractTableLaTeXFromFile(tableContent);

            if (subContent == null) return tableContent;
            
            return subContent;
        }

        private string[] SplitIntoRows(string tableContent)
        {
            return RegexSplit(tableContent, new Regex(TableNewLineRegex));
        }

        private string ExtractRow(string row)
        {
            var hLineMatcher = new Regex(HlineRegex);
            row = hLineMatcher.Replace(row, "");
            row = row.Trim();

            return row;
        }

        private Cell ExtractCell(string cell)
        {
            var content = cell.Trim();

            var textbfMatcher = new Regex(TextbfRegex);
            var textitMatcher = new Regex(TextitRegex);

            var italicMatch = textitMatcher.Match(cell);
            var boldMatch = textbfMatcher.Match(cell);

            var bold = boldMatch.Success;
            var italic = italicMatch.Success;

            if (bold && italic)
            {
                var inner = boldMatch.Length < italicMatch.Length ? boldMatch : italicMatch;

                cell = inner.Groups[1].Value;
            }
            else if (bold) 
            {
                cell = boldMatch.Groups[1].Value;
            }
            else if (italic) 
            {
                cell = italicMatch.Groups[1].Value;
            }

            cell = cell.Trim();

            return new Cell(cell, bold, italic);
        }

        private string[] SplitIntoColumns(string row)
        {
            return RegexSplit(row, new Regex(TableNewColumnRegex));
        }

        private static string[] RegexSplit(string text, Regex matcher)
        {
            var parts = new List<string>();

            Match match;
            var currentIndex = 0;
            while ((match = matcher.Match(text, currentIndex)).Success)
            {
                var row = text.Substring(currentIndex, match.Index - currentIndex);

                parts.Add(row);

                currentIndex = match.Index + match.Length;
            }

            parts.Add(text.Substring(currentIndex));

            return parts.ToArray();
        }
    }
}
