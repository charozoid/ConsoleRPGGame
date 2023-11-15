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
            switch (game.state)
            {
                case (Game.State.InGame):
                    game.graphics2D.window.Clear(Color.Black);
                    game.graphics2D.DrawTilesAroundPlayer();
                    game.graphics2D.DrawActor(Game.player);
                    break;
                case (Game.State.MainMenu):
                    break;
            }           
            game.graphics2D.window.DispatchEvents();
            game.graphics2D.window.Display();
            Console.WriteLine($"x : {Game.player.x}, y : {Game.player.y}");
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
    public enum State
    {
        MainMenu,
        InGame
    }
    public List<Actor> actors = new List<Actor>();

    public State state = State.InGame;
    public static Player player = new Player(20,20);
    public Npc npc = new Npc(6, 2);

    public Graphics2D graphics2D;
    public static Random random = new Random();
    public Game()
    {
        random = new Random();
        graphics2D = new Graphics2D();
        graphics2D.random = random;
        graphics2D.window.KeyPressed += new EventHandler<KeyEventArgs>(KeyPressed);
        graphics2D.tiles[npc.x, npc.y].actor = npc;
        actors.Add(player);
        actors.Add(npc);
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
                else if (tiles[x, y - 1].type == Tile.Type.Wall || tiles[x, y - 1].actor != null)
                {
                    return;
                }
                tiles[x, y].actor = null;
                tiles[x, y - 1].actor = player;
                player.Move(x, y - 1);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Down:
                if (y + 1 == GAME_SIZE)
                {
                    return;               
                }
                else if (tiles[x, y + 1].type == Tile.Type.Wall || tiles[x, y + 1].actor != null)
                {
                    return;
                }
                tiles[x, y].actor = null;
                tiles[x, y + 1].actor = player;
                player.Move(x, y + 1);
                break;
            case Keyboard.Key.Left:
                if (x == 0)
                {
                    return;
                }
                else if (tiles[x - 1, y].type == Tile.Type.Wall || tiles[x - 1, y].actor != null)
                {
                    return;
                }
                tiles[x, y].actor = null;
                tiles[x - 1, y].actor = player;
                player.Move(x - 1, y);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Right:
                if (x + 1 == GAME_SIZE)
                {
                    return;
                }
                else if (tiles[x + 1, y].type == Tile.Type.Wall || tiles[x + 1, y].actor != null)
                {
                    return;
                }
                tiles[x, y].actor = null;
                tiles[x + 1, y].actor = player;
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
    private Color[] grassColors = new Color[3];
    public Player player;
    public Random random;
    public Tile[,] tiles = new Tile[1024, 1024];
    public Dictionary<Tile.Type, IntRect> tileTexture = new Dictionary<Tile.Type, IntRect>();
    public Dictionary<int, IntRect> flagMap = new Dictionary<int, IntRect>();
    public Graphics2D()
    {
        player = Game.player;
        random = Game.random;
        window = new RenderWindow(mode, TITLE);
        font = new Font("../../Assets/Fonts/arial.ttf");
        tileset = CreateMask(new Texture("../../Assets/tileset.png"));
        window.SetVerticalSyncEnabled(true);
        window.Closed += (sender, args) => window.Close();
        ImportMap();
        grassColors[0] = new Color(40, 123, 0, 255);
        grassColors[1] = new Color(40, 180, 0, 255);
        grassColors[2] = new Color(40, 150, 0, 255);
        tileTexture[Tile.Type.Grass] = GridToIntRect(7, 2);
        tileTexture[Tile.Type.Wall] = GridToIntRect(13, 12);
        tileTexture[Tile.Type.StoneGround] = GridToIntRect(4, 0);

        flagMap[0b1000] = GridToIntRect(7, 13);
        flagMap[0b0100] = GridToIntRect(8, 13);
        flagMap[0b1001] = GridToIntRect(12, 11);
        flagMap[0b1010] = GridToIntRect(10, 11);
        flagMap[0b0011] = GridToIntRect(11, 11);
        flagMap[0b0101] = GridToIntRect(13, 12);

        IntRect[] wallTexture = new IntRect[6];
        wallTexture[0] = GridToIntRect(13, 12);//Wall horizontal
        wallTexture[1] = GridToIntRect(10, 11);//Wall vertical
        wallTexture[2] = GridToIntRect(9, 12);//Top left corner
        wallTexture[3] = GridToIntRect(11, 11);//Top right corner
        wallTexture[4] = GridToIntRect(12, 11);//Bottom right corner
        wallTexture[5] = GridToIntRect(8, 12);//Bottom left corner

    }
    public void DrawTilesAroundPlayer()
    {
        Player player = Game.player;
        for (int i = player.x - 20; i < player.x + 20; i++)
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
                    DrawTile(tiles[i, j], i, j);
                    if (tiles[i, j].actor != null)
                    {
                        if (tiles[i, j].actor.GetType() != typeof(Player))
                        {
                            tiles[i, j].actor.drawx = i - player.x + 20;
                            tiles[i, j].actor.drawy = j - player.y + 20;
                            DrawActor(tiles[i, j].actor);
                        }
                    }
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
                    tiles[j, i] = new Tile(j, i, (Tile.Type)int.Parse(values[j]), random);
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

    public int GetWallFlags(Tile tile, int x, int y)
    {
        int flags = 0b0000;
        bool moveLeft = x - 1 >= 0;
        bool moveRight = x + 1 < 128;
        bool moveUp = y - 1 >= 0;
        bool moveDown = y + 1 < 128;
        if (moveUp && tiles[x, y - 1].type == Tile.Type.Wall)
        {
            flags = flags | 0b1000;
        }
        if (moveRight && tiles[x + 1, y].type == Tile.Type.Wall)
        {
            flags = flags | 0b0100;
        }
        if (moveDown && tiles[x, y + 1].type == Tile.Type.Wall)
        {
            flags = flags | 0b0010;
        }
        if (moveLeft && tiles[x - 1, y].type == Tile.Type.Wall)
        {
            flags = flags | 0b0001;
        }


        return flags; ;
    }
    public Texture CreateMask(Texture tileset)
    {
        Image img = tileset.CopyToImage();
        img.CreateMaskFromColor(new Color(255, 0, 255, 255));
        return new Texture(img);
    }
    public void DrawTile(Tile tile, int x, int y)
    {
        if (tile.actor != null)
        {
            return;
        }
        Sprite sprite = new Sprite(tileset);
        switch (tile.type)
        {
            case Tile.Type.StoneGround:
                sprite.Color = new Color(125, 125, 125, 255);
                sprite.TextureRect = tileTexture[Tile.Type.StoneGround];
                break;
            case Tile.Type.Wall:
                int flag = GetWallFlags(tiles[x, y], x, y);
                if (flagMap.ContainsKey(flag))
                {
                    sprite.TextureRect = flagMap[flag];
                }
                else
                {
                    return;
                }
                sprite.Color = new Color(125, 125, 125, 255);
                break;
            case Tile.Type.Grass:
                IntRect newTextureRect = tileTexture[Tile.Type.Grass];
                int randomTile = tile.positionVariation;
                if (randomTile == 0)
                {
                    newTextureRect = ReverseRect(newTextureRect, tile.intRectVariation);
                }
                if (tile.spriteVariation == 1)
                {
                    newTextureRect.Left += 5 * 16;
                }
                sprite.Color = grassColors[tile.colorVariation];
                sprite.TextureRect = newTextureRect;                
                break;
        }
        tile.sprite = sprite;
        sprite.Position = new Vector2f(tile.x * 16, tile.y * 16);
        window.Draw(sprite);
    }
    public void DrawActor(Actor actor)
    {
        Sprite sprite = new Sprite(tileset);
        sprite.TextureRect = GridToIntRect(actor.spritex, actor.spritey);
        tiles[actor.x, actor.y].actor = actor;
        if (actor.GetType() == typeof(Player))
        {
            sprite.Position = new Vector2f(320, 320);
        }
        else
        {
            sprite.Position = new Vector2f(actor.drawx * 16, actor.drawy * 16);
        }

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
        Empty, 
        Ground, 
        Wall, 
        Grass,
        StoneGround
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
    public int drawx = 0;
    public int drawy = 0;
    public Actor actor;
    private Random random;
    public Vector2f GetPos()
    {
        return new Vector2f(x, y);
    }
    public Tile(int x, int y, Type type, Random gameRandom)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.random = gameRandom;
        if (type == Type.Grass)
        {
            spriteVariation = Game.random.Next(2);
            colorVariation = Game.random.Next(3);
            positionVariation = Game.random.Next(2);
            intRectVariation = Game.random.Next(4); 
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
    public int drawx = 0;
    public int drawy = 0;
    public int spritex = 0;
    public int spritey = 0;
    void Draw()
    {

    }
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
    public Npc(int x, int y)
    {
        this.x = x;
        this.y = y;
        spritex = 1;
        spritey = 0;
        spriteColor = new Color(255, 204, 156, 255);
    }
}
