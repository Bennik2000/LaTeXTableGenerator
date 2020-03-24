namespace LaTeXTableGenerator.UI.ViewModels
{
    public class OutputViewModel : ViewModelBase
    {
        public string Code
        {
            get => _code;
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged();
            }
        }
        private string _code;
    }
}
