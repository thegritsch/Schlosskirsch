﻿#region File Description

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
using Game1;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperG;
using TileEngine;

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
        #region Fields

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
        private List<ICollider> colliders;
        private Bullet[] bullets;
        private ProgressBar playerHealthBar;
        private ProgressBar treeHealthBar;
        #endregion Fields

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            ControllingPlayer = PlayerIndex.One;
            colliders = new List<ICollider>();
            bullets = new Bullet[BULLET_AMOUNT];
            timeSinceLastFire = FIRE_INTERVAL;
            playerHealthBar = new ProgressBar(0, 10, new Vector2(200.0f, 20.0f), Anchor.TopLeft);
            playerHealthBar.Locked = true;
            playerHealthBar.Value = 10;
            treeHealthBar = new ProgressBar(0, 10, new Vector2(200.0f, 20.0f), Anchor.TopRight);
            treeHealthBar.Locked = true;
            treeHealthBar.Value = 10;

            UserInterface.Active.AddEntity(playerHealthBar);
            UserInterface.Active.AddEntity(treeHealthBar);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(ContentManager Content)
        {
            
            spriteBatch = ScreenManager.SpriteBatch;

            //gameFont = Content.Load<SpriteFont>("gamefont");
            
            //enemyTexture = Content.Load<Texture2D>("gerd");

            player = new Player(playerTexture, 64, 64);
            player.loadPlayerContent(Content);
            player.controllingPlayer = ControllingPlayer;
            theTree = new TheTree(200, 200, new Point(600 - 128, 360 -128));
            theTree.LoadContent(Content);
            colliders.Add(theTree);

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
            background = Content.Load<Texture2D>("Custom Content/Field");
            Texture2D bulletTexture = Content.Load<Texture2D>("Custom Content/Bullet");

            for (int i = 0; i < BULLET_AMOUNT; i++)
            {
                bullets[i] = new Bullet(bulletTexture, Vector2.Zero, 32, 32);
            }
            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            //ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
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

            if (IsActive)
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

                player.Update(gameTime);

                

                MouseState mouseState = Mouse.GetState();
                Vector2 direction = mouseState.Position.ToVector2() - player.Position;
                player.Rotation = (float)Math.Atan2(direction.Y, direction.X);

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (timeSinceLastFire >= FIRE_INTERVAL)
                    {
                        for (int i = 0; i < BULLET_AMOUNT; i++)
                        {
                            if (bullets[i].IsDestroyed)
                            {
                                bullets[i].Position = player.Position;
                                bullets[i].IsDestroyed = false;
                                
                                
                                direction.Normalize();
                                
                                bullets[i].SetDirection = direction;
                                break;
                            }
                        }
                        timeSinceLastFire = 0;
                    }
                   
                }

                timeSinceLastFire += (uint)gameTime.ElapsedGameTime.Milliseconds;

                for (int i = 0; i < BULLET_AMOUNT; i++)
                {
                    if(!bullets[i].IsDestroyed)
                    {
                        bullets[i].Move(20.0f);
                        if (bullets[i].Position.X >= camera.ScreenWidth || bullets[i].Position.X <= -bullets[i].GetWidth)
                            bullets[i].IsDestroyed = true;
                        else if (bullets[i].Position.Y >= camera.ScreenHeight || bullets[i].Position.Y <= -bullets[i].GetHeight)
                        {
                            bullets[i].IsDestroyed = true;
                        }
                    }
                }

                if (player.getHealth <= 0) //if players health drops under zero
                {
                   
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
                
                  player.move(input, keyboardState, camera, colliders);
                
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
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            
            player.Draw(spriteBatch, 0, 0, camera);
            theTree.Draw(spriteBatch);
            for (int i = 0; i < BULLET_AMOUNT; i++)
            {
                if (!bullets[i].IsDestroyed)
                    bullets[i].Draw(spriteBatch);
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