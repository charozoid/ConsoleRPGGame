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
    public int spritex = 0;
    public int spritey = 0;
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
