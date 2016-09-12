using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNoise.Modifiers;
using LibNoise;
using System.Drawing;
namespace ProceduralHeightMapGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
			Bitmap earthLookupBitmap = new Bitmap("EarthLookupTable.png");
            Color[] earthLookupTable = new Color[earthLookupBitmap.Width];
            for (int i = 0; i < earthLookupBitmap.Width; i++)
                earthLookupTable[i] = earthLookupBitmap.GetPixel(i, 2);
			int resolution = 1024;
			Bitmap output = new Bitmap(resolution,resolution);
			IModule Module = GetEarthModule(0,resolution);
			for(int x = 0;x<resolution-1;x++){
				for(int y = 0;y<resolution-1;y++){
					double val = Module.GetValue((double)x-5000,(double)y,0);
					if(val>1) val = 1;
					if(val<0) val = 0;
					//val = ((val/2+.5));
					int index = (int)(val*(earthLookupTable.Length-1));
					var color = earthLookupTable[index];
					output.SetPixel(x,y,color);//Color.FromArgb((int)val,(int)val,(int)val));
				}
			}
			output.Save("Out.png");
			Console.WriteLine("Done");
			Console.ReadLine();
        }
        public static IModule GetEarthModule(int seed,int resolution)
        {
			if(true){
				
			double factor = 10.0/resolution;
			FastNoise Continentsprescale = new FastNoise(seed);
			Continentsprescale.Frequency = 1*factor;
			ScaleBiasOutput Continents = new ScaleBiasOutput(Continentsprescale);
			Continents.Scale = .65;
			Continents.Bias = .35;
			
			FastBillow LowLands = new FastBillow(seed);
			LowLands.Frequency = .4*factor;
			ScaleBiasOutput LowLandsScaled = new ScaleBiasOutput(LowLands);
			LowLandsScaled.Scale = 0.1;
			LowLandsScaled.Bias=0.5;
			
			
			FastRidgedMultifractal Mountains = new FastRidgedMultifractal(seed);
			Mountains.Frequency = 1*factor;
			Mountains.Lacunarity = 2;
			Mountains.OctaveCount = 16;
			
			ScaleBiasOutput MountainsScaledpreturb = new ScaleBiasOutput(Mountains);
			MountainsScaledpreturb.Scale = 0.4;
			MountainsScaledpreturb.Bias = 0.85;
			//FastTurbulence MountainsFinal = new FastTurbulence(MountainsScaled)
			FastTurbulence MountainsScaled= new FastTurbulence(MountainsScaledpreturb);
			MountainsScaled.Frequency=50*factor;
			MountainsScaled.Power=0.1;
			FastNoise LandFilterprescale = new FastNoise(seed+1);
			LandFilterprescale.Frequency=.6*factor;
			ScaleBiasOutput LandFilter = new ScaleBiasOutput(LandFilterprescale);
			LandFilter.Bias = 0;
			
			Select LandFinal1 = new Select(LandFilter,LowLandsScaled,MountainsScaled);
			LandFinal1.SetBounds(0,100);
			LandFinal1.EdgeFalloff=(.5);
			//AbsoluteOutput LandFinal = new AbsoluteOutput(LandFinal1);
			var LandFinal = new ScaleBiasOutput(LandFinal1);
			
			FastBillow Ocean = new FastBillow(seed);
			Ocean.Frequency=.5*factor;
			ScaleBiasOutput OceanScaled = new ScaleBiasOutput(Ocean);
			OceanScaled.Scale=0.1;
			OceanScaled.Bias = 0.1;
			
			Select Final = new Select(Continents,new Constant(0),new Constant(1));
			Final.SetBounds(0,100);
			//Final.EdgeFalloff=(0.5);
			Select Finalx = new Select(Continents,LandFinal,OceanScaled);
			Finalx.SetBounds(0,100);
			//Finalx.EdgeFalloff=(0.5);
			Select Final2 = new Select(LandFinal1,new Constant(.5),new Constant(.3));
			Final2.SetBounds(0,100);
			//Final2.EdgeFalloff = .5;
			Select Final3 = new Select(Finalx,Final,Final2);
						Final3.SetBounds(0,100);
						var outpoot = new AbsoluteOutput(new VoronoiRelaxed(){Frequency=4*factor,RelaxationFactor=.2});
			var zones = /*new VoronoiLarge(new VoronoiLarge(*/new VoronoiLarge(outpoot){Frequency=factor*20,RelaxationFactor=.75};//){Frequency=factor*8,RelaxationFactor=0}){Frequency=factor*16,RelaxationFactor=0};
			var fin = new Select(Continentsprescale,new Constant(0),zones);
			fin.SetBounds(0,100);		//Final3.EdgeFalloff = .5;
			return fin;
			}
			else{
							double factor = 1.0/resolution;
				     FastNoise fastPlanetContinents = new FastNoise(seed);
                    fastPlanetContinents.Frequency = 1.5*factor;                    

                    FastBillow fastPlanetLowlands = new FastBillow();
                    fastPlanetLowlands.Frequency = 4*factor;
                    LibNoise.Modifiers.ScaleBiasOutput fastPlanetLowlandsScaled = new ScaleBiasOutput(fastPlanetLowlands);
                    fastPlanetLowlandsScaled.Scale = 0.2;
                    fastPlanetLowlandsScaled.Bias = 0.5;

                    FastRidgedMultifractal fastPlanetMountainsBase = new FastRidgedMultifractal(seed);
                    fastPlanetMountainsBase.Frequency = 4*factor;

                    ScaleBiasOutput fastPlanetMountainsScaled = new ScaleBiasOutput(fastPlanetMountainsBase);
                    fastPlanetMountainsScaled.Scale = 0.4;
                    fastPlanetMountainsScaled.Bias = 0.85;

                    FastTurbulence fastPlanetMountains = new FastTurbulence(fastPlanetMountainsScaled);
                    fastPlanetMountains.Power = 0.1;
                    fastPlanetMountains.Frequency = 50*factor;

                    FastNoise fastPlanetLandFilter = new FastNoise(seed+1);
                    fastPlanetLandFilter.Frequency = 6*factor;

                    Select fastPlanetLand = new Select(fastPlanetLandFilter, fastPlanetLowlandsScaled, fastPlanetMountains);
                    fastPlanetLand.SetBounds(0, 1000);
                    fastPlanetLand.EdgeFalloff = 0.5;

                    FastBillow fastPlanetOceanBase = new FastBillow(seed);
                    fastPlanetOceanBase.Frequency = 15*factor;
                    ScaleOutput fastPlanetOcean = new ScaleOutput(fastPlanetOceanBase, 0.1);
                    
                    Select fastPlanetFinal = new Select(fastPlanetContinents, fastPlanetOcean, fastPlanetLand);
                    fastPlanetFinal.SetBounds(0, 1000);
                    fastPlanetFinal.EdgeFalloff = 0.5;
					return fastPlanetFinal;
			}
        }
    }
	
}
// 
// Copyright (c) 2013 Jason Bell
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 



