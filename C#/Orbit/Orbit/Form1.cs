using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;
using VRageMath;
namespace Orbit
{
    public partial class Form1 : Form{
        Vector3D poos;
        Vector3D poos1;
        Vector3D vol;
        Vector3D vol1;
        Planet planet;
        Orbit orbit;
        Orbit orbit2;
        double poo2 = 0;
        double poo = 0;
        bool first = true;
        DateTime epoc;
        Maneuver man;
        Maneuver man2;
        public Form1()
        {

            planet = new Planet(0, 9.81, 67000, new VRageMath.Vector3D(0, 0, 0));

            orbit2 = new Orbit(new VRageMath.Vector3D(100000, 0, 0), new VRageMath.Vector3D(0, planet.GetOrbitalVelocity(100000, (100000 + 100000) / 2), 0), planet);
            orbit = new Orbit(new VRageMath.Vector3D(0, 140000, 0), new VRageMath.Vector3D(-planet.GetOrbitalVelocity(140000, (100000 + 140000) / 2), 0, 0), planet);
            //Console.WriteLine(planet.GetOrbitalVelocity(2000000,1035000));
            epoc = orbit.epoch;
            Console.WriteLine(planet.GetOrbitalVelocity(100000, (100000 + 100000) / 2));
            man = (Maneuver.GetHohmannTransfer (orbit, orbit2, orbit.TrueAnomaly2(0)));

            InitializeComponent();
        }
        public Chart getChart()
        {
            return chart1;
        }

        private void chart1_Click(object sender, EventArgs e)

