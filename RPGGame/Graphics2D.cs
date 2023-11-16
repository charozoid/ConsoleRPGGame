using SFML.Graphics;
using SFML.Window;
using SFML.System;

class Graphics2D
{
    const int WIDTH = 880;
    const int HEIGHT = 640;
    const string TITLE = "Game";
    private VideoMode mode = new VideoMode(WIDTH, HEIGHT);
    private Dictionary<Tile.Type, IntRect> typeRect = new Dictionary<Tile.Type, IntRect>();
    public RenderWindow window;
    public static Font font = new Font("../../Assets/Fonts/arial.ttf");
    public static Texture tileset;
    private Color[] grassColors = new Color[3];
    public Player player;
    public Random random;
    public static Tile[,] tiles = new Tile[128, 128];
    public static Dictionary<Tile.Type, IntRect> tileTexture = new Dictionary<Tile.Type, IntRect>();
    public static Dictionary<int, IntRect> wallFlagMap = new Dictionary<int, IntRect>();
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
        InitializeWallFlags();
        InitializeTileSpritesMap();
        InitializeGrassColorArray();

        tiles[11, 11].decoration = new Decoration(GridToIntRect(6, 0), new Color(200, 150, 0, 255));
    }
    public void InitializeWallFlags()
    {
        wallFlagMap[0b0000] = GridToIntRect(14, 12); //Right end
        wallFlagMap[0b0001] = GridToIntRect(14, 11); //Right end
        wallFlagMap[0b0010] = GridToIntRect(3, 11); //Top end
        wallFlagMap[0b0011] = GridToIntRect(11, 11); //Top right corner
        wallFlagMap[0b0100] = GridToIntRect(12, 11); //Left end
        wallFlagMap[0b0101] = GridToIntRect(13, 12); //Horizontal
        wallFlagMap[0b0110] = GridToIntRect(9, 12); //top left corner
        wallFlagMap[0b0111] = GridToIntRect(2, 12); //horizontal bottom turn
        wallFlagMap[0b1000] = GridToIntRect(0, 12); //Bottom end
        wallFlagMap[0b1001] = GridToIntRect(13, 11);//Bottom right corner 
        wallFlagMap[0b1010] = GridToIntRect(10, 11); // Vertical
        wallFlagMap[0b1011] = GridToIntRect(9, 11); //Vertical wall left turn
        wallFlagMap[0b1100] = GridToIntRect(4, 13); //bottom left corner
        wallFlagMap[0b1101] = GridToIntRect(10, 12); //horizontal top turn
        wallFlagMap[0b1110] = GridToIntRect(3, 12); //vertical right turn
        wallFlagMap[0b1111] = GridToIntRect(14, 12);

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Tile tile = tiles[i, j];
                tile.wallFlag = GetWallFlags(tile);
            }
        }
    }
    public void InitializeTileSpritesMap()
    {
        tileTexture[Tile.Type.Grass] = GridToIntRect(5, 2);
        tileTexture[Tile.Type.Wall] = GridToIntRect(13, 12);
        tileTexture[Tile.Type.StoneGround] = GridToIntRect(2, 11);
        tileTexture[Tile.Type.Door] = GridToIntRect(5, 12);
        tileTexture[Tile.Type.Cursor] = GridToIntRect(4, 0);
        tileTexture[Tile.Type.Empty] = GridToIntRect(0, 0);
        tileTexture[Tile.Type.Ground] = GridToIntRect(13, 3);
    }
    public void InitializeGrassColorArray()
    {
        grassColors[0] = new Color(40, 123, 0, 255);
        grassColors[1] = new Color(40, 180, 0, 255);
        grassColors[2] = new Color(40, 150, 0, 255);
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
    public void DrawDecoration(Decoration decoration)
    {
        Sprite sprite = new Sprite(tileset);
        sprite.Color = decoration.color;
        sprite.TextureRect = decoration.intRect;
        sprite.Position = new Vector2f(decoration.x * 16, decoration.y * 16);
        window.Draw(sprite);
    }
    public void DrawAroundPlayer()
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
                    tiles[i, j].drawx = i - player.x + 20;
                    tiles[i, j].drawy = j - player.y + 20;
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
                    if (tiles[i, j].decoration != null)
                    {
                        tiles[i, j].decoration.x = i - player.x + 20;
                        tiles[i, j].decoration.y = j - player.y + 20;
                        DrawDecoration(tiles[i, j].decoration);
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
                    tiles[j, i] = new Tile(j, i, (Tile.Type)int.Parse(values[j]));
                    tiles[j, i].x = j;
                    tiles[j, i].y = i;
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
    public static int GetWallFlags(Tile tile)
    {
        int flags = 0b0000;
        int x = tile.x;
        int y = tile.y;
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
        /*if (tile.actor != null)
        {
            return;
        }*/
        Sprite sprite = new Sprite(tileset);
        sprite.TextureRect = tileTexture[tile.type];
        Sprite sprite2 = new Sprite(tileset);
        switch (tile.type)
        {
            case Tile.Type.StoneGround:
                sprite2.Color = new Color(155, 155, 155, 255);
                sprite2.TextureRect = GridToIntRect(11, 13);
                sprite2.Position = new Vector2f(tile.drawx * 16, tile.drawy * 16);
                window.Draw(sprite2);
                sprite.Color = new Color(75, 75, 75, 255);
                break;
            case Tile.Type.Wall:
                if (!wallFlagMap.ContainsKey(tile.wallFlag))
                    return;
                sprite.TextureRect = wallFlagMap[tile.wallFlag];
                sprite.Color = new Color(125, 125, 125, 255);
                break;
            case Tile.Type.Grass:
                IntRect newTextureRect = tileTexture[tile.type];
                int randomTile = tile.positionVariation;
                if (randomTile == 0)
                {
                    newTextureRect = ReverseRect(newTextureRect, tile.intRectVariation);
                }
                if (tile.spriteVariation == 1)
                {
                    newTextureRect.Left += 2 * 16;
                }
                sprite.Color = grassColors[tile.colorVariation];
                sprite.TextureRect = newTextureRect;                
                break;
            case Tile.Type.Door:
                sprite.Color = new Color(160, 160, 160, 255);
                break;

            case Tile.Type.Cursor:
                sprite.Color = new Color(255, 0, 0, 255);
                break;
        }
        sprite.Position = new Vector2f(tile.drawx * 16, tile.drawy * 16);
        window.Draw(sprite);
    }
    public static void UpdateWallFlag(Tile tile)
    {
        tile.wallFlag = GetWallFlags(tile);
    }
    public static void UpdateWallFlagsAroundTile(Tile tile)
    {
        int x = tile.x;
        int y = tile.y;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborx = x + i;
                int neighbory = y + j;
                if (neighborx >= 0 && neighborx < Game.GAME_SIZE && neighbory >= 0 && neighbory < Game.GAME_SIZE)
                {
                    UpdateWallFlag(tiles[neighborx, neighbory]);
                }
            }
        }
    }
    public void DrawActor(Actor actor)
    {
        Sprite sprite = new Sprite(tileset);
        sprite.TextureRect = actor.intRect;
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
    public static IntRect GridToIntRect(int x, int y)
    {
        return new IntRect(x * 16, y * 16, 16, 16);
    }
}
