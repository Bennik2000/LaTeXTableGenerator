using LaTeXTableGenerator.Model;

namespace LaTeXTableGenerator.UI.ViewModels
{
    public class CellViewModel : ViewModelBase
    {
        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }
        private string _text;

        public bool IsBold
        {
            get => _isBold;
            set
            {
                if (value == _isBold) return;
                _isBold = value;
                OnPropertyChanged();
            }
        }
        private bool _isBold;

        public bool IsItalic
        {
            get => _isItalic;
            set
            {
                if (value == _isItalic) return;
                _isItalic = value;
                OnPropertyChanged();
            }
        }
        private bool _isItalic;

        public CellViewModel()
        {
            Text = "Lorem ipsum";
            IsBold = true;
            IsItalic = true;
        }

        public Cell ToCell()
        {
            return new Cell(Text, IsBold, IsItalic);
        }
    }
}
