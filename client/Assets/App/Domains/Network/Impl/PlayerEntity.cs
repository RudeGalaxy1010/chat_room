namespace App.Domains.Network.Impl
{
    public class PlayerEntity : Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public PlayerEntity(int id, int x, int y, string name, string color) : base(id)
        {
            X = x;
            Y = y;
            Name = name;
            Color = color;
        }
    }
}