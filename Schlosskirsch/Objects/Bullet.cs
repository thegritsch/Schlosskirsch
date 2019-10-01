using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects
{
    public abstract class Bullet : GameObject
    {
        private Vector2 direction = Vector2.Zero;

        protected virtual float Speed { get; }

        protected Bullet(string name, Texture2D texture, Point size, float speed)
            : base(name, texture, Point.Zero, size)
        {
            this.Speed = speed;
        }

        public void Update(GameTime gameTime)
        {
            this.Move((this.direction * this.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds).ToPoint());
        }

        public void Fire(Point position, Vector2 direction)
        {
            this.Relocate(position - this.Origin.ToPoint());

            direction.Normalize();

            this.direction = direction;
        }
    }
}
