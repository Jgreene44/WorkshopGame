using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WorkshopGame
{
    public class Sprite : ICloneable
    {
        protected Texture2D _texture;
        public float _rotation;
        protected KeyboardState _currentKey;
        protected KeyboardState _previousKey;
        protected MouseState _currentMouse;
        protected MouseState _previousMouse;


        public Vector2 Position;
        public Vector2 Origin;

        public Vector2 Direction;
        public float RotationVelocity;
        public float LinearVelocity = 0.08f;

        public Sprite Parent;

        public float LifeSpan = 0f;

        public bool IsRemoved = false;

        public Sprite(Texture2D texture)
        {
            _texture = texture;
            Origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        }

        public virtual void Update(GameTime gameTime, List<Sprite> sprites)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, _rotation, Origin, 1, SpriteEffects.None, 0);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
