using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneGap
{
    public class NotificationCommand:Command
    {
        public bool accept(string instruction)
        {
            if (instruction.StartsWith("notification"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string execute(string command, string[] parameters)
        {
            return "window.gapTest = 'this is a test';";
        }
    }
}
