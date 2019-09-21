using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects
{
    /// <summary>
    /// abstract class for game objects. All other objects should inherit from this
    /// </summary>
    public abstract class GameObject
    {
        private Rectangle box;

        public string Name { get; }

        protected int ScreenWidth { get; } = MainGame.ScreenWidth;
        protected int ScreenHeight { get; } = MainGame.ScreenHeight;

        protected Texture2D Texture { get; }

        public Point Location => this.box.Location;
        public Point Center => this.box.Center;

        public float Rotation { get; set; } = 0.0F;

        public Point Size => this.box.Size;
        public Vector2 Origin => this.box.Size.ToVector2() / 2.0F;

        public virtual Color Color => Color.White;

        protected GameObject(string name, Texture2D texture, Point location, Point size)
        {
            this.Name = name;

            this.Texture = texture;

            this.box = new Rectangle(location, size);
        }

        protected void ClampToScreen()
        {
            int x = MathHelper.Clamp(this.Location.X, 0, this.ScreenWidth - this.Size.X);
            int y = MathHelper.Clamp(this.Location.Y, 0, this.ScreenHeight - this.Size.Y);

            this.Relocate(x, y);
        }

        protected virtual bool CheckCollision(GameObject collider, GameTime gameTime)
        {
            if (!this.Equals(collider))
            {
                return this.box.Intersects(collider.GetBox());
            }

            return false;
        }

        public virtual Rectangle GetBox()
        {
            return this.box;
        }

        public void Move(int x, int y)
        {
            this.Move(new Point(x, y));
        }
        public virtual void Move(Point offset)
        {
            this.box.Offset(offset);
        }

        public void Relocate(int x, int y)
        {
            this.Relocate(new Point(x, y));
        }
        public virtual void Relocate(Point location)
        {
            this.box.Location = location;
        }
        public void Resize(int width, int height)
        {
            this.Resize(new Point(width, height));
        }
        public virtual void Resize(Point size)
        {
            this.box.Size = size;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, this.box, null, this.Color);
        }
    }
}
