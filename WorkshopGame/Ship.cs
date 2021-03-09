using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WorkshopGame
{
    public class Ship : Sprite
    {
        public Bullet Bullet;

        public Ship(Texture2D texture)
            : base(texture)
        {


        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            int movementScale = 3;

            if (_currentKey.IsKeyDown(Keys.Up) || _currentKey.IsKeyDown(Keys.W)) Position += new Vector2(0, -movementScale);
            if (_currentKey.IsKeyDown(Keys.Down) || _currentKey.IsKeyDown(Keys.S)) Position += new Vector2(0, movementScale);
            if (_currentKey.IsKeyDown(Keys.Left) || _currentKey.IsKeyDown(Keys.A))
            {
                Position += new Vector2(-movementScale, 0);
                //flipped = true;
            };
            if (_currentKey.IsKeyDown(Keys.Right) || _currentKey.IsKeyDown(Keys.D))
            {
                Position += new Vector2(movementScale, 0);
                //flipped = false;
            }

            //Direction is grabbing the Mouse Location X & Y then subtracting the position of the ship's X,Y
            Direction = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - Position;
            _rotation = (float)Math.Atan2(Direction.Y, Direction.X);

            if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton != ButtonState.Pressed)
            {
                var bullet = Bullet.Clone() as Bullet;
                bullet.Direction = this.Direction;
                bullet.Position = this.Position;
                bullet.LinearVelocity = 0.08f;
                bullet.LifeSpan = 2f;
                bullet._rotation = this._rotation + (float)Math.PI / 2.0f;

                sprites.Add(bullet);
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Direction is grabbing the Mouse Location X & Y then subtracting the position of the ship's X,Y
            Vector2 direction = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) - Position;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            spriteBatch.Draw(_texture, Position, null, Color.White, (float)(rotation + (Math.PI * 0.5f)), new Vector2(50, 38), (float)0.5f, SpriteEffects.None, 1);
        }
    }

    
}
