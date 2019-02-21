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

        //Object composition
        Vector2 mid;
        int size = 50;
        int amount = 10;

        //Shaders
        Effect vertexShader;
        Effect pixelShader;


        Effect effect;

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

            for (int i = 0; i < 10f; i++)
            {
                Cylinder cyl = new Cylinder(new Vector3(0, -500 * i, 0), new Vector3(400f, 500f, 400f), 10);
                //gameObjects.Add(cyl);
            }


            //gameObjects.Add(new Plane(new Vector3(0, 0, 0), new Vector2(400, 400), 20));
            gameObjects.Add(new Plane(new Vector3(450, 0, 0), new Vector2(400, 400), 1));

            flocks = new List<Flock>();

            flocks.Add(new Flock(300, new Vector3(5000, 5000, 5000), Color.White * 0.8f, Vector3.Zero));
            flocks.Add(new Flock(200, new Vector3(5000, 5000, 4999), Color.Orange * 0.8f, Vector3.Zero));

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
            //effect.AmbientLightColor = new Vector3(.7f, .2f, .4f);
            //effect.Texture = pixel;
            //effect.EmissiveColor = new Vector3(1, 0, 0);
            //effect.TextureEnabled = true;
            //effect.EnableDefaultLighting();

            RasterizerState r = new RasterizerState();
            r.FillMode = FillMode.WireFrame;
            r.CullMode = CullMode.None;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = r;            
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

                    gameObject.rotation = Matrix.CreateLookAt(Vector3.Normalize(gameObject.pos), Vector3.Zero, new Vector3(0, 1, 0));
                    //gameObject.rotation *= Matrix.CreateRotationX(angle);
                }

                gameObject.Update(gameTime.TotalGameTime.TotalMilliseconds);

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Teal);

            effect.Parameters["ViewProjection"].SetValue(camera.ViewMatrix * camera.ProjectionMatrix);

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
        //public Vector3 Normal;

        public CustomVertex(Vector3 position, Color color)
        {
            this.Position = position;
            this.Color = color;
            //this.Normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0)
             );
    }
}

