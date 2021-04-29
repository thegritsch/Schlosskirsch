using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects.Enemies
{
    public sealed class RegionalDrone : Enemy
    {
        private const string NAME = "RegionalDrone";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const float SPEED = 0.2F;

        private const int ATTACK_SPEED = 250;
        private const int ATTACK_DAMAGE = 1;

        private static Texture2D texture;

        protected override int AttackSpeed => ATTACK_SPEED;
        protected override int AttackDamage => ATTACK_DAMAGE;

        public RegionalDrone(Point location) 
            : base(NAME, RegionalDrone.texture, location, new Point(WIDTH, HEIGHT), SPEED)
        {
            
        }

        public static void LoadTexture(Texture2D texture)
        {
            RegionalDrone.texture = texture;
        }
    }
}
