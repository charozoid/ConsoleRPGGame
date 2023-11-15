using SFML.Graphics;
using SFML.System;

class Actor
{
    int hp = 0;
    int strength = 0;
    int intelligence = 0;
    int dexterity = 0;
    public Color spriteColor = Color.White;
    public int x = 0;
    public int y = 0;
    public int drawx = 0;
    public int drawy = 0;

    public IntRect intRect = new IntRect();
    public int spritex = 0;
    public int spritey = 0;

    public Actor(int posx, int posy, int spritex, int spritey, Color color)
    {
        x = posx;
        y = posy;
        spriteColor = color;
        intRect = Graphics2D.GridToIntRect(spritex, spritey);
    }
    void Draw()
    {

    }
    public Vector2f GetPos()
    {
        return new Vector2f(x, y);
    }
    public void Move(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
