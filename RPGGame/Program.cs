using System;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using SFML.Graphics;
using SFML.Window;
using static Game;

class Program
{
    public static void Main(string[] args)
    {
        Game game = new Game();
        RenderWindow window = game.graphics2D.window;
        Editor editor = new Editor(window);
        switch (game.state)
        {
            case State.InGame:
                player = new Player(20, 20, 0, 4, new Color(255, 204, 156, 255));
                window.KeyPressed += new EventHandler<KeyEventArgs>(game.KeyPressed);
                break;
            case State.Editor:
                player = new Player(20, 20, 4, 0, new Color(255, 255, 255, 255));
                window.KeyPressed += new EventHandler<KeyEventArgs>(editor.KeyPressed);
                break;
        }
        while (window.IsOpen)
        {
            switch (game.state)
            {
                case (Game.State.InGame):
                    window.Clear(new Color(0, 0, 0, 255));
                    game.graphics2D.DrawAroundPlayer();
                    game.graphics2D.DrawActor(Game.player);
                    break;
                case (Game.State.MainMenu):
                    break;
                case (Game.State.Editor):
                    window.Clear(new Color(0, 0, 0, 255));
                    editor.DrawMenu();
                    game.graphics2D.DrawAroundPlayer();
                    game.graphics2D.DrawActor(Game.player);
                    break;
            }           
            window.DispatchEvents();
            window.Display();
            Console.WriteLine($"x : {Game.player.x}, y : {Game.player.y}");
        }
    }
}
