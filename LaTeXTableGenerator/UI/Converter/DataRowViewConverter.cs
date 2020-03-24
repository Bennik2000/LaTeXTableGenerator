using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;

namespace LaTeXTableGenerator.UI.Converter
{
    /// <summary>
    /// Copied from: https://stackoverflow.com/a/25652728/4563449
    /// </summary>
    public class DataRowViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var cell = value as DataGridCell;

            var cellDataContext = cell?.DataContext as DataRowView;

            try
            {
                return cellDataContext?.Row[cell.Column.SortMemberPath];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
