using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ControlModifiedFiles
{
    internal static class Dialog
    {
        internal static bool DialogQuestion(string textQuestion)
        {
            return MessageBox.Show(textQuestion, "Вопрос", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        internal static void ShowMessage(string textMessage)
        {
            MessageBox.Show(textMessage);
        }
    }
}
