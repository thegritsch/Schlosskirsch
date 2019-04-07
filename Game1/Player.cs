using System;
using System.Collections.Generic;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Schlosskirsch;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Schlosskirsch
{
    public enum FaceDirection
    {
        up,
        down,
        right,
        left
    }

    public class Player : GameObject
    {
        public PlayerIndex? controllingPlayer;

        private float range = 80;

        public float getRange()
        {
            return range;
        }

        public Vector2 DrawOffset;
        private int iWidth;
        private int iHeight;
        private Vector2 moveDir = Vector2.Zero;
        public List<GameObject> Inventory = new List<GameObject>();
        private Rectangle boundingBox;
        private Point prevPos = new Point();
        private bool attackstate = false;
        private bool attackhandeled = false;
        private FaceDirection faceing = 0;
        private Texture2D playerTexture;
        private Dictionary<FaceDirection, string> directionNames = new Dictionary<FaceDirection, string>();
        float currentMovementSpeed = 0.0f;
        float deltaMovement = 0.1f;
        private int health = 100;
        private bool underAttack = false;
        private const int HIT_TIME =200;
        private int timeToHit = HIT_TIME;
        private Vector2 v2Center;
        private float rotation;
        private const float MOVEMENT_SPEED = 10.0f;

        public Vector2 Center { get { return this.v2Center; } }

        public bool dealdamage(int val)
        {
            underAttack = true;
            health -= val;
            if (health <= 0)
            {
               
                return true;
            }
            return false;
        }

        public int getHealth
        {
            get { return health; }
        }

        

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public bool isAttacking()
        {
            return attackstate;
        }

        public void setAttackhandeled()
        {
            attackhandeled = true;
        }

        public bool attackIsHandeled()
        {
            return attackhandeled;
        }

        public FaceDirection facedirection()
        {
            return faceing;
        }

        public void setToPrevpos()
        {
            this.Position = prevPos;
        }

        public void loadPlayerContent(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER,"smiley_sprite"));

            directionNames.Add(FaceDirection.up, "North");
            directionNames.Add(FaceDirection.down, "South");
            directionNames.Add(FaceDirection.left, "West");
            directionNames.Add(FaceDirection.right, "East");
        }

        public Player(Texture2D texture, int Width, int Height) :base("Player", new Point(640, 500))  
        {
            DrawOffset = Vector2.Zero;
            iWidth = Width;
            iHeight = Height;
            
            boundingBox = new Rectangle(position.X, position.Y, Width, Height);
            v2Center = new Vector2(Width / 2, Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset, Camera cam)
        {
            Rectangle rectangle = new Rectangle(Position.X+ (int)v2Center.X, Position.Y + (int)v2Center.Y, 64, 64);
            Color color;
            if(underAttack)
            {
                color = new Color(1.0f, 0.0f, 0.0f);
            }
            else
            {
                color = new Color(1.0f, 1.0f, 1.0f);
            }
            spriteBatch.Draw(playerTexture, rectangle, null, color, this.rotation, v2Center, SpriteEffects.None, 0.0f);
        }

        public void Update(GameTime gameTime, List<GameObject> colliders)
        {
            if (moveDir.Length() > 0)
            {
                prevPos = new Point(Position.X, Position.Y);
                Position = new Point(Position.X + (int)moveDir.X, Position.Y);

                HandleCollisionsX(colliders, gameTime);

                Position = new Point(Position.X, Position.Y + (int)moveDir.Y);
                HandleCollisionsY(colliders, gameTime);

                if (deltaMovement <= 1.0f)
                    deltaMovement += 0.1f;
            }
            else
            {
                HandleCollisionsX(colliders, gameTime);
                HandleCollisionsY(colliders,gameTime);
                currentMovementSpeed = 0.0f;
                deltaMovement = 0.1f;
            }
            position.X = MathHelper.Clamp(Position.X, 0, Game1.ScreenWidth - iWidth); //keep the player within the world bounds by clamping the position
            position.Y = MathHelper.Clamp(Position.Y, 0, Game1.ScreenHeight - iHeight);
            if (underAttack)
            {
                timeToHit -= gameTime.ElapsedGameTime.Milliseconds;
                if(timeToHit <= 0)
                {
                    underAttack = false;
                    timeToHit = HIT_TIME;
                }
            }
        }

        public void Move(InputState inputState, KeyboardState input, Camera cam)
        {
            PlayerIndex index;

            moveDir = Vector2.Zero;
            string animation = "";
            if (input.IsKeyDown(Keys.R) && !attackstate)
            {
                attackhandeled = false;

                //keep move direction to know where you are hitting
                attackstate = true;
                Console.WriteLine("Attack");

                switch (faceing)
                {
                    case FaceDirection.up:
                        animation = "StrikeNorth";
                        break;

                    case FaceDirection.down:
                        animation = "StrikeSouth";
                        break;

                    case FaceDirection.right:
                        animation = "StrikeEast";
                        break;

                    case FaceDirection.left:
                        animation = "StrikeWest";
                        break;
                }

            }
            else
            {
                if (input.IsKeyUp(Keys.R))
                {
                    attackstate = false;
                }


                if (input.IsKeyDown(Keys.A))
                {
                    faceing = FaceDirection.left;
                    moveDir.X = -1;
                    animation = "WalkWest";

                }

                if (input.IsKeyDown(Keys.D))
                {
                    faceing = FaceDirection.right;
                    moveDir.X = 1;
                    animation = "WalkEast";

                }

                if (input.IsKeyDown(Keys.W))
                {
                    faceing = FaceDirection.up;
                    moveDir.Y = -1;
                    animation = "WalkNorth";

                }

                if (input.IsKeyDown(Keys.S))
                {
                    faceing = FaceDirection.down;
                    moveDir.Y = 1;
                    animation = "WalkSouth";

                }

            }
            currentMovementSpeed = MathHelper.Lerp(0.0f, MOVEMENT_SPEED, deltaMovement);
            moveDir.Normalize();
            moveDir = moveDir * currentMovementSpeed;

            
        }

        private bool HandleCollisionsX(List<GameObject> colliders, GameTime gameTime)
        {
            foreach (GameObject collider in colliders)
            {
                if (CheckCollision(collider, gameTime))
                {
                    Rectangle bounds = collider.GetBoundingBox();
                    if (prevPos.X < bounds.X)
                    {
                        Position = new Point(bounds.X - this.GetBoundingBox().Width, Position.Y);
                    }
                    else
                    {
                        Position = new Point(bounds.Right, Position.Y);
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
                    Rectangle bounds = collider.GetBoundingBox();
                    if (prevPos.Y < bounds.Y)
                    {
                        Position = new Point(Position.X,bounds.Y - this.GetBoundingBox().Height );
                    }
                    else
                    {
                        Position = new Point( Position.X, bounds.Bottom);
                    }
                    
                    return true;
                }

            }
            return false;
        }
        /// <summary>
        /// Returns a bounding Rectangle of the player for collission detection
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            boundingBox.Location = this.Position;
            return boundingBox;
        }

        public override bool CheckCollision(GameObject collider, GameTime gameTime)
        {
            if (this.Equals(collider))
                return false;
            return collider.CheckCollision(this, gameTime);
        }

        
    }
}