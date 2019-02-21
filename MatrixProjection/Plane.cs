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
    public class Plane : GameObject
    {
        public int divisions;

        public Plane(Vector3 pos, Vector2 size, int divisions) : base(pos, new Vector3(size.X, size.X, size.Y))
        {
            this.divisions = divisions + 1;
            MapVertices();
            InitializeVertexBuffer();
        }

        public List<List<Vector3>> MapPoints()
        {
            var points = new List<List<Vector3>>();

            var step = 1f / (divisions - 1);
            var numPoints = 4 * divisions;

            for (int x = 0; x < divisions; x++)
            {
                points.Add(new List<Vector3>());
                for (int y = 0; y < divisions; y++)
                {
                    points[x].Add(new Vector3(-0.5f + (step * x), 0, -0.5f + (step * y)));
                }
            }

            return points;
        }

        public override void MapVertices()
        {
            var points = MapPoints();
            vertices = new List<CustomVertex>();            

            for (int x = 0; x < points.Count-1; x++)
            {
                for (int y = 0; y < points[x].Count-1; y++)
                {
                    CustomVertex v = new CustomVertex();
                    CustomVertex v1 = new CustomVertex();
                    CustomVertex v2 = new CustomVertex();

                    CustomVertex v3 = new CustomVertex();
                    CustomVertex v4 = new CustomVertex();
                    CustomVertex v5 = new CustomVertex();

                    v.Position = points[x][y];
                    v1.Position = points[x+1][y];
                    v2.Position = points[x][y+1];
                    v3.Position = points[x + 1][y + 1];

                    v.Normal = getVertexNormal(v2.Position, v1.Position, v.Position);
                    v1.Normal = v.Normal;
                    v2.Normal = v.Normal;

                    vertices.Add(v2);
                    vertices.Add(v1);
                    vertices.Add(v);

                    v4 = v1;
                    v5 = v2;

                    v3.Normal = getVertexNormal(v4.Position, v5.Position, v3.Position);
                    v4.Normal = v3.Normal;
                    v5.Normal = v3.Normal;

                    vertices.Add(v3);
                    vertices.Add(v4);
                    vertices.Add(v5);


                    //  Backface
                    float offset = 0.001f;
                    v.Position.Y += offset;
                    v1.Position.Y += offset;
                    v2.Position.Y += offset;
                    v3.Position.Y += offset;
                    v4.Position.Y += offset;
                    v5.Position.Y += offset;

                    v.Normal = getVertexNormal(v.Position, v1.Position, v2.Position);
                    v1.Normal = v.Normal;
                    v2.Normal = v.Normal;                    

                    v3.Normal = getVertexNormal(v3.Position, v5.Position, v5.Position);
                    v4.Normal = v3.Normal;
                    v5.Normal = v3.Normal;

                    vertices.Add(v);
                    vertices.Add(v1);
                    vertices.Add(v2);
                    
                    vertices.Add(v5);
                    vertices.Add(v4);
                    vertices.Add(v3);
                    
                }
            }
        }

        public override void Update(double gameTime)
        {
            base.Update(gameTime);

            float speedFactor = 4f;
            float angle = (float)(gameTime / 1000f) * speedFactor;

            for (int i = 0; i < vertices.Count; i += 3)
            {
                Vector3 v = vertices[i].Position;
                Vector3 v1 = vertices[i + 1].Position;
                Vector3 v2 = vertices[i + 2].Position;

                float a = 500;
                float r = 15;

                float yPos = (Vector2.Distance(new Vector2(pos.X, pos.Z), new Vector2(pos.X + v.X, pos.Z + v.Z)));
                v.Y += (float)Math.Sin(angle + (yPos * r)) / a;

                float yPos2 = (Vector2.Distance(new Vector2(pos.X, pos.Z), new Vector2(pos.X + v1.X, pos.Z + v1.Z)));
                v1.Y += (float)Math.Sin(angle + (yPos2 * r)) / a;

                float yPos3 = (Vector2.Distance(new Vector2(pos.X, pos.Z), new Vector2(pos.X + v2.X, pos.Z + v2.Z)));
                v2.Y += (float)Math.Sin(angle + (yPos3 * r)) / a;

                var newVertex = vertices[i];
                var newVertex1 = vertices[i + 1];
                var newVertex2 = vertices[i + 2];

                var normal = getVertexNormal(v, v1, v2);

                newVertex.Position = v;
                newVertex.Normal = normal;

                newVertex1.Position = v1;
                newVertex1.Normal = normal;

                newVertex2.Position = v2;
                newVertex2.Normal = normal;

                vertices[i] = newVertex;
                vertices[i + 1] = newVertex1;
                vertices[i + 2] = newVertex2;
            }

            SetVertexData();
        }

        public override void Draw(GraphicsDevice graphicsDevice, Camera camera, Effect effect)
        {
            Vector4 oldColor = effect.Parameters["Color"].GetValueVector4();
            effect.Parameters["Color"].SetValue(Color.Red.ToVector4());

            base.Draw(graphicsDevice, camera, effect);

            effect.Parameters["Color"].SetValue(oldColor);
        }
    }
}
