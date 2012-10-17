using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Pong_Protocol.Sprites;
using Pong_Protocol.Networking;

namespace Pong_Protocol {
    public enum PongState {
        Title,
        Game
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PongState state;
        Paddle me;
        Paddle you;
        Server server;
        Client client;
        bool keyPressed;

        public Game1() {
            server = null;
            state = PongState.Title;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        void OnServerConnect(object sender, EventArgs e) {
            client = Client.SetClient(server.client);

            me = new Paddle(this, new Vector2(720, 300));
            you = new Paddle(this, new Vector2(80, 300));
            Components.Add(me);
            Components.Add(you);

            state = PongState.Game;
        }
        void OnClientConnect(object sender, EventArgs e) {
            me = new Paddle(this, new Vector2(80, 300));
            you = new Paddle(this, new Vector2(720, 300));
            Components.Add(me);
            Components.Add(you);

            state = PongState.Game;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Allows the game to exit
            KeyboardState currentState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                this.Exit();
            }

            if (state == PongState.Title) {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !keyPressed) {
                    keyPressed = true;
                    Console.WriteLine("hit"); //LOG
                    server = Server.CreateSession("127.0.0.1");
                    server.onClientConnect += OnServerConnect;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space) && !keyPressed) {
                    keyPressed = true;
                    client = Client.Connect("127.0.0.1");
                    client.onClientConnect += OnClientConnect;                    
                }
                if (keyPressed &&
                    (Keyboard.GetState().IsKeyUp(Keys.Enter) && Keyboard.GetState().IsKeyUp(Keys.Space))) {
                    keyPressed = false;
                }

            } else if(state == PongState.Game) {
                if (you.Enabled) {
                    you.Enabled = false;
                }
                if (server != null) {
                    server.Send(me._info);
                } else {
                    if (client.recieve != null) {
                        you.Update(client.recieve.velocity);
                    }
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args) {
            base.OnExiting(sender, args);
        }
    }
}
