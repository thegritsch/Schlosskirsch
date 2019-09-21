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
using System.IO;

namespace Schlosskirsch
{
    public class TitleScreen : GameScreen
    {
        SpriteBatch spriteBatch;
        Texture2D titleBackground;
        Texture2D headerText;
        Panel panel;
        private Rectangle viewPortRectangle;
        private Rectangle textureRectangle;

        public TitleScreen()
        {
            viewPortRectangle = new Rectangle(0, 0, MainGame.ScreenWidth, MainGame.ScreenHeight);
            
        }

        public override void LoadContent(ContentManager content)
        {
            spriteBatch = ScreenManager.SpriteBatch;
            titleBackground = content.Load <Texture2D> (Path.Combine(MainGame.CONTENT_SUBFOLDER,"Titlescreen"));
            headerText = content.Load<Texture2D>(Path.Combine(MainGame.CONTENT_SUBFOLDER, "ivii_logo_transparent"));
            textureRectangle = new Rectangle(titleBackground.Width - MainGame.ScreenWidth, titleBackground.Height - MainGame.ScreenHeight, MainGame.ScreenWidth, MainGame.ScreenHeight);
            // create a panel and position in center of screen
            panel = new Panel(new Vector2(MainGame.ScreenWidth, MainGame.ScreenHeight), PanelSkin.None, Anchor.Center);
            UserInterface.Active.AddEntity(panel);

            Image img = new Image(texture: headerText, size: new Vector2(headerText.Width, headerText.Height), drawMode: ImageDrawMode.Stretch, anchor: Anchor.TopCenter, offset: new Vector2(0, 0));
            // add title and text
            panel.AddChild(img);
            panel.AddChild(new Header("In Wonderland"));
            

            Button startButton = new Button("Start", ButtonSkin.Default, Anchor.Center, new Vector2(400, 50));
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
            spriteBatch.Draw(titleBackground, viewPortRectangle, textureRectangle, Color.White);
            
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
