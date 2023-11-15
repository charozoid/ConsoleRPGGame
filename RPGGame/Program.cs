using System;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

class Program
{
    public static void Main(string[] args)
    {
        Game game = new Game();

        while (game.graphics2D.window.IsOpen)
        {
            game.graphics2D.window.Clear(Color.Black);
            game.graphics2D.window.DispatchEvents();
            game.graphics2D.DrawTilesAroundPlayer(game.player);
            //graphics2D.DrawTile(testTile);
            game.graphics2D.DrawActor(game.player);
            //Console.WriteLine($"{game.player.x}, {game.player.y}");



            game.graphics2D.window.Display();
        }
    }
}

class Game
{
    public const int GAME_SIZE = 128;
    public enum Direction
    {
        Up, Down, Left, Right, Default
    }
    public static List<Actor> actors = new List<Actor>();

    public Player player = new Player(20, 20);
    public Graphics2D graphics2D = new Graphics2D();
    public Game()
    {
        graphics2D.window.KeyPressed += new EventHandler<KeyEventArgs>(KeyPressed);
        graphics2D.player = player;
    }

    public void KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        int x = player.x;
        int y = player.y;
        Tile[,] tiles = graphics2D.tiles;
        switch (e.Code)
        {
            case Keyboard.Key.Up:
                if (y == 0)
                {
                    return;
                }
                else if (tiles[x, y - 1].type == Tile.Type.Wall)
                {
                    return;
                }
                player.Move(x, y - 1);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Down:
                if (y + 1 == GAME_SIZE)
                {
                    return;               
                }
                else if (tiles[x, y + 1].type == Tile.Type.Wall)
                {
                    return;
                }
                player.Move(x, y + 1);
                break;
            case Keyboard.Key.Left:
                if (x == 0)
                {
                    return;
                }
                else if (tiles[x - 1, y].type == Tile.Type.Wall)
                {
                    return;
                }
                player.Move(x - 1, y);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Right:
                if (x + 1 == GAME_SIZE)
                {
                    return;
                }
                else if (tiles[x + 1, y].type == Tile.Type.Wall)
                {
                    return;
                }
                player.Move(x + 1, y);
                Console.WriteLine("Moved");
                break;
        }
    }
}
class Graphics2D
{
    const int WIDTH = 720;
    const int HEIGHT = 640;
    const string TITLE = "Game";
    private VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    private Dictionary<Tile.Type, IntRect> typeRect = new Dictionary<Tile.Type, IntRect>();
    public RenderWindow window;
    private Font font;
    private Texture tileset;
    private IntRect[] grassRects = new IntRect[2];
    private Color[] grassColors = new Color[3];
    public Player player;
    public Tile[,] tiles = new Tile[1024, 1024];
    public Graphics2D()
    {
        window = new RenderWindow(mode, TITLE);
        font = new Font("../../Assets/Fonts/arial.ttf");
        tileset = CreateMask(new Texture("../../Assets/tileset.png"));
        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();
        ImportMap();
        grassRects[0] = GridToIntRect(7, 2);
        grassRects[1] = GridToIntRect(12, 2);
        grassColors[0] = new Color(34, 123, 0, 255);
        grassColors[1] = new Color(40, 225, 0, 255);
        grassColors[2] = new Color(40, 180, 0, 255);

    }
    public void DrawTilesAroundPlayer(Player player)
    {
        for (int i = player.x - 20; i < player.x + 19; i++)
        {
            for (int j = player.y - 20; j < player.y + 20; j++)
            {
                if (i < 0 || j < 0 || i > 127 || j > 127)
                {


                }
                else
                {
                    tiles[i, j].x = i - player.x + 20;
                    tiles[i, j].y = j - player.y + 20;
                    DrawTile(tiles[i, j]);
                }               
            }
        }
    }
    public void ImportMap()
    {
        string mapPath = @"../../Assets/map.txt";

        string[] lines = File.ReadAllLines(mapPath);
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(",");
            for (int j = 0; j < cols; j++)
            {
                try
                {
                    tiles[j, i] = new Tile(j, i, (Tile.Type)int.Parse(values[j]));
                }
                catch
                {
                    Console.WriteLine("Map data is invalid");
                }
            }
        }
    }
    public IntRect ReverseRect(IntRect sprite, int variation)
    {
        switch (variation)
        {
            case 0:
                return new IntRect(sprite.Left, sprite.Top + 16, 16, -16);
            case 1:
                return new IntRect(sprite.Left + 16, sprite.Top, -16, 16);
            case 2:
                return new IntRect(sprite.Left + 16, sprite.Top + 16, -16, -16);
        }
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
                    newTextureRect = ReverseRect(newTextureRect, tile.intRectVariation);
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
        sprite.Position = new Vector2f(320, 320);
        sprite.Color = actor.spriteColor;
        window.Draw(sprite);
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
        Empty = 0, Ground = 1, Wall = 2, Grass = 3
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
    public int intRectVariation = 0;
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
            colorVariation = random.Next(3);
            positionVariation = random.Next(2);
            intRectVariation = random.Next(4); 
        }
    }
}
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
        spritex = 0;
        spritey = 4;
        spriteColor = new Color(255, 204, 156, 255);
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
class Monster : Actor
{

}
class Npc : Actor
{

}
