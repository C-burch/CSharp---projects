using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FlappyBird
{
    public class Game1 : Game
    {
        // Graphics and Game Objects
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _birdTexture;
        private Texture2D _pipeTexture;
        private Vector2 _birdPosition;
        private float _birdVelocity;
        private List<Rectangle> _pipes;
        private float _pipeSpacing = 200;
        private float _pipeSpeed = 2f;
        private float _timeSinceLastPipe = 0f;
        private Random _random = new Random();
        private int _score;
        private SpriteFont _font;

        // Game constants
        private const int BirdWidth = 50;
        private const int BirdHeight = 50;
        private const int PipeWidth = 80;
        private const int PipeHeight = 400;
        private const int GroundHeight = 100;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _birdTexture = new Texture2D(GraphicsDevice, 1, 1);
            _birdTexture.SetData(new Color[] { Color.Yellow });
            _pipeTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pipeTexture.SetData(new Color[] { Color.Green });
            _font = Content.Load<SpriteFont>("Arial");

            _birdPosition = new Vector2(100, 250);
            _birdVelocity = 0f;
            _pipes = new List<Rectangle>();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bird movement and gravity
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                _birdVelocity = -5f; // Flap the bird
            }

            _birdVelocity += 0.25f; // Gravity effect
            _birdPosition.Y += _birdVelocity;

            // Prevent the bird from falling out of bounds
            if (_birdPosition.Y > _graphics.PreferredBackBufferHeight - GroundHeight - BirdHeight)
            {
                _birdPosition.Y = _graphics.PreferredBackBufferHeight - GroundHeight - BirdHeight;
                _birdVelocity = 0f;
            }

            if (_birdPosition.Y < 0)
            {
                _birdPosition.Y = 0;
                _birdVelocity = 0f;
            }

            // Pipe logic
            _timeSinceLastPipe += elapsedTime;
            if (_timeSinceLastPipe > 2f) // Generate new pipe every 2 seconds
            {
                _pipes.Add(CreatePipe());
                _timeSinceLastPipe = 0f;
            }

            // Move pipes
            for (int i = 0; i < _pipes.Count; i++)
            {
                _pipes[i] = new Rectangle(_pipes[i].X - (int)_pipeSpeed, _pipes[i].Y, _pipes[i].Width, _pipes[i].Height);
            }

            // Remove pipes that have gone off-screen
            _pipes.RemoveAll(p => p.X + PipeWidth < 0);

            // Collision detection
            Rectangle birdRectangle = new Rectangle((int)_birdPosition.X, (int)_birdPosition.Y, BirdWidth, BirdHeight);
            foreach (var pipe in _pipes)
            {
                if (birdRectangle.Intersects(pipe))
                {
                    // Game Over
                    _birdVelocity = 0f; // Stop the bird
                    _pipes.Clear(); // Clear pipes
                    _score = 0; // Reset score
                }
            }

            // Check if bird has passed through a pipe
            foreach (var pipe in _pipes)
            {
                if (pipe.X + PipeWidth < _birdPosition.X && !pipe.Equals(_pipes[0]))
                {
                    _score++;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Cyan);

            _spriteBatch.Begin();

            // Draw bird
            _spriteBatch.Draw(_birdTexture, _birdPosition, new Rectangle(0, 0, BirdWidth, BirdHeight), Color.White);

            // Draw pipes
            foreach (var pipe in _pipes)
            {
                _spriteBatch.Draw(_pipeTexture, pipe, Color.White);
            }

            // Draw score
            _spriteBatch.DrawString(_font, $"Score: {_score}", new Vector2(10, 10), Color.Black);

            // Draw ground (a simple bar at the bottom)
            _spriteBatch.Draw(_pipeTexture, new Rectangle(0, _graphics.PreferredBackBufferHeight - GroundHeight, _graphics.PreferredBackBufferWidth, GroundHeight), Color.Brown);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Rectangle CreatePipe()
        {
            int height = _random.Next(100, 300);
            return new Rectangle(_graphics.PreferredBackBufferWidth, 0, PipeWidth, height); // Top pipe
        }
    }
}
