using SFML.Window;
using SFML.Graphics;
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
        InGame,
        Editor
    }

    public static List<Actor> actors = new List<Actor>();

    public State state = State.Editor;
    public static Player player;
    public const string mapPath = @"../../Assets/map.txt";
    public const string actorsPath = @"../../Assets/actors.txt";
    public const string decorationsPath = @"../../Assets/decorations.txt";

    public Graphics2D graphics2D;
    public static Random random = new Random();
    public Game()
    {
        random = new Random();
        graphics2D = new Graphics2D();
        graphics2D.random = random;
        LoadActors();
        LoadDecorations();
    }
    public void KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        int x = player.x;
        int y = player.y;
        Tile[,] tiles = Graphics2D.tiles;
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
            case Keyboard.Key.Enter:
                SaveActors();
                break;
        }
    }

    public static void SaveMap()
    {
        File.Delete(mapPath);
        using (FileStream fs = File.Create(mapPath))
        {
            fs.Close();
        }

        Tile[,] tiles = Graphics2D.tiles;
        string[] mapData = new string[128];
        for (int i = 0; i < 128; i++)
        {
            string line = "";
            for (int j = 0; j < 128; j++)
            {
                line += $"{(int)tiles[j, i].type}";
                if (tiles[j, i].color != Color.White)
                {
                    Color color = tiles[j, i].color;
                    line += $";{color.R};{color.G};{color.B}";
                }
                if (j < 127)
                {
                    line += $",";
                }
            }
            mapData[i] = line;
        }
        File.AppendAllLines(mapPath, mapData);
    }
    public static void SaveDecorations()
    {
        File.Delete(decorationsPath);
        using (FileStream fs = File.Create(decorationsPath))
        {
            fs.Close();
        }
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                Tile tile = Graphics2D.tiles[i,j];
                if (tile.decoration != null)
                {
                    byte r = tile.decoration.color.R;
                    byte g = tile.decoration.color.G;
                    byte b = tile.decoration.color.B;
                    File.AppendAllText(decorationsPath, $"{(int)tile.decoration.type},{tile.x},{tile.y},{r},{g},{b}\n");
                }
            }
        }

    }
    public void SaveActors()
    {
        string[] stringArr = new string[actors.Count];
        int index = 0;
        foreach (Actor actor in actors.Take(actors.Count))
        {
                string saveData = $"{actor.x},{actor.y},{actor.intRect.Left / 16},{actor.intRect.Top / 16}".Trim();
                stringArr[index] = saveData;
                index++;
        }
        File.WriteAllLines(actorsPath, stringArr);
    }
    public void LoadActors()
    {
        string[] lines = File.ReadAllLines(actorsPath);
        int cols = lines[0].Split(',').Length;
        int[] data = new int[cols];
        foreach (string line in lines)
        {
            string[] values = line.Split(",");
            int x = int.Parse(values[0]);
            int y = int.Parse(values[1]);
            int rectLeft = int.Parse(values[2]);
            int rectRight = int.Parse(values[3]);
            byte r = byte.Parse(values[4]);
            byte g = byte.Parse(values[5]);
            byte b = byte.Parse(values[6]);
            Actor actor = new Actor(x, y, rectLeft, rectRight, new Color(r, g, b));
            Graphics2D.tiles[x, y].actor = actor;
        }
    }
    public void LoadDecorations()
    {
        string[] lines = File.ReadAllLines(decorationsPath);
        int cols = lines[0].Split(',').Length;
        int[] data = new int[cols];
        foreach (string line in lines)
        {
            string[] values = line.Split(",");
            int type = int.Parse(values[0]);
            int x = int.Parse(values[1]);
            int y = int.Parse(values[2]);
            byte r = byte.Parse(values[3]);
            byte g = byte.Parse(values[4]);
            byte b = byte.Parse(values[5]);
            Decoration decoration = new Decoration(x, y, new Color(r,g,b), (Decoration.Type)(type));
            Graphics2D.tiles[x, y].decoration = decoration;
        }
    }
}
