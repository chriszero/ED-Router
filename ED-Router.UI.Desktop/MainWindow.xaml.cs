using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NHotkey.Wpf;

namespace ED_Router.UI.Desktop
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (App.IsFromVA)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                Dispose();
            }
            
            base.OnClosing(e);
        }

        public void Dispose()
        {
            // Let's ensure everything is cleaned up.
            foreach (var keyBindings in InputBindings.OfType<KeyBinding>())
            {
                HotkeyManager.SetRegisterGlobalHotkey(keyBindings, false);
            }
        }
    }
}
