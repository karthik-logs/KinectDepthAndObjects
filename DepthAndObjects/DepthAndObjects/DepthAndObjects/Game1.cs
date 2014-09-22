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

namespace DepthAndObjects
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int _ScreenWidth = 640;
        const int _ScreenHeight = 480;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KinectDepthData _depthData;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _depthData = new KinectDepthData(this);
            graphics.PreferredBackBufferWidth = _ScreenWidth;
            graphics.PreferredBackBufferHeight = _ScreenHeight;
            graphics.ApplyChanges();
            String error = _depthData.InitKinect();
            if(error != "")
            {
                Exit();
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.Opaque);
            _depthData.DrawDepthImage(spriteBatch,GraphicsDevice, new Rectangle(0,0,_ScreenWidth,_ScreenHeight));
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
