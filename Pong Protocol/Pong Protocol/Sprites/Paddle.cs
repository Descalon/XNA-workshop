using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Pong_Protocol.Sprites {
    [Serializable]
    public class PaddleInfo : ISerializable {
        public Vector2 velocity;
        public PaddleInfo() {
        }
        protected PaddleInfo(SerializationInfo info, StreamingContext context) {
            if (info == null) {
                throw new ArgumentNullException("info can't be null!");
            }
            velocity = (Vector2)info.GetValue("velocity", typeof(Vector2));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            if (info == null) {
                throw new ArgumentNullException("info can't be null!");
            }
            info.AddValue("velocity", velocity);
        }
    }
    public class Paddle : DrawableGameComponent {
        private SpriteBatch _sb;
        public PaddleInfo _info;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 drag;
        public Texture2D image;
        public Rectangle box;

        public Paddle(Game game, Vector2 position)
            : base(game) {
            this.position = position;
        }
        public override void Initialize() {
            drag = new Vector2(3, 3);
            _sb = new SpriteBatch(Game.GraphicsDevice);
            _info = new PaddleInfo();
            base.Initialize();
        }
        protected override void LoadContent() {
            image = (Texture2D)Game.Content.Load<Texture2D>("rect");
            box = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime) {
            _sb.Begin();
            _sb.Draw(image, position, Color.White);
            _sb.End();

            base.Draw(gameTime);
        }
        public void Update(Vector2 velocity) {
            if (Enabled) {
                Enabled = false;
            }
            position += velocity;

            if (position.Y >= Game.GraphicsDevice.Viewport.Height - box.Height) {
                position.Y = Game.GraphicsDevice.Viewport.Height - box.Height;
            }
            if (position.Y <= 0) {
                position.Y = 0;
            }
        }

        public override void Update(GameTime gameTime) {

            box.X = (int)position.X;
            box.Y = (int)position.Y;
            velocity = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                velocity.Y -= drag.Y;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                velocity.Y += drag.Y;
            }

            position += velocity;
            _info.velocity = velocity;

            if (position.Y >= Game.GraphicsDevice.Viewport.Height - box.Height) {
                position.Y = Game.GraphicsDevice.Viewport.Height - box.Height;
            }
            if (position.Y <= 0) {
                position.Y = 0;
            }
            base.Update(gameTime);
        }
    }
}
