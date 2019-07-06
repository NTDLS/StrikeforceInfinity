﻿using AI2D.GraphicObjects;
using AI2D.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AI2D.Engine
{
    public class Utility
    {
        const double RADIAN_CONV = Math.PI / 180.0;
        const double OFFSET90DEGREES = 180.0 * RADIAN_CONV;
        const double FULLCIRCLE = 360.0 * RADIAN_CONV;

        #region Graphics.

        public static Bitmap RotateImageWithClipping(Bitmap bmp, double angle, Color backgroundColor)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height, backgroundColor == Color.Transparent ?
                                             PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);

            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Fill in the specified background color if necessary
                if (backgroundColor != Color.Transparent)
                {
                    g.Clear(backgroundColor);
                }

                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform((float)angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }

        public static Bitmap RotateImageWithUpsize(Image inputImage, double angleDegrees, Color backgroundColor)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)inputImage.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = inputImage.Width;
            int oldHeight = inputImage.Height;

            double angleRadians = angleDegrees * Math.PI / 180d;
            double cos = Math.Abs(Math.Cos(angleRadians));
            double sin = Math.Abs(Math.Sin(angleRadians));
            int newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
            int newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);

            // Create the new bitmap object. If background color is transparent it must be 32-bit, 
            //  otherwise 24-bit is good enough.
            Bitmap newBitmap = new Bitmap(newWidth, newHeight, backgroundColor == Color.Transparent ?
                                             PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            // Create the Graphics object that does the work
            using (Graphics graphicsObject = Graphics.FromImage(newBitmap))
            {
                graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                // Fill in the specified background color if necessary
                if (backgroundColor != Color.Transparent)
                    graphicsObject.Clear(backgroundColor);

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                graphicsObject.RotateTransform((float)angleDegrees);
                graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result 
                graphicsObject.DrawImage(inputImage, 0, 0);
            }

            return newBitmap;
        }

        public static Image ResizeImage(Image image, int new_height, int new_width)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        #endregion

        #region Math.

        public static double RequiredAngleTo(BaseGraphicObject from, BaseGraphicObject to)
        {
            return RequiredAngleTo(from.Location, to.Location);
        }

        public static double RequiredAngleTo(PointD from, PointD to)
        {
            var fRadians = Math.Atan2((to.Y - from.Y), (to.X - from.X));
            var fDegrees = ((fRadians * (180 / Math.PI) + 360) + 90) % 360;
            return fDegrees;
        }

        public static bool IsPointingAt(BaseGraphicObject fromObj, BaseGraphicObject atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(GetDeltaAngle(fromObj, atObj));
            return deltaAngle < toleranceDegrees;
        }

        public static double GetDeltaAngle(BaseGraphicObject fromObj, BaseGraphicObject atObj)
        {
            double angleTo = RequiredAngleTo(fromObj, atObj);

            if (fromObj.Velocity.Angle.Degree < 0) fromObj.Velocity.Angle.Degree = (0 - fromObj.Velocity.Angle.Degree);
            if (angleTo < 0) angleTo = (0 - angleTo);

            return fromObj.Velocity.Angle.Degree - angleTo;
        }

        public static PointD AngleToXY(double angle)
        {
            double radians = (Math.PI / 180) * (angle - 90);

            PointD result = new PointD()
            {
                X = Math.Cos(radians),
                Y = Math.Sin(-radians)
            };

            return result;
        }

        public static double CalculeDistance(PointD from, PointD to)
        {
            var deltaX = Math.Pow((to.X - from.X), 2);
            var deltaY = Math.Pow((to.Y - from.Y), 2);

            var distance = Math.Sqrt(deltaY + deltaX);

            return distance;
        }

        public static double CalculeDistance(BaseGraphicObject from, BaseGraphicObject to)
        {
            return CalculeDistance(from.Location, to.Location);
        }

        #endregion

        #region Random.

        public static Random Random = new Random();
        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static Double RandomNumber(double min, double max)
        {
            return Random.Next(0, 1000) % max;
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(0, 1000) % max;
        }

        #endregion

    }
}

