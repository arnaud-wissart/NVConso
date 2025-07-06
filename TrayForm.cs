using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVConso
{
    public class TrayForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private List<ToolStripMenuItem> powerItems = new();

        public TrayForm()
        {
            trayMenu = new ContextMenuStrip();

            if (!NvmlManager.Initialize())
            {
                trayMenu.Items.Add("Erreur : NVML non initialisé");
            }
            else
            {
                uint current = NvmlManager.GetCurrentPowerLimit();
                uint eco = NvmlManager.GetEcoLimit();
                uint perf = NvmlManager.GetPerformanceLimit();

                var ecoItem = new ToolStripMenuItem($"🧘 Mode Éco ({eco / 1000} W)")
                {
                    Tag = eco,
                    Checked = Math.Abs((int)eco - (int)current) < 200
                };

                var perfItem = new ToolStripMenuItem($"🔥 Mode Performance ({perf / 1000} W)")
                {
                    Tag = perf,
                    Checked = Math.Abs((int)perf - (int)current) < 200
                };

                ecoItem.Click += OnPowerLimitSelected;
                perfItem.Click += OnPowerLimitSelected;

                trayMenu.Items.Add(ecoItem);
                trayMenu.Items.Add(perfItem);
                powerItems.Add(ecoItem);
                powerItems.Add(perfItem);
            }

            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add("Quitter", null, (s, e) =>
            {
                NvmlManager.Shutdown();
                Application.Exit();
            });

            trayIcon = new NotifyIcon()
            {
                Text = "Gestion GPU",
                Icon = new Icon("Assets/NVConso.ico"),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.MouseUp += TrayIcon_MouseUp;
        }

        private void OnPowerLimitSelected(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem) return;
            uint mw = (uint)clickedItem.Tag;

            bool success = NvmlManager.SetPowerLimit(mw);
            if (success)
            {
                foreach (var item in powerItems)
                    item.Checked = false;

                clickedItem.Checked = true;
                trayIcon.ShowBalloonTip(1000, "GPU", $"Limite fixée à {mw / 1000.0:F1} W", ToolTipIcon.Info);
            }
            else
            {
                trayIcon.ShowBalloonTip(1000, "Erreur", "Échec du changement", ToolTipIcon.Error);
            }
        }

        private void TrayIcon_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                trayMenu.Hide();
                trayMenu.Show(Cursor.Position);
                trayMenu.BringToFront();
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
