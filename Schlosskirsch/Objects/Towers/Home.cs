using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects.Guards
{
    public sealed class Home : Tower
    {
        private const string NAME = "Home";

        private const int WIDTH = 128;
        private const int HEIGHT = 128;

        private const int MAX_HEALTH = 100;
        private const int HIT_SPEED = 200;

        protected override int HitSpeed => HIT_SPEED;

        public Home(Texture2D texture, Point location) 
            : base(NAME, texture, location, new Point(WIDTH, HEIGHT), MAX_HEALTH)
        {
            
        }
    }
}
