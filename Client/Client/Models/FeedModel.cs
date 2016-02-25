using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    /// <summary>
    /// A camera feed.
    /// </summary>
    public class FeedModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the camera feed.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }
        private string name;

        /// <summary>
        /// Collection of snapshots taken by this feed.
        /// </summary>
        public ObservableCollection<SnapshotModel> Snapshots;

        /// <summary>
        /// Returns the last snapshot taken by this feed if one exists.
        /// </summary>
        public SnapshotModel LastSnapshot
        {
            get
            {
                if (Snapshots != null)
                {
                    return Snapshots.LastOrDefault();
                }
                return null;
            }
        }
        #region Property Changed Events
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
