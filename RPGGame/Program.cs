using System;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Text.Encodings.Web;
using System.Security.AccessControl;

class Game
{
    public enum Direction
    {
        Up, Down, Left, Right, Default
    }
    public static List<Actor> actors = new List<Actor>();
    public static void Main(string[] args)
    {

        Graphics2D graphics2D = new Graphics2D();
        graphics2D.window.SetVerticalSyncEnabled(true);
        graphics2D.window.Closed += (sender, args) => graphics2D.window.Close();
        Player player = new Player(3, 3);
        graphics2D.player = player;
        Console.WriteLine(graphics2D.player.x);

        Tile grass = new Tile(0, 0, Tile.Type.Grass);
        while (graphics2D.window.IsOpen)
        {
            graphics2D.window.Clear(Color.Black);
            graphics2D.window.DispatchEvents();
            graphics2D.DrawTiles();
            graphics2D.DrawWalls();

            graphics2D.DrawTile(grass);
            graphics2D.DrawActor(player);
            graphics2D.window.Display();

        }
    }

}
class Graphics2D
{
    const int WIDTH = 640;
    const int HEIGHT = 480;
    const string TITLE = "Game";
    public static VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    public Dictionary<Tile.Type, IntRect> typeRect = new Dictionary<Tile.Type, IntRect>();
    public RenderWindow window;
    public Font font;
    private Texture tileset;

    private IntRect[] grassRects = new IntRect[2];
    private Color[] grassColors = new Color[2];
    public Player player;
    Tile[,] tiles = new Tile[64, 64];
    public Graphics2D()
    {
        this.window = new RenderWindow(mode, TITLE);
        this.font = new Font("../../Assets/Fonts/arial.ttf");
        this.tileset = CreateMask(new Texture("../../Assets/tileset.png"));
        grassRects[0] = GridToIntRect(7, 2);
        grassRects[1] = GridToIntRect(12, 2);
        grassColors[0] = new Color(34, 123, 0, 255);
        grassColors[1] = new Color(40, 225, 0, 255);
        CreateEmptiness();

    }
    public void CreateEmptiness()
    {
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                tiles[j, i] = new Tile(i, j, Tile.Type.Grass);
            }

        }
    }
    public IntRect ReverseRect(IntRect sprite)
    {
        return new IntRect(sprite.Left + 16, sprite.Top + 16, -16, -16);
    }
    public Texture CreateMask(Texture tileset)
    {
        Image img = tileset.CopyToImage();
        img.CreateMaskFromColor(new Color(255, 0, 255, 255));
        return new Texture(img);
    }
    public void DrawTile(Tile tile)
    {
        if (tile.GetPos() == player.GetPos())
        {
            return;
        }
        Sprite sprite = new Sprite(tileset);
        switch (tile.type)
        {
            case Tile.Type.Wall:
                sprite.Color = new Color(125, 125, 125, 255);
                sprite.TextureRect = GridToIntRect(11, 13);
                sprite.Position = new Vector2f(tile.x * 16, tile.y * 16);
                window.Draw(sprite);
                break;
            case Tile.Type.Grass:
                IntRect newTextureRect = grassRects[tile.spriteVariation];
                int randomTile = tile.positionVariation;
                if (randomTile == 0)
                {
                    newTextureRect = ReverseRect(newTextureRect);
                }
                sprite.Color = grassColors[tile.colorVariation];
                sprite.TextureRect = newTextureRect;
                sprite.Position = new Vector2f(tile.x * 16, tile.y * 16);
                window.Draw(sprite);
                break;
        }
        tile.sprite = sprite;
    }
    public void DrawActor(Actor actor)
    {
        Sprite sprite = new Sprite(tileset);
        sprite.TextureRect = GridToIntRect(actor.spritex, actor.spritey);
        sprite.Position = new Vector2f(actor.x * 16, actor.y * 16);
        sprite.Color = actor.spriteColor;
        window.Draw(sprite);
    }
    public void DrawTiles()
    {
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                DrawTile(tiles[i, j]);
            }
        }
    }
    public void DrawWalls()
    {
        for (int i = 0; i < 10; i++)
        {
            Tile tile = new Tile(i + 1, 0, Tile.Type.Wall);
            tiles[i + 1, 0] = tile;
            DrawTile(tile);
        }
    }
    public IntRect GridToIntRect(int x, int y)
    {
        return new IntRect(x * 16, y * 16, 16, 16);
    }
}
class Tile
{
    public enum Type
    {
        Empty, Ground, Wall, Grass
    }

