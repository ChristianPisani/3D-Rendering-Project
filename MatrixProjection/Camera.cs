using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProjection
{
    public class Camera
    {
        public GraphicsDevice graphicsDevice;

        public Vector3 pos = new Vector3(0, -10, 20);
        Vector3 lookStraightVector = new Vector3(0, -.5f, -1f);

        Vector2 angle;

        Vector3 up = new Vector3(0, -1, 0);

        public float FOV = Microsoft.Xna.Framework.MathHelper.PiOver4;
        public float nearClipPlane = 1;
        public float farClipPlane = 50000;

        public Vector3 lookAt
        {
            get
            {
                var rotationMatrix = Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X);

                Vector3 lookAtVector = Vector3.Transform(lookStraightVector, rotationMatrix);
                lookAtVector += pos;
                return lookAtVector;
            }
        }

        public Matrix ViewMatrix
        {
            get
            {
                var rotationMatrix = Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X);

                Vector3 lookAtVector = Vector3.Transform(lookStraightVector, rotationMatrix);
                lookAtVector += pos;

                return Matrix.CreateLookAt(
                    pos, lookAtVector, up);
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    FOV, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Camera(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Update()
        {
            var camSpeed = 0.04f;
            var camMoveSpeed = 10;

            var forwardVector = new Vector3(0, 0, -1);
            var sideVector = new Vector3(-1, 0, 0);

            var rotationMatrix = Matrix.CreateRotationY(angle.X);
            forwardVector = Vector3.Transform(forwardVector, rotationMatrix) * camMoveSpeed;
            sideVector = Vector3.Transform(sideVector, rotationMatrix) * camMoveSpeed;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                pos += forwardVector;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                pos -= forwardVector;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                pos -= sideVector;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                pos += sideVector;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                pos.Y += camMoveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                pos.Y -= camMoveSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angle.X -= camSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angle.X += camSpeed;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                angle.Y += camSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                angle.Y -= camSpeed;
            }
        }
    }
}
