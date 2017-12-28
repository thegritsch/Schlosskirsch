using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SuperG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class TheTree : ICollider
    {
        private int width;
        private int height;
        private Point position;
        private Texture2D treeTexture;
        private const int xOffset = 25;
        private const int yOffset = 30;

        public TheTree(int width, int height, Point position)
        {
            this.width = width;
            this.height = height;
            this.position = position;
        }

        public void LoadContent(ContentManager content)
        {
            treeTexture = content.Load<Texture2D>("Custom Content/TheTree");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(treeTexture, position.ToVector2(), Color.White);
        }

        public void Update()
        {

        }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle(position.X + xOffset, position.Y + yOffset, width, height);
        }

        public bool CheckCollision(ICollider collider)
        {
            return this.GetBoundingBox().Intersects(collider.GetBoundingBox());
        }
    }
}
