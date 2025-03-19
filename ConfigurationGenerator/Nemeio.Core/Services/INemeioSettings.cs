namespace Nemeio.Core.Services
{
    /// <summary>
    /// Represent every saved settings of the keyboard.
    /// </summary>
    public interface INemeioKeyboardSettings
    {
        /// <summary>
        /// Delay before write again the same pressed char
        /// </summary>
        int DelayRepeat { get; set; }

        /// <summary>
        /// Interval between the previous and next char to write.
        /// </summary>
        int IntervalRepeat { get; set; }
    }
}
