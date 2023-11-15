using SFML.Graphics;
using SFML.Window;
using SFML.System;

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
    public Tile[,] tiles = new Tile[128, 128];
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
        DrawWallsAroundMap();
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
        flagMap[0b1101] = GridToIntRect(10, 12);
        flagMap[0b0111] = GridToIntRect(11, 12);

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Tile tile = tiles[i, j];
                tile.wallFlag = GetWallFlags(tile, i, j);
            }
        }

        IntRect[] wallTexture = new IntRect[6];
        wallTexture[0] = GridToIntRect(13, 12);//Wall horizontal
        wallTexture[1] = GridToIntRect(10, 11);//Wall vertical
        wallTexture[2] = GridToIntRect(9, 12);//Top left corner
        wallTexture[3] = GridToIntRect(11, 11);//Top right corner
        wallTexture[4] = GridToIntRect(12, 11);//Bottom right corner
        wallTexture[5] = GridToIntRect(8, 12);//Bottom left corner
    }

    public void DrawWallsAroundMap()
    {

        for (int i = 0; i < 128; i++)
        {
            tiles[0, i].type = Tile.Type.Wall;
            tiles[i, 0].type = Tile.Type.Wall;
            tiles[127, i].type = Tile.Type.Wall;
            tiles[i, 127].type = Tile.Type.Wall;
        }
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


        return flags;
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
                if (flagMap.ContainsKey(tile.wallFlag))
                {
                    sprite.TextureRect = flagMap[tile.wallFlag];
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
