using Microsoft.Xaml.Behaviors;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace LaTeXTableGenerator.UI.Behaviors
{
    /// <summary>
    /// From: https://www.codeproject.com/Articles/412417/Managing-Multiple-selection-in-View-Model-NET-Metr
    /// </summary>
    public class MultiSelectBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(ObservableCollection<object>),
            typeof(MultiSelectBehavior),
            new PropertyMetadata(new ObservableCollection<object>(), PropertyChangedCallback));

        public ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }


        private bool _selectionChangedInProgress; // Flag to avoid infinite loop if same viewmodel is shared by multiple controls

        public MultiSelectBehavior()
        {
            SelectedItems = new ObservableCollection<object>();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedCellsChanged += OnSelectedCellsChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedCellsChanged -= OnSelectedCellsChanged;
        }

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = (s, e) => SelectedItemsChanged(sender, e);
            if (args.OldValue is ObservableCollection<object>)
            {
                (args.OldValue as ObservableCollection<object>).CollectionChanged -= handler;
            }

            if (args.NewValue is ObservableCollection<object>)
            {
                (args.NewValue as ObservableCollection<object>).CollectionChanged += handler;
            }
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is MultiSelectBehavior)
            {
                var listViewBase = (sender as MultiSelectBehavior).AssociatedObject;

                var listSelectedItems = listViewBase.SelectedItems;
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Remove(item);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (!listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Add(item);
                        }
                    }
                }
            }
        }

        private object GetCellFromRow(object value)
        {
            var cell = (DataGridCellInfo) value;

            var cellDataContext = cell.Item as DataRowView;

            return cellDataContext?.Row[cell.Column.SortMemberPath];
        }

        private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (_selectionChangedInProgress) return;
            _selectionChangedInProgress = true;

            // Using GetCellFromRow to obtain the actual cell. This ist necessary 
            // because we use a DataTable as item source

            foreach (var item in e.RemovedCells)
            {
                var cell = GetCellFromRow(item);
                if (SelectedItems.Contains(cell))
                {
                    SelectedItems.Remove(cell);
                }
            }

            foreach (var item in e.AddedCells)
            {
                var cell = GetCellFromRow(item);
                if (!SelectedItems.Contains(cell))
                {
                    SelectedItems.Add(cell);
                }
            }

            _selectionChangedInProgress = false;
        }
    }
}
