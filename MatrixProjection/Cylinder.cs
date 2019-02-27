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

        public Cylinder(Vector3 pos, Vector3 size, int divisions) : base(pos, size)
        {
            this.divisions = divisions;

            MapVertices();
            InitializeVertexBuffer();
        }

        public List<List<Vector4>> MapPoints()
        {
            var points = new List<List<Vector4>>();

            points.Add(new List<Vector4>());
            points.Add(new List<Vector4>());

            var step = (Math.PI * 2f) / (divisions);

            for (int x = 0; x < divisions; x++)
            {
                points[0].Add(new Vector4(((float)Math.Cos(step * x) * 0.5f), -1f, ((float)Math.Sin(step * x) * 0.5f), 1));
                points[1].Add(new Vector4(((float)Math.Cos(step * x) * 0.3f), 0f, ((float)Math.Sin(step * x) * 0.3f), 1));
            }

            return points;
        }

        public override void MapVertices()
        {
            /*
             * 
             * I could say I will refactor this later, but that is a big fat lie
             * 
             */

            var points = MapPoints();
            vertices = new List<CustomVertex>();

            for (int y = 0; y < points.Count; y++)
            {
                for (int x = 0; x < points[y].Count; x++)
                {
                    CustomVertex v = new CustomVertex();
                    CustomVertex v1 = new CustomVertex();
                    CustomVertex v2 = new CustomVertex();

                    v.Position = points[y][x];
                    v1.Position = points[y][(x + 1) % points[y].Count];
                    v2.Position = new Vector4(0, points[y][x].Y, 0, 1);

                    Vector3 normal = getVertexNormal(new Vector3(v2.Position.X, v2.Position.Y, v2.Position.Z),
                                                     new Vector3(v1.Position.X, v1.Position.Y, v1.Position.Z),
                                                     new Vector3(v.Position.X, v.Position.Y, v.Position.Z));

                    if (y == 1)
                    {
                        normal *= -1;
                    }

                    v.Normal = normal;
                    v1.Normal = normal;
                    v2.Normal = normal;
                    if (y == 0)
                    {
                        vertices.Add(v2);
                        vertices.Add(v1);
                        vertices.Add(v);
                    } else
                    {
                        vertices.Add(v);
                        vertices.Add(v1);
                        vertices.Add(v2);
                    }

                    if (y < 1)
                    {
                        CustomVertex v3 = new CustomVertex();
                        CustomVertex v4 = new CustomVertex();
                        CustomVertex v5 = new CustomVertex();

                        v3.Position = points[y][x];
                        v4.Position = points[y + 1][x];
                        v5.Position = points[y][(x + 1) % points[y].Count];

                        normal = getVertexNormal(new Vector3(v5.Position.X, v5.Position.Y, v5.Position.Z),
                                                 new Vector3(v4.Position.X, v4.Position.Y, v4.Position.Z),
                                                 new Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z));
                        v3.Normal = normal;
                        v4.Normal = normal;
                        v5.Normal = normal;

                        vertices.Add(v5);
                        vertices.Add(v4);
                        vertices.Add(v3);
                    }
                    if(y >= 1)
                    {
                        CustomVertex v3 = new CustomVertex();
                        CustomVertex v4 = new CustomVertex();
                        CustomVertex v5 = new CustomVertex();

                        v3.Position = points[y][(x + 1) % points[y].Count];
                        v4.Position = points[y - 1][(x + 1) % points[y].Count];
                        v5.Position = points[y][x];

                        normal = getVertexNormal(new Vector3(v5.Position.X, v5.Position.Y, v5.Position.Z),
                                                 new Vector3(v4.Position.X, v4.Position.Y, v4.Position.Z),
                                                 new Vector3(v3.Position.X, v3.Position.Y, v3.Position.Z));
                        v3.Normal = normal;
                        v4.Normal = normal;
                        v5.Normal = normal;

                        vertices.Add(v5);
                        vertices.Add(v4);
                        vertices.Add(v3);
                    }
                }
            }
        }

        public override void Update(double gameTime)
        {
            base.Update(gameTime);

            this.rotation = Matrix.CreateRotationY((float)gameTime/1000f) * Matrix.CreateRotationZ((float)Math.Sin(gameTime/1000f));
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
