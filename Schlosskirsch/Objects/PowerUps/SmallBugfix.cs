using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects.PowerUps
{
    /// <summary>
    /// Represents a <see cref="PowerUp"/> to <see cref="HealthObject.Heal(int)"/> any colliding <see cref="HealthObject"/> by a fixed heal amount.
    /// </summary>
    public sealed class SmallBugfix : PowerUp
    {
        private const string NAME = "SmallBugfix";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const int HEAL_AMOUNT = 10;

        private static Texture2D texture;

        public SmallBugfix(Point location)
            : base(NAME, SmallBugfix.texture, location, new Point(WIDTH, HEIGHT))
        {
        }

        public static void LoadTexture(Texture2D texture)
        {
            SmallBugfix.texture = texture;
        }

        protected override void OnCollision(GameObject collider, GameTime gameTime)
        {
            base.OnCollision(collider, gameTime);

            var healthObject = collider as HealthObject;
            if (healthObject != null)
            {
                healthObject.Heal(HEAL_AMOUNT);
            }
        }
    }
}
