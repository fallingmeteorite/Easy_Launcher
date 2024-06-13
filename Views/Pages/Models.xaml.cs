﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Awake.Models;
using Wpf.Ui.Controls;

namespace Awake.Views.Pages
{
    /// <summary>
    /// ModelManager.xaml 的交互逻辑
    /// </summary>
    public partial class Models : UiPage
    {
        public ObservableCollection<Embedding> HysCollection = new();
        public ObservableCollection<Embedding> EmbCollection = new();
        public ObservableCollection<CheckPoint> CksCollection = new();
        public ObservableCollection<CheckPoint> CksOnlineCollection = new();
        public ObservableCollection<CheckPoint> VaesCollection = new();
        public ObservableCollection<CheckPoint> VaesOnlineCollection = new();
        public ObservableCollection<CheckPoint> LorasCollection = new();
        public Models()
        {
            InitializeComponent();
        }
        private void OpenCkpt_Click(object sender, EventArgs e)
        {
            Process.Start("Explorer.exe", 
                AppDomain.CurrentDomain.BaseDirectory+"..\\models\\stable-diffusion");
        }
        private void OpenVAE_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe",
               AppDomain.CurrentDomain.BaseDirectory+"..\\models\\vae");
        }
        private void OpenEmb_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe",
               AppDomain.CurrentDomain.BaseDirectory+"..\\embeddings");
        }
        private void OpenHys_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe",
               AppDomain.CurrentDomain.BaseDirectory+"..\\models\\hypernetworks");
        }
        private void OpenLoRA_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Explorer.exe",
               AppDomain.CurrentDomain.BaseDirectory+"..\\models\\lora");
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                Debug.WriteLine(tabs.SelectedIndex);
                if (tabs.SelectedIndex == 0)
                {
                    CksCollection.Clear();

                    SD_CKPT_Info();

                    cks.ItemsSource = CksCollection;
                } else if (tabs.SelectedIndex == 1)
                {
                    EmbCollection.Clear();
                    var dir = new DirectoryInfo("..\\embeddings");
                    FileInfo[] files = dir.GetFiles();
    
                    foreach (FileInfo fInfo in files)
                    {
                        if (fInfo.Extension != ".pt" && fInfo.Extension != ".safetensors") continue;

                        DateTime date = fInfo.CreationTime;
                        FileStream fileStream = fInfo.Open(FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        fileStream.Close();

                        Embedding ck = new Embedding();
                        ck.Name = fInfo.Name;
                        ck.Date = date;
                        ck.Size = fInfo.Length / 1024 ;

                        EmbCollection.Add(ck);
                    }
               
                    embs.ItemsSource = EmbCollection;
                }
                else if (tabs.SelectedIndex == 2)
                {
                    HysCollection.Clear();
                    var dir = new DirectoryInfo("..\\models\\hypernetworks");
                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo fInfo in files)
                    {
                        if (fInfo.Extension != ".pt" && fInfo.Extension != ".safetensors") continue;

                        DateTime date = fInfo.CreationTime;
                        FileStream fileStream = fInfo.Open(FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        fileStream.Close();

                        Embedding ck = new Embedding();
                        ck.Name = fInfo.Name;
                        ck.Date = date;
                        ck.Size = fInfo.Length / 1024;

                        HysCollection.Add(ck);
                    }

                    hys.ItemsSource = HysCollection;
                } else if (tabs.SelectedIndex == 3)
                {
                    VaesCollection.Clear();
                    var dir = new DirectoryInfo("..\\models\\vae");
                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo fInfo in files)
                    {
                        if (fInfo.Extension != ".ckpt" && fInfo.Extension != ".safetensors") continue;

                        DateTime date = fInfo.CreationTime;
                        FileStream fileStream = fInfo.Open(FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        fileStream.Close();

                        CheckPoint ck = new CheckPoint();
                        ck.Name = fInfo.Name;
                        ck.Date = date;
                        ck.Size = fInfo.Length / 1024 / 1024;

                        VaesCollection.Add(ck);
                    }

                    vaes.ItemsSource = VaesCollection;
                }
                else if (tabs.SelectedIndex == 4)
                {
                    LorasCollection.Clear();
                    var dir = new DirectoryInfo("..\\models\\lora");
                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo fInfo in files)
                    {
                        if (fInfo.Extension != ".ckpt" && fInfo.Extension != ".safetensors") continue;

                        DateTime date = fInfo.CreationTime;
                        FileStream fileStream = fInfo.Open(FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        fileStream.Close();

                        CheckPoint ck = new CheckPoint();
                        ck.Name = fInfo.Name;
                        ck.Date = date;
                        ck.Size = fInfo.Length / 1024 / 1024;

                        LorasCollection.Add(ck);
                    }

                    loras.ItemsSource = LorasCollection;
                }
            }
        }
        public void SD_CKPT_Info()
        {
            var dir = new DirectoryInfo("..\\models\\stable-diffusion\\");
            FileInfo[] files = dir.GetFiles();
            using (SHA256 mySHA256 = SHA256.Create())
            {
                int idx = 0;
                foreach (FileInfo fInfo in files)
                {
                    try
                    {
                        if (fInfo.Extension != ".ckpt" && fInfo.Extension != ".safetensors") continue;

                        DateTime date = fInfo.CreationTime;
                        FileStream fileStream = fInfo.Open(FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        fileStream.Close();

                        CheckPoint ck = new CheckPoint();
                        ck.Name = fInfo.Name;
                        ck.ShortHash = "";
                        ck.Date = date;
                        ck.Index = idx++;
                        ck.isRemote = false;
                        ck.Size = fInfo.Length / 1024 / 1024;
                       
                        CksCollection.Add(ck);
                    }
                    catch (IOException e)
                    {
                        Debug.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Debug.WriteLine($"Access Exception: {e.Message}");
                    }
                }
            }
        }
        public class CheckPoint
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public long Size { get; set; }
            public DateTime Date { get; set; }
            public string ShortHash { get; set; }
            public bool isRemote { get; set; }
        }
        public class CheckPointOnline
        {
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Hash { get; set; }
        }
        public class Embedding
        {
            public string Name { get; set; }
            public string Desc { get; set; }
            public long Size { get; set; }
            public DateTime Date { get; set; }
        }
        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Debug.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Debug.Write(" ");
            }
            Debug.WriteLine("");
        }

        
    }
}


