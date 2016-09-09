using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Engine;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game;
using VRageMath;

namespace Space_Engineers_Script
{
    public class Program : MyGridProgram
    {
        public IMyGridTerminalSystem GridTerminalSystem;
        #region Programming Block code
        bool comfound = true;
        public SingleAxisThrustShip ship;
        IMyTerminalBlock timer;
        Vector3D lastPos;
        Vector3D lastG;
        Vector3D center;
        IMyTerminalBlock remote;
        Vector3D targvel;
        List<Vector3D> ds = new List<Vector3D>();
        List<Vector3D> xs = new List<Vector3D>();
        Vector3D beginningvel;
        bool isfirst2 = false;
        bool test = false;
        bool isfirst = true;
        IMyTextPanel panel;
        void Main(string argument)
        {
            switch (argument)
            {
                case "Dampen":
                    test = true;
                    break;
                default:
                    if (isfirst2)
                    {
                        //beginningvel = ship.Velocity;    
                        isfirst2 = false;
                    }
                    if (isfirst)
                    {
                        //Vector3D targ = new Vector3D(0, -60000, 0);

                        remote = GridTerminalSystem.GetBlockWithName("Remote");
                        timer = GridTerminalSystem.GetBlockWithName("Timer");

                        ship = new SingleAxisThrustShip(remote, GridTerminalSystem, this);
                        ship.Target = targ;
                        isfirst = false;
                        isfirst2 = true;
                        panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCDebug");
                        

                    }





                    //Echo(foundCenter.ToString());   


                   // double dist = Ship.SphericalDist(remote.GetPosition(), ship.Target, foundCenter, remote.GetPosition().Length());
                    // double longitude = ship.GetLongitude(remote.GetPosition()) * 57.2958;    
                    // double latitude = ship.GetLatitude(remote.GetPosition()) * 57.2958;    
                    // double heading = ship.GetCurrentHeading() * 57.2958;    
                    //Vector3D kek = ship.Velocity;    
                    //ship.WriteToScreen(ship.GetDisplayString() + "\nDistance From Center: " + remote.GetPosition().Length(), "LCDebug", false);
                    //Vector3D ForwardVel = Projection(kek, remote.WorldMatrix.Forward);    
                    // Echo(ForwardVel.Length().ToString());    
                    Matrix orient;
                    remote.Orientation.GetMatrix(out orient);
                    // ship.set_oriented_gyros(new Vector3D(0, 0, 0), orient.Down);       
                    //Echo(ship.GetMassTemp().ToString());                
                    MatrixD InvWorld = MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.
                    Forward, remote.WorldMatrix.Forward));
                    // if (dist > 800)
                    // if (ship.SetForwardVelocity_Planet(100)) test = true;
               
