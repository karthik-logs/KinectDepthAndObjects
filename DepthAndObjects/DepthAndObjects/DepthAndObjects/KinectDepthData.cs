using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;

namespace DepthAndObjects
{
    class KinectDepthData
    {
        const int _ScreenWidth = 640;
        const int _ScreenHeight = 480;
        Game1 _Game;
        KinectSensor _kinect;

        byte[] _convertedPixels;

        public Texture2D DepthImage;
        public Color[,] pixelInfo;

        private const int _RedIndex = 2;
        private const int _GreenIndex = 1;
        private const int _BlueIndex = 0;

        public KinectDepthData(Game1 game)
        {
            _Game = game;
            pixelInfo = new Color[_ScreenWidth, _ScreenHeight];
        }

        public string InitKinect()
        {
            if(KinectSensor.KinectSensors.Count == 0)
            {
                return "Error : No Kinect Sensors Found";
            }
            _kinect = KinectSensor.KinectSensors[0];
            _kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            _kinect.DepthStream.Range = DepthRange.Near;
            _kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(kinect_DepthFrameReady);
            _kinect.Start();
            return "";
        }

        private void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using(DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                short[] pixelsFromFrame = new short[depthImageFrame.PixelDataLength];
                depthImageFrame.CopyPixelDataTo(pixelsFromFrame);
                _convertedPixels = ConvertDepthFrame(pixelsFromFrame, ((KinectSensor)sender).DepthStream, _ScreenWidth * _ScreenHeight * 4);
            }
        }

        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream, int depthFrame32Length)
        {
            int tooNearDepth = depthStream.TooNearDepth;
            int tooFarDepth = depthStream.TooFarDepth;
            int unknownDepth = depthStream.UnknownDepth;
            byte[] depthFrame32 = new byte[depthFrame32Length];
            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                byte intensity = (byte)(~(realDepth >> 4));
                if (player == 0 && realDepth == tooNearDepth)
                {
                    depthFrame32[i32 + _RedIndex] = 0;
                    depthFrame32[i32 + _GreenIndex] = 0;
                    depthFrame32[i32 + _BlueIndex] = 0;
                }
                else if (player == 0 && realDepth == 0)
                {
                    depthFrame32[i32 + _RedIndex] = 0;
                    depthFrame32[i32 + _GreenIndex] = 0;
                    depthFrame32[i32 + _BlueIndex] = 0;
                }
                else if (player == 0 && realDepth == tooFarDepth)
                {
                    depthFrame32[i32 + _RedIndex] = 0;
                    depthFrame32[i32 + _GreenIndex] = 0;
                    depthFrame32[i32 + _BlueIndex] = 0;
                }
                else if (player == 0 && realDepth == unknownDepth)
                {
                    depthFrame32[i32 + _RedIndex] = 0;
                    depthFrame32[i32 + _GreenIndex] = 0;
                    depthFrame32[i32 + _BlueIndex] = 0;
                }
                else
                {
                    depthFrame32[i32 + _RedIndex] = 1;
                    depthFrame32[i32 + _GreenIndex] = 0;
                    depthFrame32[i32 + _BlueIndex] = 50;
                }
            }
            return depthFrame32;
        }

        public void DrawDepthImage(SpriteBatch spriteBatch, GraphicsDevice device, Rectangle bounds)
        {
            if (_convertedPixels == null)
                return;
            DepthImage = new Texture2D(device, _ScreenWidth, _ScreenHeight);
            DepthImage.SetData<byte>(_convertedPixels);
            spriteBatch.Draw(DepthImage, bounds, Color.White);
            pixelInfo = TextureTo2DArray(DepthImage);
        }

        public Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[,] colors2D = new Color[texture.Width, texture.Height];
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);
            for(int x =0; x < texture.Width;x++)
            {
                for(int y=0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }
            return colors2D;
        }
    }
}
