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
using Schlosskirsch.Objects.PowerUps;
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
        private static Random random = new Random();

        #region Fields

        private readonly List<GameObject> gameObjects = new List<GameObject>();

        private SpriteBatch spriteBatch;

        private Texture2D background;

        private Camera camera;
        private float pauseAlpha;

        private int enemieCount = 3;
        private int enemieScore = 15;

        private Header scoreHeader;
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

            this.scoreHeader = new Header("Score: ", Anchor.TopCenter);
            UserInterface.Active.AddEntity(this.scoreHeader);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(ContentManager content)
        {
            this.spriteBatch = ScreenManager.SpriteBatch;

            this.background = content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "Field"));

            Texture2D playerTexture = content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "smiley_sprite"));
            Texture2D bulletTexture = content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "mvBCX1"));
            this.gameObjects.Add(new Smily(playerTexture, new Point(900, 500), new Gun(bulletTexture), PlayerIndex.One));

            Texture2D towerTexture = content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "cube"));
            this.gameObjects.Add(new Home(towerTexture, new Point(550, 450)));

            BasicDrone.LoadTexture(content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "Data-Matrix-Code")));
            SmallBugfix.LoadTexture(content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "bug-128")));

            if (camera == null)
            {
                camera = new Camera(2.0f, ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height, 4, 5);
            }
            camera.mapWidthInpx = ScreenManager.GraphicsDevice.Viewport.Width;
            camera.mapHeightInpx = ScreenManager.GraphicsDevice.Viewport.Height;
            
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
            UserInterface.Active.RemoveEntity(this.scoreHeader);
        }

        #endregion Initialization

        private Point getSpawnLocation()
        {
            switch (GameplayScreen.random.Next(4))
            {
                default:
                case 0: return new Point(GameplayScreen.random.Next(-100, MainGame.ScreenWidth + 101), -100);
                case 1: return new Point(-100, GameplayScreen.random.Next(-100, MainGame.ScreenHeight + 101));
                case 2: return new Point(GameplayScreen.random.Next(-100, MainGame.ScreenWidth + 101), MainGame.ScreenHeight + 100);
                case 3: return new Point(MainGame.ScreenWidth + 100, GameplayScreen.random.Next(-100, MainGame.ScreenHeight + 101));
            }
        }

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

                var powerUps = this.gameObjects.OfType<PowerUp>().ToList();
                foreach (PowerUp powerUp in powerUps)
                {
                    if (powerUp.IsConsumed)
                    {
                        this.gameObjects.Remove(powerUp);
                    }
                    else
                    {
                        powerUp.Update(gameTime, this.gameObjects);
                    }
                }

                var enemies = this.gameObjects.OfType<Enemy>();
                for (int index = 0; index < this.gameObjects.OfType<Enemy>().Count(); index++)
                {
                    Enemy enemy = enemies.ElementAt(index);

                    if (enemy.IsDestroyed)
                    {
                        this.gameObjects.Remove(enemy);

                        this.score += 1;
                        this.scoreHeader.Text = "Score: " + this.score.ToString();

                        if (this.score >= this.enemieScore)
                        {
                            this.enemieCount += 1;
                            this.enemieScore += this.enemieScore;
                        }

                        if (!this.gameObjects.OfType<SmallBugfix>().Any() && this.score % 10 == 0) //TODO: Spawn a single power up at destroyed enemy to regen health in a better way
                        {
                            this.gameObjects.Add(new SmallBugfix(enemy.Location));
                        }
                    }
                    else
                    {
                        enemy.Update(this.gameObjects, gameTime);
                    }
                }

                while (this.gameObjects.OfType<Enemy>().Count() < this.enemieCount)
                {
                    this.gameObjects.Add(new BasicDrone(this.getSpawnLocation()));
                }

                foreach (HealthObject health in this.gameObjects.OfType<HealthObject>())
                {
                    if (health.Health <= 0)
                    {
                        ScreenManager.AddScreen(new GameOverScreen(score), new PlayerIndex());
                        this.ExitScreen();
                    }
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

            foreach (Player player in this.gameObjects.OfType<Player>())
            {
                if (player.HandleInput(input))
                {
                    //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                }
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            this.spriteBatch.Draw(this.background, new Rectangle(0, 0, MainGame.ScreenWidth, MainGame.ScreenHeight), Color.White);
            
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