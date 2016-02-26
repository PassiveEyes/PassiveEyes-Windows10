namespace Client.OneDrive.Directory
{
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
                .ToDictionary(x => x.Key, x => x.ToList());

            return new Directory { Records = groups };
        }
    }
}
