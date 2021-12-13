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
        public Vector3 Direction;
        public float Velocity = 250f;
        public Vector3 Position;
        public bool IsVisible = true;
        public bool shooting = true;
        public Model model;
        public Matrix bulletsWorld = Matrix.Identity;
        public Vector3 Up = Vector3.Up;
        public float bulletCount; //counter for how many updates the bullet has been drawn for
        
        public Vector3 bulletPosition
        {
            set
            {
                bulletsWorld.Translation = value;
                bulletsWorld = Matrix.CreateWorld(bulletsWorld.Translation, bulletsWorld.Forward, Up); //sets and gets the position of the bullet
            }
            get { return bulletsWorld.Translation; }
        }
    }
}
