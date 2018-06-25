using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Schlosskirsch
{
    /// <summary>
    /// abstract class for game objects. All other objects should inherit from this
    /// 
    /// </summary>
    public abstract class GameObject
    {
        private Rectangle boundingBox;
        
        private string name;

        private Vector2 position;

        public Rectangle BoundingBox
        {
            get { return boundingBox; }
        }

        public Vector2 Position { get { return this.position; } }

        public string Name
        {
            get { return name; }
        }

        

        public GameObject(string Name, Rectangle Bounds, Vector2 Position)
        {
            this.name = Name;
            this.position = Position;
            this.boundingBox = Bounds;
        }

        
    }
}
