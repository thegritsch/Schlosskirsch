using GameStateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GeonBit.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GeonBit.UI.Entities;

namespace Game1
{
    public class TitleScreen : GameScreen
    {
        SpriteBatch spriteBatch;
        Texture2D titleBackground;
        Panel panel;

        public TitleScreen()
        {
            
        }

        public override void LoadContent(ContentManager content)
        {
            spriteBatch = ScreenManager.SpriteBatch;
            titleBackground = content.Load <Texture2D> ("Custom Content/Titlescreen");
            // create a panel and position in center of screen
            panel = new Panel(new Vector2(400, 200), PanelSkin.None, Anchor.Center);
            UserInterface.Active.AddEntity(panel);

            // add title and text
            panel.AddChild(new Header("Schloss Kirsch"));
            panel.AddChild(new HorizontalLine());

            Button startButton = new Button("Start", ButtonSkin.Default, Anchor.BottomCenter);
            startButton.OnClick = new EventCallback(ButtonClick);
            // add a button at the bottom
            panel.AddChild(startButton);
            base.LoadContent(content);
        }

        private void ButtonClick(Entity entity)
        {
            this.ScreenManager.AddScreen(new GameplayScreen(), new PlayerIndex());
            this.ExitScreen();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            UserInterface.Active.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(titleBackground, Vector2.Zero, Color.White);
            spriteBatch.End();

            UserInterface.Active.Draw(spriteBatch);
            base.Draw(gameTime);
            
        }

        public override void UnloadContent()
        {
            UserInterface.Active.RemoveEntity(panel);
            base.UnloadContent();
        }
    }
}
