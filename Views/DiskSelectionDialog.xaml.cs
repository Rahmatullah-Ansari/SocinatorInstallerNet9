using SocinatorInstaller9.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SocinatorInstaller9.Views
{
    /// <summary>
    /// Interaction logic for DiskSelectionDialog.xaml
    /// </summary>
    public partial class DiskSelectionDialog : Window
    {
        public string SelectedDisk { get; private set; }
        public long RequiredDiskSpaceSize {  get; private set; }
        public DiskSelectionDialog(long RequiredSpace):this()
        {
            RequiredDiskSpaceSize = RequiredSpace;
            LoadDisks();
        }
        public DiskSelectionDialog()
        {
            InitializeComponent();
        }
        private void LoadDisks()
        {
            var diskInfoList = new List<DiskInfo>();
            try
            {

                // Get all drives available on the system
                DriveInfo[] drives = DriveInfo.GetDrives();

                // Populate the ListBox with available drives
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        diskInfoList.Add(new DiskInfo
                        {
                            Volume = !string.IsNullOrWhiteSpace(drive.Name) ? drive.Name.TrimEnd('\\') : !string.IsNullOrWhiteSpace(drive.VolumeLabel) ? drive.VolumeLabel : "Un",
                            DiskSize = $"{(drive.TotalSize / Math.Pow(1024, 3)):F2}GB",
                            Available = $"{(drive.AvailableFreeSpace / Math.Pow(1024, 3)):F2}GB",
                            Required = $"{RequiredDiskSpaceSize}MB",
                            Difference = $"{((drive.AvailableFreeSpace - (RequiredDiskSpaceSize * Math.Pow(1024,2))) / Math.Pow(1024, 3)):F2}GB"
                        });
                    }
                    else
                    {
                        diskInfoList.Add(new DiskInfo
                        {
                            Volume = drive.Name,
                            DiskSize = "N/A",
                            Available = "N/A",
                            Required = "N/A",
                            Difference = "N/A"
                        });
                    }

                }

            }
            catch (Exception ex) { }
            listViewDisks.ItemsSource = diskInfoList;
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            // Ensure a disk is selected
            //if (listViewDisks.SelectedItem is DiskInfo selectedDisk)
            //{
            //    SelectedDisk = selectedDisk.Volume;
            //    DialogResult = true; // Close the dialog with OK result
            //}
            //else
            //{
            //    MessageBox.Show("Please select a disk.", "No Disk Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Close the dialog without selection
        }

        private void close_click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    public class DiskInfo : BindableBase
    {
        private string _volume;
        public string Volume
        {
            get => _volume;
            set => SetProperty(ref _volume, value);
        }
        private string _diskSize;

        public string DiskSize
        {
            get => _diskSize;
            set => SetProperty(ref _diskSize, value);
        }
        private string _available;
        public string Available
        {
            get => _available;
            set => SetProperty(ref _available, value);
        }
        private string _required;
        public string Required
        {
            get => _required;
            set => SetProperty(ref _required, value);
        }
        private string _difference;
        public string Difference
        {
            get => _difference;
            set => SetProperty(ref _difference, value);
        }
    }
}
