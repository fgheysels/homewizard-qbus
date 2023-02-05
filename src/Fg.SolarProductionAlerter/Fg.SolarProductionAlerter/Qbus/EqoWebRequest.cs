namespace Fg.SolarProductionAlerter.Qbus
{
    internal class EqoWebRequest<TValue>
    {
        public int Type { get; set; }
        public TValue Value { get; set; }
    }
}
