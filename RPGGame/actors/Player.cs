using SFML.Graphics;

class Player : Actor
{
    int weapon = 0;
    int armor = 0;
    const int INVENTORY_SIZE = 10;
    public const int SPRITEX = 0;
    public List<Item> inventory = new List<Item>();
    public Player(int posx, int posy, int spritex, int spritey, Color color) : base (posx, posy, spritex, spritey, color)
    {
        x = posx;
        y = posy;
        this.spritex = spritex;
        this.spritey = spritey;
        spriteColor = color;
    }
    public void GiveItem(Item item, int quantity)
    {
        foreach (Item items in inventory)
        {
            if (items.id == item.id && item.stackable)
            {
                items.quantity += quantity;
                return;
            }
        }
        if (inventory.Count == INVENTORY_SIZE)
        {
            Console.WriteLine("Inventory full");
        }
        else
        {
            item.quantity = quantity;
            inventory.Add(item);
        }
    }
}
