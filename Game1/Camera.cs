using System;
using Microsoft.Xna.Framework;

namespace Schlosskirsch
{
    public class Camera
    {
        private float speed;
        public Vector2 position = Vector2.Zero; //worldposition of the camera (in pixel)
        private int screenWidth;
        private int screenHeight;

        private int mapWidthInPx;
        private int mapHeightInPx;
        private int tileHeight;
        private int tileWidth;

        public int mapWidthInpx
        {
            get { return mapWidthInPx; }
            set { mapWidthInPx = value; }
        }

        /// <summary>
        /// returns the position on screen for the given point in world coordinates
        /// </summary>
        /// <param name="point">A point on the world, in tiles not pixel</param>
        /// <returns></returns>
        public Vector2 worldToScreen(Point point)
        {
            return new Vector2((position.X - (tileWidth * point.X)), (position.Y - (tileHeight * point.Y)));
        }

        public Vector2 worldToScreen(Vector2 worldPos)
        {
            return worldPos - position;
        }

        /// <summary>
        /// returns the position in the world (in tiles) for the given screenposition
        /// </summary>
        /// <param name="pos">A vector2 with the coordinates to translate</param>
        /// <returns></returns>
        public Point screenToWorld(Vector2 pos)
        {
            return new Point((int)((position.X + pos.X) / tileWidth), (int)((position.Y + pos.Y) / tileHeight));
        }

        public int mapHeightInpx
        {
            get { return mapHeightInPx; }
            set { mapHeightInPx = value; }
        }

        public int ScreenWidth
        {
            get { return screenWidth; }
        }

        public int ScreenHeight
        {
            get { return screenHeight; }
        }

        public Camera(float Speed, int ScreenWidth, int ScreenHeight, int TileWidth, int TileHeight)
        {
            speed = Speed;
            screenWidth = ScreenWidth;
            screenHeight = ScreenHeight;
            tileHeight = TileHeight;
            tileWidth = TileWidth;
        }

        public float Speed
        {
            get { return speed; }
            set { speed = (float)Math.Max(value, 1f); }
        }

        public void update(Vector2 motion)
        {
            /*
           if (motion != Vector2.Zero)
               motion.Normalize();
            */
            position += motion;

            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;

            if (position.X > mapWidthInPx - screenWidth)
                position.X = mapWidthInPx - screenWidth;
            if (position.Y > mapHeightInPx - screenHeight)
                position.Y = mapHeightInPx - screenHeight;
        }
    }
}