using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CamTest
{
    public class Basic3dExampleCamera
    {
        private GraphicsDevice graphicsDevice = null;
        private GameWindow gameWindow = null;
        public float sensitivity = 1f;
        bool invertControls = false;
        private float unitsPerSecond = 5;
        private float unitsPerSecondSprint = 10;
        private float unitsPerSecondJump = 0.15f;
        private float anglesPerSecond = 20f;
        private float gravity = -0.075f;
        public float BulletVelocity = 15f;
        

        public float FieldOfView = MathHelper.PiOver4;
        public float NearClipPlane = 0.1f;
        public float FarClipPlane = 200f;

        public Vector3 BulletDirection; //the way the cam was facing when the bullet was shot

        public int xValue = 400;
        public int yValue = 240;

        public double totalTime = 0;

        public bool jumping = false;
        public bool crouching = false;
        public bool sprinting = false;
        public bool shooting = false;

        public List<Vector3> BulletPositions = new List<Vector3>();

        /// <summary>
        /// this serves as the cameras up for fixed cameras this might not change at all ever.
        /// </summary>
        private Vector3 Up = Vector3.Up;
        /// <summary>
        /// this serves as the cameras world orientation 
        /// it holds all orientational values and is used to move the camera properly thru the world space as well.
        /// </summary>
        public Matrix camerasWorld = Matrix.Identity;
        public Matrix bulletsWorld = Matrix.Identity;
        /// <summary>
        /// The view matrix is created from the cameras world matrixs but it has special propertys.
        /// Using create look at to create this matrix you move from the world space into the view space.
        /// If you are working on world objects you should not take individual elements from this to directly operate on world matrix components.
        /// As well the multiplication of a view matrix by a world matrix moves the resulting matrix into view space itself.
        /// </summary>
        private Matrix viewMatrix = Matrix.Identity;
        /// <summary>
        /// Constructs the camera.
        /// </summary>
        public Basic3dExampleCamera(GraphicsDevice gfxDevice, GameWindow window)
        {
            graphicsDevice = gfxDevice;
            gameWindow = window;
            ReCreateWorldAndView();
        }
        /// <summary>
        /// Determines how the camera behaves true for fixed false for free.
        /// </summary>
        public bool IsFixed { get; set; } = true;
        /// <summary>
        /// Gets or sets the the camera's position in the world.
        /// </summary>
        public Vector3 Position
        {
            set
            {
                camerasWorld.Translation = value;
                // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
                ReCreateWorldAndView();
            }
            get { return camerasWorld.Translation; }
        }

        public Vector3 bulletPosition
        {
            set
            {
                bulletsWorld.Translation = value;
                ReCreateWorldAndView();
            }
            get { return bulletsWorld.Translation; }
        }

        /// <summary>
        /// Gets or Sets the direction the camera is looking at in the world.
        /// </summary>
        public Vector3 LookAtDirection
        {
            set
            {
                camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, value, Up);
                // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
                ReCreateWorldAndView();
            }
            get { return camerasWorld.Forward * 10; }
        }

        /// <summary>
        /// Sets a positional target in the world to look at.
        /// </summary>
        public Vector3 Target
        {
            set
            {
                camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, Vector3.Normalize(value - camerasWorld.Translation), Up);
                // since we know here that a change has occured to the cameras world orientations we can update the view matrix.
                ReCreateWorldAndView();
            }
        }

        /// <summary>
        /// When the cameras position or orientation changes we call this to ensure, the cameras world matrix is orthanormal and to update the view matrix.
        /// </summary>
        private void ReCreateWorldAndView()
        {
            if (IsFixed)
                Up = Vector3.Up;
            else
                Up = camerasWorld.Up;
            camerasWorld = Matrix.CreateWorld(camerasWorld.Translation, camerasWorld.Forward, Up);
            bulletsWorld = Matrix.CreateWorld(bulletsWorld.Translation, bulletsWorld.Forward, Up);
            viewMatrix = Matrix.CreateLookAt(camerasWorld.Translation, camerasWorld.Forward + camerasWorld.Translation, camerasWorld.Up);
        }
        /// <summary>
        /// Gets the view matrix.
        /// </summary>
        public Matrix ViewMatrix
        {
            get
            {
                return viewMatrix;
            }
        }
        /// <summary>
        /// Gets the projection matrix.
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get
            {
                float aspectRatio = graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Height;
                return Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearClipPlane, FarClipPlane);
            }
        }

        public void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState(gameWindow);
            KeyboardState kstate = Keyboard.GetState();
            // mouse controls
            RotateLeftOrRight(gameTime, -MouseMovement().X * sensitivity);         //mouse look left/right
            RotateUpOrDown(gameTime, -MouseMovement().Y * sensitivity);            // mouse look up/down
            Mouse.SetPosition(400, 240);

            // movement controls
            if (Position.Y > 2)
                Position = new Vector3(Position.X, Position.Y + gravity, Position.Z); //gravity

            if (Position.Y < 2)
                Position = new Vector3(Position.X,2,Position.Z); //placeholder for collision on ground

            //jump code start
            if (kstate.IsKeyDown(Keys.Space)== true && !jumping)
            {
                jumping = true;
                totalTime = 0;
            }
            if (jumping)
            {
               totalTime++;
                Position = new Vector3(Position.X, Position.Y + unitsPerSecondJump, Position.Z);
            }
            if (totalTime > 35 && jumping)
               jumping = false;         
                //jump code end


            if (kstate.IsKeyDown(Keys.W))   //forward
                MoveForward(gameTime);
            
            if (kstate.IsKeyDown(Keys.S) == true) //back 
                MoveBackward(gameTime);
            
            if (kstate.IsKeyDown(Keys.A) == true) //left
                MoveLeft(gameTime);
            
            if (kstate.IsKeyDown(Keys.D) == true) //right 
                MoveRight(gameTime);
            
            if (kstate.IsKeyDown(Keys.LeftShift) == true)  //sprint 
                sprinting = true;
            
            if (kstate.IsKeyDown(Keys.LeftShift) == false) // not sprinting 
                sprinting = false;
            
            if (!sprinting && !crouching)
                unitsPerSecond = 5;

            if (kstate.IsKeyDown(Keys.LeftControl) == true) //crouch
            {
                if(!jumping && Position.Y <= 3f)
                {
                    crouching = true;
                    Position = new Vector3(Position.X, 1.5f, Position.Z);   // crouch on ground at y = 1.5
                    unitsPerSecond = 2;
                }
            }
            if (kstate.IsKeyDown(Keys.LeftControl) == false) //not crouching
            {
                crouching = false;
                if (!jumping && !sprinting && Position.Y == 1.5)
                {
                    Position = new Vector3(Position.X, 2, Position.Z);      //normal movement height at y=2
                    unitsPerSecond = 5;                                     //normal run speed = 5
                }
            }
        }
        public Vector2 MouseMovement()               //mouse is set to x400 y240 every update, this tracks how far from these coordinates it is
        {
            int x = 0;
            int y = 0;

            if (Mouse.GetState().Position.X != 400)
                x = 400 - Mouse.GetState().Position.X;

            if (Mouse.GetState().Position.Y != 240)
                y = 240 - Mouse.GetState().Position.Y;

            Vector2 Movement = new Vector2(x, y);
            return Movement;
        }

        public void MoveForward(GameTime gameTime)
        {
            if (sprinting)
                unitsPerSecond = unitsPerSecondSprint;
            Position += (camerasWorld.Forward * unitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveBackward(GameTime gameTime)
        {
            if (sprinting)
                unitsPerSecond = unitsPerSecondSprint;
            float temp = Position.Y;
            Position += (camerasWorld.Backward * unitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveLeft(GameTime gameTime)
        {
            if (sprinting)
                unitsPerSecond = unitsPerSecondSprint;
            Position += (camerasWorld.Left * unitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public void MoveRight(GameTime gameTime)
        {
            if (sprinting)
                unitsPerSecond = unitsPerSecondSprint;
            Position += (camerasWorld.Right * unitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void RotateLeftOrRight(GameTime gameTime, float amount) //mouse look left/right
        {
            if (invertControls)
                amount = -amount;
            var radians = amount * -anglesPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Up, MathHelper.ToRadians(radians));
            LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);
            
            ReCreateWorldAndView();
        }
        public void RotateUpOrDown(GameTime gameTime, float amount) //mouse look up/down
        {
            if (invertControls)
                amount = -amount;
            var radians = amount * -anglesPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix matrix = Matrix.CreateFromAxisAngle(camerasWorld.Right, MathHelper.ToRadians(radians));
            LookAtDirection = Vector3.TransformNormal(LookAtDirection, matrix);

            ReCreateWorldAndView();
        }
    }
}