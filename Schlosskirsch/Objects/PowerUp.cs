using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Schlosskirsch.Objects
{
    public abstract class PowerUp : GameObject
    {
        public bool IsConsumed { get; private set; }

        protected PowerUp(string name, Texture2D texture, Point location, Point size)
            : base(name, texture, location, size)
        {

        }

        public virtual void Update(GameTime gameTime, List<GameObject> colliders)
        {
            if (!this.IsConsumed)
            {
                this.updateCollisions(colliders, gameTime);
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

        protected virtual void OnCollision(GameObject collider, GameTime gameTime)
        {
            this.IsConsumed = collider is HealthObject;
        }
    }
}
