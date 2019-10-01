using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace Schlosskirsch.Objects
{
    public abstract class Tower : HealthObject
    {
        protected Tower(string name, Texture2D texture, Point location, Point size, uint health)
            : base(name, texture, location, size, health)
        {
            
        }
    }
}
