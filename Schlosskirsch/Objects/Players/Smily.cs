using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch.Objects.Players
{
    public sealed class Smily : Player
    {
        private const string NAME = "Smily";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const int MAX_HEALTH = 100;
        private const int HIT_SPEED = 200;

        private const float SPEED = 0.6F;

        protected override int HitSpeed => HIT_SPEED;

        public Smily(Texture2D texture, Point location, Weapon weapon, PlayerIndex index)
            : base(NAME, texture, location, new Point(WIDTH, HEIGHT), MAX_HEALTH, SPEED, weapon, index)
        {

        }
    }
}
