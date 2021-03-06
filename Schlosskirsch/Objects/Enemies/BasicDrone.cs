﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch.Objects.Enemies
{
    public sealed class BasicDrone : Enemy
    {
        private const string NAME = "BasicDrone";

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const float SPEED = 0.4F;

        private const int ATTACK_SPEED = 250;
        private const int ATTACK_DAMAGE = 1;

        private static Texture2D texture;

        protected override int AttackSpeed => ATTACK_SPEED;
        protected override int AttackDamage => ATTACK_DAMAGE;

        public BasicDrone(Point location) 
            : base(NAME, BasicDrone.texture, location, new Point(WIDTH, HEIGHT), SPEED)
        {
            
        }

        public static void LoadTexture(Texture2D texture)
        {
            BasicDrone.texture = texture;
        }
    }
}
