using SFML.Graphics;

class Npc : Actor
{
    public Npc(int posx, int posy, int spritex, int spritey, Color color) : base(posx, posy, spritex, spritey, color)
    {
        x = posx;
        y = posy;
        spriteColor = new Color(255, 204, 156, 255);
    }
}
