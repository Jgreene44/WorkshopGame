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
        SpriteFont endGamefont;

        public SoundEffect playerHitSound;
        public SoundEffect firingLasersSound;

        public Song backgroundMusic;

        public bool resetFlag = false;
        public bool running = true;
        //public int flag = 0;

        public bool previousCollision;
        public bool currentCollision;

        public Texture2D purpleEnd;
        private SpriteBatch backgroundSpriteBatch;

        //private Spaceship spaceship;

        private GameBackground gameBackground;

        private List<Sprite> _sprites;

        List<Texture2D> asteroidTextures = new List<Texture2D>();
        List<Sprite> asteroids = new List<Sprite>();

        public Texture2D boundingTexture;

        public float distance;

        public int score = 0;

        public DateTime targetTime;

        //TODO TIE THIS INTO THE OTHER MENU SLIDER
        public int _volume = 10;


        public WorkshopGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Peril in Space";
        }

        protected override void Initialize()
        {
            gameBackground = new GameBackground(this);
            startGame();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundSpriteBatch = new SpriteBatch(GraphicsDevice);
            gameBackground.LoadContent(Content);
            font = Content.Load<SpriteFont>("myfont");
            endGamefont = Content.Load<SpriteFont>("endGameFont");
            boundingTexture = Content.Load<Texture2D>("bounding");
            backgroundMusic = Content.Load<Song>("music");

            playerHitSound = Content.Load<SoundEffect>("sfx_lose");
            firingLasersSound = Content.Load<SoundEffect>("sfx_laser1");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);
            
            purpleEnd = Content.Load<Texture2D>("purple");
            
        }

        public void startGame()
        {
            //Set the music to the volume of the other menu
            MediaPlayer.Volume = (float)_volume / 10;
            //Create the ship and the bullet

            var shipTexture = Content.Load<Texture2D>("playerShip");
            _sprites = new List<Sprite>()
            {
                new Ship(shipTexture, Content, _volume)
                {
                    Position = new Vector2(400,250),
                    Bullet = new Bullet(Content.Load<Texture2D>("bullet")),
                    Volume = _volume
                    
                }
            };

            //Create the asteroids
            for (int i = 1; i < 4; i++)
                asteroidTextures.Add(
                    Content.Load<Texture2D>("asteroid" + i.ToString()));
            asteroids.Clear();
            InitializeAsteroids();
            running = true;
            score = 0;

            //Grab the target time
            targetTime = DateTime.Now.AddSeconds(30);
            
        }

        protected override void Update(GameTime gameTime)
        {

            DateTime currentTime = DateTime.Now;
            KeyboardState keyboardState = Keyboard.GetState();
            if (currentTime.TimeOfDay > targetTime.TimeOfDay)
            {
                running = false;
            }

            //Checking to See if the game has passed its time
            if(running)
            {

                //Exits the game if escape is pressed
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                UpdateAsteroids();

                foreach (var sprite in _sprites.ToArray())
                {
                    sprite.Update(gameTime, _sprites);
                }

                for (int i = 0; i < _sprites.Count; i++)
                {

                    for (int j = 0; j < asteroids.Count; j++)
                    {
                        if (i > 0 && CollisionHelper.Collides(_sprites[i].bounds, asteroids[j].bounds))
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
            }
            if (!running)
            {
                if (keyboardState.IsKeyDown(Keys.R))
                {
                    running = true;
                    startGame();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront);
            backgroundSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, null);

            if (running)
            {
                foreach (Sprite a in asteroids)
                {
                    spriteBatch.Draw(a._texture, a.Position, null, Color.White, a._rotation, new Vector2(a.Width / 2, a.Height / 2), (float)0.5f, SpriteEffects.None, 1.0f);

                }

                foreach (var sprite in _sprites)
                {
                    sprite.Draw(spriteBatch);
                }

                gameBackground.Draw(gameTime, backgroundSpriteBatch);

                Double showTime = (targetTime - DateTime.Now).TotalSeconds;

                spriteBatch.DrawString(font, "Score: " + score, new Vector2(5, 5), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(font, "Time Remaining: " + showTime.ToString("#.00"), new Vector2(5, 25), Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            }
            if (!running)
            {
                var viewport = graphics.GraphicsDevice.Viewport;
                Rectangle source = new Rectangle(0, 0, viewport.Width, viewport.Height);
                spriteBatch.DrawString(endGamefont, "Score: " + score, new Vector2(230, 150), Color.White, 0, new Vector2(0, 0), 2.0f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(font, "Press R to restart game", new Vector2(200, 250), Color.White, 0, new Vector2(0, 0), 3.0f, SpriteEffects.None, 0f);
                backgroundSpriteBatch.Draw(purpleEnd, Vector2.Zero, source, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

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
        public int j = 0;
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

                
                if (running && CollisionHelper.Collides(_sprites[0].shipBounds, asteroids[i].bounds))
                {

                    asteroids.RemoveAt(i);
                    i--;
                    score--;
                    MakeOneAsteroid();
                    _sprites[0].color = Color.Red;
                    
                    playerHitSound.Play((float)_volume / 10, 0, 0);

                }
            }
            j++;
            if(j == 10 && running)
            {
                _sprites[0].color = Color.White;
                j = 0;
            }

        }
    }
}
