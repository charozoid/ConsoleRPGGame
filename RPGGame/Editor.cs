using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

class Editor
{
    public static Tile firstSelection;
    public static Tile secondSelection;
    private static bool hasSelectedTile = false;
    private static int firstSelectx = 0;
    private static int firstSelecty = 0;
    private static Player player = Game.player;
    public static void KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        Player player = Game.player;
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
                tiles[x, y].actor = null;
                tiles[x, y - 1].actor = player;
                player.Move(x, y - 1);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Down:
                if (y + 1 == Game.GAME_SIZE)
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
                tiles[x, y].actor = null;
                tiles[x - 1, y].actor = player;
                player.Move(x - 1, y);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Right:
                if (x + 1 == Game.GAME_SIZE)
                {
                    return;
                }
                tiles[x, y].actor = null;
                tiles[x + 1, y].actor = player;
                player.Move(x + 1, y);
                Console.WriteLine("Moved");
                break;
            case Keyboard.Key.Enter:
                if (!hasSelectedTile)
                {
                    firstSelection = Graphics2D.tiles[x, y];
                    firstSelection.type = Tile.Type.Cursor;
                    firstSelectx = x;
                    firstSelecty = y;
                    hasSelectedTile = true;
                }
                else
                {
                    int oldPlayerx = player.x;
                    int oldPlayery = player.y;
                    secondSelection = Graphics2D.tiles[x, y];
                    secondSelection.type = Tile.Type.Cursor;
                    if (firstSelectx > player.x)
                    {
                        oldPlayerx = firstSelectx;
                        firstSelectx = player.x;
                    }
                    if (firstSelecty > player.y)
                    {
                        oldPlayery = firstSelecty;
                        firstSelecty = player.y;
                    }
                    for (int i = firstSelectx; i < oldPlayerx + 1; i++)
                    {
                        for (int j = firstSelecty; j < oldPlayery + 1; j++)
                        {           
                            tiles[i, j].type = Tile.Type.Wall;
                            tiles[i, j].wallFlag = Graphics2D.GetWallFlags(tiles[i, j]);
                            Graphics2D.UpdateNeighborWallTiles(tiles[i, j]);
                        }
                    }
                    hasSelectedTile = false;
                }
                break;
        }
    }
}