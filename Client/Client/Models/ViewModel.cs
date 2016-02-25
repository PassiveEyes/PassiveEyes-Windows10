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
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            AvailableFeeds = new ObservableCollection<FeedModel>();
        }

        /// <summary>
        /// A list of all available camera feeds.
        /// </summary>
        public ObservableCollection<FeedModel> AvailableFeeds { get; set; }

        /// <summary>
        /// The feed selected for viewing.
        /// </summary>
        public FeedModel SelectedFeed
        {
            get
            {
                return selectedFeed;
            }
            set
            {
                if (selectedFeed != value)
                {
                    selectedFeed = value;
                    OnPropertyChanged();
                }
            }
        }
        private FeedModel selectedFeed;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
