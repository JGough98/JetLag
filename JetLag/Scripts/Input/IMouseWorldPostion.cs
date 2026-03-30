namespace JetLag.Scripts.Input
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHiderPostion
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }


    public interface IMouseWorldPostion
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}