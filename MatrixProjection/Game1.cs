using MatrixProjection.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixProjection
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        //Textures
        public static Texture2D pixel;
        public static SpriteFont gameFont;

        //Game variables                
        public static Camera camera;

        float angle = 0.0f;

        public static Rectangle gameBounds = new Rectangle(0, 0, 400, 400);

        public static GameWindow gameWindow;

        //Objects
        public List<GameObject> gameObjects;
        public List<Flock> flocks;


        bool Paused = false;
        bool PausePressed = false;

        bool Filled = false;
        bool FilledPressed = false;

        //Object composition
        Vector2 mid;
        int size = 50;
        int amount = 10;

        //Shaders
        Effect vertexShader;
        Effect pixelShader;


        Effect effect;


        RasterizerState rasterizerState = new RasterizerState();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 2500;
            graphics.PreferredBackBufferHeight = 1800;
            Window.AllowUserResizing = true;
            this.IsMouseVisible = true;

            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice);

            gameObjects = new List<GameObject>();

            mid = new Vector2((amount / 2) * size, (amount / 2) * size); //Vector for dist and stuff

            for (int x = 0; x < amount; x++)
            {
                for (int y = 0; y < amount; y++)
                {
                    float yPos = (Vector2.Distance(mid, new Vector2(x * size, y * size))) / 2;

                    gameObjects.Add(new Cube(new Vector3(x * 250 + size, -1000, y * 250 + size), new Vector3(size, size, size)));
                }
            }


            Cube c = new Cube(new Vector3(50, 50, 50), new Vector3(100, 100, 100));
            //gameObjects.Add(c);

            Cylinder cyl = new Cylinder(new Vector3(-500, -500, -500), new Vector3(400f, 500f, 400f), 20);
            gameObjects.Add(cyl);

            int numBoxes = 10;
            float radius = 300;
            float step = (float)(Math.PI * 2) / numBoxes;

            gameObjects.Add(new Cube(new Vector3(0, 25, 1000), new Vector3(50)));

            for (float x = 0; x < Math.PI * 2; x += step)
            {
                Vector3 orig = new Vector3(0, 25, 1000);
                Cube cube = new Cube(
                    new Vector3((float)Math.Sin(x) * radius, (float)Math.Cos(x) * radius, 1000),
                    new Vector3(50)
                );

                Cube cube2 = new Cube(
                    new Vector3((float)Math.Sin(x) * radius, 25, (float)Math.Cos(x) * radius + 1000),
                    new Vector3(50)
                );

                cube.rotation = MatrixHelper.RotateTowardMatrix(cube.pos, orig);
                cube2.rotation = MatrixHelper.RotateTowardMatrix(cube2.pos, orig);

                gameObjects.Add(cube);
                gameObjects.Add(cube2);
            }


            gameObjects.Add(new Plane(new Vector3(0, 0, 0), new Vector2(400, 400), 20));
            gameObjects.Add(new Plane(new Vector3(450, 0, 0), new Vector2(400, 400), 2));

            flocks = new List<Flock>();

            flocks.Add(new Flock(300, new Vector3(5000, 5000, 5000), Color.White, Vector3.Zero));
            flocks.Add(new Flock(200, new Vector3(5000, 5000, 4999), Color.Aqua, Vector3.Zero));

            foreach (Flock flock in flocks)
            {
                gameObjects.AddRange(flock.flock);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = Content.Load<Texture2D>("Pixel");
            gameFont = Content.Load<SpriteFont>("font1");
            vertexShader = Content.Load<Effect>("vertexShader");
            pixelShader = Content.Load<Effect>("pixelShader");
            effect = vertexShader;

            //effect = new BasicEffect(graphics.GraphicsDevice);
            effect.CurrentTechnique = effect.Techniques["BasicColorDrawing"];
            //effect.Parameters["projection"].SetValue(camera.ProjectionMatrix);
            //effect.Parameters["Color"].SetValue(Color.SeaShell.ToVector4());
            effect.Parameters["LightPos"].SetValue(new Vector3(4000, 4000, 4000));
            effect.Parameters["LightPower"].SetValue(2f);
            effect.Parameters["LightColor"].SetValue(new Vector4(0.6f, 0.3f, 0, 1));
            effect.Parameters["AmbientLightColor"].SetValue(new Vector4(.2f, .2f, .4f, 1f));
            //effect.AmbientLightColor = new Vector3(.7f, .2f, .4f);
            //effect.Texture = pixel;
            //effect.EmissiveColor = new Vector3(1, 0, 0);
            //effect.TextureEnabled = true;
            //effect.EnableDefaultLighting();

            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = rasterizerState;
        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            gameWindow = Window;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                PausePressed = true;
            }
            else
            {
                if (PausePressed)
                {
                    Paused = !Paused;
                    PausePressed = false;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                FilledPressed = true;
            }
            else
            {
                if (FilledPressed)
                {
                    RasterizerState r = new RasterizerState();
                    r.CullMode = rasterizerState.CullMode;

                    if (Filled)
                    {
                        r.FillMode = FillMode.Solid;
                        GraphicsDevice.RasterizerState = r;
                    }
                    else
                    {
                        r.FillMode = FillMode.WireFrame;
                        r.CullMode = CullMode.None;
                        GraphicsDevice.RasterizerState = r;
                    }

                    Filled = !Filled;
                    FilledPressed = false;
                }
            }
            camera.Update();

            if (Paused)
            {
                return;
            }

            angle += 0.05f;

            foreach (Flock flock in flocks)
            {
                flock.Update();
            }

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject is Cube cube)
                {
                    //cube.pos.Y = ((float)Math.Sin(angle + cube.pos.X + cube.pos.Z)) * 100;

                    float yPos = (Vector2.Distance(mid, new Vector2(gameObject.pos.X, gameObject.pos.Z)));
                    //cube.pos.Y = (float)Math.Sin(yPos / (Math.Sin(angle) * 200)) * 100;
                    //cube.pos.Y += (float)Math.Sin(angle + (yPos/150)) * 10;                    

                }

                gameObject.Update(gameTime.TotalGameTime.TotalMilliseconds);

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Teal);
            //GraphicsDevice.Clear(Color.White);

            effect.Parameters["ViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix);
            effect.Parameters["CameraPos"].SetValue(camera.pos);

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(graphics.GraphicsDevice, camera, effect);
            }

            base.Draw(gameTime);
        }
    }

    public struct CustomVertex
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public CustomVertex(Vector3 position, Color color, Vector3 normal)
        {
            this.Position = position;
            this.Color = color;
            this.Normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                 new VertexElement(sizeof(float) * 3 + sizeof(byte) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
             );
    }
}

