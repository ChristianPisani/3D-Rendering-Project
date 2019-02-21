using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Cylinder : GameObject
    {
        public int divisions;
        public int heightDivisions = 1;

        public Color color = Color.Pink; 

        public Cylinder(Vector3 pos, Vector3 size, int divisions) : base(pos, size)
        {
            this.divisions = divisions;

            MapPoints();
        }

        public void MapPoints()
        {
            var points = new List<Vector3>();

            var step = (Math.PI * 2f) / (divisions);

            for (int x = 0; x < divisions; x++)
            {                
                points.Add(new Vector3(((float)Math.Cos(step * x) * 0.5f), -1f, ((float)Math.Sin(step * x) * 0.5f)));
                points.Add(new Vector3(((float)Math.Cos(step * x) * 0.5f), 0f, ((float)Math.Sin(step * x) * 0.5f)));
            }
        }


        /*public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            List<Vector3> mappedPoints = getMappedPoints(camera);            

            for (int i = 0; i < mappedPoints.Count; i++)
            {
                var p = mappedPoints[i];


                if (i < divisions)
                {
                    var nextP = mappedPoints[i + divisions];
                    Drawing.DrawLine(p, nextP, color, 2);
                }

                if(i < divisions * 2 - 2)
                {
                    Drawing.DrawLine(p, mappedPoints[i + 2], color, 2);
                    //Drawing.DrawLine(mappedPoints[i + divisions], mappedPoints[i + divisions + 2]);
                }

                if(i % 2 == 0)
                {
                    Drawing.DrawLine(p, mappedPoints[i + 1], color, 2);
                }

                Drawing.DrawLine(mappedPoints[0], mappedPoints[mappedPoints.Count-2], color, 2);
                Drawing.DrawLine(mappedPoints[1], mappedPoints[mappedPoints.Count-1], color, 2);

                //spriteBatch.Draw(Game1.pixel, new Rectangle((int)p.X, (int)p.Y, 10, 10), Color.White);
            }

            Drawing.DrawLine(mappedPoints[0], mappedPoints[1]);
            Drawing.DrawLine(mappedPoints[1], mappedPoints[2]);
            Drawing.DrawLine(mappedPoints[2], mappedPoints[3]);
            Drawing.DrawLine(mappedPoints[3], mappedPoints[0]);

            Drawing.DrawLine(mappedPoints[4], mappedPoints[5]);
            Drawing.DrawLine(mappedPoints[5], mappedPoints[6]);
            Drawing.DrawLine(mappedPoints[6], mappedPoints[7]);
            Drawing.DrawLine(mappedPoints[7], mappedPoints[4]);

            Drawing.DrawLine(mappedPoints[0], mappedPoints[4]);
            Drawing.DrawLine(mappedPoints[1], mappedPoints[5]);
            Drawing.DrawLine(mappedPoints[2], mappedPoints[6]);
            Drawing.DrawLine(mappedPoints[3], mappedPoints[7]);
        }*/
    }
}
