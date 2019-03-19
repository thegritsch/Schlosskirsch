using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch
{
    public class TheTree : GameObject
    {
        private int width;
        private int height;
        
        private Texture2D treeTexture;
        private const int xOffset = 50;
        private const int yOffset = 60;
        private Rectangle destinationRectangle;
        private const int marginX = 40;
        private const int marginY = 30;

        public TheTree(int width, int height, Point position) : base("TheTree", position)
        {
            this.width = width;
            this.height = height;
            this.position = position;
            this.destinationRectangle = new Rectangle(position.X, position.Y, width, height);
        }

        public void LoadContent(ContentManager content)
        {
            treeTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "TheTree"));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(treeTexture, destinationRectangle, Color.White);
        }

        public void Update()
        {

        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle(position.X + xOffset, position.Y + yOffset, width - marginX, height - marginY);
        }

        public override bool CheckCollision(GameObject collider)
        {
            return this.GetBoundingBox().Intersects(collider.GetBoundingBox());
        }

        
    }
}
