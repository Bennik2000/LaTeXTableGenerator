using System.Collections.Generic;
using System.Data;
using LaTeXTableGenerator.Model;
using System.Text;

namespace LaTeXTableGenerator.LaTeX
{
    public class LaTeXOutputGenerator
    {
        public string GenerateTable(Table table)
        {
            var builder = new StringBuilder();

            AppendTableHead(builder, table);

            var latexEscape = new LaTeXTextEscape();

            foreach (var row in table.Rows)
            {
                bool isFirst = true;
                foreach (var cell in row.Cells)
                {
                    var cellLaTeX = latexEscape.Escape(cell.Text);

                    if (cell.IsBold) cellLaTeX = $"\\textbf{{{cellLaTeX}}}";
                    if (cell.IsItalic) cellLaTeX = $"\\textit{{{cellLaTeX}}}";

                    builder.Append("    ");
                    builder.Append(!isFirst ? "& " : "  ");
                    builder.AppendLine(cellLaTeX);

                    isFirst = false;
                }

                builder.AppendLine(@"\tabularnewline");

                if (table.HorizontalTableLines) 
                    builder.AppendLine(@"\hline");
            }

            AppendTableFooter(builder, table);

            return builder.ToString();
        }

        private void AppendTableHead(StringBuilder builder, Table table)
        {
            builder.AppendLine(@"\begin{longtable}[l]{");


            for (int i = 0; i < table.ColumnCount; i++)
            {
                builder.Append("  ");
                if (table.VerticalTableLines)
                    builder.Append("|");

                builder.AppendLine("l");

            }

            if (table.VerticalTableLines)
                builder.Append("|");

            builder.AppendLine("}");

            if (table.HorizontalTableLines)
                builder.AppendLine(@"\hline");
        }

        private void AppendTableFooter(StringBuilder stringBuilder, Table table)
        {
            var caption = new LaTeXTextEscape().Escape(table.TableCaption);
            
            var label = table.TableCaption?.ToLower()?.Replace(' ', '-') ?? string.Empty;

            stringBuilder.AppendFormat("\\caption{{{0}}}\n" +
                                       "\\label{{tab:{1}}}\n" +
                                       "\\end{{longtable}}", 
                caption, 
                label);
        }
    }
}
