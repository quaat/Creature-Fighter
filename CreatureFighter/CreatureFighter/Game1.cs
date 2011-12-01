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

namespace CreatureFighter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Song mySong;
        private Texture2D backgroundTexture;

      public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
        }

      Texture2D playerOneTexture;
      Texture2D playerTwoTexture;
      Texture2D enemyTexture;
      UInt16 lives = 20;
      Vector2 playerOnePosition; // = new Vector2(20.0f, 20.0f);
      Vector2 playerTwoPosition; // = Vector2(;
      SpriteFont scoreFont;
      SoundEffect bite;

      Vector2 enemyPosition = Vector2.Zero;
      Vector2 enemySpeed = new Vector2(450.0f);
      GamePadState playerOne;
      GamePadState playerTwo;
      float speed = 10.0f;

      BoundingBox playerOneBox;
      BoundingBox playerTwoBox;
      BoundingBox enemyBox;

      List<MenuWindow> menuList;  
      //Video video;
      //VideoPlayer videoPlayer;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
                     // TODO: Add your initialization logic here
          //  description = Gamer.SignedInGamers[0].GetProfile().;
          //  renderer = new AvatarRenderer(description);
          //  animation = new AvatarAnimation(AvatarAnimationPreset.FemaleWave);
            enemyPosition.X = 500;
            enemyPosition.Y = 500;

            
            playerOnePosition = new Vector2(20.0f, 20.0f);
            playerTwoPosition = new Vector2(300, 10);
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>


        private MenuWindow startGameEasy;
        private MenuWindow startGameHard;
        private bool menuIsRunning = true;
        private MenuWindow menuMain;
        private MenuWindow menuNewGame;
        private MenuWindow menuExitGame;
     
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //anakinTexture = Content.Load<Texture2D>("anakin");
            playerOneTexture = Content.Load<Texture2D>("drago");
            playerTwoTexture = Content.Load<Texture2D>("greeny");
            enemyTexture = Content.Load<Texture2D>("teiting");
            scoreFont = Content.Load<SpriteFont>("scorefont");
            bite = Content.Load<SoundEffect>("bite");
            mySong = Content.Load<Song>("Kalimba");
            backgroundTexture = Content.Load<Texture2D>("background1");
            //video = Content.Load<Video>("Wildlife");
            //videoPlayer = new VideoPlayer();
            //videoPlayer.Play(video);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(mySong);

            menuList = new List<MenuWindow>();
            menuMain = new MenuWindow(scoreFont, "Creature Fighter", backgroundTexture);
            menuNewGame = new MenuWindow(scoreFont, "Start a New Game", backgroundTexture);
            menuList.Add(menuMain);
            menuList.Add(menuNewGame);
            
            startGameEasy = new MenuWindow(null, null, null);
            startGameHard = new MenuWindow(null, null, null);
            menuExitGame = new MenuWindow(null, null, null);
            menuNewGame.AddMenuItem("Easy", startGameEasy);
            menuNewGame.AddMenuItem("Hard", startGameHard);
            menuNewGame.AddMenuItem("Back to Main Menu", menuMain);
            
            
            menuMain.AddMenuItem("New Game", menuNewGame);
            menuMain.AddMenuItem("Exit", menuExitGame);
            menuMain.WakeUp();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected void CheckCollisions()
        {
               
        }

        MenuWindow activeMenu = null;
#if (WINDOWS)
        KeyboardState lastState;
        private void MenuInput(KeyboardState currentState)
#elif (XBOX)
        GamePadState lastState;
        private void MenuInput(GamePadState currentState)
#endif        
        {
            if (activeMenu == null) activeMenu = menuMain;
            MenuWindow newActive = activeMenu.ProcessInput(lastState, currentState);

            if (newActive == menuExitGame)
            {
                this.Exit();
            }

            if (newActive == startGameEasy)
            {
                // set Level to Easy
                menuIsRunning = false;
                enemySpeed = new Vector2(450.0f);
            }

            if (newActive == startGameHard)
            {
                // set level to hard
                menuIsRunning = false;
                enemySpeed = new Vector2(650.0f);
            }

            if (newActive == null)
                this.Exit();

            else if (newActive != activeMenu)
                newActive.WakeUp();

            activeMenu = newActive;

            lastState = currentState;
        }

        Boolean direction = true;
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
#if (WINDOWS)
            KeyboardState currState = Keyboard.GetState();  
#elif(XBOX)
            GamePadState currState = GamePad.GetState(PlayerIndex.One);
