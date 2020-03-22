namespace LaTeXTableGenerator.Model
{
    public class Cell
    {
        public string Text { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public Cell()
        {
            Text = string.Empty;
        }

        public Cell(string text, bool isBold, bool isItalic)
        {
            Text = text;
            IsBold = isBold;
            IsItalic = isItalic;
        }
    }
}
