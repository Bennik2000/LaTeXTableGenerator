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

            foreach (var row in table.Rows)
            {
                bool isFirst = true;
                foreach (var cell in row.Cells)
                {
                    var cellLaTeX = cell.Text;

                    if (cell.IsBold)
                    {
                        cellLaTeX = $"\\textbf{{{cellLaTeX}}}";
                    }

                    if (cell.IsItalic)
                    {
                        cellLaTeX = $"\\textit{{{cellLaTeX}}}";
                    }

                    builder.Append("    ");

                    if (!isFirst)
                    {
                        builder.Append("& ");
                    }
                    else
                    {
                        builder.Append("  ");
                    }


                    builder
                        .Append(cellLaTeX)
                        .Append("\n");

                    isFirst = false;
                }

                builder.Append("    \\tabularnewline\n" +
                               "\\hline %------------------------------------------------------------------------------------------------------\n");
            }

            AppendTableFooter(builder, table);

            return builder.ToString();
        }

        private void AppendTableHead(StringBuilder stringBuilder, Table table)
        {
            stringBuilder.Append(
                "\\begin{longtable}[l]{\n" +
                "    | >{\\raggedright}p{0.05\\textwidth}\n" +
                "    | >{\\raggedright}p{0.225\\textwidth}\n" +
                "    | >{\\raggedright}p{0.4\\textwidth}\n" +
                "    | >{\\raggedright}p{0.2\\textwidth}\n" +
                "    |}\n");

            stringBuilder.Append(
                "\\hline %------------------------------------------------------------------------------------------------------" +
                "\n");

        }

        private void AppendTableFooter(StringBuilder stringBuilder, Table table)
        {
            var label = table.TableCaption.ToLower().Replace(' ', '-');

            stringBuilder.AppendFormat("\\caption{{{0}}}\n" +
                                 "\\label{{tab:{1}}}\n" +
                                 "\\end{{longtable}}", table.TableCaption, label);
        }
    }
}
