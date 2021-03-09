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
using CollisionExample.Collisions;


namespace WorkshopGame
{
    public class WorkshopGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        SpriteFont font;

        public int flag = 0;

        public bool previousCollision;
        public bool currentCollision;

        private SpriteBatch backgroundSpriteBatch;

        //private Spaceship spaceship;

        private GameBackground gameBackground;

        private List<Sprite> _sprites;

        List<Texture2D> asteroidTextures = new List<Texture2D>();
        List<Sprite> asteroids = new List<Sprite>();

        public Texture2D boundingTexture;

        public float distance;

        public int score = 0;


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
            font = Content.Load<SpriteFont>("myfont");
            boundingTexture = Content.Load<Texture2D>("bounding");

            var shipTexture = Content.Load<Texture2D>("playerShip");

            for (int i = 1; i < 4; i++)
                asteroidTextures.Add(
                    Content.Load<Texture2D>("asteroid" + i.ToString()));

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

            if (asteroids.Count == 0)
            {
                InitializeAsteroids();
            }

            UpdateAsteroids();

            foreach (var sprite in _sprites.ToArray())
            {
                sprite.Update(gameTime, _sprites);
            }

            for(int i = 0; i < _sprites.Count; i++)
            {

                for(int j = 0; j< asteroids.Count; j++)
                {
                    if(i>0 && CollisionHelper.Collides(_sprites[i].bounds, asteroids[j].bounds))
                    {
                        _sprites.RemoveAt(i);
                        i--;
                        asteroids.RemoveAt(j);
                        j--;
                        MakeOneAsteroid();
                        score++;
                    }

                }
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


            foreach (Sprite a in asteroids)
            {
                spriteBatch.Draw(a._texture, a.Position, null, Color.White, a._rotation, new Vector2(a.Width/2, a.Height/2), (float)0.5f, SpriteEffects.None, 1.0f);
                //spriteBatch.Draw(boundingTexture, a.bounds.Center, null, Color.White, a._rotation,new Vector2(a.Width/2, a.Height/2), (float)0.05f, SpriteEffects.None, 1.0f);
            }

            foreach (var sprite in _sprites)
            {
                sprite.Draw(spriteBatch);
            }

            //double totalGametime = Convert.ToDouble(gameTime.TotalGameTime);
            TimeSpan timeLeft = new TimeSpan(0,0,30);
            //- totalGametime;

            spriteBatch.DrawString(font, "Score: " + score, new Vector2(5, 5), Color.White, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(font, "Time Remaining: " + timeLeft.Subtract(gameTime.TotalGameTime).ToString("m\\:ss"), new Vector2(5, 25), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);


            backgroundSpriteBatch.End();
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void MakeOneAsteroid()
        {
            Random random = new Random();
            Sprite tempSprite = new Sprite(asteroidTextures[random.Next(3)]);
            asteroids.Add(tempSprite);
            asteroids[asteroids.Count - 1].Position = new Vector2(
                    (random.Next(0, graphics.GraphicsDevice.Viewport.Width)),
                    (graphics.GraphicsDevice.Viewport.Height));
            asteroids[asteroids.Count - 1].asteroidVelocity = new Vector2(
                    (float)random.NextDouble() * 4 - 1,
                    (float)random.NextDouble() * 4 - 1);
            asteroids[asteroids.Count - 1]._rotation = (float)random.NextDouble() *
                    MathHelper.Pi * 4 - (MathHelper.Pi * 2);

            asteroids[asteroids.Count - 1].bounds = new BoundingCircle(asteroids[asteroids.Count - 1].Position, asteroids[asteroids.Count - 1].Width / 2);
        }
        private void InitializeAsteroids()
        {
            Random random = new Random();

            //Initial Asteroids Creation
            
            
            for (int i = 0; i < random.Next(10, 20); i++)
            {
                Sprite tempSprite = new Sprite(asteroidTextures[random.Next(3)]);
                asteroids.Add(tempSprite);

                int height = 0;
                int randNum = random.Next(0, 1);
                if (randNum == 0)
                {
                    height = graphics.GraphicsDevice.Viewport.Height;
                }
                else if (randNum == 1) {
                    height = 0;
                }



                asteroids[i].Position = new Vector2(
                        (random.Next(0, graphics.GraphicsDevice.Viewport.Width)), height);
                asteroids[i].asteroidVelocity = new Vector2(
                        (float)random.NextDouble() * 4 - 1,
                        (float)random.NextDouble() * 4 - 1);
                asteroids[i]._rotation = (float)random.NextDouble() *
                        MathHelper.Pi * 4 - (MathHelper.Pi * 2);

                asteroids[i].bounds = new BoundingCircle(asteroids[i].Position, asteroids[i].Width / 2);
                   
            }
            



        }

        private void UpdateAsteroids()
        {





            for(int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Position += asteroids[i].asteroidVelocity;

                if (asteroids[i].Position.X + asteroids[i].Width < 0)
                {
                    asteroids[i].Position = new Vector2(graphics.GraphicsDevice.Viewport.Width, asteroids[i].Position.Y);
                }
                if (asteroids[i].Position.Y + asteroids[i].Height < 0)
                {
                    asteroids[i].Position = new Vector2(asteroids[i].Position.X, graphics.GraphicsDevice.Viewport.Height);
                }
                if (asteroids[i].Position.X - asteroids[i].Width > graphics.GraphicsDevice.Viewport.Width)
                {
                    asteroids[i].Position = new Vector2(0, asteroids[i].Position.Y);
                }
                if (asteroids[i].Position.Y - asteroids[i].Height > graphics.GraphicsDevice.Viewport.Height)
                {
                    asteroids[i].Position = new Vector2(asteroids[i].Position.X, 0);
                }

                asteroids[i].bounds = new BoundingCircle(asteroids[i].Position, asteroids[i].Width / 2);

                if (CollisionHelper.Collides(_sprites[0].shipBounds, asteroids[i].bounds))
                {

                    asteroids.RemoveAt(i);
                    i--;
                    score--;
                    MakeOneAsteroid();
                }
            }





            foreach (Sprite a in asteroids)
            {
                a.Position += a.asteroidVelocity;

                if (a.Position.X + a.Width < 0)
                {
                    a.Position = new Vector2(graphics.GraphicsDevice.Viewport.Width, a.Position.Y);
                }
                if (a.Position.Y + a.Height < 0)
                {
                    a.Position = new Vector2(a.Position.X, graphics.GraphicsDevice.Viewport.Height);
                }
                if (a.Position.X - a.Width > graphics.GraphicsDevice.Viewport.Width)
                {
                    a.Position = new Vector2(0, a.Position.Y);
                }
                if (a.Position.Y - a.Height > graphics.GraphicsDevice.Viewport.Height)
                {
                    a.Position = new Vector2(a.Position.X, 0);
                }

                a.bounds = new BoundingCircle(a.Position, a.Width / 2);


                previousCollision = currentCollision;
                currentCollision = CollisionHelper.Collides(_sprites[0].shipBounds, a.bounds);

                if (currentCollision == true && previousCollision == false)
                {
                    score--;
                }



                   
            }
        }
    }
}
