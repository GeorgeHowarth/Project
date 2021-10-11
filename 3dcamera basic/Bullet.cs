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
    public class Bullet 
    {
        public Basic3dExampleCamera cam;
        public Vector3 Direction;
        public float Velocity = 30f;
        public Vector3 Position;
        public bool IsVisible;
        public bool shooting;
        public Model model;
        public Matrix bulletsWorld = Matrix.Identity;
        public Vector3 Up = Vector3.Up;
        public float bulletCount;
        
        public Vector3 bulletPosition
        {
            set
            {
                bulletsWorld.Translation = value;
                bulletsWorld = Matrix.CreateWorld(bulletsWorld.Translation, bulletsWorld.Forward, Up);
            }
            get { return bulletsWorld.Translation; }
        }
    }
}
