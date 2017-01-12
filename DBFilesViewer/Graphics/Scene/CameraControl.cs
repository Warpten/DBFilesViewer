using System;
using System.Linq;
using System.Windows.Forms;
using DBFilesViewer.Utils;
using SharpDX;
using Point = System.Drawing.Point;

namespace DBFilesViewer.Graphics.Scene
{
    public class CameraControl
    {
        public delegate void PositionChangedHandler(Vector3 newPosition, bool updateTerrain);

        private readonly Control mWindow;
        private Point mLastCursorPos;
        private DateTime mLastUpdate = DateTime.Now;

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }

        public float TurnFactor { get; set; }
        public float SpeedFactor { get; set; } = 100.0f;
        public float SpeedFactorWheel { get; set; } = 0.5f;

        public event PositionChangedHandler PositionChanged;

        public CameraControl(Control window)
        {
            TurnFactor = 0.2f;
            mWindow = window;
        }

        public void ForceUpdate(Vector3 position)
        {
            PositionChanged?.Invoke(position, true);
        }

        public bool Update(Camera cam, bool stateOnly)
        {
            if (mWindow.Focused == false || stateOnly)
            {
                mLastCursorPos = Cursor.Position;
                mLastUpdate = DateTime.Now;
                return false;
            }

            var positionChanged = false;
            var updateTerrain = false;
            var diff = (float)(DateTime.Now - mLastUpdate).TotalSeconds;

            var mSpeedFactor = SpeedFactor;
            if (KeyHelper.IsAnyKeyDown(Keys.Shift))
                mSpeedFactor *= 1.25f;

            if (KeyHelper.IsAnyKeyDown(Keys.W, Keys.Up))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(diff * mSpeedFactor);
            }

            if (KeyHelper.IsAnyKeyDown(Keys.S, Keys.Down))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveForward(-diff * mSpeedFactor);
            }

            if (KeyHelper.IsAnyKeyDown(Keys.D, Keys.Right))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(diff * mSpeedFactor);
            }

            if (KeyHelper.IsAnyKeyDown(Keys.A, Keys.Left))
            {
                positionChanged = true;
                updateTerrain = true;
                cam.MoveRight(-diff * mSpeedFactor);
            }

            if (KeyHelper.IsAnyKeyDown(Keys.Space))
            {
                positionChanged = true;
                cam.MoveUp(diff * mSpeedFactor);
            }

            if (KeyHelper.IsAnyKeyDown(Keys.X))
            {
                positionChanged = true;
                cam.MoveUp(-diff * mSpeedFactor);
            }

            var viewpointChanged = false;
            if (KeyHelper.IsAnyKeyDown(Keys.RButton) && !KeyHelper.IsAnyKeyDown(Keys.ControlKey) && !KeyHelper.IsAnyKeyDown(Keys.Menu))
            {
                var curPos = Cursor.Position;
                var dx = curPos.X - mLastCursorPos.X;
                var dy = curPos.Y - mLastCursorPos.Y;

                if (dx != 0)
                    cam.Yaw(dx * TurnFactor * (InvertX ? 1 : -1));

                if (dy != 0)
                    cam.Pitch(dy * TurnFactor * (InvertY ? 1 : -1));

                viewpointChanged = dx != 0 || dy != 0;
            }

            if (positionChanged)
                PositionChanged?.Invoke(cam.Position, updateTerrain);

            mLastUpdate = DateTime.Now;
            mLastCursorPos = Cursor.Position;
            return positionChanged || viewpointChanged;
        }
    }

    public static class KeyHelper
    {
        /// <summary>
        /// Returns true if any of the keys in the specified argument list is down
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static bool IsAnyKeyDown(params Keys[] keys)
        {
            if (keys.Length == 0)
                return false;

            return keys.Select(key => UnsafeNativeMethods.GetKeyState((int) key)).Any(retVal => (retVal & 0x8000) == 0x8000);
        }

        public static bool IsAnyKeyToggled(params Keys[] keys)
        {
            if (keys.Length == 0)
                return false;

            return keys.Select(key => UnsafeNativeMethods.GetKeyState((int)key)).Any(retVal => (retVal & 1) == 1);
        }
    }
}
