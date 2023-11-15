using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Decoration
{
    public int x = 0;
    public int y = 0;

    public IntRect intRect = new IntRect();
    public Color color = Color.White;

    public Decoration(IntRect intRect, Color color)
    {
        this.intRect = intRect;
        this.color = color;
    }
}