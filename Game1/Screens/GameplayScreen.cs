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
using Schlosskirsch.Objects;
using Schlosskirsch.Objects.Enemies;
using Schlosskirsch.Objects.Guards;
using Schlosskirsch.Objects.Players;
using Schlosskirsch.Objects.Weapons;
using System.Linq;



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
        const short ENEMY_AMOUNT = 10;
        const uint RESPAWN_TIME = 400;

        #region Fields

        private bool isRespawning = false;

        private SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D droneTexture;

        private Vector2 enemyPosition = new Vector2(10, 10);

        private Camera camera;
        private float pauseAlpha;
        private Player player;
        
        private Point[] affected = new Point[4];
        private Rectangle lifeBarRectangle;
        private Texture2D lifeBarTexture;
        private Home home;
        private readonly List<GameObject> gameObjects = new List<GameObject>();
        private ProgressBar playerHealthBar;
        private ProgressBar treeHealthBar;
        private List<Enemy> enemies;
        private List<Point> spawnPoints;
        private Header scoreHeader;
        private uint respawnTimer;
        private int score = 0;

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

            playerHealthBar = new ProgressBar(0, 100, new Vector2(300.0f, 50.0f), Anchor.TopLeft);
            playerHealthBar.Locked = true;
            playerHealthBar.Value = 100;
            treeHealthBar = new ProgressBar(0, 100, new Vector2(300.0f, 50.0f), Anchor.TopRight);
            treeHealthBar.Locked = true;
            treeHealthBar.Value = 100;
            
            UserInterface.Active.AddEntity(playerHealthBar);
            UserInterface.Active.AddEntity(treeHealthBar);

            enemies = new List<Enemy>();
            spawnPoints = new List<Point>();

            scoreHeader = new Header("Score: ", Anchor.TopCenter);
            UserInterface.Active.AddEntity(scoreHeader);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(ContentManager content)
        {
            this.spriteBatch = ScreenManager.SpriteBatch;

            this.background = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "Field"));
            this.droneTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "Data-Matrix-Code"));

            Texture2D playerTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "smiley_sprite"));
            Texture2D towerTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "cube"));
            Texture2D bulletTexture = content.Load<Texture2D>(Path.Combine(Game1.CONTENT_SUBFOLDER, "mvBCX1"));

            this.player = new Smily(playerTexture, new Point(900, 500), new Gun(bulletTexture));
            this.gameObjects.Add(player);
            this.home = new Home(towerTexture, new Point(550, 450));
            this.gameObjects.Add(home);

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
            lifeBarRectangle = new Rectangle(3, 3, player.Health, 40);
            lifeBarTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            lifeBarTexture.SetData<Color>(new Color[] { Color.White });
            
            for (int i = 0; i < ENEMY_AMOUNT; i++)
            {
                enemies.Add(new BasicDrone(droneTexture, spawnPoints[i % spawnPoints.Count]));
            }
            gameObjects.AddRange(enemies);
            
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
                //playerScreenPos = camera.worldToScreen(player.Location);

                //if (playerScreenPos.X < 400)
                //{
                //    camera.update(new Vector2(playerScreenPos.X - 400, 0));
                //}
                //if (playerScreenPos.X > camera.ScreenWidth - 400)
                //{
                //    camera.update(new Vector2(playerScreenPos.X - (camera.ScreenWidth - 400), 0));
                //}
                //if (playerScreenPos.Y < 200)
                //{
                //    camera.update(new Vector2(0, playerScreenPos.Y - 200));
                //}
                //if (playerScreenPos.Y > camera.ScreenHeight - 200)
                //{
                //    camera.update(new Vector2(0, playerScreenPos.Y - (camera.ScreenHeight - 200)));
                //}

                foreach (HealthObject healthObject in this.gameObjects.OfType<HealthObject>())
                {
                    healthObject.Update(gameTime, this.gameObjects);
                }

                Enemy[] enemies = this.gameObjects.OfType<Enemy>().ToArray();
                for (int index = 0; index < enemies.Length; index++)
                {
                    Enemy enemy = enemies[index];

                    if (enemy.IsDestroyed)
                    {
                        this.gameObjects.Remove(enemy);

                        score++;
                        scoreHeader.Text = "Score: " + score.ToString();
                    }
                    else
                    {
                        enemy.Update(this.gameObjects, gameTime);
                    }
                }

                if (enemies.Length == 0)
                {
                    isRespawning = true;
                }

                if (isRespawning)
                {
                    respawnTimer += (uint)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (respawnTimer >= RESPAWN_TIME)
                    {
                        
                        BasicDrone d = new BasicDrone(droneTexture, spawnPoints[(int)gameTime.TotalGameTime.TotalMilliseconds % spawnPoints.Count]);
                        this.gameObjects.Add(d);
                        respawnTimer = 0;
                        if (enemies.Length == ENEMY_AMOUNT)
                            isRespawning = false;
                    }
                }

                this.playerHealthBar.Value = player.Health;
                this.treeHealthBar.Value = home.Health;

                if (player.Health <= 0 || home.Health <= 0) 
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
            if (input == null) throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)this.ControllingPlayer.Value;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            if (input.IsPauseGame(ControllingPlayer) || (input.GamePadWasConnected[playerIndex] && !input.GamePadConnected[playerIndex]))
            {
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                player.HandleInput(input, playerIndex);
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            this.spriteBatch.Draw(background, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);

            this.lifeBarRectangle.Width = this.player.Health;
            this.spriteBatch.Draw(this.lifeBarTexture, this.lifeBarRectangle, Color.Green);
            
            foreach (GameObject gameObject in this.gameObjects)
            {
                gameObject.Draw(this.spriteBatch);
            }

            this.spriteBatch.End();

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