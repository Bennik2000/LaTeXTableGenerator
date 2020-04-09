namespace LaTeXTableGenerator.Model
{
    public class ColumnProperties
    {
        public double Width { get; set; }
        public ColumnAlignment Alignment { get; set; }

        public enum ColumnAlignment
        {
            Left,
            Right,
            Centered
        }
    }
}
