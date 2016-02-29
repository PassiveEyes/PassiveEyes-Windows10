using System;

namespace SDK.Media
{
    /// <summary>
    /// Possible activity states for a camera stream.
    /// </summary>
    public enum ActivityState
    {
        /// <summary>
        /// The stream is inactive (no detected activity).
        /// </summary>
        Inactive = 0,

        /// <summary>
        /// The stream is active due to detected activity.
        /// </summary>
        Active = 1
    }
}
