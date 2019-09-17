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
        private readonly int maxHealth;

        public bool UnderAttack { get; private set; } = false;

        public int Health { get; private set; }

        protected abstract int HitSpeed { get; }
        public int HitTime { get; private set; }

        public override Color Color => this.UnderAttack ? Color.Red : base.Color;

        protected HealthObject(string name, Texture2D texture, Point location, Point size, int maxHealth)
            : base(name, texture, location, size)
        {
            this.maxHealth = maxHealth;

            this.Health = this.maxHealth;
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
            this.Health -= damage;

            return this.Health <= 0;
        }

        public virtual void Update(GameTime gameTime, List<GameObject> colliders)
        {
            this.updateHitTime(gameTime);

            this.updateCollisions(colliders, gameTime);
        }

        protected virtual void OnCollision(GameObject collider, GameTime gameTime)
        {

        }
    }
}
