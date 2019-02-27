using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public struct CustomVertex
    {
        public Vector4 Position;
        public Color Color;
        public Vector3 Normal;

        public CustomVertex(Vector4 position, Color color, Vector3 normal)
        {
            this.Position = position;
            this.Color = color;
            this.Normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 4, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                 new VertexElement(sizeof(float) * 4 + sizeof(byte) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
             );
    }
}
