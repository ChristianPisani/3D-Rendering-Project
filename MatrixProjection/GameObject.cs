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
    public class GameObject
    {
        public Vector3 pos;
        public Vector3 size;

        public Matrix rotation, scale;

        public List<CustomVertex> vertices;

        private VertexBuffer vertexBuffer;

        private GraphicsDevice graphicsDevice;

        Vector3[] points = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1) };


        int[] triangles = {
                0, 2, 1, //face front
	    		0, 3, 2,
                2, 3, 4, //face top
			    2, 4, 5,
                1, 2, 5, //face right
	    		1, 5, 6,
                0, 7, 4, //face left
			    0, 4, 3,
                5, 4, 7, //face back
	    		5, 7, 6,
                0, 6, 7, //face bottom
		    	0, 1, 6
            };

        Vector3[] normals =
        {
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, -1),
            new Vector3(0, -1, 0)
        };

        public GameObject(Vector3 pos, Vector3 size)
        {
            this.graphicsDevice = Game1.graphics.GraphicsDevice;
            this.pos = pos;
            this.size = size;

            triangles = triangles.Reverse().ToArray();

            this.scale = new Matrix(
                    new Vector4(size.X, 0, 0, 0),
                    new Vector4(0, size.Y, 0, 0),
                    new Vector4(0, 0, size.Z, 0),
                    new Vector4(0, 0, 0, 1)
                );

            this.rotation = Matrix.CreateRotationX(0);

            MapVertices();
            InitializeVertexBuffer();
        }

        public virtual void MapVertices()
        {
            vertices = new List<CustomVertex>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                CustomVertex v = new CustomVertex();
                CustomVertex v1 = new CustomVertex();
                CustomVertex v2 = new CustomVertex();

                v.Position = points[triangles[i]];
                v1.Position = points[triangles[i + 1]];
                v2.Position = points[triangles[i + 2]];

                Vector3 normal = Vector3.Zero;

                normal = getVertexNormal(v.Position, v1.Position, v2.Position);

                //v.Normal = normal;
                //v1.Normal = normal;
                //v2.Normal = normal;

                vertices.Add(v);
                vertices.Add(v1);
                vertices.Add(v2);
            }            
        }

        public void InitializeVertexBuffer()
        {
            vertexBuffer = new VertexBuffer(graphicsDevice, CustomVertex.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());
        }

        public Vector3 getVertexNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 dir = Vector3.Cross(b - a, c - a);
            Vector3 norm = dir / dir.Length();

            return norm;
        }

        public virtual void Update(double gameTime)
        {

        }

        public virtual void Draw(GraphicsDevice graphicsDevice, Camera camera, Effect effect)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                effect.Parameters["World"].SetValue(rotation * scale * Matrix.CreateTranslation(pos));

                pass.Apply();



                /*graphicsDevice.DrawUserPrimitives(
                            PrimitiveType.TriangleList,
                    vertices.ToArray(),
                    0,
                    12);*/

                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Count/3);
            }
        }
    }
}
