﻿using AudioPipe.Properties;
using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AudioPipe
{
    public sealed class TrayIcon : IDisposable
    {
        public event EventHandler Invoked;
        public event EventHandler SettingsClicked;

        private readonly System.Windows.Forms.NotifyIcon _trayIcon;
        private readonly System.Drawing.Icon _pipeInactiveIcon;
        private readonly System.Drawing.Icon _pipeActiveIcon;

        public TrayIcon()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon()
            {
                ContextMenu = new System.Windows.Forms.ContextMenu()
            };

            // TODO: adjust size based on system DPI?
            _pipeActiveIcon = IconService.CreateIcon((int)IconService.Symbol.Headphone);
            _pipeInactiveIcon = IconService.CreateIcon((int)IconService.Symbol.Speaker);

            AddMenuItems(new List<IMenuItem>
            {
                new MenuItem
                {
                    Text = Resources.ContextMenuSettingsTitle,
                    Click = SettingsItem_Click,
                },
                new Separator(),
                new MenuItem
                {
                    Text = Resources.ContextMenuExitTitle,
                    Click = ExitItem_Click,
                },
            });

            _trayIcon.MouseClick += TrayIcon_MouseClick;
            _trayIcon.Icon = _pipeInactiveIcon;
            _trayIcon.Text = Resources.TrayIconText;
            _trayIcon.Visible = true;
        }

        public void SetPipeActive(bool active)
        {
            _trayIcon.Icon = active ? _pipeActiveIcon : _pipeInactiveIcon;
        }

        private void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Invoked?.Invoke(this, EventArgs.Empty);
            }
        }

        private void AddMenuItems(IEnumerable<IMenuItem> items)
        {
            foreach (var itemDef in items)
            {
                var item = _trayIcon.ContextMenu.MenuItems.Add(itemDef.Text);
                if (itemDef.Click != null)
                {
                    item.Click += itemDef.Click;
                }
            }
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Dispose();

            Application.Current.Shutdown();
        }

        private void SettingsItem_Click(object sender, EventArgs e)
        {
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _trayIcon.Dispose();
        }

        private interface IMenuItem
        {
            string Text { get; }
            EventHandler Click { get; }
        }

        private struct MenuItem : IMenuItem
        {
            public string Text { get; set; }
            public EventHandler Click { get; set; }
        }

        private class Separator : IMenuItem
        {
            public string Text => "-";
            public EventHandler Click => null;
        }
    }
}
