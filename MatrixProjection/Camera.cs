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

        public Vector3 pos = new Vector3(-1000, -500, 1000);
        Vector3 lookStraightVector = new Vector3(0, -.5f, -1f);

        public Vector2 angle;

        Vector3 up = new Vector3(0, -1, 0);

        public bool controlsEnabled = true;

        public float FOV = Microsoft.Xna.Framework.MathHelper.PiOver4;
        public float nearClipPlane = 1;
        public float farClipPlane = 5000000;

        const float camSpeed = 0.04f;
        const float camMoveSpeed = 10;

        Matrix rotationMatrix;


        public Vector3 lookAt;
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        Vector3 createLookAt
        {
            get
            {
                Vector3 lookAtVector = Vector3.Transform(lookStraightVector, rotationMatrix);
                lookAtVector += pos;
                return lookAtVector;
            }
        }

        public Matrix createViewMatrix
        {
            get
            {                
                Vector3 lookAtVector = Vector3.Transform(lookStraightVector, rotationMatrix);
                lookAtVector += pos;

                return Matrix.CreateLookAt(
                    pos, lookAtVector, up);
            }
        }

        public Matrix createProjectionMatrix
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

            projectionMatrix = createProjectionMatrix;
        }

        public void Update()
        {            
            if(controlsEnabled)
            {
                ControlCamera();
            }
            ControlCameraLookAt();

            angle.Y = MathHelper.Clamp(angle.Y, (float)-Math.PI / 3, (float)Math.PI / 2);

            lookAt = createLookAt;
            //viewMatrix = createViewMatrix;

        }

        public void Follow(Vector3 target, Vector3 distance, float followStrength)
        {
            pos = Vector3.Lerp(pos, target - distance, followStrength);
        }

        public void RotateAround(Vector3 target, float rotateStrength)
        {
            RotateAround(target, angle, rotateStrength);
        }

        public void RotateAround(Vector3 target, Vector2 targetRotation, float rotateStrength)
        {
            Vector3 newPos;

            float dist = (target - pos).Length();
            newPos.X = target.X + (float)Math.Sin(targetRotation.X) * dist;
            newPos.Z = target.Z + (float)Math.Cos(targetRotation.X) * dist;

            newPos.Y = target.Y + (float)Math.Sin(angle.Y) * dist;

            pos = Vector3.Lerp(pos, newPos, rotateStrength);
        }

        public void LookToward(Vector3 target)
        {
            viewMatrix = Matrix.CreateLookAt(pos, target, up);
        }

        public void ControlCamera()
        {
            Vector3 forwardVector = new Vector3(0, 0, -1);
            Vector3 sideVector = new Vector3(-1, 0, 0);

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
            
            lookAt = createLookAt;
            viewMatrix = createViewMatrix;

        }

        public void ControlCameraLookAt()
        {
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

            //rotationMatrix = Matrix.CreateRotationX(angle.Y) * Matrix.CreateRotationY(angle.X);
        }        
    }
}
