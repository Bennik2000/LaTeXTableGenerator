namespace LaTeXTableGenerator.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public TableViewModel TableViewModel { get; set; }

        public MainWindowViewModel()
        {
            TableViewModel = new TableViewModel();
        }
    }
}
