using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection.PortableExecutable;

class Game
{
    public enum Direction
    {
        Up, Down, Left, Right, Default   
    }
    public static List<Actor> actors = new List<Actor>();
    public static void Main(string[] args)
    {
        Graphics graphics = new Graphics();
        graphics.ImportMap();

        Player player = new Player(3, 3, "C");
        actors.Add(player);

        Weapon sword = new Weapon(0, "Iron Sword", false);
        Item gold = new Item(1, "Gold coins", true);

        player.GiveItem(sword, 1);
        player.GiveItem(gold, 500);
        bool quit = false;

        while (!quit)
        {
            graphics.Menu(player);
        }
    }

}

class Graphics
{
    //public Dictionary<string, int> Map = new Dictionary<string, int>();

    string[,] mapArray;
    const int MAP_WIDTH = 10;
    const int MAP_HEIGHT = 3;
    const string SAVE_PATH = @"C:\game\";
    public void ImportMap()
    {
        string mapPath = $"{SAVE_PATH}map.txt";

        string[] lines = File.ReadAllLines(mapPath);

        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;
        mapArray = new string[cols, rows];
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(",");
            for (int j = 0; j < cols; j++)
            {
                try
                {
                    mapArray[j, i] = values[j];
                }
                catch
                {
                    Console.WriteLine("Map data is invalid");
                }
            }
        }
    }
    public bool CheckForWall(int x, int y)
    {
        if (mapArray[x, y] == "■")
        {
            return true;
        }
        return false;
    }
    public void PrintMap()
    {
        for (int i = 0; i < mapArray.GetLength(0); i++)
        {
            for (int j = 0; j < mapArray.GetLength(1); j++)
            {
                Console.SetCursorPosition(i, j);
                Console.Write(mapArray[i, j]);
            }
        }
    }
    public int PromptValidChoice(int min, int max)
    {
        bool success = false;

        while (!success)
        {
            int choice = 0;
            success = int.TryParse(Console.ReadLine(), out choice);
            if (!success)
            {
                Console.WriteLine("Invalid choice");
            }
            else
            {
                if (choice < min || choice > max)
                {
                    Console.WriteLine("Not within bounds");
                    success = false;
                }
                else
                {
                    return choice;
                }
            }
        }
        return 0;
    }
    public void Menu(Player player)
    {
        Console.Clear();
        PrintMap();
        PrintActors();

        Console.SetCursorPosition(0, mapArray.GetLength(1) + 1);
        Console.WriteLine("1.Control Player");
        Console.WriteLine("2.Show inventory");
        Console.Write("Make choice:");

        int choice = PromptValidChoice(1, 2);

        switch (choice)
        {
            case 1:
                MovePlayer(player);
                break;
            case 2:
                PrintInventory(player);
                break;
        }
    }
    public void MovePlayer(Player player)
    {
        Console.Clear();
        PrintMap();
        PrintActors();
        Console.SetCursorPosition(0, mapArray.GetLength(1) + 1);
        Console.WriteLine("Choose direction with arrow key. Press Q to quit.");
        bool quit = false;

        while (!quit)
        {
            PrintMap();
            PrintActors();
            Game.Direction direction = player.GetDirectionInput();

            if (direction == Game.Direction.Default)
            {
                quit = true;
            }
            else if (direction == Game.Direction.Up)
            {
                if (!CheckForWall(player.x, player.y - 1))
                {
                    player.Move(player.x, player.y - 1);
                }
            }
            else if (direction == Game.Direction.Down)
            {
                if (!CheckForWall(player.x, player.y + 1))
                {
                    player.Move(player.x, player.y + 1);
                }
            }
            else if (direction == Game.Direction.Left)
            {
                if (!CheckForWall(player.x - 1, player.y))
                {
                    player.Move(player.x - 1, player.y);
                }
            }
            else if (direction == Game.Direction.Right)
            {
                if (!CheckForWall(player.x + 1, player.y))
                {
                    player.Move(player.x + 1, player.y);
                }
            }
        }

    }
    public void PrintActors()
    {
        foreach (Actor actors in Game.actors)
        {
            Console.SetCursorPosition(actors.x, actors.y);
            Console.Write(actors.model);
        }
    }
    public void PrintInventory(Player player)
    {
        Console.Clear();
        int count = 1;
        foreach (Item items in player.inventory)
        {
            if (items.stackable)
            {
                Console.Write($"{count}.{items.name} X {items.quantity}\n");
            }
            else
            {
                Console.Write($"{count}.{items.name}\n");
            }
            count++;
        }
        Console.ReadKey();
        Menu(player);
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
    public int x = 0;
    public int y = 0;
    public string model = "";
    int hp = 0;
    int strength = 0;
    int intelligence = 0;
    int dexterity = 0;

    public void PrintActor()
    {
        Console.SetCursorPosition(x, y);
        Console.WriteLine(model);
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
    public List<Item> inventory = new List<Item>();
    public Player(int x, int y, string model)
    {
        this.x = x;
        this.y = y;
        this.model = model;
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
    public Game.Direction GetDirectionInput()
    {
        ConsoleKeyInfo keyInfo;
        do
        {
            Console.SetCursorPosition(x, y);
            keyInfo = Console.ReadKey(true);
           
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    return Game.Direction.Up;
                case ConsoleKey.DownArrow:
                    return Game.Direction.Down;
                case ConsoleKey.LeftArrow:
                    return Game.Direction.Left;
                case ConsoleKey.RightArrow:
                    return Game.Direction.Right;
            }
        } 
        while ( keyInfo.Key != ConsoleKey.Q);
        return Game.Direction.Default;
    }
}
class Monster : Actor
{

}
class Npc : Actor
{

}