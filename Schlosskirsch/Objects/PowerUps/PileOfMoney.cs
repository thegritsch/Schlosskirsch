using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Media;

namespace Schlosskirsch.Objects.PowerUps
{
    /// <summary>
    /// Represents a <see cref="PowerUp"/> that does nothing to any colliding <see cref="HealthObject"/> because it's not there.
    /// </summary>
    public sealed class PileOfMoney : PowerUp
    {
        private const string NAME = "PileOfMoney";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private static Texture2D texture;

        public PileOfMoney(Point location)
            : base(NAME, PileOfMoney.texture, location, new Point(WIDTH, HEIGHT))
        {
        }

        public static void LoadTexture(Texture2D texture)
        {
            PileOfMoney.texture = texture;
        }

        protected override void OnCollision(GameObject collider, GameTime gameTime)
        {
            base.OnCollision(collider, gameTime);
        }
    }
}
