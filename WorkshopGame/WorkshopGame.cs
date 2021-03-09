using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Runtime.InteropServices;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace WorkshopGame
{
    public class WorkshopGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteBatch backgroundSpriteBatch;

        //private Spaceship spaceship;

        private GameBackground gameBackground;

        private List<Sprite> _sprites;




        public WorkshopGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //spaceship = new Spaceship();
            gameBackground = new GameBackground(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundSpriteBatch = new SpriteBatch(GraphicsDevice);
            gameBackground.LoadContent(Content);

            var shipTexture = Content.Load<Texture2D>("playerShip");

            _sprites = new List<Sprite>()
            {
                new Ship(shipTexture)
                {
                    Position = new Vector2(400,250),
                    Bullet = new Bullet(Content.Load<Texture2D>("bullet"))
                }
            };

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach(var sprite in _sprites.ToArray())
            {
                sprite.Update(gameTime, _sprites);
            }

            for(int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i].IsRemoved)
                {
                    _sprites.RemoveAt(i);
                    i--;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin();
            backgroundSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, null);
            gameBackground.Draw(gameTime, backgroundSpriteBatch);

            foreach(var sprite in _sprites)
            {
                sprite.Draw(spriteBatch);
            }



            backgroundSpriteBatch.End();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
