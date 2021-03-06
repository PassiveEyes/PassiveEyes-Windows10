﻿namespace Client.Models
{
    using OneDrive.Directory;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System;
    using System.Linq;

    /// <summary>
    /// The code-behind view model for the <see cref="ViewerPage"/>.
    /// </summary>
    public class ViewerViewModel
    {
        /// <summary>
        /// Top snapshots to be displayed.
        /// </summary>
        public ObservableCollection<SnapshotModel> MostRecentSnapshots { get; set; } = new ObservableCollection<SnapshotModel>();

        /// <summary>
        /// Downloads the top snapshot images from known feed directories.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            var directory = await Directory.FromFolderPath("PassiveEyes");
            var snapshots = await directory.GetTopSnapshots();
            
            this.MostRecentSnapshots.Clear();
            
            foreach (var snapshot in snapshots)
            {
                this.MostRecentSnapshots.Add(snapshot);
            }

            //new Timer(this.RefreshSnapshots, null, 1000, Timeout.Infinite);
        }

        public async void RefreshSnapshots(object _)
        {
            var directory = await Directory.FromFolderPath("PassiveEyes");
            var newSnapshots = (await directory.GetTopSnapshots()).ToArray();

            for (int i = 0; i < this.MostRecentSnapshots.Count; i += 1)
            {
                if (newSnapshots[i].TimeStamp != this.MostRecentSnapshots[i].TimeStamp)
                {
                    this.MostRecentSnapshots[i] = newSnapshots[i];
                }
            }

            new Timer(this.RefreshSnapshots, null, 500, Timeout.Infinite);
        }

        #region Property Changed events
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
    }
}
