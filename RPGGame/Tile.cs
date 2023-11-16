using SFML.Graphics;
using SFML.System;

class Tile
{
    public enum Type
    {
        Empty, //0
        Ground, //1
        Wall, //2
        Grass, //3
        StoneGround,//4
        Door, //5,
        Cursor
    }
    public int x;
    public int y;
    public Type type;
    public int wallType = 0;
    public bool drawn = false;
    public int colorVariation = 0;
    public int spriteVariation = 0;
    public int positionVariation = 0;
    public int intRectVariation = 0;
    public int drawx = 0;
    public int drawy = 0;
    public int wallFlag = 0;
    public Decoration decoration;
    public Actor actor;
    private Random random;
    public Vector2f GetPos()
    {
        return new Vector2f(x, y);
    }
    public Tile(int x, int y, Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        random = Game.random;
        if (type == Type.Grass)
        {
            spriteVariation = Game.random.Next(2);
            colorVariation = Game.random.Next(3);
            positionVariation = Game.random.Next(2);
            intRectVariation = Game.random.Next(4); 
        }
    }

}
