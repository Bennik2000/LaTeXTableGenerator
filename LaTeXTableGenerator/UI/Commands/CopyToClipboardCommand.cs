using System;
using System.Windows.Input;

namespace LaTeXTableGenerator.UI.Commands
{
    public class CopyToClipboardCommand : ICommand
    {
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            if (parameter is string str)
            {
                System.Windows.Clipboard.SetText(str);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