                    //if(ship.ThrustTowardsVector(-Vector3D.Normalize(remote.GetPosition()))) timer.GetActionWithName("OnOff_Off").Apply(timer); ;                
      
            }
            //ship.set_oriented_gyros_planet(targ, new Vector3D(0, 0, 0), orient.Right);        
            if ((timer as IMyFunctionalBlock).Enabled)
            {
                timer.GetActionWithName("TriggerNow").Apply(timer);
            }

        }
        public Vector3D Projection(Vector3D thi, Vector3D that)
        {
            // (scalar/scalar)*(vector) = (vector)    
            return (Vector3D.Dot(that, thi) / Vector3D.Dot(that, that)) * that;
        }
        public class Ship
        {
            // Lel I blatantly stole this regex ha created by "me 10 Jin"
            System.Text.RegularExpressions.Regex reactorRegex = new System.Text.RegularExpressions.Regex(
                "Max Output: (\\d+\\.?\\d*) (\\w?)W.*Current Output: (\\d+\\.?\\d*) (\\w?)W",
                System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Regex batteryRegex = new System.Text.RegularExpressions.Regex(
"Current Input: (\\d+\\.?\\d*) (\\w?)W.*Current Output: (\\d+\\.?\\d*) (\\w?)W.*Stored power: (\\d+\\.?\\d*) (\\w?)Wh"
, System.Text.RegularExpressions.RegexOptions.Singleline);
            System.Text.RegularExpressions.Regex batteryRegex2 = new System.Text.RegularExpressions.Regex(
"Max Output: (\\d+\\.?\\d*) (\\w?)W"
, System.Text.RegularExpressions.RegexOptions.Singleline);
            String Modifiers = ".kMGTPEZY";
            public bool first = true;
            public MyGridProgram Me;
            public List<double> AccelGuesses = new List<double>();
            public List<double> MassGuesses = new List<double>();
            public IMyGridTerminalSystem GridTerminalSystem;
            public List<IMyTerminalBlock> upThrusts = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> downThrusts = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> forThrusts = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> backThrusts = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> leftThrusts = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> rightThrusts = new List<IMyTerminalBlock>();
            public Vector3D lastAcc;
            public Vector3D planetCenter;
            public double upThrust;
            public double leftThrust;
            public double downThrust;
            public double rightThrust;
            public double forThrust;
            public double backThrust;
            public double upAccel;
            public double leftAccel;
            public double downAccel;
            public double rightAccel;
            public double forAccel;
            public double backAccel;
            public double Mass;
            public double WattsReactors;
            public double kgOfUranium;
            public Vector3D lastPos;
            public DateTime lastVelTime = DateTime.Now;
            public Vector3D lastVel = new Vector3D(0, 0, 0);
            public DateTime lastAccelTime = DateTime.Now;
            public const double largeAtmoThrust = 5400000;
            public const double smallAtmoThrust = 420000;
            public const double largeHydroThrust = 6000000;
            public const double smallHydroThrust = 900000;
            public const double largeIonThrust = 3600000;
            public const double smallIonThrust = 288000;
            public const double smalllargeAtmoThrust = 408000;
            public const double smallsmallAtmoThrust = 80000;
            public const double smalllargeHydroThrust = 400000;
            public const double smallsmallHydroThrust = 82000;
            public const double smalllargeIonThrust = 144000;
            public const double smallsmallIonThrust = 12000;
            Vector3D startingVelocity;
            public Vector3D currentVelocity;
            bool firstCoM = true;
            bool foundCoM = false;
            bool topKek = false;
            Vector3D targDir;
            VRage.ModAPI.IMyEntity target;
            public Vector3D lastTargPos;
            public Vector3D Target;
            List<Vector3D> samples = new List<Vector3D>();
            int sampleCount;

            public Vector3D localCoM = default(Vector3D);
            public bool TestForInitialCoM(String Name = "LCDebug")
            {

                if (foundCoM) return true;
                Matrix orient;
                remote.Orientation.GetMatrix(out orient);
                Quaternion offset = Quaternion.CreateFromAxisAngle((Vector3)remote.WorldMatrix.Up, 0.174533f);
                if (firstCoM == true)
                {
                    targDir = Vector3D.Transform(remote.WorldMatrix.Forward, offset);
                    samples.Add(remote.GetPosition());
                    sampleCount = 1;
                    firstCoM = false;
                }
                bool found = set_oriented_gyros(targDir + remote.GetPosition(), (Vector3D)orient.Forward);
                if (found)
                {
                    targDir = Vector3D.Transform(remote.WorldMatrix.Forward, offset);
                    samples.Add(remote.GetPosition());
                    sampleCount++;
                    if (sampleCount > 2)
                    {
                        bool pp;
                        double radius;
                        MatrixD test1 = MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.Forward, remote.WorldMatrix.Up));
                        Vector3D CoMPos = CircleBy3Points(samples[0], samples[1], samples[2], out pp, out radius);
                        Vector3D test = CoMPos - remote.GetPosition();
                        localCoM = Vector3D.Transform(test, test1);
                        StoreDataInScreen(Name);
                        return true;
                    }
                }
                return false;
            }
            public Vector3D targVelocity
            {
                get
                {
                    Vector3D pos = target.GetPosition();
                    Vector3D vel = (lastTargPos - pos)*60;
                    return vel;
                }
                set;
            }
            public bool MatchVelocityWithTarget()
            {
                Vector3D targVel = targVelocity - Velocity;

            }
            public void StoreDataInScreen(String Name)
            {
                IMyTextPanel panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(Name);
                panel.WritePublicTitle(localCoM + "," + planetCenter);
            }
            public void GetDataFromScreen(String Name)
            {
                IMyTextPanel panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(Name);
                String data = panel.GetPublicTitle();
                String[] datas = data.Split(",".ToCharArray());
                if (Vector3D.TryParse(datas[0], out localCoM)) localCoM = new Vector3D(0, 0, 0);
                Vector3D.TryParse(datas[1], out planetCenter);
            }
            public String GetDisplayString()
            {
                double dist = Ship.SphericalDist(remote.GetPosition(), Target, planetCenter, remote.GetPosition().Length());
                double longitude = GetLongitude(remote.GetPosition()) * 57.2958;
                double latitude = GetLatitude(remote.GetPosition()) * 57.2958;
                double heading = GetCurrentHeading() * 57.2958;
                Vector3D kek = Velocity;
                String output = "";
                //"Time to Arrival: " + Ship.TimeToDistance(dist, 100) + "\nDistance: " + dist + "\nLongitude: " + longitude + "\nLatitude: " + latitude + "\nHeading: " + heading + "\nVelocity: " + kek.Length() + "\nCoP: " + foundCenter, "LCDebug", false);    
                if (Target != new Vector3D(0, 0, 0))
                {
                    output += "Time to Arrival: " + Ship.TimeToDistance(dist, 100) + "\nDistance: " + dist;
                }
                if ((remote as IMyRemoteControl).GetNaturalGravity().Length() > 0.05)
                {
                    output += "\nLongitude: " + longitude + "\nLatitude: " + latitude + "\nHeading: " + heading;
                }
                //double time = CalculateReactorTime();
                // float powerUsage = CalculatePowerUsage();
                //string timePrefix=" Minutes";


                // output += "\nPower Time Left: " + (float)time/(powerUsage)+timePrefix;
                //output += "\nPower Usage: " + powerUsage*100 + "%";
                output += "\nVelocity: " + kek;
                return output;
            }
            // This method was heavily inspired by this site:
            // http://thefinalfrontier.se/power-output-on-lcd-panel/
            public double CalculateReactorTime()
            {
                WattsReactors = 0;
                kgOfUranium = 0;
                List<IMyTerminalBlock> reactors = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors);
                for (int i = 0; i < reactors.Count; i++)
                {
                    if ((reactors[i] as IMyFunctionalBlock).Enabled == false) continue;
                    double currentOutput;
                    System.Text.RegularExpressions.Match match = reactorRegex.Match(reactors[i].DetailedInfo);
                    if (match.Success)
                    {
                        if (Double.TryParse(match.Groups[1].Value, out currentOutput))
                        {
                            WattsReactors += currentOutput * Math.Pow(1000, Modifiers.IndexOf(match.Groups[2].Value));
                        }
                    }
                    var items = ((IMyInventoryOwner)reactors[i]).GetInventory(0).GetItems();
                    for (int k = 0; k < items.Count; k++)
                    {
                        if (items[k].Content.SubtypeName == "Uranium")
                        {
                            kgOfUranium += (double)items[k].Amount.RawValue;
                        }
                    }
                }
                List<IMyTerminalBlock> Batteries = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(Batteries);
                double batteryInput = 0;
                double batteryOutput = 0;
                double batteryStoredPower = 0;
                for (int i = 0; i < Batteries.Count; i++)
                {
                    if ((Batteries[i] as IMyFunctionalBlock).Enabled == false) continue;
                    double currentInput;
                    double currentOutput;
                    double currentStoredPower;
                    System.Text.RegularExpressions.Match match = batteryRegex.Match(Batteries[i].DetailedInfo);
                    System.Text.RegularExpressions.Match match2 = batteryRegex2.Match(Batteries[i].DetailedInfo);
                    if (match.Success)
                    {
                        if (Double.TryParse(match2.Groups[1].Value, out currentOutput))
                        {
                            batteryOutput += currentOutput * Math.Pow(1000, Modifiers.IndexOf(match2.Groups[2].Value));
                        }
                        if (Double.TryParse(match.Groups[1].Value, out currentInput))
                        {
                            batteryInput += currentInput * Math.Pow(1000, Modifiers.IndexOf(match.Groups[2].Value));
                        }
                        if (Double.TryParse(match.Groups[5].Value, out currentStoredPower))
                        {

                            batteryStoredPower += currentStoredPower * Math.Pow(1000, Modifiers.IndexOf(match.Groups[6].Value));

                        }
                    }
                }
                double batteryDifference = batteryOutput - batteryInput;
                Me.Echo(((((kgOfUranium + Math.Abs(batteryStoredPower / 1200)) / (WattsReactors + Math.Abs(batteryOutput)))) * 60).ToString());
                double batpower = double.IsNaN(Math.Abs(batteryStoredPower / batteryOutput)) ? 0 : Math.Abs(batteryStoredPower / batteryOutput);
                return (((kgOfUranium + Math.Abs(batteryStoredPower / 1200)) / (WattsReactors + Math.Abs(batteryOutput)))) * 60;
            }
            public float CalculatePowerUsage()
            {
                //WattsReactors = 0;
                //kgOfUranium = 0;
                List<IMyTerminalBlock> reactors = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactors);
                double outpoots = 0;
                double maxoutpoots = 0;
                for (int i = 0; i < reactors.Count; i++)
                {
                    if ((reactors[i] as IMyFunctionalBlock).Enabled == false) continue;

                    double currentOutput;
                    double maxOutput;
                    System.Text.RegularExpressions.Match match = reactorRegex.Match(reactors[i].DetailedInfo);
                    if (match.Success)
                    {
                        if (Double.TryParse(match.Groups[3].Value, out currentOutput))
                        {
                            outpoots += currentOutput * Math.Pow(1000, Modifiers.IndexOf(match.Groups[4].Value));
                        }
                        if (Double.TryParse(match.Groups[1].Value, out maxOutput))
                        {
                            maxoutpoots += maxOutput * Math.Pow(1000, Modifiers.IndexOf(match.Groups[2].Value));
                        }
                    }

                }
                List<IMyTerminalBlock> Batteries = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(Batteries);



                for (int i = 0; i < Batteries.Count; i++)
                {
                    if ((Batteries[i] as IMyFunctionalBlock).Enabled == false) continue;
                    double currentOutput;
                    double currentMaxOutput;
                    System.Text.RegularExpressions.Match match = batteryRegex.Match(Batteries[i].DetailedInfo);
                    System.Text.RegularExpressions.Match match2 = batteryRegex2.Match(Batteries[i].DetailedInfo);
                    if (match.Success)
                    {
                        if (Double.TryParse(match.Groups[3].Value, out currentOutput))
                        {
                            outpoots += currentOutput * Math.Pow(1000, Modifiers.IndexOf(match.Groups[4].Value));
                        }
                        if (Double.TryParse(match2.Groups[1].Value, out currentMaxOutput))
                        {

                            maxoutpoots += currentMaxOutput * Math.Pow(1000, Modifiers.IndexOf(match2.Groups[2].Value));
                            //Me.Echo(currentMaxOutput.ToString());
                        }
                    }
                }
                return (float)(outpoots / maxoutpoots);
            }
            public Vector3D CalcCenterOfPlanet(List<Vector3D> x, List<Vector3D> d, String Name = "LCDebug", int kek = 1)
            {
                Vector3D FirstPos = x[0];
                Vector3D FirstGravVector = d[0] + x[0];
                Vector3D LastPos = x[kek];
                Vector3D LastGravVector = d[kek] + x[kek];
                double aX = FirstPos.GetDim(0);
                double aY = FirstPos.GetDim(1);
                double aZ = FirstPos.GetDim(2);
                double bX = FirstGravVector.GetDim(0);
                double bY = FirstGravVector.GetDim(1);
                double bZ = FirstGravVector.GetDim(2);

                double cX = LastPos.GetDim(0);
                double cY = LastPos.GetDim(1);
                double cZ = LastPos.GetDim(2);
                double sX = LastGravVector.GetDim(0);
                double sY = LastGravVector.GetDim(1);
                double sZ = LastGravVector.GetDim(2);

                double AB2, SCxAB, ASxAB, ABxSC, SC2, ASxSC, Mk, Nk, mX, mY, mZ, nX, nY, nZ;
                AB2 = (bX - aX) * (bX - aX) + (bY - aY) * (bY - aY) + (bZ - aZ) * (bZ - aZ);
                SCxAB = (cX - sX) * (bX - aX) + (cY - sY) * (bY - aY) + (cZ - sZ) * (bZ - aZ);
                ASxAB = (sX - aX) * (bX - aX) + (sY - aY) * (bY - aY) + (sZ - aZ) * (bZ - aZ);
                ABxSC = (bX - aX) * (cX - sX) + (bY - aY) * (cY - sY) + (bZ - aZ) * (cZ - sZ);
                SC2 = (cX - sX) * (cX - sX) + (cY - sY) * (cY - sY) + (cZ - sZ) * (cZ - sZ);
                ASxSC = (sX - aX) * (cX - sX) + (sY - aY) * (cY - sY) + (sZ - aZ) * (cZ - sZ);

                Mk = ((ASxAB * SC2) - (ASxSC * SCxAB)) / ((AB2 * SC2) - (ABxSC * SCxAB));
                Nk = ((AB2 * ASxSC) - (ABxSC * ASxAB)) / ((AB2 * SC2) - (ABxSC * SCxAB));

                mX = aX + ((bX - aX) * Mk);
                mY = aY + ((bY - aY) * Mk);
                mZ = aZ + ((bZ - aZ) * Mk);

                nX = sX + ((sX - cX) * Nk);
                nY = sY + ((sY - cY) * Nk);
                nZ = sZ + ((sZ - cZ) * Nk);
                planetCenter = new Vector3D((mX + nX) / 2, (mY + nY) / 2, (mZ + nZ) / 2);
                StoreDataInScreen(Name);
                return planetCenter;
            }

            public Vector3D ComPos
            {
                get
                {

                    return Vector3D.Transform(localCoM, remote.WorldMatrix) + remote.GetPosition();
                }
            }
            public Vector3D CircleBy3Points(Vector3D p1, Vector3D p2, Vector3D p3, out bool pp, out double Radius)
            {
                Vector3D Center = new Vector3D(0, 0, 0);
                Radius = 0;
                // triangle "edges"       
                Vector3D t = p2 - p1;
                Vector3D u = p3 - p1;
                Vector3D v = p3 - p2;
                // triangle normal       
                Vector3D w = Vector3D.Cross(t, u);
                double wsl = Math.Pow(w.Length(), 2);
                if (wsl < 10e-14)
                {
                    pp = false;
                    return Center;
                }
                else
                {
                    // helpers       
                    double iwsl2 = 1.0 / (2.0 * wsl);
                    double tt = Vector3D.Dot(t, t);
                    double uu = Vector3D.Dot(u, u);
                    // result circle       
                    Center = p1 + (u * tt * Vector3D.Dot(u, v) - t * uu * Vector3D.Dot(t, v)) * iwsl2;
                    Radius = Math.Sqrt(tt * uu * Vector3D.Dot(v, v) * iwsl2 * 0.5);
                    pp = true;
                    return Center;
                }
            }
            List<IMyTerminalBlock> gyros = new List<IMyTerminalBlock>();
            public IMyTerminalBlock remote;
            public void CalculateForce(List<IMyTerminalBlock> thrusts, out double Thrust)
            {

                Thrust = 0;
                for (int i = 0; i < thrusts.Count; i++)
                {
                    IMyTerminalBlock thrust = thrusts[i];
                    string Case = thrust.BlockDefinition.SubtypeId;

                    if (Case == "SmallBlockSmallThrust")
                    {
                        Thrust += smallsmallIonThrust;
                        continue;
                    }
                    else if (Case == "SmallBlockLargeThrust")
                    {
                        Thrust += smalllargeIonThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockSmallThrust")
                    {
                        Thrust += smallIonThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockLargeThrust")
                    {
                        Thrust += largeIonThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockLargeHydrogenThrust")
                    {
                        Thrust += largeHydroThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockSmallHydrogenThrust")
                    {
                        Thrust += smallHydroThrust;
                        continue;
                    }
                    else if (Case == "SmallBlockLargeHydrogenThrust")
                    {
                        Thrust += smalllargeHydroThrust;
                        continue;
                    }
                    else if (Case == "SmallBlockSmallHydrogenThrust")
                    {
                        Thrust += smallsmallHydroThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockLargeAtmosphericThrust")
                    {
                        Thrust += largeAtmoThrust;
                        continue;
                    }
                    else if (Case == "LargeBlockSmallAtmosphericThrust")
                    {
                        Thrust += smallAtmoThrust;
                        continue;
                    }
                    else if (Case == "SmallBlockLargeAtmosphericThrust")
                    {
                        Thrust += smalllargeAtmoThrust;
                        continue;
                    }
                    else if (Case == "SmallBlockSmallAtmosphericThrust")
                    {
                        Thrust += smallsmallAtmoThrust;
                        continue;
                    }
                }
            }
            public static TimeSpan TimeToDistance(double dist, double vel)
            {
                long Ticks = (long)(dist / vel) * TimeSpan.TicksPerSecond;
                TimeSpan span = new TimeSpan(Ticks);
                return span;
            }
            public void PopulateThrusters()
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyThrust>(blocks);
                upThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Up"));
                downThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Down"));
                leftThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Left"));
                rightThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Right"));
                forThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Forward"));
                backThrusts = Where(blocks, thrust => thrust.CustomName.Contains("Backward"));
                CalculateForce(upThrusts, out upThrust);
                CalculateForce(downThrusts, out downThrust);
                CalculateForce(leftThrusts, out leftThrust);
                CalculateForce(rightThrusts, out rightThrust);
                CalculateForce(forThrusts, out forThrust);
                CalculateForce(backThrusts, out backThrust);
            }
            public List<IMyTerminalBlock> Where(List<IMyTerminalBlock> source, Func<IMyTerminalBlock, bool> condition)
            {
                List<IMyTerminalBlock> output = new List<IMyTerminalBlock>();
                source.ForEach(item =>
                {
                    if (condition.Invoke(item))
                    {
                        output.Add(item);
                    }
                });
                return output;
            }
            public void Thrust(Vector3D dirp)
            {
                Vector3D dir = dirp * 100;
                if (dir.GetDim(0) <= 0)
                {
                    //ApplyAction(leftThrusts,"OnOff_On");  
                    SetValueFloat(leftThrusts, "Override", (float)Math.Abs(dir.GetDim(0)));

                    //ApplyAction(rightThrusts, "OnOff_Off");  
                    SetValueFloat(rightThrusts, "Override", 0f);
                }

                else
                {
                    //ApplyAction(rightThrusts, "OnOff_On");  
                    SetValueFloat(rightThrusts, "Override", (float)Math.Abs(dir.GetDim(0)));
                    //ApplyAction(leftThrusts, "OnOff_Off");  

                    SetValueFloat(leftThrusts, "Override", 0f);

                }
                if (dir.GetDim(1) <= 0)
                {
                    //ApplyAction(downThrusts, "OnOff_On");  
                    SetValueFloat(downThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);


                    //ApplyAction(upThrusts, "OnOff_Off");  
                    SetValueFloat(upThrusts, "Override", 0f);

                }
                else
                {
                    //ApplyAction(upThrusts, "OnOff_On");  
                    SetValueFloat(upThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);

                    //ApplyAction(downThrusts, "OnOff_Off");  
                    //SetValueFloat(downThrusts, "Override", 0f);  

                }
                if (dir.GetDim(2) <= 0)
                {
                    //ApplyAction(forThrusts, "OnOff_On");  
                    SetValueFloat(forThrusts, "Override", (float)Math.Abs(dir.GetDim(2)));

                    //ApplyAction(backThrusts, "OnOff_Off");  
                    SetValueFloat(backThrusts, "Override", 0f);

                }
                else
                {
                    //ApplyAction(leftThrusts, "OnOff_On");  
                    SetValueFloat(backThrusts, "Override", (float)Math.Abs(dir.GetDim(2)));


                    // ApplyAction(leftThrusts, "OnOff_On");  
                    SetValueFloat(forThrusts, "Override", 0f);

                }

            }
            public void Thrust_Planet(Vector3D dirp)
            {
                Vector3D dir = dirp * 100;
                if (dir.GetDim(0) < 0)
                {
                    ApplyAction(leftThrusts, "OnOff_On");
                    SetValueFloat(leftThrusts, "Override", (float)Math.Abs(dir.GetDim(0)));

                    ApplyAction(rightThrusts, "OnOff_Off");
                    SetValueFloat(rightThrusts, "Override", 0f);
                }

                else if (dir.GetDim(0) > 0)
                {
                    ApplyAction(rightThrusts, "OnOff_On");
                    SetValueFloat(rightThrusts, "Override", (float)Math.Abs(dir.GetDim(0)));
                    ApplyAction(leftThrusts, "OnOff_Off");

                    SetValueFloat(leftThrusts, "Override", 0f);

                }
                else
                {
                    SetValueFloat(leftThrusts, "Override", 0f);
                    SetValueFloat(rightThrusts, "Override", 0f);
                }
                if (dir.GetDim(1) <= 0)
                {
                    //ApplyAction(downThrusts, "OnOff_On");  
                    SetValueFloat(downThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);


                    //ApplyAction(upThrusts, "OnOff_Off");  
                    SetValueFloat(upThrusts, "Override", 0f);

                }
                else
                {
                    //ApplyAction(upThrusts, "OnOff_On");  
                    SetValueFloat(upThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);

                    //ApplyAction(downThrusts, "OnOff_Off");  
                    SetValueFloat(downThrusts, "Override", 0f);

                }
                if (dir.GetDim(2) <= 0)
                {
                    ApplyAction(forThrusts, "OnOff_On");
                    SetValueFloat(forThrusts, "Override", (float)Math.Abs(dir.GetDim(2)));

                    ApplyAction(backThrusts, "OnOff_Off");
                    SetValueFloat(backThrusts, "Override", 0f);

                }
                else
                {
                    ApplyAction(leftThrusts, "OnOff_On");
                    SetValueFloat(backThrusts, "Override", (float)Math.Abs(dir.GetDim(2)));


                    ApplyAction(leftThrusts, "OnOff_On");
                    SetValueFloat(forThrusts, "Override", 0f);

                }
            }
            public void ThrustLast(Vector3D dirp, bool adjustOtherSide = false, bool faggot = false)// Vector that contains overrides for each axis relative to remote                   
            {
                Vector3D dir = dirp * 100;
                if (dir == new Vector3D(0, 0, 0))
                {
                    SetValueFloat(leftThrusts, "Override", 0f, faggot);
                    SetValueFloat(rightThrusts, "Override", 0f, faggot);
                    if ((remote as IMyRemoteControl).GetNaturalGravity().Length() > 0.05f)
                    {
                        SetValueFloat(upThrusts, "Override", 0f, false);
                        SetValueFloat(downThrusts, "Override", 0f, false);
                    }
                    else
                    {
                        SetValueFloat(upThrusts, "Override", 0f, faggot);
                        SetValueFloat(downThrusts, "Override", 0f, faggot);
                    }
                    SetValueFloat(forThrusts, "Override", 0f, faggot);
                    SetValueFloat(backThrusts, "Override", 0f, faggot);
                }
                if (dir.GetDim(0) <= 0)
                {

                    SetValueFloat(leftThrusts, "Override", (float)Math.Abs(dir.GetDim(0)), faggot);
                    if (adjustOtherSide)
                    {
                        double over = dir.GetDim(0) == 0 ? 0 : 1f;
                        SetValueFloat(rightThrusts, "Override", (float)over, faggot);
                    }
                }
                else
                {
                    SetValueFloat(rightThrusts, "Override", (float)Math.Abs(dir.GetDim(0)), faggot);
                    if (adjustOtherSide)
                    {
                        double over = dir.GetDim(0) == 0 ? 0 : 1f;
                        SetValueFloat(leftThrusts, "Override", (float)over, faggot);
                    }
                }
                if (dir.GetDim(1) <= 0)
                {
                    SetValueFloat(downThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);
                    if (adjustOtherSide)
                    {

                        double over = dir.GetDim(1) == 0 ? 0 : 1f;
                        SetValueFloat(upThrusts, "Override", (float)over, false);
                    }
                }
                else
                {
                    SetValueFloat(upThrusts, "Override", (float)Math.Abs(dir.GetDim(1)), false);
                    if (adjustOtherSide)
                    {
                        double over = dir.GetDim(1) == 0 ? 0 : 1f;
                        SetValueFloat(downThrusts, "Override", (float)over, false);
                    }
                }
                if (dir.GetDim(2) <= 0)
                {
                    SetValueFloat(forThrusts, "Override", (float)Math.Abs(dir.GetDim(2)), faggot);
                    if (adjustOtherSide)
                    {
                        double over = dir.GetDim(2) == 0 ? 0 : 1f;
                        SetValueFloat(backThrusts, "Override", (float)over, faggot);
                    }
                }
                else
                {
                    SetValueFloat(backThrusts, "Override", (float)Math.Abs(dir.GetDim(2)), faggot);
                    if (adjustOtherSide)
                    {
                        double over = dir.GetDim(2) == 0 ? 0 : 1f;
                        SetValueFloat(forThrusts, "Override", (float)over, faggot);
                    }
                }
            }
            public void SetValueFloat(List<IMyTerminalBlock> blocks, string key, float value, bool AdjustForThrust = false)
            {
                if (!AdjustForThrust)
                {
                    blocks.ForEach((block) =>
                    {
                        block.SetValueFloat(key, value);
                    });
                }
                else
                {
                    blocks.ForEach((block) =>
                    {

                        block.SetValueFloat(key, value);

                    });
                }
            }
            public void SetValueBool(List<IMyTerminalBlock> blocks, string key, bool value)
            {
                blocks.ForEach((block) =>
                {
                    block.SetValueBool(key, value);
                });

            }
            public void ApplyAction(List<IMyTerminalBlock> blocks, string key)
            {
                blocks.ForEach((block) =>
                {
                    block.GetActionWithName(key).Apply(block);
                });
            }
            public void WriteToScreen(string text, string name, bool append)
            {
                IMyTextPanel panel;
                panel = GridTerminalSystem.GetBlockWithName(name) as IMyTextPanel;
                if (panel != null)
                {
                    panel.WritePublicText(text + '\n', append);
                }
            }
            public void StoreInScreen(string text, string name, bool append)
            {
                IMyTextPanel panel;
                panel = GridTerminalSystem.GetBlockWithName(name) as IMyTextPanel;
                if (panel != null)
                {
                    panel.WritePublicTitle(text);
                }
            }
            public String GetStoredInScreen(string name)
            {
                IMyTextPanel panel;
                panel = GridTerminalSystem.GetBlockWithName(name) as IMyTextPanel;
                if (panel != null)
                {
                    return panel.GetPublicTitle();
                }
                else
                {
                    return "Error: Block isn't an LCD. KYS";
                }
            }
            public double GetThrust(IMyTerminalBlock block)
            {
                double Thrust = 0;
                IMyTerminalBlock thrust = block;
                string Case = thrust.BlockDefinition.SubtypeId;

                if (Case == "SmallBlockSmallThrust")
                {
                    Thrust += smallsmallIonThrust;
                    return Thrust;
                }
                else if (Case == "SmallBlockLargeThrust")
                {
                    Thrust += smalllargeIonThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockSmallThrust")
                {
                    Thrust += smallIonThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockLargeThrust")
                {
                    Thrust += largeIonThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockLargeHydrogenThrust")
                {
                    Thrust += largeHydroThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockSmallHydrogenThrust")
                {
                    Thrust += smallHydroThrust;
                    return Thrust;
                }
                else if (Case == "SmallBlockLargeHydrogenThrust")
                {
                    Thrust += smalllargeHydroThrust;
                    return Thrust;
                }
                else if (Case == "SmallBlockSmallHydrogenThrust")
                {
                    Thrust += smallsmallHydroThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockLargeAtmosphericThrust")
                {
                    Thrust += largeAtmoThrust;
                    return Thrust;
                }
                else if (Case == "LargeBlockSmallAtmosphericThrust")
                {
                    Thrust += smallAtmoThrust;
                    return Thrust;
                }
                else if (Case == "SmallBlockLargeAtmosphericThrust")
                {
                    Thrust += smalllargeAtmoThrust;
                    return Thrust;
                }
                else if (Case == "SmallBlockSmallAtmosphericThrust")
                {
                    Thrust += smallsmallAtmoThrust;
                    return Thrust;
                }
                else
                {
                    return Thrust;
                }
            }

            Vector3D lastDir;
            public bool ThrustTowardsVector(Vector3D dir)
            {
                if (first || dir != lastDir)
                {
                    startingVelocity = currentVelocity;
                    lastDir = dir;
                }
                Vector3D vel = currentVelocity;
                Vector3D targVel = startingVelocity + dir;
                Vector3D remVel = targVel - vel;
                double mod = remVel.Length() / dir.Length();
                Vector3D localremVel = Vector3D.Transform(remVel, MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.Forward, remote.WorldMatrix.Up)));
                localremVel.Normalize();
                if (remVel.Length() < 0.1)
                {
                    first = true;
                    Thrust(new Vector3D(0, 0, 0));
                    return true;
                }
                // Me.Echo(mod.ToString() + remVel.Length().ToString());    
                Thrust(localremVel * mod);
                return false;
            }
            //Credit to NMaster                
            public double GetMassSummary()
            {
                List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocks(Blocks);
                double used = 0;
                for (int i = 0; i < Blocks.Count; i++)
                {
                    for (int invId = 0; invId < 2; invId++)
                    {
                        IMyInventory inv = Blocks[i].GetInventory(invId);
                        if (inv == null)
                            continue;

                        used += (double)inv.CurrentMass;
                    }
                }
                return used * 1000;
            }
            public double GetMassTemp()
            {
                return 0;
            }
            public Vector3D Velocity
            {
                get
                {
                    Vector3D vel;
                    double interval = 60;//Me.ElapsedTime.TotalSeconds;// (interval / (TimeSpan.TicksPerMillisecond)) / 1000;                  

                    vel = (lastPos - remote.GetPosition()) * interval;
                    lastPos = remote.GetPosition();


                    //vel = (lastPos - ComPos) * interval;    
                    //lastPos = ComPos;    
                    if (vel != new Vector3D(0, 0, 0))
                        currentVelocity = vel;
                    //      lastVelTime = DateTime.Now;                  
                    return vel;
                }
            }
            public Vector3D velocity
            {
                get
                {

                    Vector3D vel = Velocity;
                    if (vel == new Vector3D(0, 0, 0))
                    {
                        vel = currentVelocity;
                    }
                    else
                    {
                        currentVelocity = vel;
                    }

                    return vel;
                }
            }
            public Vector3D Acceleration
            {
                get
                {

                    Vector3D vels = Velocity;
                    if (vels == new Vector3D(0, 0, 0))
                    {
                        vels = currentVelocity;
                    }
                    double interval = 60;//Me.ElapsedTime.TotalSeconds;       

                    Vector3D vel = (vels - lastVel) * interval;

                    lastVel = vels;
                    // lastAccelTime = DateTime.Now;                  
                    return vel;
                }
            }
            public double GetLatitude(Vector3D pos)//Pos has to be relative to planet       
            {
                Vector3D relDir = Vector3D.Normalize(pos);
                Vector3D Rejected = Vector3D.Normalize(Vector3D.Reject(relDir, new Vector3D(0, -1, 0)));
                return Math.Acos(Vector3D.Dot(Rejected, relDir));
            }
            public double GetLongitude(Vector3D pos)//Pos has to be relative to planet       
            {
                Vector3D norm = Vector3D.Normalize(pos);
                Vector3D rejectedNorm = Vector3D.Normalize(Vector3D.Reject(norm, new Vector3D(0, -1, 0)));
                double ang = Math.Atan2(rejectedNorm.GetDim(2), rejectedNorm.GetDim(0));
                if (ang < 0) ang += 2 * Math.PI;
                return ang;
            }
            public double GetCurrentHeading()
            {
                Vector3D normGrav = Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity());
                MatrixD localGravMatrix = MatrixD.Invert(MatrixD.CreateFromDir(Vector3D.Normalize(Vector3D.Reject(new Vector3D(0, 1, 0), normGrav)), normGrav));
                Vector3D localForward = Vector3D.Normalize(Vector3D.Transform((Vector3D.Reject(remote.WorldMatrix.Forward, normGrav)), localGravMatrix));
                double ang = Math.Atan2(localForward.GetDim(2), localForward.GetDim(0)) - 1.5708;
                if (ang < 0) ang += 2 * Math.PI;
                return ang;
            }
            public void PopulateGyros()
            {
                List<IMyTerminalBlock> gyroe = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyroe);
                gyros = gyroe;
            }
            public Ship(IMyTerminalBlock remote, IMyGridTerminalSystem gts, MyGridProgram pro, String Name = "LCDebug")
            {
                GridTerminalSystem = gts;
                this.remote = remote;
                PopulateGyros();
                PopulateThrusters();
                lastPos = remote.GetPosition();
                Me = pro;
                GetStoredInScreen(Name);
            }
            public bool SetForwardVelocity_Planet(double thrustLimit)
            {
                Vector3D vel = currentVelocity;
                //Me.Echo(vel.Length().ToString());  
                if ((thrustLimit - vel.Length()) < 0.1 && (thrustLimit - vel.Length()) >= 0)
                {
                    if (!topKek)
                    {
                        Thrust_Planet(new Vector3D(0, 0, 0f));
                        topKek = true;
                    }


                    return true;
                }
                else
                {
                    topKek = false;
                    if ((thrustLimit - vel.Length()) < 0)
                    {
                        Thrust_Planet(new Vector3D(0, 0, 1));
                        return false;
                    }
                }
                Thrust_Planet(new Vector3D(0, 0, -1));
                return false;
            }
            public static double SphericalDist(Vector3D origin, Vector3D targ, Vector3D center, double radius)
            {
                double circumfrence = 2 * Math.PI * radius;
                double angBetween = Math.Acos(Vector3D.Dot(Vector3D.Normalize(origin - center), Vector3D.Normalize(targ - center)));
                return circumfrence * (angBetween / (2 * Math.PI));
            }
            public bool set_oriented_gyros_planet(Vector3D tar, Vector3D planetCenter, Vector3D dir = default(Vector3D), Vector3D Down = default(Vector3D))
            {
                tar = remote.GetPosition() - tar;
                tar.Normalize();
                Matrix orientation;
                remote.Orientation.GetMatrix(out orientation);
                MatrixD invMatrix = MatrixD.Invert(orientation);
                MatrixD balls = MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.Forward, remote.WorldMatrix.Up)) * orientation;
                Vector3D localTar = Vector3D.Transform(tar, balls);
                Vector3D dirp = dir == default(Vector3D) ? (Vector3D)orientation.Forward : dir;


                double poop;
                Vector3D grav = -Vector3D.Transform(Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity()), balls);
                Vector3D poopSin;

                Vector3D angsin;
                angsin = Vector3D.Cross(orientation.Down, -Vector3D.Transform(Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity()), balls));
                Vector3D ang;
                if (Vector3D.Dot(dirp, Vector3D.Normalize(Vector3D.Reject(localTar, grav))) > 0.98)
                {

                    poopSin = Vector3D.Cross(Vector3D.CalculatePerpendicularVector(dirp), Vector3D.Normalize(Vector3D.Reject(localTar, grav)));

                }
                else
                {
                    poopSin = Vector3D.Cross(dirp, Vector3D.Normalize(Vector3D.Reject(localTar, grav)));

                }
                poop = poopSin.GetDim(2);
                ang = new Vector3D(Math.Asin(angsin.GetDim(0)), Math.Asin(angsin.GetDim(1)), Math.Asin(poop));
                bool complete = false;
                if (ang.Length() < 0.005) { ang = new Vector3D(0, 0, 0); complete = true; }

                for (int i = 0; i < gyros.Count; i++)
                {
                    Matrix gyro_or;
                    gyros[i].Orientation.GetMatrix(out gyro_or);
                    MatrixD invGyroMatrix = MatrixD.Invert(gyro_or);//invMatrix *             
                    invGyroMatrix *= (new Matrix(-1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)); //Credit to Pennywise for this matrix                   
                    Vector3D angle = Vector3D.Transform(ang, invGyroMatrix);
                    gyros[i].SetValueFloat("Pitch", (float)angle.GetDim(0));
                    gyros[i].SetValueFloat("Yaw", (float)angle.GetDim(1));
                    gyros[i].SetValueFloat("Roll", (float)angle.GetDim(2));
                }
                return complete;
            }
            public bool set_oriented_gyros(Vector3D tar, Vector3D dir = default(Vector3D))
            {
                tar = remote.GetPosition() - tar;
                tar.Normalize();
                Matrix orientation;
                remote.Orientation.GetMatrix(out orientation);
                MatrixD invMatrix = MatrixD.Invert(orientation);
                Vector3D localTar = Vector3D.Transform(tar, MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.Forward, remote.WorldMatrix.Up)) * orientation);
                Vector3D dirp = dir == default(Vector3D) ? (Vector3D)orientation.Forward : dir;


                double poop;

                Vector3D angsin;
                Vector3D ang;
                if (Vector3D.Dot(dirp, localTar) > 0.98)
                {


                    angsin = Vector3D.Cross(Vector3D.CalculatePerpendicularVector(dirp), localTar);

                }
                else
                {
                    angsin = Vector3D.Cross(dirp, localTar);

                }
                poop = angsin.GetDim(2);
                ang = new Vector3D(Math.Asin(angsin.GetDim(0)), Math.Asin(angsin.GetDim(1)), Math.Asin(poop));
                bool complete = false;
                if (ang.Length() < 0.005) { ang = new Vector3D(0, 0, 0); complete = true; }
                for (int i = 0; i < gyros.Count; i++)
                {
                    Matrix gyro_or;
                    gyros[i].Orientation.GetMatrix(out gyro_or);
                    MatrixD invGyroMatrix = MatrixD.Invert(gyro_or);//invMatrix *             
                    invGyroMatrix *= (new Matrix(-1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)); //Credit to Pennywise for this matrix                   
                    Vector3D angle = Vector3D.Transform(ang, invGyroMatrix);
                    gyros[i].SetValueFloat("Pitch", (float)angle.GetDim(0));
                    gyros[i].SetValueFloat("Yaw", (float)angle.GetDim(1));
                    gyros[i].SetValueFloat("Roll", (float)angle.GetDim(2));
                }
                return complete;
            }

            public bool set_oriented_gyros_dir(Vector3D tar, Vector3D dir = default(Vector3D))
            {
                tar.Normalize();
                Matrix orientation;
                remote.Orientation.GetMatrix(out orientation);
                MatrixD invMatrix = MatrixD.Invert(orientation);
                Vector3D localTar = Vector3D.Transform(tar, MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.Forward, remote.WorldMatrix.Up)) * orientation);
                Vector3D dirp = dir == default(Vector3D) ? (Vector3D)orientation.Forward : dir;


                double poop;
                Vector3D angsin;
                Vector3D ang;
                if (Vector3D.Dot(dirp, localTar) > 0.98)
                {


                    angsin = Vector3D.Cross(Vector3D.CalculatePerpendicularVector(dirp), localTar);

                }
                else
                {
                    angsin = Vector3D.Cross(dirp, localTar);

                }
                poop = angsin.GetDim(2);

                ang = new Vector3D(Math.Asin(angsin.GetDim(0)), Math.Asin(angsin.GetDim(1)), Math.Asin(poop));
                bool complete = false;
                if (ang.Length() < 0.005) { ang = new Vector3D(0, 0, 0); complete = true; }
                for (int i = 0; i < gyros.Count; i++)
                {
                    Matrix gyro_or;
                    gyros[i].Orientation.GetMatrix(out gyro_or);
                    MatrixD invGyroMatrix = MatrixD.Invert(gyro_or);//invMatrix *              
                    invGyroMatrix *= (new Matrix(-1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)); //Credit to Pennywise for this matrix                    
                    Vector3D angle = Vector3D.Transform(ang, invGyroMatrix);
                    gyros[i].SetValueFloat("Pitch", (float)angle.GetDim(0));
                    gyros[i].SetValueFloat("Yaw", (float)angle.GetDim(1));
                    gyros[i].SetValueFloat("Roll", (float)angle.GetDim(2));
                }
                return complete;
            }
        }
        public class SingleAxisThrustShip : Ship
        {
            public SingleAxisThrustShip(IMyTerminalBlock remote, IMyGridTerminalSystem gts, MyGridProgram pro)
                : base(remote, gts, pro)
            {

            }
            public bool singleAxisCancelVel_planet(Vector3D dir = default(Vector3D))
            {
                Matrix orientation;
                remote.Orientation.GetMatrix(out orientation);
                MatrixD invMatrix = MatrixD.Invert(orientation);
                Vector3D dirp = dir == default(Vector3D) ? (Vector3D)orientation.Forward : dir;
                Vector3D grav = Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity());
                Vector3D vel = currentVelocity;
                if (vel.Length() == 0)
                {
                    return true;
                }
                Vector3D dampen = vel - (remote as IMyRemoteControl).GetNaturalGravity();
                set_oriented_gyros_dir(Vector3D.Normalize(dampen), dir);
                return false;
            }
        }
        #endregion
    }
}
