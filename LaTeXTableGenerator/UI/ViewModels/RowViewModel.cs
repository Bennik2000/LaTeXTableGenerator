using LaTeXTableGenerator.Model;
using System.Collections.Generic;
using System.Linq;

namespace LaTeXTableGenerator.UI.ViewModels
{
    public class RowViewModel : ViewModelBase
    {
        public List<CellViewModel> Cells { get; set; }

        public RowViewModel(IEnumerable<CellViewModel> cells) 
            : this(cells.ToList()) { }

        public RowViewModel(List<CellViewModel> cells)
        {
            Cells = cells;
        }

        public Row ToRow()
        {
            return new Row(Cells.Select(c => c.ToCell()).ToList());
        }
    }
}
