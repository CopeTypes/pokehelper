namespace PokeHelper.util
{
    public class MonInfo
    {
        public string Level { get; set; }
        public string Iv { get; set; }
        public string IvFull { get; set; }
        
        public bool Shiny { get; set; }

        public MonInfo(string level, string iv, string ivFull, bool shiny)
        {
            Level = level;
            Iv = iv;
            IvFull = ivFull;
            Shiny = shiny;
        }

        public bool IsPerfect()
        {
            return IvFull == "15/15/15";
        }

        public string Summary()
        {
            return "Level: " + Level + " IV: " + Iv;
        }

        public override string ToString()
        {
            return $"Level: {Level} IV: {IvFull} ({Iv}) Shiny: {Shiny.ToString()}";
        }
    }
}