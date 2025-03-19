namespace Nemeio.Core.DataModels
{
    public abstract class ComPort : StringType<ComPort>
    {
        protected abstract string Regex { get; }

        protected ComPort(string comPort) : base(comPort)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(comPort, Regex))
            {
                throw new InvalidComPortException(comPort);
            }
        }
    }
}
