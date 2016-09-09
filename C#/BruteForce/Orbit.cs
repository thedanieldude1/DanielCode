using System;
public static class Program
{
    public static void Main(string[] args)
    {
        Planet planet = new Planet(60000,9.81,60000);
        Trajectory traj = new Trajectory(planet, new Vector3D(0, 120000, 0), new Vector3D(1000, 1000, 0));

        bool cancel=true;
        while (cancel)
        {
            traj.Step();
            Console.WriteLine(traj.pos+" "+traj.vel);
            if (traj.pos.Magnitude() < planet.Radius)
            {
                //cancel = false;
                //break;
            }
        }
        Console.ReadLine();
    }
}
public class Vector3D
{
    public double x;
    public double y;
    public double z;
    public Vector3D(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

    }
    
    public static Vector3D operator +(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static Vector3D operator -(Vector3D a, Vector3D b)
    {
        return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static Vector3D operator *(Vector3D a, double b)
    {
        return new Vector3D(a.x*b, a.y * b, a.z * b);
    }
    public Vector3D Normalized()
    {
        double length = x + y + z;
        return new Vector3D(x / length, y / length, z / length);
    }
    public double Magnitude()
    {
        return Math.Sqrt(Math.Pow(x,  2)+ Math.Pow(y , 2)+Math.Pow( z , 2));
    }
    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }
}
public class Planet{
    public double Mass;
    public double Gravity;
    public int Radius;
    public Planet(double mass, double Gravity, int Radius)
    {
        this.Mass = mass;
        this.Gravity = -Gravity;
        this.Radius = Radius;
    }
}
public class Trajectory
{
    public Planet planet;
    public Vector3D pos;
    public Vector3D vel;
    public Trajectory(Planet planet,Vector3D pos,Vector3D vel)
    {
        this.planet = planet;
        this.pos = pos;
        this.vel = vel;
    }
    public void Step()
    {
        float mod=10f;
        Vector3D g = pos.Normalized()*(planet.Gravity*(1/Math.Pow((pos.Magnitude() / planet.Radius), 2))) * mod;
        //Vector3D g = new Vector3D(0,-9.81,0);
        //Console.WriteLine(g);
        pos +=   g*.5 + (vel * mod);
        vel += g;
    }
}