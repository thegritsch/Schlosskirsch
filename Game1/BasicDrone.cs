using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch
{
    public sealed class BasicDrone : Enemy
    {
        private const string NAME = "BasicDrone";
        private const float SPEED = 4.0f;

        public bool IsDestroyed { get; private set; }

        public BasicDrone(Rectangle boundingRectangle, Texture2D texture, Point Position) : base(NAME, boundingRectangle, texture, Position)
        {
            this.IsDestroyed = false;
        }

        public override bool CheckCollision(GameObject collider)
        {
            if(base.CheckCollision(collider))
            {
                OnCollision(collider);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = new Rectangle(Position.X, Position.Y, 64, 64);
            spriteBatch.Draw(this.texture, rectangle, Color.White);
        }

        

        public override void Update(Player player, GameObject objective)
        {
            Vector2 direction = (player.Position -this.position ).ToVector2();
            direction.Normalize();
            Random random = new Random(player.Position.X);
            int multiX = random.Next(-1, 1);
            int multiY = random.Next(-1, 1);
            
            this.position += (direction * SPEED).ToPoint();
            

            this.position.X = MathHelper.Clamp(this.position.X + multiX, 0, Game1.ScreenWidth - 64);
            this.position.Y = MathHelper.Clamp(this.position.Y + multiY, 0, Game1.ScreenHeight - 64);

            
        }

        private void OnCollision(GameObject collider)
        {
            if(collider is Bullet)
            {
                this.IsDestroyed = true;
            }
        }
    }
}
