﻿using AudioPipe.Extensions;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AudioPipe.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : UserControl
    {
        public Version AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version;
        public string VersionText => string.Format(Properties.Resources.Version, AssemblyVersion);

        public AboutPage()
        {
            InitializeComponent();

            this.EnableHyperlinks();
        }
    }
}
