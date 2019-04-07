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
        private int health = 100;
        private bool underAttack = false;
        private const int HIT_TIME = 200;
        private int timeToHit = HIT_TIME;
        public int Health { get { return this.health; } }

        public TheTree(int width, int height, Point position) : base("TheTree", position)
        {
            this.width = width;
            this.height = height;
            this.position = position;
            this.destinationRectangle = new Rectangle(position.X, position.Y, width, height);
        }

        public bool DealDamage(int value)
        {
            underAttack = true;
            health -= value;
            if (health <= 0)
            {

                return true;
            }
            return false;
        }

        public void LoadContent(ContentManager content)
        {
            treeTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "cube"));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color;
            if (underAttack)
            {
                color = new Color(1.0f, 0.0f, 0.0f);
            }
            else
            {
                color = new Color(1.0f, 1.0f, 1.0f);
            }
            spriteBatch.Draw(treeTexture, destinationRectangle, color);
        }

        public void Update(GameTime gameTime)
        {
            if (underAttack)
            {
                timeToHit -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeToHit <= 0)
                {
                    underAttack = false;
                    timeToHit = HIT_TIME;
                }
            }
        }

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle(position.X, position.Y , width , height);
        }

        public override bool CheckCollision(GameObject collider, GameTime gameTime)
        {
            return collider.GetBoundingBox().Intersects(this.GetBoundingBox());
        }

        
    }
}
