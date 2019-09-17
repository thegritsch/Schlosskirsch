using System;
using System.Collections.Generic;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Schlosskirsch.Objects.Weapons;

namespace Schlosskirsch.Objects
{
    public abstract class Player : HealthObject
    {
        private Vector2 direction;

        protected virtual float Speed { get; }

        public Weapon Weapon { get; private set; }

        protected Player(string name, Texture2D texture, Point location, Point size, int maxHealth, float speed, Weapon weapon) 
            : base(name, texture, location, size, maxHealth)  
        {
            this.Speed = speed;

            this.Weapon = weapon;
        }

        private bool handleMouseInput(MouseState mouse)
        {
            if (mouse == null) return false;
            
            Vector2 direction = (mouse.Position - this.Center).ToVector2();
            this.Rotation = (float)Math.Atan2(direction.Y, direction.X);

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                this.Weapon.Fire(this.Center, direction);
            }

            if (mouse.RightButton == ButtonState.Pressed)
            {
                this.direction = direction;
            }

            return true;
        }

        private bool handleKeyboardInput(KeyboardState keyboard)
        {
            if (keyboard == null) return false;

            this.direction = this.handleKeyboardDirection(keyboard);

            if (keyboard.IsKeyDown(Keys.R))
            {
                this.Weapon.Fire(this.Center, direction);
            }

            return true;
        }
        private Vector2 handleKeyboardDirection(KeyboardState keyboard)
        {
            Vector2 direction = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.A) && keyboard.IsKeyUp(Keys.D))
            {
                direction.X = -1;
            }
            if (keyboard.IsKeyUp(Keys.A) && keyboard.IsKeyDown(Keys.D))
            {
                direction.X = 1;
            }
            if (keyboard.IsKeyDown(Keys.W) && keyboard.IsKeyUp(Keys.S))
            {
                direction.Y = -1;
            }
            if (keyboard.IsKeyUp(Keys.W) && keyboard.IsKeyDown(Keys.S))
            {
                direction.Y = 1;
            }

            return direction;
        }

        private void updateLocation(GameTime gameTime)
        {
            if (this.direction.Length() > 0)
            {
                this.direction.Normalize();

                this.Move((this.direction * this.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds).ToPoint());

                this.ClampToScreen();
            }
        }

        public virtual void HandleInput(InputState input, int playerIndex)
        {
            this.handleMouseInput(input.MouseState);

            this.handleKeyboardInput(input.CurrentKeyboardStates[playerIndex]);
        }

        public virtual void Change(Weapon weapon)
        {
            this.Weapon = weapon;
        }

        public override void Update(GameTime gameTime, List<GameObject> colliders)
        {
            this.updateLocation(gameTime);

            if (this.Weapon != null)
            {
                this.Weapon.Update(gameTime);

                foreach (Enemy enemy in colliders.OfType<Enemy>())
                {
                    this.Weapon.Check(enemy, gameTime);
                }
            }

            base.Update(gameTime, colliders);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, new Rectangle(this.Center, this.Size), null, this.Color, this.Rotation, this.Origin, SpriteEffects.None, 0.0f);

            if (this.Weapon != null)
            {
                this.Weapon.Draw(spriteBatch);
            }
        }
    }
}