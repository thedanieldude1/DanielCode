using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetwork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace NeuralNetworkSimulator
{

    public class Agent
    {
        public static float TimeMultiplier = 1;
        public Network brain;
        public Vector2 Pos;
        public float Angle;
        public float Velocity;
        public Game1 Master;
        public Rectangle Box;
        float intersect;
        float HealthLoseRate = 0;
        public float Health = 100;
        public bool Colliding = false;
        public bool lastCollidng = false;
        public bool Dead = false;
        Agent Intersected;
        public float Fitness = 0;
        //Random random = new Random();
        public Agent(Random random,Game1 master)
        {
            brain = new Network(6, 4, 2);
            Master = master;
            Pos = new Vector2(Game1.Width/2+(float)(random.Next(-200, 200)), Game1.Height/2+(float)(random.Next(-200, 200)));
            Angle = (float)(random.Next(0, 100)/100f) * 360;
            Box = new Rectangle(0, 0, 40, 15);
        }
        public void Tick()
        {
            
            Ray2D ray = new Ray2D(Pos, Pos+new Vector2((float)Math.Cos(Angle * (float)(Math.PI / 180)), (float)Math.Sin(Angle * (float)(Math.PI / 180)))*100);
            if (Intersected == null)
            {
                foreach (Agent n in Master.Agents)
                {
                    if (n == this) { continue; }
                    if (ray.Intersects(n.Box) != Vector2.Zero)
                    {

                        if (intersect == 0)
                        {
                           // Console.WriteLine("Kys " + Master.Agents.IndexOf(n));
                        }
                        Intersected = n;
                        intersect = 1;
                        break;
                    }
                    else
                    {
                        if (intersect == 1)
                        {
                           // Console.WriteLine("No longer kys " + Master.Agents.IndexOf(n));
                        }
                        intersect = 0;
                    }

                }
            }
            else
            {
                if (Box.Intersects(Intersected.Box)&&intersect<= 42.72f && intersect>0)
                {

                    if (Math.Abs(Velocity) > Math.Abs(Intersected.Velocity) && !Colliding)
                    {
                        Health += 20;
                        Fitness += 20;
                        Intersected.Health -= 20;
                        HealthLoseRate = 0;
                        //Console.WriteLine("Collider Won");
                    }
                    else if (!Colliding)
                    {
                        Health -= 20;
                        Intersected.Health += 20;
                       // Console.WriteLine("Collider Lost");
                    }
                    Colliding = true;
                }
                else
                {

                    if (Colliding == true)
                    {
                        //Console.WriteLine("I HATE YOU!");
                    }
                    Colliding = false;
                }
            
            if (ray.Intersects(Intersected.Box) != Vector2.Zero)
                {

                }
                else
                {
                    foreach (Agent n in Master.Agents)
                    {
                        if (n == this) { continue; }

                        if (ray.Intersects(n.Box) != Vector2.Zero)
                        {

                            if (intersect == 0)
                            {
                               // Console.WriteLine("Kys " + Master.Agents.IndexOf(this));
                            }
                            Intersected = n;
                            intersect = (Math.Abs((float)((ray.Intersects(n.Box)-Pos).Length())-100)/100)*2-1;

                            break;
                        }
                        else
                        {
                            if (intersect == 1)
                            {
                                //Console.WriteLine("No longer kys " + Master.Agents.IndexOf(this));
                            }
                            intersect = 0;
                            
                        }

                    }
                }
            }

            float AngleToNearestAgent = 0;
            Vector2 currentMinima = new Vector2(100000000, 100000000);
            foreach (Agent n in Master.Agents)
            {
                if (n == this)
                {
                    continue;
                }
                if (n.Pos.Length() < currentMinima.Length())
                {
                    currentMinima = n.Pos;
                }
            }
            
            Fitness += 0.1f * TimeMultiplier;

            AngleToNearestAgent = (float)(((float)Math.Atan2((currentMinima - Pos).Y, (currentMinima - Pos).X) <0? (float)Math.Atan2((currentMinima - Pos).Y, (currentMinima - Pos).X) +Math.PI*2: (float)Math.Atan2((currentMinima - Pos).Y, (currentMinima - Pos).X)/ (float)(Math.PI / 180))-Angle)/180f;//(float)Math.Acos(Vector2.Dot(Vector2.Normalize((currentMinima - Pos)), Vector2.Normalize(new Vector2((float)Math.Cos(Angle * (float)(Math.PI / 180)), (float)Math.Sin(Angle * (float)(Math.PI / 180))))))/3.14f;
            //Console.WriteLine(Pos.Length()< currentMinima.Length());
            float enemyhealth = 0;
            if (Intersected != null&&intersect>0)
            {
                enemyhealth = Intersected.Velocity;//Math.Abs((Intersected.Health)-100)/100f;
            }
            float[] outputs = brain.Fire(new float[]{ (currentMinima - Pos).Length()/100, Velocity/TimeMultiplier, AngleToNearestAgent, Math.Abs((Health-100)/100f)*2-1 ,enemyhealth,(Angle/360) * 2 - 1 });
            Angle = ((Angle + outputs[0]*1.5f * TimeMultiplier) %360);
            HealthLoseRate += 0.000001f;
            Health -= HealthLoseRate * TimeMultiplier + Math.Abs(Velocity / 20)+ Math.Abs(outputs[0]/20);
            //Console.WriteLine(Pos);
            Pos += new Vector2((float)Math.Cos(Angle * (float)(Math.PI / 180)) * (outputs[1]), (float)Math.Sin(Angle * (float)(Math.PI / 180)) * (outputs[1])) * TimeMultiplier;
            Pos = new Vector2(Pos.X % Game1.Width, Pos.Y % Game1.Height);
            Box.X = (int)Pos.X;
            Box.Y = (int)Pos.Y;
            
            Velocity= (new Vector2((float)Math.Cos(Angle * (float)(Math.PI / 180)) * (outputs[1]), (float)Math.Sin(Angle * (float)(Math.PI / 180)) * (outputs[1]))).Length() * TimeMultiplier * 2 - 1;
        }
    }
    public struct Ray2D
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private readonly List<Point> result;

        public Ray2D(Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            result = new List<Point>();
        }

        /// <summary>  
        /// Determine if the ray intersects the rectangle  
        /// </summary>  
        /// <param name="rectangle">Rectangle to check</param>  
        /// <returns></returns>
        /// 
        public Vector2 Intersects(Rectangle rectangle)
        {
            Point p0 = new Point((int)startPos.X, (int)startPos.Y);
            Point p1 = new Point((int)endPos.X, (int)endPos.Y);

            foreach (Point testPoint in BresenhamLine(p0, p1))
            {
                if (rectangle.Contains(testPoint))
                    return new Vector2((float)testPoint.X, (float)testPoint.Y);
            }
            return Vector2.Zero;
        }

        // Swap the values of A and B  

        private void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1   

        private List<Point> BresenhamLine(Point p0, Point p1)
        {
            return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
        }

        // Returns the list of points from (x0, y0) to (x1, y1)  

        private List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            // Optimization: it would be preferable to calculate in  
            // advance the size of "result" and to use a fixed-size array  
            // instead of a list.  

            result.Clear();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1; else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) result.Add(new Point(y, x));
                else result.Add(new Point(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
    }
}
