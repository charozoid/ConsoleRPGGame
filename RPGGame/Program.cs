using System;
using System.ComponentModel.Design;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using SFML.Graphics;

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
                    game.graphics2D.window.Clear(new Color(0, 0, 0, 255));
                    game.graphics2D.DrawAroundPlayer();
                    game.graphics2D.DrawActor(Game.player);
                    break;
                case (Game.State.MainMenu):
                    break;
                case (Game.State.Editor):

                    game.graphics2D.window.Clear(new Color(0, 0, 0, 255));
                    game.graphics2D.DrawAroundPlayer();
                    game.graphics2D.DrawActor(Game.player);
                    break;
            }           
            game.graphics2D.window.DispatchEvents();
            game.graphics2D.window.Display();
            Console.WriteLine($"x : {Game.player.x}, y : {Game.player.y}");
        }
    }
}
