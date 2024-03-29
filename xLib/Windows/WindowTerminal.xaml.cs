﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xLib.Common;
using xLib.UI;

namespace xLib.Windows
{
    /// <summary>
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class WindowTerminal : Window
    {
        private static WindowTerminal window;
        private UIButton but_pause = new UIButton("Resume", "Pause", UIProperty.RED, UIProperty.GREEN);
        public ObservableCollection<ReceivePacketInfo> NoteInfo { get; set; } = xTracer.Info;
        public ObservableCollection<ReceivePacketInfo> NoteLog { get; set; } = xTracer.Notes;

        public WindowTerminal()
        {
            InitializeComponent();
            ListViewInfo.ItemsSource = NoteInfo;
            ListViewLog.ItemsSource = NoteLog;

            but_pause.State = xTracer.Pause;
            ButPause.DataContext = but_pause;

            NoteInfo.CollectionChanged += NoteInfoCollectionChanged;
        }

        private void NoteInfoCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (NoteInfo.Count > 0)
            {
                ListViewInfo.ScrollIntoView(NoteInfo[NoteInfo.Count - 1]);
            }
        }

        public static void OpenClick(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new WindowTerminal();
                window.Closed += new EventHandler(Close_Click);
                window.Show();
            }
            else window.Activate();
        }

        public static void Close_Click(object sender, EventArgs e) { window?.Close(); window = null; }
        public static void Dispose() { window?.Close(); window = null; }

        private void ButSend_Click(object sender, RoutedEventArgs e)
        {
            //xTcp.Send(TextBoxData.Text);
        }

        private void ButClear_Click(object sender, RoutedEventArgs e)
        {
            TabItem item = (TabItem)TabControl.SelectedItem;
            if (item.Header.ToString() == "Log" && NoteLog != null) { NoteLog.Clear(); return; }
            if (item.Header.ToString() == "Info" && NoteInfo != null) { NoteInfo.Clear(); return; }
        }

        private void ButPause_Click(object sender, RoutedEventArgs e)
        {
            but_pause.State ^= true;
            xTracer.Pause = but_pause.State;
        }
    }
}
