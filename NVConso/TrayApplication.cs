namespace NVConso
{
    public class TrayApplication : IDisposable
    {
        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenuStrip _menu;
        private readonly Form _hiddenForm;
        private readonly INvmlManager _manager;

        public TrayApplication(INvmlManager manager)
        {
            _manager = manager;

            if (!_manager.Initialize())
            {
                MessageBox.Show("Échec de l'initialisation NVML. L'application va se fermer.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            _hiddenForm = new HiddenForm();
            _menu = BuildContextMenu();

            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("Assets/NVConso.ico"),
                Visible = true,
                Text = "NVConso - Gestion GPU"
            };

            _trayIcon.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                    _menu.Show(_hiddenForm, _hiddenForm.PointToClient(Cursor.Position));
            };
        }

        private ContextMenuStrip BuildContextMenu()
        {
            var menu = new ContextMenuStrip { ShowImageMargin = false };
            var eco = _manager.GetPowerLimit(GpuPowerMode.Eco);
            var perf = _manager.GetPowerLimit(GpuPowerMode.Performance);

            menu.Items.Add($"Mode Éco ({eco / 1000.0:F1} W)", null, (s, e) => SetPower(eco));
            menu.Items.Add($"Mode Performance ({perf / 1000.0:F1} W)", null, (s, e) => SetPower(perf));
            menu.Items.Add("-");
            menu.Items.Add("Quitter", null, (s, e) => ExitApplication());

            return menu;
        }

        private void SetPower(uint targetMilliwatt)
        {
            bool success = _manager.SetPowerLimit(targetMilliwatt);
            if (!success)
                MessageBox.Show("Impossible de définir la limite.\nLancez l'application en tant qu'administrateur.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ExitApplication()
        {
            _trayIcon.Visible = false;
            _manager.Shutdown();
            Application.Exit();
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
            _menu?.Dispose();
            _hiddenForm?.Dispose();
        }

        private class HiddenForm : Form
        {
            public HiddenForm()
            {
                ShowInTaskbar = false;
                FormBorderStyle = FormBorderStyle.None;
                Opacity = 0;
                Width = 0;
                Height = 0;
                StartPosition = FormStartPosition.Manual;
                Location = new Point(-32000, -32000);
                Show();
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    var cp = base.CreateParams;
                    cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
                    return cp;
                }
            }
        }
    }
}