namespace LibNoise
{
	public class ValueNoise:ValueNoiseBasis,IModule{
		public double Frequency{get;set;}
		public ValueNoise(){
			Frequency=1;
		}
		public double GetValue(double x,double y,double z){
			x*=Frequency;
			y*=Frequency;
			z*=Frequency;
			return (double)ValueNoise((int)x,(int)y,(int)z);
		}
	}
    public class VoronoiLarge
        : ValueNoiseBasis,IModule
    {
        public double Frequency { get; set; }
        public double Displacement { get; set; }
        public bool DistanceEnabled { get; set; }
        public int Seed { get; set; }
		public IModule source;
		public double RelaxationFactor{get;set;}
        public VoronoiLarge(IModule Source)
        {
            Frequency = 1.0;
            Displacement = 1.0;
            Seed = 0;
            DistanceEnabled = false;
			source = Source;
			RelaxationFactor=1;
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
                        double xPos = xCur + ValueNoise(xCur, yCur, zCur, Seed)*RelaxationFactor;
                        double yPos = yCur + ValueNoise(xCur, yCur, zCur, Seed + 1)*RelaxationFactor;
                        double zPos = zCur + ValueNoise(xCur, yCur, zCur, Seed + 2)*RelaxationFactor;
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            double value;
            if (DistanceEnabled)
            {
                // Determine the distance to the nearest seed point.
                double xDist = xCandidate - x;
                double yDist = yCandidate - y;
                double zDist = zCandidate - z;
                value = (System.Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                  ) * Math.Sqrt3 - 1.0;
            }
            else
            {
                value = 0.0;
            }

            int x0 = (xCandidate > 0.0 ? (int)(xCandidate/Frequency) : (int)((xCandidate - 1)/Frequency));
            int y0 = (yCandidate > 0.0 ? (int)(yCandidate/Frequency) : (int)((yCandidate - 1)/Frequency));
            int z0 = (zCandidate > 0.0 ? (int)(zCandidate/Frequency) : (int)((zCandidate - 1)/Frequency));

            // Return the calculated distance with the displacement value applied.
            return value + (Displacement * source.GetValue(x0, y0, z0));
        }
    }
	// 
// Copyright (c) 2013 Jason Bell
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 


    public class VoronoiRelaxed
        : ValueNoiseBasis, IModule
    {
        public double Frequency { get; set; }
        public double Displacement { get; set; }
        public bool DistanceEnabled { get; set; }
        public int Seed { get; set; }
		public double RelaxationFactor{get;set;}
        public VoronoiRelaxed()
        {
            Frequency = 1.0;
            Displacement = 1.0;
            Seed = 0;
            DistanceEnabled = false;
			RelaxationFactor=1;
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
                        double xPos = xCur + ValueNoise(xCur, yCur, zCur, Seed)*RelaxationFactor;
                        double yPos = yCur + ValueNoise(xCur, yCur, zCur, Seed + 1)*RelaxationFactor;
                        double zPos = zCur + ValueNoise(xCur, yCur, zCur, Seed + 2)*RelaxationFactor;
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            double value;
            if (DistanceEnabled)
            {
                // Determine the distance to the nearest seed point.
                double xDist = xCandidate - x;
                double yDist = yCandidate - y;
                double zDist = zCandidate - z;
                value = (System.Math.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                  ) * Math.Sqrt3 - 1.0;
            }
            else
            {
                value = 0.0;
            }

            int x0 = (xCandidate > 0.0 ? (int)xCandidate : (int)xCandidate - 1);
            int y0 = (yCandidate > 0.0 ? (int)yCandidate : (int)yCandidate - 1);
            int z0 = (zCandidate > 0.0 ? (int)zCandidate : (int)zCandidate - 1);


            // Return the calculated distance with the displacement value applied.
            return value + (Displacement * (double)ValueNoise(x0, y0, z0));
        }
    }


}
