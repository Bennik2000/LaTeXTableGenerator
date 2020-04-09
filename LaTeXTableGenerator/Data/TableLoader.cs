using LaTeXTableGenerator.Model;
using System.IO;
using System.Threading.Tasks;

namespace LaTeXTableGenerator.Data
{
    public class TableLoader
    {
        public Task<Table> LoadTable(string path)
        {
            var reader = GetReader(path);

            if (reader == null)
                return Task.FromResult<Table>(null);

            return reader.ReadTable();
        }

        private ITableReader GetReader(string path)
        {
            var extension = Path.GetExtension(path);

            extension = extension?.ToLower();

            if (extension == ".md")
            {
                return new MarkdownTableReader(path);
            }

            if (extension == ".xls" || extension == ".xlsx")
            {
                return new ExcelTableReader(path);
            }

            if (extension == ".tex")
            {
                return new LaTeXTableReader(path);
            }

            return null;
        }
    }
}
