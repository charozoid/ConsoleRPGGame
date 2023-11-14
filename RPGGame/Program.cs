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
    const int WIDTH = 640;
    const int HEIGHT = 480;
    const string TITLE = "Game";
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
        player.spritex = 0;
        player.spritey = 4;

        while (graphics2D.window.IsOpen)
        {
            graphics2D.window.Clear(Color.Black);
            graphics2D.window.DispatchEvents();
            graphics2D.DrawWalls();
            Tile corner = new Tile(0, 0, Tile.Type.Wall);
            corner.wallType = 1;
            graphics2D.DrawTile(corner);
            graphics2D.DrawActor(player);
            graphics2D.window.Display();

        }
        /*Graphics graphics = new Graphics();
        graphics.ImportMap();

        Player player = new Player(3, 3, "C");
        actors.Add(player);

        Weapon sword = new Weapon(0, "Iron Sword", false);
        Item gold = new Item(1, "Gold coins", true);

        player.GiveItem(sword, 1);
        player.GiveItem(gold, 500);
        bool quit = false;

        while (!quit)
        {
            graphics.Menu(player);
        }*/
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
    Tile[,] tiles = new Tile[64,64];
    public Graphics2D()
    {
        this.window = new RenderWindow(mode, TITLE);
        this.font = new Font("../../Assets/Fonts/arial.ttf");
        this.tileset = CreateMask(new Texture("../../Assets/tileset.png"));
        CreateEmptiness();
    }
    public void CreateEmptiness()
    {
        for (int i = 0; i < 64; i++)
        {
            for (int j = 0; j < 64; j++)
            {
                tiles[j, i] = new Tile(i, j, Tile.Type.Empty);
            }

        }
    }
    public Texture CreateMask(Texture tileset)
    {
        Image img = tileset.CopyToImage();
        img.CreateMaskFromColor(new Color(255, 0, 255, 255));
        return new Texture(img);
    }
    public void DrawTile(Tile tile)
    {
        switch (tile.type)
        {
            case Tile.Type.Wall:

                Sprite sprite = new Sprite(tileset);
                sprite.Color = new Color(125, 125, 125, 255);
                sprite.TextureRect = GridToIntRect(11, 13);
                sprite.Position = new Vector2f(tile.x * 16, tile.y * 16);
                window.Draw(sprite);
                break;
        }
    }
    public void DrawWall(Tile tile, int type)
    {
        Sprite sprite = new Sprite(tileset);
        sprite.TextureRect = GridToIntRect(11, 13);
        sprite.Position = new Vector2f(tile.x * 16, tile.y * 16);
        window.Draw(sprite);
    }
    public IntRect GetWallSprite(Tile tile)
    {
        int x = tile.x;
        int y = tile.y;
        if (tiles[x + 1, y].type == Tile.Type.Wall)
        {

        }
        return new IntRect();
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
                DrawTile(tiles[i,j]);
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
    public void CreateTiles()
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i,j] = new Tile(i, j, Tile.Type.Ground);
            }
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
        Empty, Ground, Wall 
    }

    public Sprite sprite;
    public int x;
    public int y;
    public int spritex;
    public int spritey;
    public Type type;
    public int wallType = 0;
    public Tile(int x, int y, Type type)
    {
        this.x = x; 
        this.y = y;
        this.type = type;
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
    public bool CheckForWall(int x, int y)
    {
        if (mapArray[x, y] == "■")
        {
            return true;
        }
        return false;
    }
    public void PrintMap()
    {
        for (int i = 0; i < mapArray.GetLength(0); i++)
        {
            for (int j = 0; j < mapArray.GetLength(1); j++)
            {
                Console.SetCursorPosition(i, j);
                Console.Write(mapArray[i, j]);
            }
        }
    }
    public int PromptValidChoice(int min, int max)
    {
        bool success = false;

        while (!success)
        {
            int choice = 0;
            success = int.TryParse(Console.ReadLine(), out choice);
            if (!success)
            {
                Console.WriteLine("Invalid choice");
            }
            else
            {
                if (choice < min || choice > max)
                {
                    Console.WriteLine("Not within bounds");
                    success = false;
                }
                else
                {
                    return choice;
                }
            }
        }
        return 0;
    }
    public void Menu(Player player)
    {
        Console.Clear();
        PrintMap();
        PrintActors();

        Console.SetCursorPosition(0, mapArray.GetLength(1) + 1);
        Console.WriteLine("1.Control Player");
        Console.WriteLine("2.Show inventory");
        Console.Write("Make choice:");

        int choice = PromptValidChoice(1, 2);

        switch (choice)
        {
            case 1:
                MovePlayer(player);
                break;
            case 2:
                PrintInventory(player);
                break;
        }
    }
    public void MovePlayer(Player player)
    {
        Console.Clear();
        PrintMap();
        PrintActors();
        Console.SetCursorPosition(0, mapArray.GetLength(1) + 1);
        Console.WriteLine("Choose direction with arrow key. Press Q to quit.");
        bool quit = false;

        while (!quit)
        {
            PrintMap();
            PrintActors();
            Game.Direction direction = player.GetDirectionInput();

            if (direction == Game.Direction.Default)
            {
                quit = true;
            }
            else if (direction == Game.Direction.Up)
            {
                if (!CheckForWall(player.x, player.y - 1))
                {
                    player.Move(player.x, player.y - 1);
                }
            }
            else if (direction == Game.Direction.Down)
            {
                if (!CheckForWall(player.x, player.y + 1))
                {
                    player.Move(player.x, player.y + 1);
                }
            }
            else if (direction == Game.Direction.Left)
            {
                if (!CheckForWall(player.x - 1, player.y))
                {
                    player.Move(player.x - 1, player.y);
                }
            }
            else if (direction == Game.Direction.Right)
            {
                if (!CheckForWall(player.x + 1, player.y))
                {
                    player.Move(player.x + 1, player.y);
                }
            }
        }

    }
    public void PrintActors()
    {
        foreach (Actor actors in Game.actors)
        {
            Console.SetCursorPosition(actors.x, actors.y);
        }
    }
    public void PrintInventory(Player player)
    {
        Console.Clear();
        int count = 1;
        foreach (Item items in player.inventory)
        {
            if (items.stackable)
            {
                Console.Write($"{count}.{items.name} X {items.quantity}\n");
            }
            else
            {
                Console.Write($"{count}.{items.name}\n");
            }
            count++;
        }
        Console.ReadKey();
        Menu(player);
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

    /*public void PrintActor()
    {
        Console.SetCursorPosition(x, y);
        Console.WriteLine(model);
    }*/
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
        while ( keyInfo.Key != ConsoleKey.Q);
        return Game.Direction.Default;
    }

}
class Monster : Actor
{

}
class Npc : Actor
{

}
