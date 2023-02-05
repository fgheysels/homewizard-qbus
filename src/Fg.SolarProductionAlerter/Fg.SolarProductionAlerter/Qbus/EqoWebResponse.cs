namespace Fg.SolarProductionAlerter.Qbus
{
    internal class EqoWebResponse<TValue>
    {
        public int Type { get; set; }
        public TValue Value { get; set; }
    }
}
