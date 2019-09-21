using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Schlosskirsch.Objects.Bullets;

namespace Schlosskirsch.Objects.Weapons
{
    public sealed class Gun : Weapon<Simple>
    {
        private const ushort MAGAZINE_SIZE = 10;

        private const float FIRE_RATE = 5.0F;

        public Gun(Texture2D bulletTexture) 
            : base(bulletTexture, MAGAZINE_SIZE, FIRE_RATE)
        {
            
        }

        protected override Simple Create(Texture2D bulletTexture)
        {
            return new Simple(bulletTexture);
        }

        public override bool Fire(Point position, Vector2 direction)
        {
            if (this.MagazineClip == 0) this.Reload();

            return base.Fire(position, direction);
        }
    }
}
