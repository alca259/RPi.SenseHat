using System;
using Windows.UI;
using Emmellsoft.IoT.Rpi.SenseHat;

namespace RPi.SenseHat.Demo.Demos
{
    /// <summary>
    /// Single color scroll-text.
    /// Click on the joystick to change drawing mode!
    /// </summary>
    public class BinaryClock2 : SenseHatDemo
    {
        private string lstrScreenText;                     // String used to update the ScreenText
        private static readonly Color ColorOff = Colors.Black;

        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="senseHat"></param>
        /// <param name="scrollText"></param>
		public BinaryClock2(ISenseHat senseHat, Action<string> scrollText)
            : base(senseHat, scrollText)
        {
        }

        public override void Run()
        {
            // Flip the Display (I wanted to watch it mirrored)
            SenseHat.Display.FlipHorizontal = true;
            SenseHat.Display.FlipVertical = true;

            while (isRunning)
            {
                // do that
                UpdateBinaryClock(DateTime.Now);

                // Change the Screentext to DateTime now.
                lstrScreenText = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");

                // set the Text of the GUI
                SetScreenText?.Invoke(lstrScreenText);

                // Pause for a short while.
                Sleep(TimeSpan.FromMilliseconds(200));
            }
        }

        /// <summary>
        /// This updates a Binary Bit @[xPos, yPos]
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="value">Byte Value</param>
        /// <param name="checkedValue">Pattern to check against</param>
        /// <param name="lColor">If Bit should be set then True then set this Color</param>
        /// <param name="DefaultColor"> ... else use this Color</param>
        private void UpdateBinaryBit(byte xPos, byte yPos, byte value, byte checkedValue, Color lColor, Color DefaultColor)
        {
            if ((value & checkedValue) != 0)
            {
                SenseHat.Display.Screen[xPos, yPos] = lColor; // Draw the pixel.
            }
            else
            {
                SenseHat.Display.Screen[xPos, yPos] = DefaultColor; // Delete the pixel.
            }
        }

        /// <summary>
        /// This sub updates a row of the Display
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="value"></param>
        /// <param name="lColor"></param>
        private void UpdateBinaryDigit(byte xPos, byte value, Color lColor)
        {
            byte yPos = 0;
            Color defaultColor = ColorOff;

            UpdateBinaryBit(xPos, yPos, value, 1, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 2, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 4, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 8, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 16, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 32, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 64, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 128, lColor, defaultColor);

            // If the Value is 0 use an "E"
            if (value == 0)
            {
                UpdateBinaryDigit(xPos, 15, lColor);
            }
        }

        /// <summary>
        /// This updates only the upper part of a Row
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="value"></param>
        /// <param name="lColor"></param>
        private void UpdateUpperBinaryDigit(byte xPos, byte value, Color lColor)
        {
            byte yPos = 4;
            Color defaultColor = ColorOff;

            UpdateBinaryBit(xPos, yPos, value, 1, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 2, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 4, lColor, defaultColor); yPos++;
            UpdateBinaryBit(xPos, yPos, value, 8, lColor, defaultColor);


            // If the Value is 0 use an "E"
            if (value == 0)
            {
                UpdateUpperBinaryDigit(xPos, 15, lColor);
            }
        }

        /// <summary>
        /// This updates the binary Clock with myTime. Change Colors here, if you want to
        /// </summary>
        /// <param name="myTime"></param>
        private void UpdateBinaryClock(DateTime myTime)
        {
            byte yPos = 0;
            Color myColor = Colors.MidnightBlue;

            // overwrite unused Rows
            UpdateBinaryDigit(0, 15, ColorOff);
            UpdateBinaryDigit(1, 0, ColorOff);

            // Show Time
            UpdateBinaryDigit(2, (byte)Math.Floor((double)myTime.Hour / 10), myColor);
            UpdateBinaryDigit(3, (byte)(myTime.Hour % 10), myColor);
            myColor = Colors.SlateBlue;
            UpdateBinaryDigit(4, (byte)Math.Floor((double)myTime.Minute / 10), myColor);
            UpdateBinaryDigit(5, (byte)(myTime.Minute % 10), myColor);
            myColor = Colors.Aqua;
            UpdateBinaryDigit(6, (byte)Math.Floor((double)myTime.Second / 10), myColor);
            UpdateBinaryDigit(7, (byte)(myTime.Second % 10), myColor);

            // Show Date in upper Row
            yPos = 0;
            myColor = Colors.Yellow;
            UpdateUpperBinaryDigit(yPos, (byte)(Math.Floor((double)myTime.Year / 1000) % 10), myColor); yPos++;
            UpdateUpperBinaryDigit(yPos, (byte)(Math.Floor((double)myTime.Year / 100) % 10), myColor); yPos++;
            UpdateUpperBinaryDigit(yPos, (byte)Math.Floor(((double)myTime.Year / 10) % 10), myColor); yPos++;
            UpdateUpperBinaryDigit(yPos, (byte)(myTime.Year % 10), myColor); yPos++;
            myColor = Colors.Orange;
            UpdateUpperBinaryDigit(yPos, (byte)Math.Floor((double)myTime.Month / 10), myColor); yPos++;
            UpdateUpperBinaryDigit(yPos, (byte)(myTime.Month % 10), myColor); yPos++;
            myColor = Colors.Red;
            UpdateUpperBinaryDigit(yPos, (byte)Math.Floor((double)myTime.Day / 10), myColor); yPos++;
            UpdateUpperBinaryDigit(yPos, (byte)(myTime.Day % 10), myColor);

            SenseHat.Display.Update(); // Update the physical display
        }
    }
}