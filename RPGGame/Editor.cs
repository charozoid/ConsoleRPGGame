using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

class Editor
{
    public Tile firstSelection;
    public Tile secondSelection;
    private bool hasSelectedTile = false;
    private int firstSelectx = 0;
    private int firstSelecty = 0;
    private Player player = Game.player;
    private Text title = new Text("Map editor", Graphics2D.font, 30);
    private Text saveString = new Text("S.Save", Graphics2D.font, 16);
    private Text quitString = new Text("Esc.Exit", Graphics2D.font, 16);
    private Font font = Graphics2D.font;
    private static DateTime lastKeyPressTime = DateTime.MinValue;
    private static TimeSpan debounceDelay = TimeSpan.FromMilliseconds(50);
    private Color selectedColor = Color.White;
    int colorValuesIndex = 0;
    int colorTypeIndex = 0;
    byte[] colorValues = new byte[3];
    private int selection = 0;
    private bool isTyping = false;
    public RenderWindow window;
    private string enteredText = "";

    public Editor(RenderWindow window)
    {
        this.window = window;
    }
    public void DrawSprite(Tile.Type type)
    {
        Sprite sprite = new Sprite(Graphics2D.tileset);
        sprite.Color = new Color(155, 155, 155, 255);
        sprite.TextureRect = Graphics2D.tileTexture[type];
        sprite.Position = new Vector2f(672, 0);
        window.Draw(sprite);
    }
    public void DrawMenu()
    {
        Sprite sprite = new Sprite(Graphics2D.tileset);
        sprite.Color = new Color(155, 155, 155, 255);
        title.FillColor = Color.White;
        title.Position = new Vector2f(680, 30);
        saveString.FillColor = Color.White;
        saveString.Position = new Vector2f(650, 600);
        quitString.FillColor = Color.White;
        quitString.Position = new Vector2f(710, 600);
        int startingy = 100;
        int len = Enum.GetValues(typeof(Tile.Type)).Length;
        for (int i = 0; i < len; i++)
        {
            string str = $"{i + 1}.{Enum.GetName(typeof(Tile.Type), i)}";
            Text text = new Text(str, font, 14);
            sprite.Color = selectedColor;
            sprite.TextureRect = Graphics2D.tileTexture[(Tile.Type)i];
            if (i == selection)
            {
                text.FillColor = Color.Red;
            }
            else
            {
                text.FillColor = Color.White;
            }

            text.Position = new Vector2f(680, i * 35 + startingy);
            sprite.Position = new Vector2f(650, i * 35 + startingy);
            startingy += 32;
            window.Draw(quitString);
            window.Draw(saveString);
            window.Draw(sprite);
            window.Draw(text);
        }
        window.Draw(title);
    }
    public void ColorPrompt(object sender, TextEventArgs e)
    {
        if (!isTyping)
            return;
        if ((DateTime.Now - lastKeyPressTime).TotalMilliseconds < debounceDelay.TotalMilliseconds)
        {
            return;
        }
        if (e.Unicode == "c")
        {
            return;
        }
        enteredText += e.Unicode;
        if (enteredText.Length == 3) 
        {
            bool success = byte.TryParse(enteredText, out colorValues[colorValuesIndex]);
            if (!success)
            {
                enteredText = "";
            }
            else
            {
                colorValuesIndex++;
                if (colorValuesIndex == 3)
                {
                    selectedColor = new Color(colorValues[0], colorValues[1], colorValues[2]);
                    isTyping = false;
                    colorValuesIndex = 0;
                }
                enteredText = "";
            }


        }
        lastKeyPressTime = DateTime.Now;
    }
    public void EnterColorValue(RenderWindow window)
    {
        enteredText = "";
        isTyping = true;
        window.TextEntered += ColorPrompt;
    }
    public void KeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        Player player = Game.player;
        int x = player.x;
        int y = player.y;

        Tile[,] tiles = Graphics2D.tiles;
        if (isTyping)
            return;
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
                            tiles[i, j].color = selectedColor;
                            tiles[i, j].type = (Tile.Type)(selection);
                            Graphics2D.UpdateWallFlagsAroundTile(tiles[i, j]);
                        }
                    }
                    hasSelectedTile = false;
                }
                break;

            case Keyboard.Key.C:
                EnterColorValue(window);
                break;
            case Keyboard.Key.Num1:
                selection = 0;
                break;
            case Keyboard.Key.Num2:
                selection = 1;
                break;
            case Keyboard.Key.Num3:
                selection = 2;
                break;
            case Keyboard.Key.Num4:
                selection = 3;
                break;
            case Keyboard.Key.Num5:
                selection = 4;
                break;
            case Keyboard.Key.Num6:
                selection = 5;
                break;
            case Keyboard.Key.Num7:
                selection = 6;
                break;
            case Keyboard.Key.Num8:
                selection = 7;
                break;
            case Keyboard.Key.Num9:
                selection = 8;
                break;
            case Keyboard.Key.S:
                Game.SaveMap();
                Game.SaveDecorations();
                break;
            case Keyboard.Key.Escape:
                window.Close();
                break;
        }
    }
}