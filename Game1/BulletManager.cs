using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch
{
    public class BulletManager<B> where B : Bullet
    {
        private List<B> activeBullets;
        private List<B> destroyedBullets;
        private int screenHeight, screenWidth;

        public BulletManager(int screenWidth, int screenHeight)
        {
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
            this.activeBullets = new List<B>();
        }

        public B GetBullet()
        {
            return this.destroyedBullets.First();
        }

        public void AddBullets(B[] bullets)
        {
            destroyedBullets = new List<B>(bullets);
            
        }

        public void Update()
        {
            for (int i = 0; i < activeBullets.Count; i++)
            {
                Bullet b = activeBullets[i];
                b.Move(20.0f);
                if (b.Position.X >= this.screenWidth || b.Position.X <= -b.GetWidth)
                {
                    activeBullets.Remove(b as B);
                    destroyedBullets.Add(b as B);
                    
                }
                else if (b.Position.Y >= this.screenHeight || b.Position.Y <= -b.GetHeight)
                {
                    activeBullets.Remove(b as B);
                    destroyedBullets.Add(b as B);
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            foreach(Bullet b in activeBullets)
            {
                b.Draw(batch);
            }
        }

        public void CheckCollision(GameObject collider, GameTime gameTime)
        {
            foreach (Bullet b in activeBullets)
            {
                collider.CheckCollision(b, gameTime);
            }
        }

        public void Fire(Point position, Vector2 direction)
        {
            if(destroyedBullets.Any())
            {
                B bullet = destroyedBullets.First();
                bullet.Position = new Point(position.X - bullet.GetWidth / 2, position.Y - bullet.GetHeight / 2);
                bullet.IsDestroyed = false;


                direction.Normalize();

                bullet.SetDirection = direction;

                destroyedBullets.Remove(bullet);
                activeBullets.Add(bullet);
            }
        }
    }
}