        {
           // Console.WriteLine(orbit2.FindInterceptPoint(orbit));
            Chart chart = sender as Chart;
            // = form.getChart();
            chart.Series.Clear(); //ensure that the chart is empty
            chart.Series.Add("Series0");
            chart.Series.Add("Series1");
            chart.Series.Add("Series2");
            chart.Series.Add("Series3");
            chart.Series.Add("Series4");
            chart.Series.Add("Series5");
            chart.Series[0].ChartType = SeriesChartType.Line;
            chart.Series[1].ChartType = SeriesChartType.Line;
            chart.Series[2].ChartType = SeriesChartType.Line;
            chart.Series[3].ChartType = SeriesChartType.Line;
            chart.Series[4].ChartType = SeriesChartType.Line;
            chart.Series[5].ChartType = SeriesChartType.Line;
            chart.Legends.Clear();

            //Trajectory traj = new Trajectory(planet, new VRageMath.Vector3D(0, 70000, 0), new VRageMath.Vector3D(-planet.GetOrbitalVelocity(70000, (70000 + 2000000) / 2), 0, 0));

            Console.WriteLine("Orbit 1: "+orbit.ToString1(poo));
            Console.WriteLine("Orbit 2: " + orbit2.ToString1(poo));

            //Maneuver man2 = Maneuver.GetHohmannTransfer(orbit2, orbit, orbit2.TrueAnomaly2(poo));
            //orbit.KepToCart(out poos, out vol,poo);
            //orbit2.KepToCart(out poos1, out vol1, poo);
           // poos.Normalize();
            //Console.WriteLine(orbit.TrueAnomalyAtPosition(poos)+" "+orbit.TrueAnomaly2(poo));
            //Console.WriteLine("Orbit 1"+poos+" "+vol);
            //Console.WriteLine("Ascending Node: "+orbit.AscendingNodeTrueAnomaly(orbit2));
            DateTime temp = epoc;
            
            orbit.KepToCartAtTrueAnomaly(out poos1, out vol1, man.TrueAnomaly);
            if (first)
            {
                poo = 0;
                poo2 = 0;
                orbit2.KepToCart(out poos, out vol, orbit.TimeOfTrueAnomaly(man.TrueAnomaly));
                orbit2.SetEpoch(temp.AddSeconds(orbit.TimeOfTrueAnomaly(man.TrueAnomaly)));
                first = false;
            }
            else
            {
                orbit2.KepToCart(out poos, out vol, 0);
            }
            int r = 0;
            double er = orbit.a > orbit2.a ? orbit.a*2 : orbit2.a*2;
            chart.Series[2].Points.AddXY(-er, -er);
            chart.Series[2].Points.AddXY(er, er);
            chart.Series[1].Points.AddXY(0, 0);
            chart.Series[2].Points.AddXY(poos.GetDim(0),poos.GetDim(1) );
            chart.Series[2].Points.AddXY(poos1.GetDim(0), poos1.GetDim(1));
            Orbit orbit1 = new Orbit(poos1, vol1 + (man.velocity * man.direction), planet);
            Console.WriteLine("Maneuver Details: " + "Direction: "+man.direction+" DeltaV: "+man.velocity+" True Anomaly: "+man.TrueAnomaly);
            Console.WriteLine("Real True Anomaly: " + orbit.TrueAnomaly2(poo) + " Current Time: " + poo + " Calculated Time: " + orbit.TimeOfTrueAnomaly(orbit.TrueAnomaly2(poo))+" Period: "+orbit.Period);
            
           

            

            
            Console.WriteLine("Closest Approach: " + orbit1.NextClosestApproachDistance(orbit2,poo));

            orbit1.KepToCart(out poos1, out vol1, poo);

            orbit2.KepToCart(out poos, out vol, poo);

           // Console.WriteLine(Planet.GetPlanetCenter(new Vector3D(-120000, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 120000, 0), new Vector3D(0, -1, 0)));
            //Console.WriteLine(planet.Mass+" "+Planet.GetPlanetMass(new Vector3D(0,0,0),new Vector3D(67000,0,0),9.81));
 
            for (int x = 0; x < 360; x++)
            {
                double i=x * (Math.PI / (double)180);
                chart.Series[1].Points.AddXY(Math.Sin(i)*60000, Math.Cos(i)*60000);
            }
            for (int x = 0; x < 360; x++)
            {
                double i = x * (Math.PI / (double)180);
                chart.Series[4].Points.AddXY(Math.Sin(i) * 5000 + poos.GetDim(0), Math.Cos(i) * 5000 + poos.GetDim(1));
            }
            for (int x = 0; x < 360; x++)
            {
                double i = x * (Math.PI / (double)180);
                chart.Series[5].Points.AddXY(Math.Sin(i) * 5000 + poos1.GetDim(0), Math.Cos(i) * 5000 + poos1.GetDim(1));
            }
                while (true)
                {
                    //traj.Step();

                   // orbit2.KepToCart(out poos1, out vol1, r * 10);
                    orbit1.KepToCart(out poos, out vol, r * 10);

                   // Console.WriteLine(poos + " " + vol);
                    //Console.WriteLine(poos+" "+orbit.ToString1(r*10));
                    chart.Series[0].Points.AddXY(poos.GetDim(0),poos.GetDim(1));
                    //chart.Series[2].Points.AddXY(poos1.GetDim(0), poos1.GetDim(1));
                   // chart.Series[2].Points.AddXY(traj.pos.GetDim(0), traj.pos.GetDim(1));
                    r++;
                    if (r > 5000)
                    {
                        break;
                    }
                }
                r = 0;
                while (true)
                {
                    //traj.Step();

                    orbit2.KepToCart(out poos1, out vol1, r * 10);
                   // orbit.KepToCart(out poos, out vol, r * 10);
                    // Console.WriteLine(poos);
                    //chart.Series[0].Points.AddXY(poos.GetDim(0), poos.GetDim(1));
                    chart.Series[3].Points.AddXY(poos1.GetDim(0), poos1.GetDim(1));
                    // chart.Series[2].Points.AddXY(traj.pos.GetDim(0), traj.pos.GetDim(1));
                    r++;
                    if (r > 10000)
                    {
                        break;
                    }
                }
                poo += 100;
                poo2 = Math.Floor(poo/orbit1.Period);
        }
    }
}
