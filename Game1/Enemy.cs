using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Schlosskirsch
{
    /// <summary>
    /// Base enemy class
    /// </summary>
    public class Enemy : GameObject
    {
        protected Rectangle boundingBox;
        
        protected Texture2D texture;

        public Enemy(string Name, Rectangle boundingRectangle, Texture2D texture, Point Position) : base(Name, Position)
        {
            
            boundingBox = boundingRectangle;
            this.texture = texture;
        }

        public override bool CheckCollision(GameObject collider)
        {
            return collider.GetBoundingBox().Intersects(this.GetBoundingBox());
        }

        public override Rectangle GetBoundingBox()
        {
            this.boundingBox.Location = this.position;
            
            return this.boundingBox;
        }

        public virtual void Update(Player player, GameObject objective)
        {
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
