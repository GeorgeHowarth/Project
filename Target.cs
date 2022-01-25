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
    public class Target
    {
        public Vector3 Direction;
        public float Velocity;
        public Vector3 Position;
        public bool IsVisible = true;
        public Model model;
        public Matrix targetsWorld = Matrix.Identity;
        public Vector3 Up = Vector3.Up;
        public int Counter;
        public int leftright;
        public int backforward;

        public Vector3 targetPosition
        {
            set
            {
                targetsWorld.Translation = value;
                targetsWorld = Matrix.CreateWorld(targetsWorld.Translation, targetsWorld.Forward, Up); //sets and gets the position of the bullet
            }
            get { return targetsWorld.Translation; }
        }
    }
}   