namespace LaTeXTableGenerator.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public TableViewModel TableViewModel
        {
            get => _tableViewModel;
            set
            {
                if (Equals(value, _tableViewModel)) return;
                _tableViewModel = value;
                OnPropertyChanged();
            }
        }
        private TableViewModel _tableViewModel;

        public OutputViewModel OutputViewModel
        {
            get => _outputViewModel;
            set
            {
                if (Equals(value, _outputViewModel)) return;
                _outputViewModel = value;
                OnPropertyChanged();
            }
        }
        private OutputViewModel _outputViewModel;

        public MainWindowViewModel()
        {
            TableViewModel = new TableViewModel();
        }
    }
}
