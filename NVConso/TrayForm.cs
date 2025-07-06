namespace NVConso
{
    public class TrayForm : Form
    {
        private readonly NotifyIcon trayIcon;
        private readonly ContextMenuStrip trayMenu;
        private readonly List<ToolStripMenuItem> powerItems = new();
        private readonly INvmlManager _nvml;

        public TrayForm(INvmlManager nvmlManager)
        {
            _nvml = nvmlManager;
            trayMenu = new ContextMenuStrip();

            if (!_nvml.Initialize())
            {
                trayMenu.Items.Add("❌ NVML non initialisé");
            }
            else
            {
                uint current = _nvml.GetCurrentPowerLimit();
                AddPowerMenuItem("🧘 Mode Éco", _nvml.GetPowerLimit(GpuPowerMode.Eco), current);
                AddPowerMenuItem("🔥 Mode Performance", _nvml.GetPowerLimit(GpuPowerMode.Performance), current);
            }

            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Quitter", null, (s, e) =>
            {
                _nvml.Shutdown();
                Application.Exit();
            });

            trayIcon = new NotifyIcon
            {
                Text = "NVConso - Gestion GPU",
                Icon = new Icon("Assets/NVConso.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.MouseUp += (s, e) =>
            {
                if (e.Button is MouseButtons.Left or MouseButtons.Right)
                {
                    trayMenu.Hide();
                    trayMenu.Show(Cursor.Position);
                }
            };
        }

        private void AddPowerMenuItem(string label, uint targetLimit, uint current)
        {
            var item = new ToolStripMenuItem($"{label} ({targetLimit / 1000.0:F1} W)")
            {
                Tag = targetLimit,
                Checked = Math.Abs((int)targetLimit - (int)current) < 200
            };

            item.Click += OnPowerLimitSelected;
            trayMenu.Items.Add(item);
            powerItems.Add(item);
        }

        private void OnPowerLimitSelected(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem) return;

            if (clickedItem.Tag is not uint mw) return;

            bool success = _nvml.SetPowerLimit(mw);
            if (success)
            {
                foreach (var item in powerItems)
                    item.Checked = false;

                clickedItem.Checked = true;
                trayIcon.ShowBalloonTip(1000, "GPU", $"Limite fixée à {mw / 1000.0:F1} W", ToolTipIcon.Info);
            }
            else
            {
                trayIcon.ShowBalloonTip(1000, "Erreur", "Impossible de modifier la limite. Lancement en tant qu'admin ?", ToolTipIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
            base.OnLoad(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            trayIcon.Visible = false;
            base.OnFormClosed(e);
        }
    }
}
