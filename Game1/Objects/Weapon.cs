using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch.Objects
{
    public abstract class Weapon
    {
        protected int ScreenWidth { get; } = Game1.ScreenWidth;
        protected int ScreenHeight { get; } = Game1.ScreenHeight;

        public ushort MagazineSize { get; }
        public abstract ushort MagazineClip { get; }

        public float FireRate { get; }

        internal Weapon(ushort magazineSize, float fireRate)
        {
            this.MagazineSize = magazineSize;

            this.FireRate = 1 / fireRate;
        }

        public abstract void Reload();

        public abstract bool Fire(Point position, Vector2 direction);

        public abstract void Update(GameTime gameTime);

        public abstract void Check(Enemy enemy, GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public abstract class Weapon<TBullet> : Weapon where TBullet : Bullet
    {
        private readonly Texture2D bulletTexture;

        private readonly List<TBullet> magazine = new List<TBullet>();
        private readonly List<TBullet> shooted = new List<TBullet>();

        private double shootTime;

        public sealed override ushort MagazineClip => (ushort)this.magazine.Count;

        protected Weapon(Texture2D bulletTexture, ushort magazineSize, float fireRate)
            : base(magazineSize, fireRate)
        {
            if (bulletTexture == null) throw new ArgumentNullException(nameof(bulletTexture));

            this.bulletTexture = bulletTexture;

            this.shootTime = this.FireRate;

            this.Reload();
        }

        protected abstract TBullet Create(Texture2D bulletTexture);

        public override void Reload()
        {
            for (int count = this.MagazineClip; count < this.MagazineSize; count++)
            {
                this.magazine.Add(this.Create(this.bulletTexture));
            }
        }

        public override bool Fire(Point position, Vector2 direction)
        {
            if (this.shootTime < this.FireRate) return false;

            TBullet bullet = this.magazine.FirstOrDefault();
            if (bullet == null) return false;

            this.shootTime -= this.FireRate;

            bullet.Fire(position, direction);

            this.magazine.Remove(bullet);
            this.shooted.Add(bullet);

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.shootTime < this.FireRate)
            {
                this.shootTime += gameTime.ElapsedGameTime.TotalSeconds;
            }

            for (int index = 0; index < this.shooted.Count; index++)
            {
                TBullet bullet = this.shooted[index];

                if (bullet.Location.X >= this.ScreenWidth || bullet.Location.X <= -bullet.Size.X ||
                    bullet.Location.Y >= this.ScreenHeight || bullet.Location.Y <= -bullet.Size.Y)
                {
                    this.shooted.Remove(bullet);
                }
                else
                {
                    bullet.Update(gameTime);
                }
            }
        }

        public override void Check(Enemy enemy, GameTime gameTime)
        {
            for (int index = 0; index < this.shooted.Count; index++)
            {
                TBullet bullet = this.shooted[index];

                if (enemy.Check(bullet, gameTime))
                {
                    this.shooted.Remove(bullet);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (TBullet bullet in this.shooted)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
