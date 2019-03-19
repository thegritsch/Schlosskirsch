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
            
            boundingBox = new Rectangle(Width / 4, Height / 2, Width / 2, Height / 2);
            v2Center = new Vector2(Width / 2, Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch, int XOffset, int YOffset, Camera cam)
        {
            Rectangle rectangle = new Rectangle(Position.X, Position.Y, 64, 64);
            spriteBatch.Draw(playerTexture, rectangle, null, Color.White, this.rotation, v2Center, SpriteEffects.None, 0.0f);
        }

        public void Update(GameTime time)
        {

        }

        public void Move(InputState inputState, KeyboardState input, Camera cam, List<GameObject> colliders)
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
                    case FaceDirection.up: animation = "StrikeNorth";
                        break;

                    case FaceDirection.down: animation = "StrikeSouth";
                        break;

                    case FaceDirection.right: animation = "StrikeEast";
                        break;

                    case FaceDirection.left: animation = "StrikeWest";
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

            if (moveDir.Length() > 0)
            {
                prevPos = new Point(Position.X, Position.Y);
                Position += moveDir.ToPoint();
                if(deltaMovement <= 1.0f)
                    deltaMovement += 0.1f;
            }
            else
            {
                animation = "Idle" + directionNames[faceing];
                currentMovementSpeed = 0.0f;
                deltaMovement = 0.1f;
            }
            position.X = MathHelper.Clamp(Position.X, 0, cam.mapWidthInpx - iWidth); //keep the player within the world bounds by clamping the position
            position.Y = MathHelper.Clamp(Position.Y, 0, cam.mapHeightInpx - iHeight);

            foreach (GameObject collider in colliders)
            {
                if (CheckCollision(collider))
                {
                    Position = prevPos;
                }
                    
            }
        }

        /// <summary>
        /// Returns a bounding Rectangle of the player for collission detection
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetBoundingBox()
        {
            boundingBox.X = Position.X + iWidth / 4;
            boundingBox.Y = Position.Y + iHeight / 2;
            return boundingBox;
        }

        public override bool CheckCollision(GameObject collider)
        {
            return this.GetBoundingBox().Intersects(collider.GetBoundingBox());
        }

        
    }
}