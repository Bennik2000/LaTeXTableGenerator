using LaTeXTableGenerator.Model;
using System.Threading.Tasks;

namespace LaTeXTableGenerator.Data
{
    interface ITableReader
    {
        Task<Table> ReadTable();
    }
}
