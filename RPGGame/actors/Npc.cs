using SFML.Graphics;

class Npc : Actor
{
    public Npc(int x, int y)
    {
        this.x = x;
        this.y = y;
        spritex = 1;
        spritey = 0;
        spriteColor = new Color(255, 204, 156, 255);
    }
}