#endif
            if (menuIsRunning)
            {
                foreach (MenuWindow currentMenu in menuList)
                    currentMenu.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
                MenuInput(currState);
            }
            else
            {
                CheckCollisions();
                playerOneBox = new BoundingBox(new Vector3(playerOnePosition.X, playerOnePosition.Y, 0),
                    new Vector3(playerOnePosition.X + playerOneTexture.Width, playerOnePosition.Y + playerOneTexture.Height, 0));

                playerTwoBox = new BoundingBox(new Vector3(playerTwoPosition.X, playerTwoPosition.Y, 0),
                    new Vector3(playerTwoPosition.X + playerTwoTexture.Width, playerTwoPosition.Y + playerTwoTexture.Height, 0));

                enemyBox = new BoundingBox(new Vector3(enemyPosition.X, enemyPosition.Y, 0),
                    new Vector3(enemyPosition.X + enemyTexture.Width, enemyPosition.Y + enemyTexture.Height, 0));

                if ((playerOneBox.Intersects(enemyBox)) || (playerTwoBox.Intersects(enemyBox)))
                {
                    bite.Play();
                    lives--;
                    // enemySpeed.X *= 1.1f ;
                    //enemySpeed.Y *= 1.1f ;
                    if (lives <= 0) {
                        menuIsRunning = true;
                        activeMenu = menuMain;
                        activeMenu.WakeUp();
                        lives = 20;
                        return;
                    }                     
                    else
                        direction = !direction;
                }


                // speed += gameTime.ElapsedGameTime.Seconds * 0.1f;

                playerOne = GamePad.GetState(PlayerIndex.One);
                playerTwo = GamePad.GetState(PlayerIndex.Two);

                int MaxX = graphics.GraphicsDevice.Viewport.Width - enemyTexture.Width;
                int MinX = 0;
                int MaxY = graphics.GraphicsDevice.Viewport.Height - enemyTexture.Height;
                int MinY = 0;

                if (playerOnePosition.X < MinX) playerOnePosition.X = MinX;
                if (playerOnePosition.Y < MinY) playerOnePosition.Y = MinY;
                if (playerTwoPosition.X < MinX) playerTwoPosition.X = MinX;
                if (playerTwoPosition.Y < MinY) playerTwoPosition.Y = MinY;

                if (playerOnePosition.X <= MaxX)
                    playerOnePosition.X += Math.Abs(speed) * playerOne.ThumbSticks.Left.X;
                else playerOnePosition.X = MaxX;

                if (playerTwoPosition.X <= MaxX)
                    playerTwoPosition.X += Math.Abs(speed) * playerTwo.ThumbSticks.Left.X;
                else playerTwoPosition.X = MaxX;

                if (playerOnePosition.Y <= MaxY)
                    playerOnePosition.Y -= Math.Abs(speed) * playerOne.ThumbSticks.Left.Y;
                else playerOnePosition.Y = MaxY;

                if (playerTwoPosition.Y <= MaxY)
                    playerTwoPosition.Y -= Math.Abs(speed) * playerTwo.ThumbSticks.Left.Y;
                else playerTwoPosition.Y = MaxY;
                // TODO: Add your update logic here
                // animation.Update(gameTime.ElapsedGameTime);
                if (direction)
                    enemyPosition += enemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else
                    enemyPosition -= enemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                // TODO: Add your update logic here



                // Check for bounce.
                if (enemyPosition.X > MaxX)
                {
                    enemySpeed.X *= -1;
                    enemyPosition.X = MaxX;
                }

                else if (enemyPosition.X < MinX)
                {
                    enemySpeed.X *= -1;
                    enemyPosition.X = MinX;
                }

                if (enemyPosition.Y > MaxY)
                {
                    enemySpeed.Y *= -1;
                    enemyPosition.Y = MaxY;
                }

                else if (enemyPosition.Y < MinY)
                {
                    enemySpeed.Y *= -1;
                    enemyPosition.Y = MinY;
                }

                //    if (playerOneBox.Intersects(playerTwoBox))
                //    {
                //        Vector2 tempPos = playerOnePosition;

                //       playerOnePosition = playerTwoPosition;
                //       playerTwoPosition = tempPos;
                //      playerTwoPosition.X += 200.0f;


                //  }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

      
        protected override void Draw(GameTime gameTime)
        {
            //destinationRectangle.Width = 800;
            //destinationRectangle.Height = 600;
            //spriteBatch.Begin();
            //spriteBatch.Draw(videoPlayer.GetTexture(), destinationRectangle, Color.White);
            //spriteBatch.End();

            GraphicsDevice.Clear(Color.White);


            if (menuIsRunning)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                foreach (MenuWindow currentMenu in menuList)
                    currentMenu.Draw(spriteBatch);
                spriteBatch.End();
                Window.Title = "Menu is running";

            }
            else
            {
                // TODO: Add your drawing code here
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                foreach (MenuWindow currentMenu in menuList) currentMenu.Draw(spriteBatch);

                spriteBatch.Draw(this.backgroundTexture, Vector2.Zero, Color.White);
                //spriteBatch.Draw(anakinTexture, Vector2.Zero, Color.Transparent;
                spriteBatch.Draw(playerOneTexture, playerOnePosition, Color.White);
                spriteBatch.Draw(playerTwoTexture, playerTwoPosition, Color.White);
                spriteBatch.Draw(enemyTexture, enemyPosition, Color.White);
                spriteBatch.DrawString(scoreFont, "Lives: " + lives, new Vector2(600, 20), Color.White);
                spriteBatch.DrawString(scoreFont, "Lives: " + lives, new Vector2(601, 21), Color.Blue);
                spriteBatch.End();

                Window.Title = "Game is running";
            }
            
            base.Draw(gameTime);
        }
    }
}
