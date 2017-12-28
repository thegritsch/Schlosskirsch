using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    /// <summary>
    /// abstract class for game objects. All other objects should inherit from this
    /// and implement the Interact() virtual method.
    /// </summary>
    public abstract class GameObject
    {
        private Rectangle boundingBox;
        private byte type;
        private string name;

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

        public byte Type
        {
            get { return type; }
        }

        public string Name
        {
            get { return name; }
        }

        

        public GameObject(string name, byte type, Rectangle bounds)
        {
            this.name = name;
            this.type = type;
            this.boundingBox = bounds;
        }

        public virtual void Interact(Object player, Object ScreenManager)
        {
        }
    }
}
