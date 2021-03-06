﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
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

                    if (!string.IsNullOrWhiteSpace(cellLaTeX))
                    {
                        if (cell.IsBold) cellLaTeX = $"\\textbf{{{cellLaTeX}}}";
                        if (cell.IsItalic) cellLaTeX = $"\\textit{{{cellLaTeX}}}";
                    }

                    builder.Append("    ");
                    builder.Append(!isFirst ? "& " : "  ");
                    builder.Append(cellLaTeX + " ");

                    if (cell != row.Cells.Last())
                        builder.AppendLine();

                    isFirst = false;
                }

                builder.Append(@"\\ ");

                if (table.HorizontalTableLines)
                    builder.AppendLine(@"\hline");
                else
                    builder.AppendLine();
            }

            AppendTableFooter(builder, table);

            return builder.ToString();
        }

        private void AppendTableHead(StringBuilder builder, Table table)
        {
            builder.Append(@"\begin{longtable}[l]{");


            for (int i = 0; i < table.ColumnCount; i++)
            {
                if (table.VerticalTableLines)
                    builder.Append("|");

                builder.Append("l");

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
