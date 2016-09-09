using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Net;
using System.Net.Mail;
using System.Timers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
namespace ConsoleApplication1
{

    public static class Program
    {
 	public static void Main(string[] args){
		int resolution = 1024;
		Bitmap map = new Bitmap(resolution,resolution);
		Bitmap map1 = new Bitmap(resolution,resolution);
				Random rand = new Random();
		MySimplexFast gen = new MySimplexFast(rand.Next(),2);
		MySimplexFast gen2 = new MySimplexFast(rand.Next(),32);
		MySimplexFast gen3 = new MySimplexFast(rand.Next(),32);
		MySimplexFast gen4 = new MySimplexFast(rand.Next(),32);
		MySimplexFast gen5 = new MySimplexFast(rand.Next(),32);
		var vor = new Voronoi();
		vor.Frequency = 0.01;
		//vor.DistanceEnabled = true;
		var chooser = new Select();
		for(int y = -resolution/2;y<resolution/2;y++){
			for(int x = -resolution/2;x<resolution/2;x++){
				Vector3D samp = new Vector3D(-x,-y,resolution/2);
				//samp.Normalize();
				//samp.MapToSphere();
				double biomeval1 = (gen2.GetValue(samp.X,samp.Y,samp.Z,1,0.0008,2,2,0.6));
				
				double val1 = -1;//(gen.GetValue(samp.X,samp.Y,samp.Z,8,0.01,3,2,0.7));
				double biomeval2 = (gen3.GetValue(samp.X,samp.Y,samp.Z,1,0.001,2,2,0.6)+1)/2;
				double val2 = 1;//(gen4.GetValue(samp.X,samp.Y,samp.Z,16,0.003,2,2,.3));
				//double val2 = (gen2.GetValue((double)x/resolution,(double)y/resolution,0,1,16,2,2,0.3));
				//double val3 = (gen3.GetValue((double)x/resolution,(double)y/resolution,0,1,16,2,2,0.3));
				double biomeval3 = 1-Math.Abs(biomeval2-biomeval1);
				double val3 = 0;//(gen.GetValue(samp.X,samp.Y,samp.Z,8,2,3,.2,0.7))*0.1;
				
				var totalval=(biomeval1+biomeval2);
				//biomeval1/=totalval;
				//biomeval2/=totalval;
				//biomeval2 = Lerp(biomeval3,biomeval2,biomeval2);
				//biomeval1 = Lerp(biomeval2,biomeval1,biomeval1);
				double val = chooser.Choose(val1,val2,vor.GetValue(samp.X,samp.Y,samp.Z));//(val1*biomeval1+val2*biomeval2+val3*biomeval3);
				//if(biomeval1>biomeval2) val = Lerp(val2,val1,biomeval1);
				//if(biomeval2>biomeval1) val = Lerp(val2,val1,biomeval2);
				if(val>1) val = 1;
				if(val<-1) val = -1;
				//val*=255;
				val= vor.GetValue(x,y,0);
		  		val = ((val+1)/(1+1) *(1+0)+0)*255;
				double red =255*((biomeval1+1)/2);
				double green = 255*((biomeval2+1)/2);
		  		//val2 = ((val2+1)/(1+1) *(1+0)+0)*255;
		  		//val3 = ((val3+1)/(1+1) *(1+0)+0)*255;
				var test = (rand.NextDouble()/255)*32730+37;
				//Console.Beep((int)test,100);
				map.SetPixel(x+resolution/2,y+resolution/2,Color.FromArgb((int)val,(int)val,(int)val));
				map1.SetPixel(x+resolution/2,y+resolution/2,Color.FromArgb((int)red,(int)green,(int)0));
				Console.ReadLine();
}}
map.Save("test.png",ImageFormat.Png);
map1.Save("test1.png",ImageFormat.Png);
		Bitmap map2 = new Bitmap(resolution,resolution);
		for(int y = -resolution/2;y<resolution/2;y++){
			for(int x = -resolution/2;x<resolution/2;x++){
				Vector3D samp = new Vector3D(-resolution/2,-y,-(x));
				//samp.Normalize();
				//samp.MapToSphere();

				double val = (gen.GetValue(samp.X,samp.Y,samp.Z,1,0.002,2,2,0.6));
				//double val2 = (gen2.GetValue((double)x/resolution,(double)y/resolution,0,1,16,2,2,0.3));
				//double val3 = (gen3.GetValue((double)x/resolution,(double)y/resolution,0,1,16,2,2,0.3));
		  		val = ((val+1)/(1+1) *(1+0)+0)*255;
		  		//val2 = ((val2+1)/(1+1) *(1+0)+0)*255;
		  		//val3 = ((val3+1)/(1+1) *(1+0)+0)*255;
				map2.SetPixel(x+resolution/2,y+resolution/2,Color.FromArgb((int)val,(int)val,(int)val));
}}
map2.Save("test2.png",ImageFormat.Png);
	}
public static double Lerp(double val1,double val2,double x){
	return val1+(val2-val1)*x;
}
public static double Sigmoid(double x){
	return 1/(1+Math.Pow(Math.E,-x));
}
    }
public class Select{
	double lowerbound=.5;
	double upperbound=1;
	double edgefalloff=.125;
	public Select(){
		double bounds = upperbound-lowerbound;
		edgefalloff = (edgefalloff>bounds/2)?bounds/2:edgefalloff;
	}
	public double Choose(double val1,double val2,double control){
		if(edgefalloff>0){
			if(control<(lowerbound-edgefalloff)){
				return val1;
			}
			else if(control<(lowerbound+edgefalloff)){
				double lowercurve = lowerbound - edgefalloff;
				double uppercurve = lowerbound + edgefalloff;
				double alpha = SCurve3((control-lowercurve)/(uppercurve-lowercurve));
				return Program.Lerp(val1,val2,alpha);
			}
			else if(control<(upperbound - edgefalloff)){
				return val2;
			}
			else if(control<(upperbound + edgefalloff)){
				double lowercurve = upperbound - edgefalloff;
				double uppercurve = upperbound + edgefalloff;
				double alpha = SCurve3((control-lowercurve)/(uppercurve-lowercurve));
				return Program.Lerp(val2,val1,alpha);
			}
			else{ return val1;}
		}
		return 0;
	}
	public double SCurve3 (double a){
		return (a * a * (3.0 - 2.0 * a));
	}
}
public struct Vector3D{
public double X;
public double Y;
public double Z;
public Vector3D(double x, double y, double z){
	X=x;
	Y=y;
	Z=z;
}
public void MapToSphere(){
	X*=Math.Sqrt(1-Y*Y/2.0-Z*Z/2.0+(Y*Y*Z*Z)/3.0);
	Y*=Math.Sqrt(1-Z*Z/2.0-X*X/2.0+(Z*Z*X*X)/3.0);
	Z*=Math.Sqrt(1-X*X/2.0-Y*Y/2.0+(X*X*Y*Y)/3.0);
	
}
public void Normalize(){
	double length = Math.Sqrt(X*X+Y*Y+Z*Z);
	X/=length;
	Y/=length;
	Z/=length;
}
}
public class Voronoi:ValueNoiseBasis{
     public double Frequency { get; set; }
        public double Displacement { get; set; }
        public bool DistanceEnabled { get; set; }
        public int Seed { get; set; }

