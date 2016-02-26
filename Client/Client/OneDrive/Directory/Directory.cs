namespace Client.OneDrive.Directory
{
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A representation of a OneDrive folder of <see cref="Record"/>s.
    /// </summary>
    public class Directory
    {
        /// <summary>
        /// Records for each webcam, in chronologically descending order.
        /// </summary>
        public Dictionary<string, List<Record>> Records { get; set; }

        /// <summary>
        /// Gets the top snapshot models from the records.
        /// </summary>
        /// <returns>Top snapshot models from the records.</returns>
        public async Task<IEnumerable<SnapshotModel>> GetTopSnapshots()
        {
            var topRecords = this.Records.Values.Select(records => records.First());
            var snapshots = new List<SnapshotModel>();

            foreach (var record in topRecords)
            {
                snapshots.Add(await SnapshotModel.FromRecord(record));
            }

            return snapshots;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Directory"/> class.
        /// </summary>
        /// <param name="folderPath">The path to the folder on OneDrive.</param>
        /// <returns>An equivalent directory to the OneDrive folder.</returns>
        public static async Task<Directory> FromFolderPath(string folderPath)
        {
            var children = await PieceOfCrap.RunAction(
                async (PieceOfCrap crap)
                    => await crap.GetItemChildren<Children>("PassiveEyes"));

            var items = children.Value.Select(child => Record.FromChild(child));

            var groups = items
                .GroupBy(item => item.Webcam)
                .ToDictionary(
                    group => group.Key,
                    group =>
                    {
                        var list = group.ToList();
                        list.Sort((a, b) => (b.Timestamp - a.Timestamp).Milliseconds);
                        return list;
                    });

            return new Directory { Records = groups };
        }
    }
}
