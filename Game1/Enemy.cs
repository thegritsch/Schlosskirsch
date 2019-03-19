using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Schlosskirsch
{
    public class Enemy : GameObject
    {
        private Rectangle boundingBox;
        private int width;
        private int height;

        public Enemy(string Name, int Width, int Height, Point Position) : base(Name, Position)
        {
            this.width = Width;
            this.height = Height;
            boundingBox = new Rectangle(Position.X, Position.Y, width, height);
        }

        public override bool CheckCollision(GameObject collider)
        {
            return collider.GetBoundingBox().Intersects(this.boundingBox);
        }

        public override Rectangle GetBoundingBox()
        {
            return this.boundingBox;
        }

        public virtual void Update(Player player)
        {
            //
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
