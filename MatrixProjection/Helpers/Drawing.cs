using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection.Helpers
{
    class Drawing
    {
        public static void DrawLine(Vector3 p1, Vector3 p2, Color color, int lineWeight)
        {
            if (p1 == Vector3.Zero || p2 == Vector3.Zero)
            {
                return;
            }

            Vector3 diff = p2 - p1;
            diff.Z = 0;
            float length = diff.Length();
            double angle = Math.Atan2(diff.Y, diff.X);


            Game1.spriteBatch.Draw(Game1.pixel,
                new Vector2(p1.X, p1.Y),
                new Rectangle(0, 0, (int)length, lineWeight),
                color,
                (float)angle,
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                1);

        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            DrawLine(p1, p2, new Color(150, 220, 255) * 0.7f, 2);
        }

        public static void DrawLine(Vector4 p1, Vector4 p2)
        {
            DrawLine(new Vector3(p1.X, p1.Y, p1.Z), new Vector3(p2.X, p2.Y, p2.Z));
        }
    }
}
