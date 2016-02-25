using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Client.Models
{
    /// <summary>
    /// Represents a single image along with metadata captured by a camera feed.
    /// </summary>
    public class SnapshotModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Enumeration of states the camera is aware of.
        /// </summary>
        public enum SnapshotState
        {
            Idle = 0,
            Alert = 1
        }

        /// <summary>
        /// The name of the camera feed that captured this snapshot.
        /// </summary>
        public string FeedName
        {
            get
            {
                return feedName;
            }
            set
            {
                if (feedName != value)
                {
                    feedName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string feedName;

        /// <summary>
        /// The time at which this snapshot was taken.
        /// </summary>
        public DateTimeOffset TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                if (timeStamp != value)
                {
                    timeStamp = value;
                    OnPropertyChanged();
                }
            }
        }
        private DateTimeOffset timeStamp;

        /// <summary>
        /// State calculated by the camera at the time of snapshot.
        /// </summary>
        public SnapshotState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    state = value;
                    OnPropertyChanged();
                }
            }
        }
        private SnapshotState state;

        /// <summary>
        /// The image captured by the camera in bitmap format.
        /// </summary>
        public BitmapImage Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                if (bitmap != value)
                {
                    bitmap = value;
                    OnPropertyChanged();
                }
            }
        }
        private BitmapImage bitmap;
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
