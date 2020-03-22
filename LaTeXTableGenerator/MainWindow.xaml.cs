using System.Windows;
using System.Windows.Controls;

namespace LaTeXTableGenerator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_OnAutoGeneratingColumn(
            object sender, 
            DataGridAutoGeneratingColumnEventArgs e)
        {
            var dataTemplate = (DataTemplate) Resources["CellDataTemplate"];
            var editDataTemplate = (DataTemplate) Resources["EditCellDataTemplate"];

            var column = new DataGridTemplateColumn()
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                CellTemplate = dataTemplate,
                CellEditingTemplate = editDataTemplate,
                Header = e.Column.Header,
                HeaderTemplate = e.Column.HeaderTemplate,
                HeaderStringFormat = e.Column.HeaderStringFormat,
                SortMemberPath = e.PropertyName // this is used to index into the DataRowView so it MUST be the property's name (for this implementation anyways)
            };
            e.Column = column;
        }
    }
}