        public Voronoi()
        {
            Frequency = 1.0;
            Displacement = 1.0;
            Seed = 0;
            DistanceEnabled = false;
        }

        public double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            int xInt = (x > 0.0 ? (int)x : (int)x - 1);
            int yInt = (y > 0.0 ? (int)y : (int)y - 1);
            int zInt = (z > 0.0 ? (int)z : (int)z - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;
	    double secondBestDist = 2147483647.0;
            double xsecondBest = 0;
            double ysecondBest = 0;
            double zsecondBest = 0;
            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++)
                {
                    for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                    {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        double xPos = xCur + ValueNoise(xCur, yCur, zCur, Seed);
                        double yPos = yCur + ValueNoise(xCur, yCur, zCur, Seed + 1);
                        double zPos = zCur + ValueNoise(xCur, yCur, zCur, Seed + 2);
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
			    secondBestDist = minDist;
                            minDist = dist;
			    xsecondBest = xCandidate;
			    ysecondBest = yCandidate;
			    zsecondBest = zCandidate;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;

                        }
			else if (dist<secondBestDist){
			    xsecondBest = xPos;
			    ysecondBest = yPos;
			    zsecondBest = zPos;
			    secondBestDist = dist;
			}
                    }
                }
            }
			
            double value;
	    double valuesecondBest=0;
		//double xsecondBestDist = xsecondBest-x;
		//double ysecondBestDist = ysecondBest-y;
		//double zsecondBestDist = zsecondBest-z;
	//double seconddistbest = xsecondBestDist*xsecondBestDist+ysecondBestDist*ysecondBestDist+zsecondBestDist*zsecondBestDist;
            if (DistanceEnabled)
            {
                // Determine the distance to the nearest seed point.
                double xDist = xCandidate - x;
                double yDist = yCandidate - y;
                double zDist = zCandidate - z;

                value = (System.Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                  ) * MathHelper.Sqrt3 - 1.0;
                //valuesecondBest = (System.Math.Sqrt(xsecondBestDist * xsecondBestDist + ysecondBestDist * ysecondBestDist + zsecondBestDist * zsecondBestDist)
               //  ) * MathHelper.Sqrt3 - 1.0;
            }
            else
            {
                value = 0.0;
		//valuesecondBest = 0.0;
            }
		
            int x0 = (xCandidate > 0.0 ? (int)xCandidate : (int)xCandidate - 1);
            int y0 = (yCandidate > 0.0 ? (int)yCandidate : (int)yCandidate - 1);
            int z0 = (zCandidate > 0.0 ? (int)zCandidate : (int)zCandidate - 1);
			int x1 = (xsecondBest > 0.0 ? (int)xsecondBest : (int)xsecondBest - 1);
			int y1 = (ysecondBest > 0.0 ? (int)ysecondBest : (int)ysecondBest - 1);
			int z1 = (zsecondBest > 0.0 ? (int)zsecondBest : (int)zsecondBest - 1);
			float midx = (-xCandidate+xsecondBest)/2;
			float midy = (-yCandidate+ysecondBest)/2;
			float linenormalx = -midy;
			float linenormaly = midx;
			float slope = linenormaly/linenormalx;
			float distancefromline = Math.Abs(slope*(xCandidate-midx)+(yCandidate-midy))/Math.Sqrt(slope*slope+1);
			float interpval = 0;
			if(distancefromline<=10){
				interpval = distancefromline/10f;
			}
			float mynoise = (double)ValueNoise(x0, y0, z0);
			float othernoise = (double)ValueNoise(x1, y1, z1)
			value = mynoise-(mynoise-othernoise)*interpval;
            // Return the calculated distance with the displacement value applied.
            return  value;//+ (Displacement * ((double)ValueNoise(x0, y0, z0)));//Program.Lerp((double)ValueNoise(x0, y0, z0),(double)ValueNoise(x1, y1, z1),ratio));
        }
    }
    public class ValueNoiseBasis        
    {
        private const int XNoiseGen = 1619;
        private const int YNoiseGen = 31337;
        private const int ZNoiseGen = 6971;
        private const int SeedNoiseGen = 1013;
        private const int ShiftNoiseGen = 8;

        public int IntValueNoise(int x, int y, int z, int seed)
        {
            // All constants are primes and must remain prime in order for this noise
            // function to work correctly.
            int n = (
                XNoiseGen * x
              + YNoiseGen * y
              + ZNoiseGen * z
              + SeedNoiseGen * seed)
              & 0x7fffffff;
            n = (n >> 13) ^ n;
            return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
        }

        public double ValueNoise(int x, int y, int z)
        {
            return ValueNoise(x, y, z, 0);
        }

        public double ValueNoise(int x, int y, int z, int seed)
        {
            return 1.0 - ((double)IntValueNoise(x, y, z, seed) / 1073741824.0);
        }     
    }
   public class MySimplexFast
    {
        private static Grad[] grad3 = { new Grad(1,1,0), new Grad(-1, 1,0), new Grad(1,-1, 0), new Grad(-1,-1, 0),
                                        new Grad(1,0,1), new Grad(-1, 0,1), new Grad(1, 0,-1), new Grad(-1, 0,-1),
                                        new Grad(0,1,1), new Grad( 0,-1,1), new Grad(0, 1,-1), new Grad( 0,-1,-1) };

        private int m_seedSimplex;

        private byte[] m_permSimplex = new byte[512];
        private byte[] m_gradSimplex = new byte[512];

        public int Seed
        {
            get { return m_seedSimplex; }
            set
            {
                m_seedSimplex = value;

                var rnd = new MyRNG(m_seedSimplex);

                for (int i = 0; i < 256; i++)
                {
                    m_permSimplex[i] = (byte)rnd.NextIntRange(0f, 255f);
                    m_permSimplex[256 + i] = m_permSimplex[i];
                    m_gradSimplex[i] = (byte)(m_permSimplex[i] % 12);
                    m_gradSimplex[256 + i] = m_gradSimplex[i];
                }
            }
        }

        public double Frequency { get; set; }
	double[] weights = new double[17];
        public MySimplexFast(int seed = 1, double frequency = 1.0)
        {
            Seed      = seed;
            Frequency = frequency;
		double f = 1.0;
	    for(int i =0;i<17;i++){
		weights[i] = Math.Pow(f,-1.0);
		f*=frequency;
	}
        }
	public double GetValue(double x, double y, double z, int octaves=1, double multiplier = 25, double amplitude = 0.5f, double lacunarity = 2, double persistence = 0.9f) {
		double val = 0;
		Frequency = multiplier;
		double maxAmp=0;
		for (int n = 0; n < octaves; n++) {
	  	  double current = GetValue(x,y,z);
		  current*=amplitude;
		  val+=current;
		  Frequency *= lacunarity;
		  
		  maxAmp+=amplitude;
		amplitude *= persistence;

		}
		return val/maxAmp;
	}
	
        public double GetValue(double x)
        {
            x *= Frequency;

            int i0 = MathHelper.Floor(x);
            var x0 = x - i0;
            var x1 = x0 - 1.0;

            double n0, n1;

            var t0 = 1.0 - x0*x0;
            var t1 = 1.0 - x1*x1;

            t0 *= t0;
            t1 *= t1;

            n0 = t0*t0 * Dot(grad3[m_gradSimplex[ i0      & 0xFF]], x0);
            n1 = t1*t1 * Dot(grad3[m_gradSimplex[(i0 + 1) & 0xFF]], x1);

            // The maximum value of this noise is 8*(3/4)^4 = 2.53125
            // A factor of 0.395 scales to fit exactly within [-1,1]
            return 0.395*(n0 + n1);
        }

        public double GetValue(double x, double y)
        {
            const double SKEW   = 0.3660254037844386; // ( sqrt(3) - 1 ) / 2
            const double UNSKEW = 0.2113248654051871; // ( 3 - sqrt(3) ) / 6

            x *= Frequency;
            y *= Frequency;

            var s = (x + y)*SKEW; // Hairy factor for 2D
            int i = MathHelper.Floor(x + s);
            int j = MathHelper.Floor(y + s);

            var t  = (i + j)*UNSKEW;
            var x0 = x - i + t;
            var y0 = y - j + t;

            int i1, j1;

            if   (x0 > y0) { i1 = 1; j1 = 0; } // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            else           { i1 = 0; j1 = 1; } // upper triangle, YX order: (0,0)->(0,1)->(1,1)

            var x1 = x0 - i1  + UNSKEW;
            var y1 = y0 - j1  + UNSKEW;
            var x2 = x0 - 1.0 + UNSKEW + UNSKEW;
            var y2 = y0 - 1.0 + UNSKEW + UNSKEW;

            int ii = i & 0xFF;
            int jj = j & 0xFF;

            var t0 = 0.5 - x0*x0 - y0*y0;
            var t1 = 0.5 - x1*x1 - y1*y1;
            var t2 = 0.5 - x2*x2 - y2*y2;

            double n0, n1, n2;

            if (t0 < 0.0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0  = t0*t0 * Dot(grad3[m_gradSimplex[(ii + m_permSimplex[jj]) & 0xFF]], x0, y0);
            }

            if (t1 < 0.0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1  = t1*t1 * Dot(grad3[m_gradSimplex[(ii + i1 + m_permSimplex[(jj + j1) & 0xFF]) & 0xFF]], x1, y1);
            }

            if (t2 < 0.0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2  = t2*t2 * Dot(grad3[m_gradSimplex[(ii + 1 + m_permSimplex[(jj + 1) & 0xFF]) & 0xFF]], x2, y2);
            }

            return 70.0*(n0 + n1 + n2);
        }

        public double GetValue(double x, double y, double z)
        {
            // Skewing and unskewing factors
            const double SKEW   = 0.3333333333333333; // 1 / 3
            const double UNSKEW = 0.1666666666666667; // 1 / 6

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            // Skew the input space to determine which simplex cell we're in
            var s = (x + y + z)*SKEW; // Very nice and simple skew factor for 3D
            int i = MathHelper.Floor(x + s);
            int j = MathHelper.Floor(y + s);
            int k = MathHelper.Floor(z + s);

            var t = (i + j + k)*UNSKEW;

            // Unskew the cell origin back to (x,y,z) space
            var x0 = x - i + t; // The x,y,z distances from the cell origin
            var y0 = y - j + t;
            var z0 = z - k + t;

            // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
            // Determine which simplex we are in.
            int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
            int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

            if (x0 >= y0)
            {
                if      (y0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
                else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
                else               { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
            }
            else
            {
                if      (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
                else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
                else              { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
            }

            // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
            // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
            // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where c = 1/6.
            var x1 = x0 - i1  + UNSKEW; // Offsets for second corner in (x,y,z) coords
            var y1 = y0 - j1  + UNSKEW;
            var z1 = z0 - k1  + UNSKEW;
            var x2 = x0 - i2  + UNSKEW*2.0; // Offsets for third corner in (x,y,z) coords
            var y2 = y0 - j2  + UNSKEW*2.0;
            var z2 = z0 - k2  + UNSKEW*2.0;
            var x3 = x0 - 1.0 + UNSKEW*3.0; // Offsets for last corner in (x,y,z) coords
            var y3 = y0 - 1.0 + UNSKEW*3.0;
            var z3 = z0 - 1.0 + UNSKEW*3.0;

            // Work out the hashed gradient indices of the four simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;

            // Calculate the contribution from the four corners
            var t0 = 0.6 - x0*x0 - y0*y0 - z0*z0;
            var t1 = 0.6 - x1*x1 - y1*y1 - z1*z1;
            var t2 = 0.6 - x2*x2 - y2*y2 - z2*z2;
            var t3 = 0.6 - x3*x3 - y3*y3 - z3*z3;

            double n0, n1, n2, n3;

            if (t0 < 0.0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0  = t0*t0 * Dot(grad3[m_gradSimplex[ii + m_permSimplex[jj + m_permSimplex[kk]]]], x0, y0, z0);
            }

            if (t1 < 0.0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1  = t1*t1 * Dot(grad3[m_gradSimplex[ii + i1 + m_permSimplex[jj + j1 + m_permSimplex[kk + k1]]]], x1, y1, z1);
            }

            if (t2 < 0.0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2  = t2*t2 * Dot(grad3[m_gradSimplex[ii + i2 + m_permSimplex[jj + j2 + m_permSimplex[kk + k2]]]], x2, y2, z2);
            }

            if (t3 < 0.0) n3 = 0.0;
            else
            {
                t3 *= t3;
                n3  = t3*t3 * Dot(grad3[m_gradSimplex[ii + 1 + m_permSimplex[jj + 1 + m_permSimplex[kk + 1]]]], x3, y3, z3);
            }

            // Add contributions from each corner to get the final noise value.
            return 32.0*(n0 + n1 + n2 + n3);
        }

        // Inner class to speed up gradient computations (array access is slower than member access)
        private class Grad
        {
            public double x, y, z;

            public Grad(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        private double Dot(Grad g, double x)
        {
            return g.x*x;
        }

        private double Dot(Grad g, double x, double y)
        {
            return g.x*x + g.y*y;
        }

        private double Dot(Grad g, double x, double y, double z)
        {
            return g.x*x + g.y*y + g.z*z;
        }
    }
    public static class MathHelper
    {
        /// <summary>
        /// Represents the mathematical constant e.
        /// </summary>
        public const float E = 2.718282f;
        /// <summary>
        /// Represents the log base two of e.
        /// </summary>
        public const float Log2E = 1.442695f;
        /// <summary>
        /// Represents the log base ten of e.
        /// </summary>
        public const float Log10E = 0.4342945f;
        /// <summary>
        /// Represents the value of pi.
        /// </summary>
        public const float Pi = 3.141593f;
        /// <summary>
        /// Represents the value of pi times two.
        /// </summary>
        public const float TwoPi = 6.28318530718f;
        /// <summary>
        /// Represents the value of pi times two.
        /// </summary>
        public const float FourPi = 12.5663706144f;
        /// <summary>
        /// Represents the value of pi divided by two.
        /// </summary>
        public const float PiOver2 = 1.570796f;
        /// <summary>
        /// Represents the value of pi divided by four.
        /// </summary>
        public const float PiOver4 = 0.7853982f;
        /// <summary>
        /// Represents the value of the square root of two
        /// </summary>
        public const float Sqrt2 = 1.4142135623730951f;
        /// <summary>
        /// Represents the value of the square root of three
        /// </summary>
        public const float Sqrt3 = 1.7320508075688773f;
        /// <summary>
        /// 60 / 2*pi
        /// </summary>
        public const float RadiansPerSecondToRPM = 9.549296585513720f;
        /// <summary>
        /// 2*pi / 60
        /// </summary>
        public const float RPMToRadiansPerSecond = 0.104719755119660f;
        /// <summary>
        /// 2*pi / 60000
        /// </summary>
        public const float RPMToRadiansPerMillisec = 0.00010471975512f;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        public static float ToRadians(float degrees)
        {
            return (degrees / 360.0f) * TwoPi;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        public static float ToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        public static double ToDegrees(double radians)
        {
            return radians * 57.29578;
        }

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        /// <summary>
        /// Returns the lesser of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Min(float value1, float value2)
        {
            return Math.Min(value1, value2);
        }

        /// <summary>
        /// Returns the greater of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static float Max(float value1, float value2)
        {
            return Math.Max(value1, value2);
        }

        /// <summary>
        /// Returns the lesser of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static double Min(double value1, double value2)
        {
            return Math.Min(value1, value2);
        }

        /// <summary>
        /// Returns the greater of two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param>
        public static double Max(double value1, double value2)
        {
            return Math.Max(value1, value2);
        }

        /// <summary>
        /// Restricts a value to be within a specified range. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value. If value is less than min, min will be returned.</param><param name="max">The maximum value. If value is greater than max, max will be returned.</param>
        public static float Clamp(float value, float min, float max)
        {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Restricts a value to be within a specified range. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value. If value is less than min, min will be returned.</param><param name="max">The maximum value. If value is greater than max, max will be returned.</param>
        public static double Clamp(double value, double min, double max)
        {
            value = (double)value > (double)max ? max : value;
            value = (double)value < (double)min ? min : value;
            return value;
        }

        /// <summary>
        /// Restricts a value to be within a specified range. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value. If value is less than min, min will be returned.</param><param name="max">The maximum value. If value is greater than max, max will be returned.</param>


        /// <summary>
        /// Restricts a value to be within a specified range. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="value">The value to clamp.</param><param name="min">The minimum value. If value is less than min, min will be returned.</param><param name="max">The maximum value. If value is greater than max, max will be returned.</param>
        public static int Clamp(int value, int min, int max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Performs interpolation on logarithmic scale.
        /// </summary>
        public static float InterpLog(float value, float amount1, float amount2)
        {
            Debug.Assert(amount1 != 0f);
            Debug.Assert(amount2 != 0f);
            return (float)(Math.Pow((double)amount1, 1.0 - (double)value) * Math.Pow((double)amount2, (double)value));
        }

        public static float InterpLogInv(float value, float amount1, float amount2)
        {
            Debug.Assert(amount1 != 0f);
            Debug.Assert(amount2 != 0f);
            return (float)Math.Log(value / amount1, amount2 / amount1);
        }

        /// <summary>
        /// Returns the Cartesian coordinate for one axis of a point that is defined by a given triangle and two normalized barycentric (areal) coordinates.
        /// </summary>
        /// <param name="value1">The coordinate on one axis of vertex 1 of the defining triangle.</param><param name="value2">The coordinate on the same axis of vertex 2 of the defining triangle.</param><param name="value3">The coordinate on the same axis of vertex 3 of the defining triangle.</param><param name="amount1">The normalized barycentric (areal) coordinate b2, equal to the weighting factor for vertex 2, the coordinate of which is specified in value2.</param><param name="amount2">The normalized barycentric (areal) coordinate b3, equal to the weighting factor for vertex 3, the coordinate of which is specified in value3.</param>
        public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
        {
            return (float)((double)value1 + (double)amount1 * ((double)value2 - (double)value1) + (double)amount2 * ((double)value3 - (double)value1));
        }

        /// <summary>
        /// Interpolates between two values using a cubic equation.
        /// </summary>
        /// <param name="value1">Source value.</param><param name="value2">Source value.</param><param name="amount">Weighting value.</param>
        public static float SmoothStep(float value1, float value2, float amount)
        {
            Debug.Assert(amount >= 0f && amount <= 1f, "Wrong amount value for SmoothStep");
            return MathHelper.Lerp(value1, value2, SCurve3(amount));
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param><param name="value2">The second position in the interpolation.</param><param name="value3">The third position in the interpolation.</param><param name="value4">The fourth position in the interpolation.</param><param name="amount">Weighting factor.</param>
        public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
        {
            float num1 = amount * amount;
            float num2 = amount * num1;
            return (float)(0.5 * (2.0 * (double)value2 + (-(double)value1 + (double)value3) * (double)amount + (2.0 * (double)value1 - 5.0 * (double)value2 + 4.0 * (double)value3 - (double)value4) * (double)num1 + (-(double)value1 + 3.0 * (double)value2 - 3.0 * (double)value3 + (double)value4) * (double)num2));
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">Source position.</param><param name="tangent1">Source tangent.</param><param name="value2">Source position.</param><param name="tangent2">Source tangent.</param><param name="amount">Weighting factor.</param>
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            float num1 = amount;
            float num2 = num1 * num1;
            float num3 = num1 * num2;
            float num4 = (float)(2.0 * (double)num3 - 3.0 * (double)num2 + 1.0);
            float num5 = (float)(-2.0 * (double)num3 + 3.0 * (double)num2);
            float num6 = num3 - 2f * num2 + num1;
            float num7 = num3 - num2;
            return (float)((double)value1 * (double)num4 + (double)value2 * (double)num5 + (double)tangent1 * (double)num6 + (double)tangent2 * (double)num7);
        }

        /// <summary>
        /// Reduces a given angle to a value between π and -π.
        /// </summary>
        /// <param name="angle">The angle to reduce, in radians.</param>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder((double)angle, 6.28318548202515);
            if ((double)angle <= -3.14159274101257)
                angle += 6.283185f;
            else if ((double)angle > 3.14159274101257)
                angle -= 6.283185f;
            return angle;
        }

        public static int GetNearestBiggerPowerOfTwo(int v)
        {
            --v;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            ++v;
            return v;
        }

        public static uint GetNearestBiggerPowerOfTwo(uint v)
        {
            --v;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            ++v;
            return v;
        }

        /// <summary>
        /// Returns nearest bigger power of two
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int GetNearestBiggerPowerOfTwo(float f)
        {
            int x = 1;
            while (x < f)
            {
                x <<= 1;
            }

            return x;
        }

        public static int GetNearestBiggerPowerOfTwo(double f)
        {
            int x = 1;
            while (x < f)
            {
                x <<= 1;
            }

            return x;
        }

        public static float Max(float a, float b, float c)
        {
            float abMax = a > b ? a : b;

            return abMax > c ? abMax : c;
        }

        public static int Max(int a, int b, int c)
        {
            int abMax = a > b ? a : b;
            return abMax > c ? abMax : c;
        }

        public static float Min(float a, float b, float c)
        {
            float abMin = a < b ? a : b;

            return abMin < c ? abMin : c;
        }

        public static double Max(double a, double b, double c)
        {
            double abMax = a > b ? a : b;

            return abMax > c ? abMax : c;
        }

        public static double Min(double a, double b, double c)
        {
            double abMin = a < b ? a : b;

            return abMin < c ? abMin : c;
        }

        public static int ComputeHashFromBytes(byte[] bytes)
        {
            int size = bytes.Length;
            size -= (size % 4); // Ignore bytes past the aligned section.
            GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            int hash = 0;
            unsafe
            {
                try
                {
                    int* numPtr = (int*)gcHandle.AddrOfPinnedObject().ToPointer();
                    for (int i = 0; i < size; i += 4, ++numPtr)
                        hash ^= (*numPtr);
                    return hash;
                }
                finally
                {
                    gcHandle.Free();
                }
            }
        }

        public static float RoundOn2(float x)
        {
            return ((int)(x * 100)) / 100.0f; // Oriznuti staci :)
        }
        /// <summary>
        /// Returns true if value is power of two
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int x)
        {
            return ((x > 0) && ((x & (x - 1)) == 0));
        }

        public static float  SCurve3(float  t)
        {
            return t*t*(3f - 2f*t);
        }
        public static double SCurve3(double t)
        {
            return t*t*(3 - 2*t);
        }
        public static float  SCurve5(float  t)
        {
            return t*t*t*(t*(t*6f - 15f) + 10f);
        }
        public static double SCurve5(double t)
        {
            return t*t*t*(t*(t*6 - 15) + 10);
        }

        public static float  Saturate(float  n)
        {
            return (n < 0f) ? 0f : (n > 1f) ? 1f : n;
        }
        public static double Saturate(double n)
        {
            return (n < 0.0) ? 0.0 : (n > 1.0) ? 1.0 : n;
        }

        public static int Floor(float  n)
        {
            return n < 0f ? (int)n - 1 : (int)n;
        }
        public static int Floor(double n)
        {
            return n < 0.0 ? (int)n - 1 : (int)n;
        }

        private static readonly int[] lof2floor_lut = new int[]
        {
             0,  9,  1, 10, 13, 21,  2, 29,
            11, 14, 16, 18, 22, 25,  3, 30,
             8, 12, 20, 28, 15, 17, 24,  7,
            19, 27, 23,  6, 26,  5,  4, 31
        };

        /**
         * Fast integer Floor(Log2(value)).
         * 
         * Uses a DeBruijn-like method to find quickly the MSB.
         * 
         * Algorithm:
         * https://en.wikipedia.org/wiki/De_Bruijn_sequence#Uses
         * 
         * This implementation:
         * http://stackoverflow.com/a/11398748
         */
        public static int Log2Floor(int value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return lof2floor_lut[(uint)(value * 0x07C4ACDD) >> 27];
        }

        /**
         * Based on the above and this discussion:
         * http://stackoverflow.com/questions/3272424/compute-fast-log-base-2-ceiling
         * 
         */
        public static int Log2Ceiling(int value)
        {
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value = lof2floor_lut[(uint)(value * 0x07C4ACDD) >> 27];
            return (value & (value - 1)) != 0 ? value + 1 : value;
        }

        public static int Log2(int n)
        {
            int r = 0;

            while ((n >>= 1) > 0)
                ++r;

            return r;
        }

        public static int Log2(uint n)
        {
            int r = 0;

            while ((n >>= 1) > 0)
                ++r;

            return r;
        }

        /// <summary>
        /// Returns 2^n
        /// </summary>
        public static int Pow2(int n)
        {
            return 1 << n;
        }

        public static double CubicInterp(double p0, double p1, double p2, double p3, double t)
        {
            double P  = (p3 - p2) - (p0 - p1);
            double Q  = (p0 - p1) - P;
            double t2 = t*t;

            return P*t2*t + Q*t2 + (p2 - p0)*t + p1;
        }

        /// <summary>
        /// Returns angle in range 0..2*PI
        /// </summary>
        /// <param name="angle">in radians</param>
        public static void LimitRadians2PI(ref double angle)
        {
            if (angle > TwoPi)
            {            
                angle = angle % TwoPi;
            }
            else if (angle < 0)
            {
                angle = angle % TwoPi + TwoPi;
            }
        }

        /// <summary>
        /// Returns angle in range 0..2*PI
        /// </summary>
        /// <param name="angle">in radians</param>
        public static void LimitRadians(ref float angle)
        {
            if (angle > TwoPi)
            {
                angle = angle % TwoPi;
            }
            else if (angle < 0)
            {
                angle = angle % TwoPi + TwoPi;
            }
        }

        /// <summary>
        /// Returns angle in range -PI..PI
        /// </summary>
        /// <param name="angle">radians</param>
        public static void LimitRadiansPI(ref double angle)
        {
            if (angle > Pi)
            {
                angle = angle % Pi - Pi;
            }
            else if (angle < -Pi)
            {
                angle = angle % Pi + Pi;
            }
        }

        /// <summary>
        /// Returns angle in range -PI..PI
        /// </summary>
        /// <param name="angle">radians</param>
        public static void LimitRadiansPI(ref float angle)
        {
            if (angle > Pi)
            {
                angle = angle % Pi - Pi;
            }
            else if (angle < Pi)
            {
                angle = angle % Pi + Pi;
            }
        }



        public static float MonotonicCosine(float radians)
        {
            if (radians > 0)
                return 2 - (float)Math.Cos(radians);
            else
                return (float)Math.Cos(radians);
        }

        public static float MonotonicAcos(float cos)
        {
            if (cos > 1)
                return (float)Math.Acos(2 - cos);
            else
                return (float)-Math.Acos(cos);
        }
    }
    public struct MyRNG
    {
        const uint MAX_MASK = 0x7FFFFFFF;
        const float MAX_MASK_FLOAT = MAX_MASK;
        // set seed with a 31 bit integer <1, 0X7FFFFFFF>
        public uint Seed;

        public MyRNG(int seed = 1)
        {
            Seed = (uint)seed;
        }

        // provides the next pseudorandom number as an integer (31 bits)
        public uint NextInt()
        {
            return Gen();
        }

        // provides the next pseudorandom number as a float between nearly 0 and nearly 1.0.
        public float NextFloat()
        {
            return Gen() / MAX_MASK_FLOAT;
        }

        // provides the next pseudorandom number as an integer (31 bits) betweeen a given range.
        public int NextIntRange(float min, float max)
        {
            int result = (int)((min + (max - min)*NextFloat()) + 0.5f);
            Debug.Assert(min <= result && result <= max);
            return result;
        }
        
        // provides the next pseudorandom number as a float between a given range.
        public float NextFloatRange(float min, float max)
        {
            return min + ((max - min)*NextFloat());
        }
        
        // generator: new = (old * 16807) mod (2^31 - 1)
        private uint Gen()
        {
            return Seed = (Seed * 16807) & MAX_MASK;
        }
    }
}
