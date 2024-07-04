namespace DNS_Checker.models
{
    public class IndexXY
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public IndexXY(int x, int y)
        {
            this.X = (ushort)x;
            this.Y = (ushort)y;
        }
        public IndexXY(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }
    }
}
