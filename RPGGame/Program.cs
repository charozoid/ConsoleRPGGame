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
