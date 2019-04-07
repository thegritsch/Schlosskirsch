#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using System;
using System.Collections.Generic;
using Schlosskirsch;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Schlosskirsch.Screens;



#endregion Using Statements

namespace GameStateManagement
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    internal class GameplayScreen : GameScreen
    {
        const uint FIRE_INTERVAL = 200;
        const short BULLET_AMOUNT = 10;
        const uint RESPAWN_TIME = 400;
        const int treePosX = 550;
        const int treePosY = 450;
        #region Fields
        private bool isRespawning = false;
        private ContentManager content;
        private SpriteFont gameFont;
        private SpriteBatch spriteBatch;
        private Texture2D background;
        private Vector2 enemyPosition = new Vector2(10, 10);

        private Camera camera;
        private float pauseAlpha;
        private Player player;
        private Texture2D playerTexture;
        private Texture2D enemyTexture;
        private Vector2 playerScreenPos;
        private  uint timeSinceLastFire = 0;
        
        private Point[] affected = new Point[4];
        private Rectangle lifeBarRectangle;
        private Texture2D lifeBarTexture;
        private TheTree theTree;
        private bool acceptingInput;
        private List<GameObject> gameObjects;
        private Bullet[] bullets;
        private ProgressBar playerHealthBar;
        private ProgressBar treeHealthBar;
        private Rectangle viewPortRectangle;
        private Rectangle textureRectangle;
        private BulletManager<Bullet> playerBullet;
        private List<BasicDrone> drones;
        private List<Point> spawnPoints;
        Texture2D droneTexture;
        Rectangle droneBounds;
        private Header scoreHeader;
        private uint respawnTimer;
        int score = 0;
        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            ControllingPlayer = PlayerIndex.One;
            gameObjects = new List<GameObject>();
            bullets = new Bullet[BULLET_AMOUNT];
            timeSinceLastFire = FIRE_INTERVAL;
            playerHealthBar = new ProgressBar(0, 100, new Vector2(300.0f, 50.0f), Anchor.TopLeft);
            playerHealthBar.Locked = true;
            playerHealthBar.Value = 100;
            treeHealthBar = new ProgressBar(0, 100, new Vector2(300.0f, 50.0f), Anchor.TopRight);
            treeHealthBar.Locked = true;
            treeHealthBar.Value = 100;
            viewPortRectangle = new Rectangle(0, 0, Schlosskirsch.Game1.ScreenWidth, Schlosskirsch.Game1.ScreenHeight);
            UserInterface.Active.AddEntity(playerHealthBar);
            UserInterface.Active.AddEntity(treeHealthBar);

            this.playerBullet = new BulletManager<Bullet>(Schlosskirsch.Game1.ScreenWidth, Schlosskirsch.Game1.ScreenHeight);
            drones = new List<BasicDrone>();
            spawnPoints = new List<Point>();
            scoreHeader = new Header("Score: ", Anchor.TopCenter);
            UserInterface.Active.AddEntity(scoreHeader);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(ContentManager Content)
        {
            
            spriteBatch = ScreenManager.SpriteBatch;
            content = Content;
            //gameFont = Content.Load<SpriteFont>("gamefont");
            
            //enemyTexture = Content.Load<Texture2D>("gerd");

            player = new Player(playerTexture, 64, 64);
            player.loadPlayerContent(Content);
            player.controllingPlayer = ControllingPlayer;
            player.Position = new Point(900, 500);
            theTree = new TheTree(128, 128, new Point(treePosX,treePosY));
            theTree.LoadContent(Content);
            gameObjects.Add(theTree);


            spawnPoints.Add(new Point(50, 500));
            spawnPoints.Add(new Point(1100, 500));
            spawnPoints.Add(new Point(600, 50));
            spawnPoints.Add(new Point(600, 1000));
            spawnPoints.Add(new Point(50, 50));
            spawnPoints.Add(new Point(1000, 1000));

            if (camera == null)
            {
                camera = new Camera(2.0f, ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height, 4, 5);
            }
            camera.mapWidthInpx = ScreenManager.GraphicsDevice.Viewport.Width;
            camera.mapHeightInpx = ScreenManager.GraphicsDevice.Viewport.Height;


            //Load tiletextures for the specified tilelayer

            lifeBarRectangle = new Rectangle(3, 3, player.getHealth, 40);
            lifeBarTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            lifeBarTexture.SetData<Color>(new Color[] { Color.White });
            acceptingInput = true;
            background = Content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER , "Field"));
            textureRectangle = new Rectangle(0, 0, Schlosskirsch.Game1.ScreenWidth, Schlosskirsch.Game1.ScreenHeight);
            Texture2D bulletTexture = Content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "mvBCX1"));

            for (int i = 0; i < BULLET_AMOUNT; i++)
            {
                bullets[i] = new Bullet(bulletTexture, Point.Zero, 64, 64);
            }
            this.playerBullet.AddBullets(bullets);

            droneTexture = Content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "Data-Matrix-Code"));
             droneBounds = new Rectangle(0, 0, 64, 64);
            for (int i = 0; i < BULLET_AMOUNT; i++)
            {
                drones.Add(new BasicDrone(droneBounds, droneTexture, spawnPoints[i % spawnPoints.Count]));
            }
            gameObjects.AddRange(drones);
            gameObjects.Add(player);
            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            UserInterface.Active.RemoveEntity(this.playerHealthBar);
            UserInterface.Active.RemoveEntity(this.treeHealthBar);
            UserInterface.Active.RemoveEntity(this.scoreHeader);
        }

        #endregion Initialization

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            UserInterface.Active.Update(gameTime);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive && ScreenState == ScreenState.Active)
            {
                

                playerScreenPos = camera.worldToScreen(player.Position);

                if (playerScreenPos.X < 400)
                {
                    camera.update(new Vector2(playerScreenPos.X - 400, 0));
                }

                if (playerScreenPos.X > camera.ScreenWidth - 400)
                {
                    camera.update(new Vector2(playerScreenPos.X - (camera.ScreenWidth - 400), 0));
                }

                if (playerScreenPos.Y < 200)
                {
                    camera.update(new Vector2(0, playerScreenPos.Y - 200));
                }

                if (playerScreenPos.Y > camera.ScreenHeight - 200)
                {
                    camera.update(new Vector2(0, playerScreenPos.Y - (camera.ScreenHeight - 200)));
                }

                player.Update(gameTime, gameObjects);

                

                MouseState mouseState = Mouse.GetState();
                Vector2 direction = (mouseState.Position - player.Position).ToVector2();
                player.Rotation = (float)Math.Atan2(direction.Y, direction.X);

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (timeSinceLastFire >= FIRE_INTERVAL)
                    {
                        this.playerBullet.Fire(player.Position, direction);
                        timeSinceLastFire = 0;
                    }
                   
                }

                timeSinceLastFire += (uint)gameTime.ElapsedGameTime.Milliseconds;

                this.playerBullet.Update();
                for(int i = 0; i < drones.Count; i++)
                {
                    BasicDrone d = drones[i];
                    this.playerBullet.CheckCollision(d, gameTime);
                    
                    
                    
                    if (!d.IsDestroyed)
                    {
                        
                        d.Update(player, theTree, gameObjects, gameTime);
                    }
                    else
                    {
                        gameObjects.Remove(d);
                        drones.RemoveAt(i);
                        score++;
                        scoreHeader.Text = "Score: " + score.ToString();
                    }
                }

                if(drones.Count == 0)
                {
                    isRespawning = true;
                }

                if(isRespawning)
                {
                    respawnTimer += (uint)gameTime.ElapsedGameTime.Milliseconds;
                    if (respawnTimer >= RESPAWN_TIME)
                    {
                        
                        BasicDrone d = new BasicDrone(droneBounds, droneTexture, spawnPoints[(int)gameTime.TotalGameTime.Milliseconds % spawnPoints.Count]);
                        drones.Add(d);
                        gameObjects.Add(d);
                        respawnTimer = 0;
                        if (drones.Count == BULLET_AMOUNT)
                            isRespawning = false;
                    }
                }
                theTree.Update(gameTime);
                this.playerHealthBar.Value = player.getHealth;
                this.treeHealthBar.Value = theTree.Health;
                if (player.getHealth <= 0 || theTree.Health <= 0) 
                {
                    ScreenManager.AddScreen(new GameOverScreen(score), new PlayerIndex());
                    this.ExitScreen();
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;
           
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];

            //GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            //bool gamePadDisconnected = !gamePadState.IsConnected &&
            //input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer))
            {
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (acceptingInput)
            {
                
                  player.Move(input, keyboardState, camera);
                
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.Black, 0, 0);

            // Our player and enemy are both actually just text strings.

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //position data for debugging

            lifeBarRectangle.Width = player.getHealth;

            spriteBatch.Draw(lifeBarTexture, lifeBarRectangle, Color.Green);
            //spriteBatch.DrawString(gameFont, player.getHealth + "/100", enemyPosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(background, viewPortRectangle, null, Color.White);
            
            player.Draw(spriteBatch, 0, 0, camera);
            theTree.Draw(spriteBatch);
            this.playerBullet.Draw(spriteBatch);
            

            foreach(BasicDrone drone in this.drones)
            {
                if(!drone.IsDestroyed)
                {
                    drone.Draw(spriteBatch);
                }
            }
            spriteBatch.End();
            UserInterface.Active.Draw(spriteBatch);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion Update and Draw
    }
}