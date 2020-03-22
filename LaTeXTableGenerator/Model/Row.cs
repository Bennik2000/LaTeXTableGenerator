using System.Collections.Generic;

namespace LaTeXTableGenerator.Model
{
    public class Row
    {
        public List<Cell> Cells { get; private set; }

        public Row(List<Cell> cells)
        {
            Cells = cells;
        }
    }
}
