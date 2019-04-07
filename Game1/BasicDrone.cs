using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Schlosskirsch
{
    public sealed class BasicDrone : Enemy
    {
        private const string NAME = "BasicDrone";
        private const float SPEED = 5.0f;
        private readonly float MAX_PLAYER_DISTANCE = 500.0f;
        private Point previousPosition;
        private const int attackSpeed = 250;
        private int attackTime = attackSpeed;

        public bool IsDestroyed { get; private set; }

        public BasicDrone(Rectangle boundingRectangle, Texture2D texture, Point Position) : base(NAME, boundingRectangle, texture, Position)
        {
            this.IsDestroyed = false;
            this.previousPosition = position;
        }

        public override bool CheckCollision(GameObject collider, GameTime gameTime)
        {
            if (this.IsDestroyed)
                return false;

            if (base.CheckCollision(collider, gameTime))
            {
                OnCollision(collider, gameTime);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rectangle = new Rectangle(Position.X, Position.Y, 64, 64);
            spriteBatch.Draw(this.texture, rectangle, Color.White);
        }

        

        public override void Update(Player player, GameObject objective, List<GameObject> gameObjects, GameTime gameTime)
        {
            Vector2 directionToPlayer = (player.Position -this.position ).ToVector2();
            Vector2 directionToObjective = (objective.Position - this.position).ToVector2();
            Vector2 direction;
            if (directionToPlayer.Length() < MAX_PLAYER_DISTANCE)
            {
                direction = directionToPlayer;
            }
            else
            {
                direction = directionToObjective;
            }
            direction.Normalize();
            direction = direction * SPEED;

            if (direction.Length() > 0)
            {
                this.previousPosition = new Point(position.X, position.Y);

                Position = new Point(Position.X + (int)direction.X, Position.Y);

                HandleCollisionsX(gameObjects, gameTime);
                    
                Position = new Point(Position.X, Position.Y + (int)direction.Y);
                HandleCollisionsY(gameObjects, gameTime);

                


                this.position.X = MathHelper.Clamp(this.position.X, 0, Game1.ScreenWidth - 64);
                this.position.Y = MathHelper.Clamp(this.position.Y, 0, Game1.ScreenHeight - 64);
            }
            
        }

        private void OnCollision(GameObject collider, GameTime gameTime)
        {
            if(collider is Bullet)
            {
                this.IsDestroyed = true;
            }
            else if(collider is Player)
            {
                attackTime += gameTime.ElapsedGameTime.Milliseconds;
                if (attackTime >= attackSpeed)
                {
                    attackTime = 0;
                    Player player = collider as Player;
                    player.dealdamage(1);
                }
                
            }
            else if (collider is TheTree)
            {
                attackTime += gameTime.ElapsedGameTime.Milliseconds;
                if (attackTime >= attackSpeed)
                {
                    attackTime = 0;
                    TheTree tree = collider as TheTree;
                    tree.DealDamage(1);
                }
            }
        }

        private bool HandleCollisionsX(List<GameObject> colliders, GameTime gameTime)
        {
            foreach (GameObject collider in colliders)
            {
                if (CheckCollision(collider, gameTime))
                {
                    OnCollision(collider, gameTime);
                    Rectangle bounds = collider.GetBoundingBox();
                    if (previousPosition.X < bounds.X)
                    {
                        Position = new Point(bounds.X - this.boundingBox.Width, previousPosition.Y);
                    }
                    else
                    {
                        Position = new Point(bounds.Right, previousPosition.Y);
                    }
                    
                    return true;
                }

            }
            return false;
        }

        private bool HandleCollisionsY(List<GameObject> colliders, GameTime gameTime)
        {
            foreach (GameObject collider in colliders)
            {
                if (CheckCollision(collider, gameTime))
                {
                    OnCollision(collider, gameTime);
                    Rectangle bounds = collider.GetBoundingBox();
                    if (previousPosition.Y < bounds.Y)
                    {
                        Position = new Point(previousPosition.X, bounds.Y - this.boundingBox.Height);
                    }
                    else
                    {
                        Position = new Point(previousPosition.X, bounds.Bottom);
                    }
                    
                    return true;
                }

            }
            return false;
        }
    }
}
