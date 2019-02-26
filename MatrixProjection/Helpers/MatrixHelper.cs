using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection.Helpers
{
    class MatrixHelper
    {
        public static Matrix pointTranslationMatrix(Vector3 orig, Vector3 point)
        {
            return new Matrix(
                    new Vector4(1, 0, 0, 0),
                    new Vector4(0, 1, 0, 0),
                    new Vector4(0, 0, 1, 0),
                    new Vector4(orig.X + point.X, orig.Y + point.Y, orig.Z + point.Z, 1)
                );
        }

        public static Matrix viewMatrix(Camera camera)
        {
            float cosPitch = (float)Math.Cos(camera.lookAt.X);
            float sinPitch = (float)Math.Sin(camera.lookAt.X);
            float cosYaw = (float)Math.Cos(camera.lookAt.Y);
            float sinYaw = (float)Math.Sin(camera.lookAt.Y);

            Vector3 xaxis = new Vector3(cosYaw, 0, -sinYaw);
            Vector3 yaxis = new Vector3(sinYaw * sinPitch, cosPitch, cosYaw * sinPitch);
            Vector3 zaxis = new Vector3(sinYaw * cosPitch, -sinPitch, cosPitch * cosYaw);

            // Create a 4x4 view matrix from the right, up, forward and eye position vectors
            return new Matrix(
                new Vector4(xaxis.X, yaxis.X, zaxis.X, 0),
                new Vector4(xaxis.Y, yaxis.Y, zaxis.Y, 0),
                new Vector4(xaxis.Z, yaxis.Z, zaxis.Z, 0),
                new Vector4(-Vector3.Dot(xaxis, camera.pos), -Vector3.Dot(yaxis, camera.pos), -(Vector3.Dot(zaxis, camera.pos)), 1)
            );
        }

        public static Matrix projectionMatrix(Camera camera)
        {
            //https://www.scratchapixel.com/lessons/3d-basic-rendering/perspective-and-orthographic-projection-matrix/building-basic-perspective-projection-matrix
            float FOV = camera.FOV;
            float far = 1f;
            float near = 0.0001f;
            float S = (float)(1f / (Math.Tan((FOV / 2) * (Math.PI / 180f))));

            return new Matrix(
                    new Vector4(S, 0, 0, 0),
                    new Vector4(0, S, 0, 0),
                    new Vector4(0, 0, -(far / (far - near)), -1),
                    new Vector4(0, 0, -((far * near) / (far - near)), 0)
                );
        }

        public static Vector3 MapToScreenSpace(Vector3 P, Matrix M)
        {
            Vector4 screenSpace = new Vector4()
            {
                X = P.X * M[0] + P.Y * M[4] + P.Z * M[8] + M[12],
                Y = P.X * M[1] + P.Y * M[5] + P.Z * M[9] + M[13],
                Z = P.X * M[0] + P.Y * M[4] + P.Z * M[8] + M[12],
                W = 1 / (P.X * M[3] + P.Y * M[7] + P.Z * M[11] + M[15])
            };

            if (screenSpace.W > 0)
            {
                return Vector3.Zero;
            }

            screenSpace.X *= screenSpace.W;
            screenSpace.Y *= screenSpace.W;
            screenSpace.Z *= screenSpace.W;

            if (screenSpace.X > 1 || screenSpace.X < -1 ||
           screenSpace.Y > 1 || screenSpace.Y < -1 ||
           screenSpace.Z > 1 || screenSpace.Z < -1)
            {
                return Vector3.Zero;
            }

            var sx = screenSpace.X * Game1.graphics.PreferredBackBufferWidth + Game1.graphics.PreferredBackBufferWidth / 2.0f;
            var sy = -screenSpace.Y * Game1.graphics.PreferredBackBufferHeight + Game1.graphics.PreferredBackBufferHeight / 2.0f;

            return new Vector3(sx, sy, screenSpace.Z);
        }

        public static Vector3 getMappedPoint(Vector3 point, Camera camera)
        {
            Vector3 mapped = point;

            Matrix world = pointTranslationMatrix(Vector3.Zero, mapped) * viewMatrix(camera) * projectionMatrix(camera);

            return MapToScreenSpace(mapped, world);
        }

        public static Matrix getWorldMatrix(Vector3 point, Camera camera)
        {
            Vector3 mapped = point;

            Matrix world = pointTranslationMatrix(Vector3.Zero, mapped) * viewMatrix(camera) * projectionMatrix(camera);

            return world;
        }

        public static Matrix directionRotationMatrix(Vector3 direction)
        {
            Vector3 up = new Vector3(0, 1, 0);

            Vector3 xaxis = Vector3.Cross(up, direction);
            xaxis.Normalize();

            Vector3 yaxis = Vector3.Cross(direction, xaxis);
            yaxis.Normalize();

            Matrix rotation = new Matrix(
                new Vector4(xaxis.X, yaxis.X, direction.X, 0),
                new Vector4(xaxis.Y, yaxis.Y, direction.Y, 0),
                new Vector4(xaxis.Z, yaxis.Z, direction.Z, 0),
                new Vector4(0,0,0,1)

            );

            return rotation;
        }

        // From here (translated from c++): https://stackoverflow.com/questions/26017467/rotate-object-to-look-at-another-object-in-3-dimensions
        public static Matrix RotateTowardMatrix(Vector3 origin, Vector3 target)
        {
            Vector3 objectUpVector = new Vector3(0.0f, 1.0f, 0.0f);

            Vector3 zaxis = Vector3.Normalize(target - origin);
            Vector3 xaxis = Vector3.Normalize(Vector3.Cross(objectUpVector, zaxis));
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            Matrix pm = new Matrix(
                new Vector4(xaxis.X, xaxis.Y, xaxis.Z, 0),
                new Vector4(yaxis.X, yaxis.Y, yaxis.Z, 0),
                new Vector4(zaxis.X, zaxis.Y, zaxis.Z, 0),
                new Vector4(0, 0, 0, 1)
            );

            return pm;
        }
    }
}
