using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch.Objects
{
    public abstract class HealthObject : GameObject
    {
        private readonly Texture2D healthBarBackground;
        private readonly Texture2D healthBarForeground;

        private readonly uint maxHealth;

        public bool UnderAttack { get; private set; } = false;

        public int Health { get; private set; }

        protected abstract int HitSpeed { get; }
        public int HitTime { get; private set; }

        public override Color Color => this.UnderAttack ? Color.Red : base.Color;

        protected HealthObject(string name, Texture2D texture, Point location, Point size, uint maxHealth)
            : base(name, texture, location, size)
        {
            this.maxHealth = maxHealth;

            this.healthBarBackground = new Texture2D(texture.GraphicsDevice, 1, 1);
            this.healthBarBackground.SetData(new Color[] { Color.White });
            this.healthBarForeground = new Texture2D(texture.GraphicsDevice, 1, 1);
            this.healthBarForeground.SetData(new Color[] { Color.White });

            this.Health = (int)this.maxHealth;
            this.HitTime = this.HitSpeed;
        }

        private void updateHitTime(GameTime gameTime)
        {
            if (this.UnderAttack)
            {
                this.HitTime -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (this.HitTime <= 0)
                {
                    this.UnderAttack = false;
                    this.HitTime = this.HitSpeed;
                }
            }
        }

        private void updateCollisions(List<GameObject> colliders, GameTime gameTime)
        {
            foreach (GameObject collider in colliders)
            {
                if (this.CheckCollision(collider, gameTime))
                {
                    this.OnCollision(collider, gameTime);
                }
            }
        }

        public virtual bool Damage(int damage)
        {
            this.UnderAttack = true;

            if (!MainGame.GoodMode)
            {
                this.Health -= damage;
            }

            return this.Health <= 0;
        }

        public virtual bool Heal(int amount)
        {
            var newHealth = this.Health + amount;
            if (newHealth > maxHealth)
            {
                this.Health = (int)maxHealth;
            }
            else
            {
                this.Health = newHealth;
            }

            return true;
        }

        public virtual void Update(GameTime gameTime, List<GameObject> colliders)
        {
            this.updateHitTime(gameTime);

            this.updateCollisions(colliders, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            this.DrawHealtBar(spriteBatch);
        }
        protected void DrawHealtBar(SpriteBatch spriteBatch)
        {
            Rectangle healthBarBackgroundDestination = new Rectangle(this.Location.X, this.Location.Y - 15, this.Size.X, 10);
            spriteBatch.Draw(this.healthBarBackground, healthBarBackgroundDestination, Color.Black);

            float size = ((this.Size.X - 2.0F) * this.Health) / this.maxHealth;
            Rectangle healthBarForegroundDestination = new Rectangle(this.Location.X + 1, this.Location.Y - 14, (int)Math.Round(size), 8);
            spriteBatch.Draw(this.healthBarForeground, healthBarForegroundDestination, Color.Red);
        }

        protected virtual void OnCollision(GameObject collider, GameTime gameTime)
        {

        }
    }
}
