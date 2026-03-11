public class Player
{
    public string Name { get; }
    public Guid ID { get; }
    public bool IsHider { get; set; }


    public Player(string name, bool isHider)
    {
        Name = name;
        ID = Guid.NewGuid();
        IsHider = isHider;
    }
}