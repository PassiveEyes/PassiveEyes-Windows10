namespace Client.Models
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    /// A code-behind for the <see cref="ReorderPage"/>.
    /// </summary>
    public class ReorderPageViewModel
    {
        /// <summary>
        /// Available cameras.
        /// </summary>
        public ObservableCollection<Camera> Cameras { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderPageViewModel"/> class.
        /// </summary>
        public ReorderPageViewModel()
        {
            this.Cameras = new ObservableCollection<Camera>();
        }

        /// <summary>
        /// Collects available cameras and initializes them.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            foreach (var camera in await Camera.CollectCameras())
            {
                this.Cameras.Add(camera);
            }
        }
    }
}
