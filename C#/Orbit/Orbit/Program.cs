using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;
using VRageMath;
namespace Orbit
{
    static class Program 
    {
        


        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static IMyGridTerminalSystem GridTerminalSystem;
        
        public void set_oriented_gyros(IMyGyro[] gyros, IMyTerminalBlock pivot, Vector3D tar){
            Matrix orientation;


            pivot.Orientation.GetMatrix(out orientation);
            MatrixD invMatrix = MatrixD.Invert(orientation);
            Vector3D localTar = Vector3D.Transform(tar, MatrixD.Invert(MatrixD.CreateFromDir(pivot.WorldMatrix.Forward, pivot.WorldMatrix.Up)) * orientation);
            Vector3D angsin = Vector3D.Cross(orientation.Forward, localTar);
            Vector3D ang = new Vector3D(Math.Sin(angsin.GetDim(0)), Math.Sin(angsin.GetDim(1)), Math.Sin(angsin.GetDim(2)));
            for (int i = 0; i < gyros.Length; i++)
            {
                Matrix gyro_or;
                gyros[i].Orientation.GetMatrix(out gyro_or);
                MatrixD invGyroMatrix = invMatrix*MatrixD.Invert(gyro_or);
                Vector3D angle = Vector3D.Transform(ang,invGyroMatrix);
                gyros[i].SetValueFloat("Pitch", (float)angle.GetDim(0));
                gyros[i].SetValueFloat("Yaw", (float)angle.GetDim(1));
                gyros[i].SetValueFloat("Roll", (float)angle.GetDim(2));
            }
        }
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Application.Run(form);

        }
    }

    public class Planet
    {
        public double u;
        public double Gravity;
        public int Radius;
        public Vector3D Pos;
        public Planet(double mass, double Gravity, int Radius, Vector3D pos)
        {
            Pos = pos;
            this.u = Gravity * Math.Pow(Radius, 2);
            this.Gravity = Gravity;
            this.Radius = Radius;
        }
        public static double GetPlanetMass(Vector3D origin, Vector3D currentPos, double grav)
        {
            double radius = Vector3D.Distance(origin,currentPos);
            double mass = Math.Pow(radius,2)*grav;
            return mass;
        }
        public double GetOrbitalVelocity(double distance,double a)
        {
            return Math.Sqrt(u*((2/distance)-(1/a)));
        }
        public static Vector3D GetPlanetCenter(Vector3D posA,Vector3D dirA,Vector3D posB,Vector3D dirB) {// Get 2 measurements of gravity vector at 2 positions
   
	Vector3D p; //Guess for planet center
	{   // Start off using to (hopefully) farthest points
		Vector3D x1=posA;
		Vector3D d1=dirA;
		Vector3D x2=posB;
		Vector3D d2=dirB;
 
		//Find point closest to both lines
		MatrixD dd=new MatrixD(
			d1.GetDim(0), d2.GetDim(0), 0,0,
			d1.GetDim(1), d2.GetDim(1), 0,0,
			d1.GetDim(2), d2.GetDim(2), 1,0,
			0,0,0,1);
		MatrixD ddT=Matrix.Transpose(dd);
		Vector3D tt=Vector3D.Transform(x1-x2,
			dd*Matrix.Invert(ddT*dd) );
 
		double t1=-tt.GetDim(0);
		double t2=tt.GetDim(1);
		p=0.5*(x1+d1*t1 + x2+d2*t2);
	}
 
	Vector3D pold=new Vector3D(0);
	//Refine estimate to be point closest to all lines
	//(in the least squares sense)
    List<Vector3D> x = new List<Vector3D>();
    x.Add(posA);
    x.Add(posB);
    List<Vector3D> d = new List<Vector3D>();
    x.Add(dirA);
    x.Add(dirB);
	while( (p-pold).Length() > 0.01 )
	{
		//Echo("Err: "+(p-pold).Length().ToString());
		pold=p;
 
		p=new Vector3D(0);
		double sum=0;
		for(int i=0;i<x.Count;++i)
		{   //Find point on line closest to pold
			double t=(pold-x[i]).Dot(d[i]);
			Vector3D pp=x[i]+t*d[i];
 
			p+=pp;
		}
		p/=x.Count;
		p=0.5*pold + 0.5*p;
	}
    return p;
    }
    }
    public class Trajectory
    {
        public Planet planet;
        public Vector3D pos;
        public Vector3D vel;
        public Trajectory(Planet planet, Vector3D pos, Vector3D vel)
        {
            this.planet = planet;
            this.pos = pos;
            this.vel = vel;
        }
        public void Step()
        {
            float mod = 1f;
            Vector3D grav = new Vector3D(-pos.GetDim(0), -pos.GetDim(1), -pos.GetDim(2));
            grav.Normalize();
            Vector3D g = grav * (1 / Math.Pow(Vector3D.Distance(pos, planet.Pos) / planet.Radius, 2) * planet.Gravity * mod);
            //Vector3D g = new Vector3D(0,-9.81,0);
            //Console.WriteLine(pos.Normalized());
            pos += g * .5 + (vel * mod);
            vel += g;
        }
    }
    public class Orbit
    {
        public double e; //Eccentricity
        public Vector3D E; // Eccentricity Vector
        public double a; //Semi-Major Axis
        public double I; // Inclination
        public double w; // Argument of periapsis
        public double q; // Longitude of Ascending Node
        public Vector3D vel;
        public Vector3D pos;
        public double M; // Mean Anomaly
        double lastM;
        public double Period;//orbital period
        public Planet planet;
        public double n; // Mean Angular Motion
        Vector3D N; // Unit normal to xy reference plane
        Vector3D h; // Specific Angular Momentum
        double en; // Energy of Orbiting Body
        public double lastTa;
        public DateTime epoch;
        public double v;// True Anomaly
        public double EC;// Eccentric Anomaly
        public double MeanAnomaly
        {
            get
            {
                M = lastM + n * ((DateTime.Now - epoch).TotalMilliseconds/1000);
                return M;
            }

        }
        public double Meananomaly(double t)
        {
            double m = lastM + n * ((t));
            if (m < 0)
            {
                M = Math.PI*2+m;
                return M;
            }
            else
            {
                M = m;
                return m;
            }
        }
        public double EccentricAnomaly
        {
            get
            {
                int limit = 0;
                double Ec = MeanAnomaly;
                double Ecl = EC;
                double m = Ec;
               
                while (true)
                {
                    Ec = Ec - (Ec - e * Math.Sin(Ec) - m) / (1 - e * cos(Ec));
                   
                    if (Ec - Ecl < 0.01||limit>1000)
                    {
                        EC = Ec;
                        
                        return EC;
                    }
                    else
                    {
                        Ecl = Ec;
                        limit++;
                    }
                }
            }
        }

        public double EccentricAnomaly2(double t)
        {

                int limit = 0;
                double Ec = Meananomaly(t);
                double Ecl = Ec;
                double m = Ec;
                
                while (true)
                {
                    Ec = Ec - (Ec - e * Math.Sin(Ec) - m)/(1-e*cos(Ec));
                    if (Ec - Ecl < 0.001 || limit > 1000)
                    {
                        EC = Ec;
                        return EC;
                    }
                    else
                    {
                        Ecl = Ec;
                        limit++;
                    }
                
            }
        }
        public double TrueAnomaly
        {
            get
            {
                double Ec = EccentricAnomaly;
                v =2 * Math.Atan2(Math.Sqrt(1+e)*Math.Sin(Ec/2),Math.Sqrt(1-e)*Math.Cos(Ec/2));
                
                return v;
            }
        }
        public double TrueAnomaly2(double t)
        {

                double Ec = EccentricAnomaly2(t);
                
                if(e>1){
                v = Math.Acos((e-Math.Cosh(Ec))/(Math.Cosh(Ec)*e-1));
                if(Ec<0)
                {
                    v=-v;
                }
                
                return v;
            }
            else
            {
                v = 2 * Math.Atan2(Math.Sqrt(1 + e) * Math.Sin(Ec / 2), Math.Sqrt(1 - e) * Math.Cos(Ec / 2));
                if (v < 0)
                {
                    v = v+2*Math.PI;
                    return v;
                }
                else { 
                return v;
                }
            }
        }
        public static double RadtoDeg(double rads)
        {
            return rads * 57.2958;
        }
        public static double DegtoRad(double deg)
        {
            return deg / 57.2958;
        }
        public static double ClampRadiansTwoPi(double angle)
        {
            angle = angle % (2 * Math.PI);
            if (angle < 0) return angle + 2 * Math.PI;
            else return angle;
        }
        public double TimeOfAscendingNode( Orbit b)
        {
            return TimeOfTrueAnomaly(AscendingNodeTrueAnomaly(b));
        }
        public double TimeOfTrueAnomaly(double a)
        {

            //  v = double.IsNaN(v) ? 0 : v;
            double sinE = sin(a) * Math.Sqrt(1 - Math.Pow(e, 2)) / (1 + e * cos(a));
            double cosE = (e + cos(a)) / (1 + e * cos(a));
            double Ec = Math.Atan2(sinE, cosE);//2 * Math.Atan(Math.Tan(a / 2) / Math.Sqrt(1 + e / 1 - e));
            Ec = Ec < 0 ? Ec + Math.PI * 2 : Ec;
            double m = Ec - e * Math.Sin(Ec);
            return TimeOfMeanAnomaly(m);
        }
        public double TimeOfTrueAnomalyFromEpoch(double a)
        {

            //  v = double.IsNaN(v) ? 0 : v;
            double Ec = 2 * Math.Atan(Math.Tan(a / 2) / Math.Sqrt(1 + e / 1 - e));
            double m = Ec - e * Math.Sin(Ec);
            return TimeOfMeanAnomalyFromEpoch(m);
        }
        public double TimeOfMeanAnomaly(double a)
        {
            double MeanDif = (a-lastM);
            if (MeanDif < 0) MeanDif = Math.PI * 2 + MeanDif;
            //if(e<1) MeanDif=ClampRadiansTwoPi(MeanDif);
            return ((MeanDif / n));
        }
        public double TimeOfMeanAnomalyFromEpoch(double a)
        {
            double MeanDif = a-lastM;
           
            if(e<1) MeanDif=ClampRadiansTwoPi(MeanDif);
            return ((MeanDif / n));
        }
        public void CartToKep(Vector3D pos, Vector3D vel)//This doesn't work, feel free to try and fix it.
        {
            this.pos = pos;
            this.vel = vel;
            en = vel.LengthSquared() / 2 - planet.u / pos.Length();
            a = Math.Abs(planet.u / (2 * en));
            h = Vector3D.Cross(pos, vel);
            E = Vector3D.Cross(vel, h) / planet.u - (pos / pos.Length());
            Console.WriteLine(E);
            e = E.Length();
            I = Math.Acos(h.GetDim(2) / h.Length());
            N = new Vector3D(-h.GetDim(1), h.GetDim(0), 0);
            if (N.GetDim(1) >= 0)
            {
                q = Math.Acos(N.GetDim(0) / N.Length());
                q = double.IsNaN(q) ? 0 : q;
            }
            else
            {
                q = 2 * Math.PI - Math.Acos(N.GetDim(0) / N.Length());
                q = double.IsNaN(q) ? 2*Math.PI : q;
            }
            
            n = Math.Sqrt(planet.u / Math.Pow(a, 3));
            if (q != 0)
            {
                w = E.GetDim(2) >= 0 ? Math.Acos(Vector3D.Dot(N, E) / (N.Length() * E.Length())) : Math.PI * 2 - Math.Acos(Vector3D.Dot(N, E) / (N.Length() * E.Length()));

            }
            else
            {
                double W=Math.Atan2(E.GetDim(1),E.GetDim(0));
                w = h.GetDim(2) < 0 ? 2*Math.PI-W:W;


            }

            if (double.IsNaN(w)) w = 0;
            if (w < 0) w += Math.PI * 2;
            epoch = DateTime.Now;
            if (E.Length() != 0)
            {
                v = Vector3D.Dot(pos, vel) >= 0 ? Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length())) : Math.PI * 2 - Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length()));
            }
            else
            {
                if (I != 0)
                {
                    v = Vector3D.Dot(N, vel) <= 0 ? Math.Acos(Vector3D.Dot(N, pos) / (N.Length() * pos.Length())) : Math.PI * 2 - Math.Acos(Vector3D.Dot(N, pos) / (N.Length() * pos.Length()));
                    Console.WriteLine(N);
                }
                else
                {
                    v = vel.GetDim(0) <= 0 ? Math.Acos(pos.GetDim(0) / (pos.Length())) : Math.PI * 2 - Math.Acos(pos.GetDim(0) / (pos.Length()));
                }
            }
            if (double.IsNaN(v)) v = 0;
            lastTa = v;
            double sinE = sin(a) * Math.Sqrt(1 - Math.Pow(e, 2)) / (1 + e * cos(a));
            double cosE = (e + cos(a)) / (1 + e * cos(a));
            double Ec = Math.Atan2(sinE, cosE);
            EC = 2 * Math.Atan(Math.Tan(v / 2) / Math.Sqrt(1 + e / 1 - e));
            EC = EC < 0 ? EC + Math.PI * 2 : EC;
            if (double.IsNaN(EC)) EC = 0;
            M = EC - e * Math.Sin(EC);
            lastM = M;
            Period = Math.PI * 2 * Math.Sqrt(Math.Pow(a,3)/planet.u);
        }
        public double GetPhaseAngle2( Orbit b, double t)
        {
            Vector3D velA;
            Vector3D velB;
            Vector3D posB;
            Vector3D normalA = NormalVector();
            Vector3D posA;
            KepToCart(out posA,out velA,t);
            b.KepToCart(out posB, out velB, t);

            Vector3D projectedB = Vector3D.Reject(posB, normalA);
            posA.Normalize();
            
            double angle = Math.Acos(Vector3D.Dot(posA, projectedB));
            if (Vector3D.Dot(Vector3D.Cross(normalA, posA), projectedB) < 0)
            {
                angle = Math.PI*2 - angle;
            }
            return angle;
        }
        public double GetPhaseAngle2TrueAnomaly(Orbit b, double t)
        {
            Vector3D velA;
            Vector3D velB;
            Vector3D posB;
            Vector3D normalA = NormalVector();
            Vector3D posA;
            KepToCartAtTrueAnomaly(out posA, out velA, t);
            b.KepToCart(out posB, out velB, TimeOfTrueAnomaly(t));

            Vector3D projectedB = Vector3D.Reject(posB, normalA);
            posA.Normalize();

            double angle = Math.Acos(Vector3D.Dot(posA, projectedB));
            if (Vector3D.Dot(Vector3D.Cross(normalA, posA), projectedB) < 0)
            {
                angle = Math.PI * 2 - angle;
            }
            return angle;
        }

        public void CartToKep(Vector3D pos, Vector3D vel, DateTime time)
        {
            CartToKep(pos, vel);
            epoch = time;
        }
        public static void KepToCart(out Vector3D pos, out Vector3D vel, double a, double I, double w, double q, double e, double M, double E, double V, Planet planet)
        {//This doesn't work, feel free to try and fix it.
            
            double r = a*(1 - e * Math.Cos(E));
            Vector3D Op = new Vector3D(r*Math.Cos(V),r*Math.Sin(V),0);
            Vector3D Ov = new Vector3D(Math.Sqrt(planet.u * a) / r * -Math.Sin(E), Math.Sqrt(planet.u * a) / r*Math.Sqrt(1-Math.Pow(e,2))*Math.Cos(E),0);
            double Px = Op.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Op.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            double Py = Op.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Op.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            double Pz = Op.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Op.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            double Vx = Ov.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Ov.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            double Vy = Ov.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            double Vz = Ov.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            pos = new Vector3D(Px,Py,Pz);
            vel = new Vector3D(Vx,Vy,Vz);
            
        }
        public double FindInterceptPoint(Orbit orbit)
        {
            DateTime now = DateTime.Now;
            Vector3D orbitP;
            Vector3D orbitV;
            Vector3D myP;
            Vector3D myV;
            double lastDist=1000000000000000;
            for (int i = 0; i <= Period; i++)
            {
                KepToCart(out orbitP, out orbitV,i);
                orbit.KepToCart(out myP,out myV,i);
                double d = Vector3D.Distance(myP,orbitP);
                if (d > lastDist)
                {
                    return (lastDist);
                }
                else
                {
                    lastDist = d;
                }
            }
            return double.PositiveInfinity;
        }
        
        public void KepToCart(out Vector3D pos, out Vector3D vel) //This doesn't work, feel free to try and fix it.
        {
            double h = Math.Sqrt(planet.u * a * (1 - Math.Pow(e, 2)));
            double E = this.EccentricAnomaly;
            double V = this.TrueAnomaly;
            
            //Console.WriteLine(r);
            double p = Math.Pow(h, 2);
            double r = a * (1 - e * Math.Cos(E));
            // Vector3D Op = new Vector3D(r * Math.Cos(V), r * Math.Sin(V), 0);
            // Vector3D Ov = new Vector3D(Math.Sqrt(planet.Mass * a) / r * -Math.Sin(E), Math.Sqrt(planet.Mass * a) / r * Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Cos(E), 0);
            double Px = r * (cos(q) * cos(w + V) - sin(q) * sin(w + V) * cos(I));
            double Py = r * (sin(q) * cos(w + V) + cos(q) * sin(w + V) * cos(I));
            double Pz = r * (sin(I) * sin(w + V));
            double Vx = ((Px * h * e) / (r * p)) * sin(V) - (h / r) * (cos(q) * sin(w + V) + sin(q) * cos(w + V * cos(I)));
            double Vy = ((Py * h * e) / (r * p)) * sin(V) - (h / r) * (sin(q) * sin(w + V) - cos(q) * cos(w + V * cos(I)));
            double Vz = ((Py * h * e) / (r * p)) * sin(V) + (h / r) * sin(I) * cos(w + V);
            pos = new Vector3D(Px, Py, Pz);
            vel = new Vector3D(Vx, Vy, Vz);
           // Console.WriteLine(Op);
           // Console.WriteLine((Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)));
        } // + (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q))
        public static double cos(double i) { return Math.Cos(i); }
        public static double sin(double i) { return Math.Sin(i); }

        public void KepToCart2(out Vector3D pos, out Vector3D vel, double t) 
        {
            double h = Math.Sqrt(planet.u*a*(1-Math.Pow(e,2)));
            double E = this.EccentricAnomaly2(t);
            double V = this.TrueAnomaly2(t);
            
            double r = a * (1 - e * Math.Cos(E));
          //  Console.WriteLine(this.E);
            double p = Math.Pow(h,2);
           // Vector3D Op = new Vector3D(r * Math.Cos(V), r * Math.Sin(V), 0);
           // Vector3D Ov = new Vector3D(Math.Sqrt(planet.Mass * a) / r * -Math.Sin(E), Math.Sqrt(planet.Mass * a) / r * Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Cos(E), 0);
            double Px =r*(cos(q)*cos(w+V)-sin(q)*sin(w+V)*cos(I));
            double Py =r*(sin(q)*cos(w+V)+cos(q)*sin(w+V)*cos(I));
            double Pz =r*(sin(I)*sin(w+V));
            double Vx = ((Px * h * e) / (r * p)) * sin(V) - (h / r) * (cos(q) * sin(w + V) + sin(q) * cos(w + V * cos(I)));
            double Vy = ((Py * h * e) / (r * p)) * sin(V) - (h / r) * (sin(q) * sin(w + V) - cos(q) * cos(w + V * cos(I)));
            double Vz = ((Py * h * e) / (r * p)) * sin(V) + (h / r)*sin(I)*cos(w+V);
            pos = new Vector3D(Px, Py, Pz);
            vel = new Vector3D(Vx, Vy, Vz);
            // Console.WriteLine((Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)));
        } // + (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q))
        public void KepToCart(out Vector3D pos, out Vector3D vel, double t)
        {
            double h = Math.Sqrt(planet.u * a * (1 - Math.Pow(e, 2)));
            double E = this.EccentricAnomaly2(t);
            double V = this.TrueAnomaly2(t);
            double r = a * (1 - e * Math.Cos(E));
            Vector3D Op = new Vector3D(r * Math.Cos(V), r * Math.Sin(V), 0);
            Vector3D Ov = new Vector3D(Math.Sqrt(planet.u * a) / r * -Math.Sin(E), Math.Sqrt(planet.u * a) / r * Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Cos(E), 0);
           // double Px = Op.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Op.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            //double Py = Op.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Op.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            //double Pz = Op.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Op.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            //double Vx = Ov.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Ov.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            //double Vy = Ov.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            //double Vz = Ov.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            pos = Vector3D.Transform(Op,rotationToReferenceFrame());
            vel = Vector3D.Transform(Ov, rotationToReferenceFrame());
            // Console.WriteLine((Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)));
        } // + (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q))
        public void KepToCartAtTrueAnomaly(out Vector3D pos, out Vector3D vel, double t)
        {
            double h = Math.Sqrt(planet.u * a * (1 - Math.Pow(e, 2)));
            double sinE = sin(t) * Math.Sqrt(1 - Math.Pow(e, 2)) / (1 + e * cos(t));
            double cosE = (e + cos(t)) / (1 + e * cos(t));
            double Ec = Math.Atan2(sinE, cosE);
            double E = 2 * Math.Atan(Math.Tan(t / 2) / Math.Sqrt(1 + e / 1 - e));
            E = E < 0 ? E + Math.PI * 2 : E;
            double V = t;
            double r = a * (1 - e * Math.Cos(E));
            Vector3D Op = new Vector3D(r * Math.Cos(V), r * Math.Sin(V), 0);
            Vector3D Ov = new Vector3D(Math.Sqrt(planet.u * a) / r * -Math.Sin(E), Math.Sqrt(planet.u * a) / r * Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Cos(E), 0);
            // double Px = Op.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Op.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            //double Py = Op.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Op.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            //double Pz = Op.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Op.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            //double Vx = Ov.GetDim(0) * (Math.Cos(w) * Math.Cos(q) - Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) - Ov.GetDim(1) * (Math.Sin(w) * Math.Cos(q) + Math.Cos(w) * Math.Cos(I) * Math.Sin(q));
            //double Vy = Ov.GetDim(0) * (Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q));
            //double Vz = Ov.GetDim(0) * (Math.Sin(w) * Math.Sin(I)) + Ov.GetDim(1) * (Math.Cos(w) * Math.Sin(I));
            pos = Vector3D.Transform(Op, rotationToReferenceFrame());
            vel = Vector3D.Transform(Ov, rotationToReferenceFrame());
            // Console.WriteLine((Math.Cos(w) * Math.Sin(q) + Math.Sin(w) * Math.Cos(I) * Math.Cos(q)));
        } // + (Math.Cos(w) * Math.Cos(q) * Math.Cos(I) - Math.Sin(w) * Math.Sin(q))
        public Orbit(Vector3D pos, Vector3D vel,Planet planet)
        {
            this.planet = planet;
            CartToKep(pos, vel);
        }
        public Orbit(Vector3D pos, Vector3D vel, Planet planet,DateTime t)
        {
            this.planet = planet;
            CartToKep(pos, vel,t);
        }
        public override string ToString()
        {
            return "Semi-Major Axis: " + a + " Eccentricity: "+e+" Inclination: "+RadtoDeg(I)+" Longitude of Ascending Node: "+RadtoDeg(q)+" Argument of Periapsis: "+RadtoDeg(w)+" Mean Anomaly: "+RadtoDeg(MeanAnomaly)+" Eccentric Anomaly: "+RadtoDeg(EccentricAnomaly)+" True Anomaly: "+RadtoDeg(TrueAnomaly);
        }
        public string ToString1(double t)
        {
            return "Semi-Major Axis: " + a + " Eccentricity: " + e + " Inclination: " + RadtoDeg(I) + " Longitude of Ascending Node: " + RadtoDeg(q) + " Argument of Periapsis: " + RadtoDeg(w) + " Mean Anomaly: " + RadtoDeg(Meananomaly(t)) + " Eccentric Anomaly: " + RadtoDeg(EccentricAnomaly2(t)) + " True Anomaly: " + RadtoDeg(TrueAnomaly2(t));
        }
        public Quaternion rotationToReferenceFrame()
        {
            Vector3D axisOfInclination=new Vector3D(cos(-w),sin(-w),0);
            return Quaternion.Concatenate(Quaternion.CreateFromAxisAngle(new Vector3D(0, 0, 1), (float)(q + w)), Quaternion.CreateFromAxisAngle(axisOfInclination, (float)I));
        }
        public Vector3D NormalVector()
        {
            return Vector3D.Transform(new Vector3D(0, 0, 1),rotationToReferenceFrame());
        }
        public double GetPhaseAngle(Orbit a,double t)
        {
            Vector3D n = NormalVector();
            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            KepToCart(out pos1, out vel1, t);
            a.KepToCart(out pos2, out vel2, t);
            pos2 = pos2 - (n * Vector3D.Dot(pos2, n));
            double r1 = pos1.Length();
            double r2 = pos2.Length();
            double phaseAngle = Math.Acos(Vector3D.Dot(pos1, pos2) / (r1 * r2));
            if(Vector3D.Dot(Vector3D.Cross(pos1,pos2),n)<0){
                phaseAngle = 2 * Math.PI - phaseAngle;
            }
            if (a.a < this.a)
            {
                phaseAngle = phaseAngle-2 * Math.PI;
            }
            return phaseAngle;
        }
        public double GetPhaseAngleTrueAnomaly(Orbit a, double t)
        {
            Vector3D n = NormalVector();
            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            KepToCartAtTrueAnomaly(out pos1, out vel1, t);
            a.KepToCart(out pos2, out vel2, TimeOfTrueAnomaly(t));
            pos2 = pos2 - (n * Vector3D.Dot(pos2, n));
            double r1 = pos1.Length();
            double r2 = pos2.Length();
            double phaseAngle = Math.Acos(Vector3D.Dot(pos1, pos2) / (r1 * r2));
            if (Vector3D.Dot(Vector3D.Cross(pos1, pos2), n) < 0)
            {
                phaseAngle = 2 * Math.PI - phaseAngle;
            }
            if (a.a < this.a)
            {
                phaseAngle = phaseAngle - 2 * Math.PI;
            }
            return phaseAngle;
        }
        public double TrueAnomalyAtPosition(Vector3D pos,Vector3D vel){
            //Vector3D b = Vector3D.Transform(a,Quaternion.Conjugate(rotationToReferenceFrame()));
            //return Math.Atan2(b.GetDim(1),b.GetDim(0));
            h = Vector3D.Cross(pos, vel);
            E = Vector3D.Cross(vel, h) / planet.u - (pos / pos.Length());
            v = Vector3D.Dot(pos, vel) >= 0 ? Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length())) : Math.PI * 2 - Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length()));
            return v;
        }
        public double TrueAnomalyAtPosition(Vector3D a)
        {
            Vector3D b = Vector3D.Transform(a,Quaternion.Conjugate(rotationToReferenceFrame()));
            double V= Math.Atan2(b.GetDim(1),b.GetDim(0));
           // h = Vector3D.Cross(pos, vel);
           // E = Vector3D.Cross(vel, h) / planet.u - (pos / pos.Length());
           // v = Vector3D.Dot(pos, vel) >= 0 ? Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length())) : Math.PI * 2 - Math.Acos(Vector3D.Dot(E, pos) / (E.Length() * pos.Length()));
           // return v;
            if (V < 0)
            {
                V = V + 2 * Math.PI;
            }
            return V;
        }
        public void SetEpoch(DateTime time)
        {
           
            //Console.WriteLine(EC);

            double dT = (time-epoch).TotalSeconds;
            Vector3D pos1;
            Vector3D vel1;
            KepToCart(out pos1, out vel1, dT);
            CartToKep(pos1, vel1, time);
        }
        public double NextClosestApproachDistance( Orbit b, double UT)
        {
            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            KepToCart(out pos1, out vel1, NextClosestApproachTime(b, UT));
            b.KepToCart(out pos2, out vel2, b.NextClosestApproachTime(this,UT));
            return Vector3D.Distance(pos1,pos2);
        }
        public double NextClosestApproachTime(Orbit b, double UT)
        {
            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            KepToCart(out pos1, out vel1, UT);
            b.KepToCart(out pos2, out vel2, UT);
            double closestApproachTime = UT;
            double closestApproachDistance = Double.MaxValue;
            double minTime = UT;
            double interval = Period;
            if (e > 1)
            {
               // interval = 100 / ((2*Math.PI)/Period); //this should be an interval of time that covers a large chunk of the hyperbolic arc
            }
            double maxTime =UT+ interval;
            const int numDivisions = 20;

            for (int iter = 0; iter < 10; iter++)
            {
                double dt = (maxTime - minTime) / numDivisions;
                for (int i = 0; i < numDivisions; i++)
                {
                    double t = minTime + i * dt;
                    KepToCart(out pos1, out vel1, t);
                    b.KepToCart(out pos2, out vel2, t);
                    double distance = Vector3D.Distance(pos1,pos2);
                    if (distance < closestApproachDistance)
                    {
                        closestApproachDistance = distance;
                        closestApproachTime = t;
                    }

                }
                minTime = MathHelper.Clamp(closestApproachTime - dt, UT, UT + interval);
                maxTime = MathHelper.Clamp(closestApproachTime + dt, UT, UT + interval);
                
            }

            return closestApproachTime;
        }


        //keeps angles in the range -180 to 180
        public static double ClampDegrees360(double angle)
        {
            angle = angle % 360.0;
            if (angle < 0) return angle + 360.0;
            else return angle;
        }
        public double AscendingNodeTrueAnomaly(Orbit b)
        {
            Vector3D an = Vector3D.Cross(NormalVector(), b.NormalVector());
            
            return TrueAnomalyAtPosition(an);
        }
        public double DescendingNodeTrueAnomaly(Orbit b)
        {
            Vector3D an = Vector3D.Cross(NormalVector(), b.NormalVector());

            return (TrueAnomalyAtPosition(-an));
        }
    }
    public class Maneuver
    {
        public DateTime time;
        public double TrueAnomaly;
        public Vector3D direction;
        public double velocity;
        public Maneuver(DateTime time, Vector3D direction, double velocity)
        {
            this.time = time;
            this.direction = direction;
            this.velocity = velocity;
        }
        public Maneuver(double time, Vector3D direction, double velocity)
        {
            this.TrueAnomaly = time;
            this.direction = direction;
            this.velocity = velocity;
        }
        public static MatrixD GetDirections(Vector3D pos,Vector3D vel)
        {
            Vector3D f = vel;
            f.Normalize();
            Vector3D u = -pos;
            u.Normalize();
            return MatrixD.CreateFromDir(f, u);
        }

        public static Maneuver GetHohmannTransfer(Orbit orbit1,Orbit orbit2,double t) {
            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            double now = 0;
            double deltaV=0;
            double TransferTime = Math.PI * Math.Sqrt(Math.Pow((orbit1.a + orbit2.a), 3) / (8 * orbit1.planet.u));
            double h2 = Math.Sqrt(orbit1.planet.u/Math.Pow(orbit2.a,3));
            double angle = Orbit.RadtoDeg(Math.PI - h2 * TransferTime);
            bool test = true;
            Vector3D dir = new Vector3D(0,0,-1);
            //for (int k = 0; k < 6; k++) {
                if (test == false)
                {
                    //break;
                }
                for (int i = (int)t; i < t + 36000; i++)
                {

                    orbit1.KepToCartAtTrueAnomaly(out pos1, out vel1, Orbit.DegtoRad(i));
                    orbit2.KepToCartAtTrueAnomaly(out pos2, out vel2, Orbit.DegtoRad(i));
                    pos1.Normalize();
                    pos2.Normalize();
                    double ang = Orbit.RadtoDeg(orbit1.GetPhaseAngleTrueAnomaly(orbit2, Orbit.DegtoRad(i)));


                    if (ang <= angle + 1 && ang >= angle - 1)
                    {
                        now = i;
                       // if (k >= 4)
                        //{
                            MatrixD directions = GetDirections(pos1, vel1);
                            dir = Vector3D.Transform(dir, directions);
                       // }
                        orbit1.KepToCartAtTrueAnomaly(out pos1, out vel1, Orbit.DegtoRad(i));
                        orbit2.KepToCart(out pos2, out vel2, orbit1.TimeOfTrueAnomaly(Orbit.DegtoRad(i)) + TransferTime);
                        double r1 = pos1.Length();
                        double r2 = pos2.Length();
                        deltaV = orbit1.planet.GetOrbitalVelocity(r1, (r1 + r2) / 2) - (vel1.Length()); //Math.Sqrt(orbit1.planet.u / r1) * (Math.Sqrt((2 * r2) / (r2 + r1)) - 1);
                        TransferTime = Math.PI * Math.Sqrt(Math.Pow((r1 + r2), 3) / (8 * orbit1.planet.u));
                        angle = Orbit.RadtoDeg(Math.PI - h2 * TransferTime);
                        test = false;
                        break;
                    }

                }
               // }

            //Console.WriteLine(angle + " Found ang");
            Maneuver man = new Maneuver(Orbit.DegtoRad(now), dir, deltaV);
            return man;
            
        }
        public static Maneuver CircularizeAtApoapsis(Orbit a,double t)
        {
            double aoapsisHeight = Math.Sqrt(((1-a.e)*a.planet.u)/((1+a.e)*a.a));
            Vector3D pos;
            Vector3D vel;
            a.KepToCartAtTrueAnomaly(out pos,out vel, a.lastTa);
            pos=-pos;
            Vector3D dir = new Vector3D(0,0,-1);
            double apoapsisTa=a.TrueAnomalyAtPosition(pos);
            double deltaV = a.planet.GetOrbitalVelocity(aoapsisHeight,aoapsisHeight );
            MatrixD directions = GetDirections(pos, vel);
            dir = Vector3D.Transform(dir, directions);
            return new Maneuver(apoapsisTa,dir,deltaV);
        }
        public static Maneuver GetPhasingManeuver(Orbit orbit1,Orbit orbit2)
        {
            double h1 = Math.Sqrt(orbit1.planet.u*orbit1.a*(1-Math.Pow(orbit1.e,2)));

            Vector3D pos1;
            Vector3D vel1;
            Vector3D pos2;
            Vector3D vel2;
            DateTime now = DateTime.Now;//orbit1.FindInterceptPoint(orbit2);
            orbit1.KepToCart(out pos1, out vel1, now.Second);
            orbit2.KepToCart(out pos2, out vel2, now.Second);
            pos1.Normalize();
            pos2.Normalize();

            double ang = Math.Acos(Vector3D.Dot(pos1, pos2));
            double E = 2*Math.Atan(Math.Sqrt((1-orbit1.e)/(1+orbit1.e))*Math.Tan(ang/2));
            double t = (orbit1.Period/(Math.PI*2))*(E-orbit1.e*Math.Sin(E));
            double T = orbit1.Period - t;
            double a = Math.Pow((Math.Sqrt(orbit1.planet.u)*T)/(2*Math.PI),(2.0/3.0));
            orbit1.KepToCart(out pos1, out vel1, now.Second);
            orbit2.KepToCart(out pos2, out vel2, now.Second);
            double rp = pos1.Length();
            double ra = 2*a-rp;
            double h2 = Math.Sqrt(2 * orbit1.planet.u) * Math.Sqrt(Math.Abs(ra-rp)/(ra+rp));
            MatrixD directions = GetDirections(pos1, vel1);
            Vector3D dir = new Vector3D(0, 0, 1);
            dir = Vector3D.Transform(dir, directions);

            double deltaV = h1 / pos1.Length() - h2 / pos1.Length();

            return new Maneuver(now,dir,deltaV);
        }
        public static Maneuver GetPlaneMatch(Orbit orbit1, Orbit orbit2,double t)
        {
            Vector3D pos1;
            Vector3D vel1;
            orbit1.KepToCartAtTrueAnomaly(out pos1, out vel1, t);
            double dI=orbit1.I-orbit2.I;
            double deltaV=(2*Math.Sin(dI/2)*Math.Sqrt(1-Math.Pow(orbit1.e,2))*Math.Cos(orbit1.w+t)*orbit1.n*orbit1.a)/(1+orbit1.e*Math.Cos(t));
            double V= orbit1.AscendingNodeTrueAnomaly(orbit2);
            MatrixD directions = GetDirections(pos1, vel1);
            Vector3D dir = new Vector3D(0, 1, 0);
            dir = Vector3D.Transform(dir, directions);
            return new Maneuver(V,dir,deltaV);
        }
    }
}
