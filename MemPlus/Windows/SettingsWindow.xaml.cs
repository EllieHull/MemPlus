﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MemPlus.Classes.GUI;
using MemPlus.Classes.LOG;
using Microsoft.Win32;

namespace MemPlus.Windows
{
    /// <inheritdoc cref="Syncfusion.Windows.Shared.ChromelessWindow" />
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        #region Variables
        /// <summary>
        /// The MainWindow object that can be used to change the properties
        /// </summary>
        private readonly MainWindow _mainWindow;
        /// <summary>
        /// The LogController object that can be used to add logs
        /// </summary>
        private readonly LogController _logController;
        #endregion

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new SettingsWindow object
        /// </summary>
        /// <param name="mainWindow">The MainWindow object that can be used to change the properties</param>
        /// <param name="logController">The LogController object that can be used to add logs</param>
        public SettingsWindow(MainWindow mainWindow, LogController logController)
        {
            _logController = logController;
            _logController.AddLog(new ApplicationLog("Initializing SettingsWindow"));

            InitializeComponent();
            ChangeVisualStyle();
            LoadProperties();

            _mainWindow = mainWindow;

            _logController.AddLog(new ApplicationLog("Done initializing SettingsWindow"));
        }

        /// <summary>
        /// Change the visual style of the controls, depending on the settings.
        /// </summary>
        private void ChangeVisualStyle()
        {
            _logController.AddLog(new ApplicationLog("Changing SettingsWindow theme style"));

            StyleManager.ChangeStyle(this);

            _logController.AddLog(new ApplicationLog("Done changing SettingsWindow theme style"));
        }

        /// <summary>
        /// Load the appropriate properties into the GUI
        /// </summary>
        private void LoadProperties()
        {
            _logController.AddLog(new ApplicationLog("Loading SettingsWindow properties"));

            try
            {
                //General
                ChbAutoUpdate.IsChecked = Properties.Settings.Default.AutoUpdate;
                if (Properties.Settings.Default.Topmost)
                {
                    ChbTopmost.IsChecked = Properties.Settings.Default.Topmost;
                    Topmost = true;
                }
                else
                {
                    Topmost = false;
                }

                ChbRamMonitor.IsChecked = Properties.Settings.Default.RamMonitor;
                ChbDisableInactive.IsChecked = Properties.Settings.Default.DisableOnInactive;
                ItbRamMonitorTimeout.Value = Properties.Settings.Default.RamMonitorInterval;

                ChbFileSystemCache.IsChecked = Properties.Settings.Default.FileSystemCache;
                ChbStandByCache.IsChecked = Properties.Settings.Default.StandByCache;
                if (Properties.Settings.Default.ProcessExceptions != null)
                {
                    foreach (string s in Properties.Settings.Default.ProcessExceptions)
                    {
                        LsvExclusions.Items.Add(s);
                    }
                }
                else
                {
                    LsvExclusions.Items.Clear();
                }

                //Theme
                CboStyle.Text = Properties.Settings.Default.VisualStyle;
                CpMetroBrush.Color = Properties.Settings.Default.MetroColor;
                IntBorderThickness.Value = Properties.Settings.Default.BorderThickness;
            }
            catch (Exception ex)
            {
                _logController.AddLog(new ApplicationLog(ex.Message));
                MessageBox.Show(ex.Message, "MemPlus", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _logController.AddLog(new ApplicationLog("Done loading SettingsWindow properties"));
        }

        /// <summary>
        /// Save all properties
        /// </summary>
        private void SaveProperties()
        {
            _logController.AddLog(new ApplicationLog("Saving properties"));
            try
            {
                //General
                if (ChbAutoUpdate.IsChecked != null) Properties.Settings.Default.AutoUpdate = ChbAutoUpdate.IsChecked.Value;
                if (ChbTopmost.IsChecked != null) Properties.Settings.Default.Topmost = ChbTopmost.IsChecked.Value;
                if (ChbRamMonitor.IsChecked != null) Properties.Settings.Default.RamMonitor = ChbRamMonitor.IsChecked.Value;
                if (ChbDisableInactive.IsChecked != null) Properties.Settings.Default.DisableOnInactive = ChbDisableInactive.IsChecked.Value;
                if (ItbRamMonitorTimeout.Value != null) Properties.Settings.Default.RamMonitorInterval = (int)ItbRamMonitorTimeout.Value;

                //RAM Optimizer
                if (ChbFileSystemCache.IsChecked != null) Properties.Settings.Default.FileSystemCache = ChbFileSystemCache.IsChecked.Value;
                if (ChbStandByCache.IsChecked != null) Properties.Settings.Default.StandByCache = ChbStandByCache.IsChecked.Value;
                List<string> exclusionList = LsvExclusions.Items.Cast<string>().ToList();
                Properties.Settings.Default.ProcessExceptions = exclusionList;

                //Theme
                Properties.Settings.Default.VisualStyle = CboStyle.Text;

                Properties.Settings.Default.MetroColor = CpMetroBrush.Color;
                if (IntBorderThickness.Value != null) Properties.Settings.Default.BorderThickness = (int)IntBorderThickness.Value;

                Properties.Settings.Default.Save();

                _mainWindow.ChangeVisualStyle();
                _mainWindow.LoadProperties();
                ChangeVisualStyle();

                _logController.AddLog(new ApplicationLog("Properties have been saved"));

                MessageBox.Show("All settings have been saved!", "MemPlus", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logController.AddLog(new ApplicationLog(ex.Message));
                MessageBox.Show(ex.Message, "MemPlus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reset all properties to their default values
        /// </summary>
        private void ResetSettings()
        {
            _logController.AddLog(new ApplicationLog("Resetting properties"));

            try
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();

                _mainWindow.ChangeVisualStyle();
                _mainWindow.LoadProperties();
                LoadProperties();

                _logController.AddLog(new ApplicationLog("Properties have been reset"));

                MessageBox.Show("All settings have been reset!", "MemPlus", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logController.AddLog(new ApplicationLog(ex.Message));
                MessageBox.Show(ex.Message, "MemPlus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method that is called when all properties should be reset to their default values
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            ResetSettings();
        }

        /// <summary>
        /// Method that will be called when all properties should be saved
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveProperties();
        }

        /// <summary>
        /// Method that is called when an OpenFileDialog should be shown
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void BtnFileView_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "All files (*.*)|*.*" };

            if (ofd.ShowDialog() == true)
            {
                TxtExclusion.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// Method that is called when an exclusion should be added to the list
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void BtnAddExclusion_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtExclusion.Text.Length == 0) return;

            if (System.IO.File.Exists(TxtExclusion.Text))
            {
                LsvExclusions.Items.Add(TxtExclusion.Text);
                TxtExclusion.Text = "";
            }
            else
            {
                MessageBox.Show("The selected file does not exist!", "MemPlus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method that is called when an exclusion should be copied to the clipboard
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void CopyExclusionMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (LsvExclusions.SelectedItems.Count == 0) return;
            try
            {
                Clipboard.SetText(LsvExclusions.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                _logController.AddLog(new ApplicationLog(ex.Message));
                MessageBox.Show(ex.Message, "MemPlus", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method that is called when an exclusion should be deleted from the list
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void DeleteExclusionMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (LsvExclusions.SelectedItems.Count == 0) return;
            LsvExclusions.Items.Remove(LsvExclusions.SelectedItem);
        }

        /// <summary>
        /// Method that is called when all exclusions should be cleared
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void ClearExclusionsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LsvExclusions.Items.Clear();
        }
    }
}