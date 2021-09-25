using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ED_Router.Model;
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

        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var fp = EdRouter.Instance.FlightPlan;
            if (e.PropertyName == nameof(StarSystem.Refuel))
            {
                if (fp.RefuelDataAvailable)
                {
                    var binding = (e.Column as DataGridTextColumn)?.Binding;
                    e.Column = new DataGridCheckBoxColumn()
                    {
                        Binding = binding,
                        Header = "Refuel"
                    };
                }
                else
                {
                    e.Cancel = true;
                }
            }
            if (e.PropertyName == nameof(StarSystem.Jumps))
            {
                if (fp.PlanType != PlanType.NeutronPlotterAPI && fp.PlanType != PlanType.NeutronPlotterCSV)
                {
                    e.Cancel = true;
                }
            }
            if (e.PropertyName == nameof(StarSystem.FuelLeft))
            {
                if (fp.PlanType != PlanType.GalaxyPlotterCSV && fp.PlanType != PlanType.GalaxyPlotterAPI)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Column.Header = "Fuel Left";
                }
            }
            if (e.PropertyName == nameof(StarSystem.HasNeutronStar))
            {
                e.Column.Header = "Neutron";
            }
            if (e.PropertyName == nameof(StarSystem.DistanceToStar))
            {
                e.Column.Header = "Distance to waypoint";
            }
            if (e.PropertyName == nameof(StarSystem.DistanceLeft))
            {
                e.Column.Header = "Distance remaining";
            }
            if (e.PropertyName == nameof(StarSystem.FuelUsed))
            {
                if (fp.PlanType != PlanType.GalaxyPlotterCSV && fp.PlanType != PlanType.GalaxyPlotterAPI)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Column.Header = "Fuel cost";
                }
            }
        }
    }
}
