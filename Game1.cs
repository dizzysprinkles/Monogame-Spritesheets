using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monogame_Spritesheets
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D characterSpritesheet, rectangleTexture;
        Rectangle window;
        KeyboardState keyboardState;

        List<Rectangle> barriers;

        int rows, columns;
        int frame; //Frame number (column) to be drawn
        int frames; //Number of frames in each column
        int directionRow; // Row # containing frames for current direction (up, down, left, right)
        int leftRow, rightRow, upRow, downRow;
        int width; //of each frame
        int height; //of each frame

        float speed; //speed of player
        float time; // store elasped time
        float frameSpeed; // how fast player frames transition; kinda like animation speed from summative

        Vector2 playerLocation; // location of player collision sprite
        Vector2 playerDirection; //direction vector of player!\

        Rectangle playerCollisionRect, playerDrawRect;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            window = new Rectangle(0, 0, 800, 500);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(100,100,30,150));
            barriers.Add(new Rectangle(10, 400,300,30));
            barriers.Add(new Rectangle(400, 200, 200, 30));
            barriers.Add(new Rectangle(700, 450, 30, 100));

            speed = 1.5f;

            columns = 9;
            rows = 4;
            upRow = 0;
            leftRow = 1;
            downRow = 2;
            rightRow = 3;

            time = 0.0f;
            frameSpeed = 0.08f;
            frames = 9;
            frame = 0;


            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(20, 20, 20, 48);
            playerDrawRect = new Rectangle(20, 20, 50, 65);
            UpdatePlayerRects(); //Updates the rectangles so the hitbox surrounds our sprite's draw rect 

            base.Initialize();
            directionRow = downRow; // Player will start facing down
            width = characterSpritesheet.Width / columns;
            height = characterSpritesheet.Height / rows;

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            characterSpritesheet = Content.Load<Texture2D>("Images/skeleton_spritesheet");
            rectangleTexture = Content.Load<Texture2D>("Images/rectangle");
        }

        protected override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame += 1;
                if (frame >= frames)
                    frame = 0;
            }

            keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SetPlayerDirection();
            playerLocation += playerDirection * speed;
            UpdatePlayerRects();



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (Rectangle barrier in barriers)
                _spriteBatch.Draw(rectangleTexture, barrier, Color.Black);

            _spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f); //hit box for testing purposes
            _spriteBatch.Draw(characterSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void UpdatePlayerRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 15;
            playerDrawRect.Y = playerCollisionRect.Y - 15;
        }

        private void SetPlayerDirection()
        {
            playerDirection = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A))
                playerDirection.X += -1;
            if (keyboardState.IsKeyDown(Keys.D))
                playerDirection.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                playerDirection.Y += -1;
            if (keyboardState.IsKeyDown(Keys.S))
                playerDirection.Y += 1;

            if (playerDirection != Vector2.Zero)
            {
                playerDirection.Normalize();
                if (playerDirection.X < 0) // Moving left
                    directionRow = leftRow;
                else if (playerDirection.X > 0) // Moving right
                    directionRow = rightRow;
                else if (playerDirection.Y < 0) // Moving up
                    directionRow = upRow;
                else
                    directionRow = downRow; // Moving down

            }
            else
                frame = 0;
        }
    }
}
