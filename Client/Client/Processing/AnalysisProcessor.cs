﻿namespace Client.Processing
{
    using System;

    /// <summary>
    /// Runs analysis on media capture images.
    /// </summary>
    public class AnalysisProcessor
    {
        /// <summary>
        /// The minimum threshold of change 
        /// </summary>
        private static readonly double ChangeThreshold = 11.7;

        /// <summary>
        /// 
        /// </summary>
        private byte[] OldBytes;

        /// <summary>
        /// Determines whether the image has changed significantly since last frame.
        /// </summary>
        /// <returns>Whether the change from the last frame was significant.</returns>
        public bool CheckForSignificantImageChanges(byte[] newBytes)
        {
            var delta = this.CalculateBytesDifference(this.OldBytes, newBytes);

            this.OldBytes = newBytes;

            return delta < ChangeThreshold;
        }

        /// <summary>
        /// Calculates the approximate average bytes difference between two byte arrays.
        /// </summary>
        /// <param name="a">A byte array.</param>
        /// <param name="b">A byte array.</param>
        /// <returns>The approximate byte difference between the arrays.</returns>
        private double CalculateBytesDifference(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return -1;
            }

            int interval = (int)Math.Ceiling((double)a.Length / 25) * 4;
            double total = 0;

            for (int i = 0; i < a.Length; i += interval)
            {
                for (int j = i; j < i + 3; j += 1)
                {
                    total += Math.Abs(a[j] - b[j]);
                }
            }

            total /= interval / 4;

            return total;
        }
    }
}
