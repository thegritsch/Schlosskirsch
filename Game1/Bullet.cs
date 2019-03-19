using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch
{
    public class Bullet : GameObject
    {
        private bool isDestroyed;
        
        private int width;
        private int height;
        private Rectangle destRectangle;
        private Vector2 direction;
        private static Texture2D bulletTexture;

        public static Texture2D GetBulletTexture
        {
            get { return bulletTexture; }
        }

        

        public Vector2 SetDirection
        {
            set { direction = value; }
        }

        public bool IsDestroyed
        {
            get { return isDestroyed; }
            set { isDestroyed = value; }
        }

        public int GetWidth
        {
            get { return width; }
        }

        public int GetHeight
        {
            get { return height; }
        }

        public Bullet(Texture2D bulletTexture, Point position, int height, int width):base("Bullet", position)
        {
            if (Bullet.bulletTexture == null)
                Bullet.bulletTexture = bulletTexture;
            this.position = position;
            this.height = height;
            this.width = width;
            destRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
            isDestroyed = true;
        }

        public void Move( float Speed)
        {
            position += (direction * Speed).ToPoint();
            destRectangle.X = position.X;
            destRectangle.Y = position.Y;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Bullet.bulletTexture, destRectangle, Color.White);
        }

        public override bool CheckCollision(GameObject collider)
        {
            return GetBoundingBox().Intersects(collider.GetBoundingBox());
        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        
    }
}
