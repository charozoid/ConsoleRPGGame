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
        InGame
    }

    public List<Actor> actors = new List<Actor>();

    public State state = State.InGame;
    public static Player player = new Player(20, 20, 0, 4, new Color(255, 204, 156, 255));
    public Npc npc = new Npc(6, 3, 2, 0, new Color(255, 204, 156, 255));
    public Actor chest = new Actor(5, 3, 2, 9, new Color(200, 200, 20, 255));

    public Graphics2D graphics2D;
    public static Random random = new Random();
    public Game()
    {
        random = new Random();
        graphics2D = new Graphics2D();
        graphics2D.random = random;
        graphics2D.window.KeyPressed += new EventHandler<KeyEventArgs>(KeyPressed);
        graphics2D.tiles[npc.x, npc.y].actor = npc;
        graphics2D.tiles[chest.x, chest.y].actor = chest;
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
