using System;
using System.Collections.Generic;
using System.Linq;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Schlosskirsch;
using Microsoft.Xna.Framework.Content;

namespace SuperG
{
    public enum faceDirection
    {
        up,
        down,
        right,
        left
    }

    public class Player : ICollider
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
        private Vector2 prevPos = new Vector2();
        private bool attackstate = false;
        private bool attackhandeled = false;
        private faceDirection faceing = 0;
        private Texture2D playerTexture;
        private Dictionary<faceDirection, string> directionNames = new Dictionary<faceDirection, string>();
        float currentMovementSpeed = 0.0f;
        float deltaMovement = 0.1f;
        private int health = 100;
        private Vector2 v2Position;
        private Vector2 v2Center;
        private float rotation;
        private const float MOVEMENT_SPEED = 10.0f;

        public Vector2 Center { get { return this.v2Center; } }

        public bool dealdamage(int val)
        {
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

        public Vector2 Position
        {
            get { return v2Position; }
            private set
            {
                v2Position = value;
            }
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

        public faceDirection facedirection()
        {
            return faceing;
        }

        public void setToPrevpos()
        {
            this.Position = prevPos;
        }

        public void loadPlayerContent(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>("Custom Content/smiley_sprite");

            directionNames.Add(faceDirection.up, "North");
            directionNames.Add(faceDirection.down, "South");
            directionNames.Add(faceDirection.left, "West");
            directionNames.Add(faceDirection.right, "East");
        }

        public Player(Texture2D texture, int Width, int Height)   
        {
            DrawOffset = Vector2.Zero;
            iWidth = Width;
            iHeight = Height;
            v2Position = new Vector2(640.0f, 500.0f);
            boundingBox = new Rectangle(Width / 4, Height / 2, Width / 2, Height / 2);
            v2Center = new Vector2(Width / 2, Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset, Camera cam)
        {
            Rectangle rectangle = new Rectangle((int)v2Position.X, (int)v2Position.Y, 64, 64);
            spriteBatch.Draw(playerTexture, null, rectangle , null, v2Center,rotation, null, Color.White, SpriteEffects.None, 0.0f);
        }

        public void Update(GameTime time)
        {

        }

        public void move(InputState inputState, KeyboardState input, Camera cam, List<ICollider> colliders)
        {
            PlayerIndex index;

            moveDir = Vector2.Zero;
            string animation = "";
            if (input.IsKeyDown(Keys.A) && !attackstate)
            {
                attackhandeled = false;

                //keep move direction to know where you are hitting
                attackstate = true;
                Console.WriteLine("Attack");

                switch (faceing)
                {
                    case faceDirection.up: animation = "StrikeNorth";
                        break;

                    case faceDirection.down: animation = "StrikeSouth";
                        break;

                    case faceDirection.right: animation = "StrikeEast";
                        break;

                    case faceDirection.left: animation = "StrikeWest";
                        break;
                }
               
            }
            else
            {
                if (input.IsKeyUp(Keys.A))
                {
                    attackstate = false;
                }
                

                if (input.IsKeyDown(Keys.Left))
                {
                    faceing = faceDirection.left;
                    moveDir.X = -1;
                    animation = "WalkWest";
                   
                }
                
                if (input.IsKeyDown(Keys.Right))
                {
                    faceing = faceDirection.right;
                    moveDir.X = 1;
                    animation = "WalkEast";
                    
                }
               
                if (input.IsKeyDown(Keys.Up))
                {
                    faceing = faceDirection.up;
                    moveDir.Y = -1;
                    animation = "WalkNorth";
                   
                }
                
                if (input.IsKeyDown(Keys.Down))
                {
                    faceing = faceDirection.down;
                    moveDir.Y = 1;
                    animation = "WalkSouth";
                    
                }
               
            }
            currentMovementSpeed = MathHelper.Lerp(0.0f, MOVEMENT_SPEED, deltaMovement);
            moveDir.Normalize();
            moveDir = moveDir * currentMovementSpeed;

            if (moveDir.Length() > 0)
            {
                prevPos = new Vector2(v2Position.X, v2Position.Y);
                v2Position += moveDir;
                if(deltaMovement <= 1.0f)
                    deltaMovement += 0.1f;
            }
            else
            {
                animation = "Idle" + directionNames[faceing];
                currentMovementSpeed = 0.0f;
                deltaMovement = 0.1f;
            }
            v2Position.X = MathHelper.Clamp(v2Position.X, 0, cam.mapWidthInpx - iWidth); //keep the player within the world bounds by clamping the position
            v2Position.Y = MathHelper.Clamp(v2Position.Y, 0, cam.mapHeightInpx - iHeight);

            foreach (ICollider collider in colliders)
            {
                if (CheckCollision(collider))
                {
                    v2Position = prevPos;
                }
                    
            }
        }

        /// <summary>
        /// Returns a bounding Rectangle of the player for collission detection
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBoundingBox()
        {
            boundingBox.X = (int)v2Position.X + iWidth / 4;
            boundingBox.Y = (int)v2Position.Y + iHeight / 2;
            return boundingBox;
        }

        public int getfraction()
        {
            return 1;
        }

        public bool CheckCollision(ICollider collider)
        {
            return this.GetBoundingBox().Intersects(collider.GetBoundingBox());
        }

        public ColliderType GetColliderType()
        {
            return ColliderType.player;
        }
    }
}