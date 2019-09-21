using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects
{
    /// <summary>
    /// Base enemy class
    /// </summary>
    public abstract class Enemy : GameObject
    {
        private double attackTime = 0;

        public bool IsAttacking { get; private set; }
        public bool IsDestroyed { get; private set; }

        protected virtual float Speed { get; }

        protected abstract int AttackSpeed { get; }
        protected abstract int AttackDamage { get; }

        protected Enemy(string name, Texture2D texture, Point location, Point size, float speed) 
            : base(name, texture, location, size)
        {
            this.Speed = speed;

            this.attackTime = this.AttackSpeed;
        }

        private void dealDamage(HealthObject healthObject, GameTime gameTime)
        {
            this.attackTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.attackTime >= this.AttackSpeed)
            {
                this.attackTime = 0;
                healthObject.Damage(this.AttackDamage);
            }
        }

        private Vector2 moveDirection(IEnumerable<HealthObject> healthObjects)
        {
            float minimumDistance = float.MaxValue;
            Vector2 minimumDirection = Vector2.Zero;

            foreach (HealthObject healtObject in healthObjects)
            {
                Vector2 direction = (healtObject.Center - this.Center).ToVector2();

                if (direction.Length() < minimumDistance)
                {
                    minimumDistance = direction.Length();
                    minimumDirection = direction;
                }
            }

            return minimumDirection;
        }

        private Point offset(Rectangle guard)
        {
            Rectangle box = this.GetBox();

            int x = this.offsetX(box, guard);
            int y = this.offsetY(box, guard);

            return Math.Abs(x) < Math.Abs(y) ? new Point(x, 0) : new Point(0, y);
        }
        private int offsetX(Rectangle box, Rectangle guard)
        {
            int offsetLeft = guard.Left - box.Right;
            int offsetRight = guard.Right - box.Left;

            return Math.Abs(offsetLeft) < Math.Abs(offsetRight) ? offsetLeft : offsetRight;
        }
        private int offsetY(Rectangle box, Rectangle guard)
        {
            int offsetTop = guard.Top - box.Bottom;
            int offsetBottom = guard.Bottom - box.Top;

            return Math.Abs(offsetTop) < Math.Abs(offsetBottom) ? offsetTop : offsetBottom;
        }

        private void updateLocation(List<GameObject> gameObjects, GameTime gameTime)
        {
            Vector2 direction = this.moveDirection(gameObjects.OfType<HealthObject>());

            if (direction.Length() > 0)
            {
                direction.Normalize();

                this.Move((direction * this.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds).ToPoint());
            }
        }
        private void updateCollisions(List<GameObject> colliders, GameTime gameTime)
        {
            this.IsAttacking = false;

            foreach (GameObject collider in colliders)
            {
                if (this.CheckCollision(collider, gameTime))
                {
                    this.OnCollision(collider, gameTime);
                }
            }
        }

        public virtual bool Check(Bullet bullet, GameTime gameTime)
        {
            if (this.CheckCollision(bullet, gameTime))
            {
                this.IsDestroyed = true;

                return true;
            }

            return false;
        }

        protected override bool CheckCollision(GameObject collider, GameTime gameTime)
        {
            if (this.IsDestroyed) return false;

            return base.CheckCollision(collider, gameTime);
        }

        public virtual void Update(List<GameObject> gameObjects, GameTime gameTime)
        {
            this.updateLocation(gameObjects, gameTime);

            this.updateCollisions(gameObjects, gameTime);
        }

        protected virtual void OnCollision(GameObject collider, GameTime gameTime)
        {
            switch (collider)
            {
                case HealthObject healthObject:
                    this.IsAttacking = true;

                    this.Move(this.offset(healthObject.GetBox()));
                    this.dealDamage(healthObject, gameTime);
                    break;
                case Enemy enemy:
                    if (!this.IsAttacking)
                    {
                        this.Move(this.offset(enemy.GetBox()));
                    }
                    break;
            }
        }
    }
}
