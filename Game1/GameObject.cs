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
        protected Point position;

        public Point Position {
            get
            { return position; }
            set
            { position = value; }
        }

        public string Name { get; }

        public abstract bool CheckCollision(GameObject collider, GameTime gameTime);

        public abstract Rectangle GetBoundingBox();

        public GameObject(string Name, Point Position)
        {
            this.Name = Name;
            this.Position = Position;
        }

        
    }
}
