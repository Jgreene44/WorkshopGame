using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Runtime.InteropServices;

namespace WorkshopGame
{
    public class Spaceship
    {

        public Bullet bullet;

        public GamePadState gamePadState;

        private KeyboardState keyboardState;

        private Texture2D texture;

        public Vector2 position = new Vector2(400, 250);

        public Vector2 direction;

        private bool flipped;

        public float rotation = 0f;


        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("playerShip");
        }

        public void Update(GameTime gameTime, ContentManager content)
        {

            int movementScale = 3;


            gamePadState = GamePad.GetState(0);
            keyboardState = Keyboard.GetState();

            // Apply the gamepad movement with inverted Y axis
            position += gamePadState.ThumbSticks.Left * new Vector2(1, -1);
            //if (gamePadState.ThumbSticks.Left.X < 0) flipped = true;
            //if (gamePadState.ThumbSticks.Left.X > 0) flipped = true;

            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) position += new Vector2(0, -movementScale);
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) position += new Vector2(0, movementScale);
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                position += new Vector2(-movementScale, 0);
                //flipped = true;
            };
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                position += new Vector2(movementScale, 0);
                //flipped = false;
            }
            //TODO set limit


        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Direction is grabbing the Mouse Location X & Y then subtracting the position of the ship's X,Y
            direction = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - position;
            rotation = (float)Math.Atan2(direction.Y, direction.X);


            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, position, null, Color.White, (float)(rotation + (Math.PI * 0.5f)), new Vector2(50, 38), (float)0.5f, spriteEffects, 0);
            
        }
    }
}
