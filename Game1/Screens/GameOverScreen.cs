using GameStateManagement;
using GeonBit.UI;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schlosskirsch.Screens
{
    public class GameOverScreen : GameScreen
    {
        SpriteBatch spriteBatch;
        
        Panel panel;
        private Rectangle viewPortRectangle;
        private Rectangle textureRectangle;
        private int score;

        public GameOverScreen(int score)
        {
            this.score = score;
        }

        public override void LoadContent(ContentManager content)
        {
            spriteBatch = ScreenManager.SpriteBatch;
            
            
            // create a panel and position in center of screen
            panel = new Panel(new Vector2(Game1.ScreenWidth, Game1.ScreenHeight), PanelSkin.None, Anchor.Center);
            UserInterface.Active.AddEntity(panel);

            Header header = new Header("Game over!", Anchor.TopCenter);
            panel.AddChild(header);
            header = new Header("Score: " + score.ToString(), Anchor.TopCenter, new Vector2(0, 100));
            panel.AddChild(header);
            Button startButton = new Button("Title", ButtonSkin.Default, Anchor.Center, new Vector2(400, 50));
            startButton.OnClick = new EventCallback(ButtonClick);
            // add a button at the bottom
            panel.AddChild(startButton);
        }

        private void ButtonClick(Entity entity)
        {
            this.ScreenManager.AddScreen(new TitleScreen(), new PlayerIndex());
            this.ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            
            UserInterface.Active.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UserInterface.Active.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void UnloadContent()
        {
            UserInterface.Active.RemoveEntity(panel);
            base.UnloadContent();
        }
    }
}
