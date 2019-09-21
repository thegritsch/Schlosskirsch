using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects.Bullets
{
    public sealed class Simple : Bullet
    {
        private const string NAME = "Simple";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const float SPEED = 1.5F;

        public Simple(Texture2D texture) 
            : base(NAME, texture, new Point(WIDTH, HEIGHT), SPEED)
        {

        }
    }
}