    public Sprite sprite;
    public int x;
    public int y;
    public Type type;
    public int wallType = 0;
    public bool drawn = false;
    public int colorVariation = 0;
    public int spriteVariation = 0;
    public int positionVariation = 0;
    private Random random = new Random();
    public Vector2f GetPos()
    {
        return new Vector2f(x, y);
    }
    public Tile(int x, int y, Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        if (type == Type.Grass)
        {
            spriteVariation = random.Next(2);
            colorVariation = random.Next(2);
            positionVariation = random.Next(2);
        }
    }
}

/*class Graphics
{
    string[,] mapArray;
    const int MAP_WIDTH = 10;
    const int MAP_HEIGHT = 3;
    const string SAVE_PATH = @"C:\game\";
    public void ImportMap()
    {
        string mapPath = $"{SAVE_PATH}map.txt";

        string[] lines = File.ReadAllLines(mapPath);

        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        mapArray = new string[cols, rows];
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(",");
            for (int j = 0; j < cols; j++)
            {
                try
                {
                    mapArray[j, i] = values[j];
                }
                catch
                {
                    Console.WriteLine("Map data is invalid");
                }
            }
        }
    }

}*/
class Item
{
    public int weight = 0;
    public int price = 0;
    public string name = "";
    public int id = 0;
    public bool stackable = false;
    public int quantity = 0;
    public Item(int id, string name, bool stackable)
    {
        this.id = id;
        this.name = name;
        this.stackable = stackable;
    }
    public void OnUse()
    {

    }
}
class Weapon : Item
{
    public int strRequirements = 0;
    public int attackBonus = 0;

    public Weapon(int id, string name, bool stackable) : base(id, name, stackable)
    {
        this.id = id;
        this.name = name;
        this.stackable = stackable;
    }

}
class Armor : Item
{
    public Armor(int id, string name, bool stackable) : base(id, name, stackable)
    {
        this.id = id;
        this.name = name;
        this.stackable = stackable;
    }
}
class Actor
{
    int hp = 0;
    int strength = 0;
    int intelligence = 0;
    int dexterity = 0;
    public Color spriteColor = Color.White;
    public int x = 0;
    public int y = 0;
    public int spritex = 0;
    public int spritey = 0;
    public Vector2f GetPos()
    {
        return new Vector2f(x, y);
    }
    public void Move(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
class Player : Actor
{
    int weapon = 0;
    int armor = 0;
    const int INVENTORY_SIZE = 10;
    public const int SPRITEX = 0;
    public List<Item> inventory = new List<Item>();
    public Player(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.spritex = 0;
        this.spritey = 4;
        this.spriteColor = new Color(255, 204, 156, 255);
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
    public Game.Direction GetDirectionInput()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            Console.SetCursorPosition(x, y);
            keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    return Game.Direction.Up;
                case ConsoleKey.DownArrow:
                    return Game.Direction.Down;
                case ConsoleKey.LeftArrow:
                    return Game.Direction.Left;
                case ConsoleKey.RightArrow:
                    return Game.Direction.Right;
            }
        }
        while (keyInfo.Key != ConsoleKey.Q);
        return Game.Direction.Default;
    }

}
class Monster : Actor
{

}
class Npc : Actor
{

}
