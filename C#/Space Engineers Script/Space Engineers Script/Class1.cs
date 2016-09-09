using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using VRage.Game.ModAPI.Ingame;
using VRageMath;
using Sandbox.ModAPI.Interfaces;
using System;
namespace Space_Engineers_Script
{
    public class Program : MyGridProgram
    {
        
        public IMyGridTerminalSystem GridTerminalSystem;

        #region script
        public int Multiplier = 10;
        bool comfound = true;
        public SingleAxisThrustShip ship;
        IMyTerminalBlock timer;
        Vector3D lastPos;
        Vector3D lastG;
        Vector3D center;
        IMyTerminalBlock remote;
        Planet planet = Planet.EARTHLIKE;
        Vector3D targvel;
        List<Vector3D> ds = new List<Vector3D>();
        List<Vector3D> xs = new List<Vector3D>();
        Vector3D beginningvel;
        ArmController arm;
        Vector3D Center;
        bool isfirst2 = false;
        bool test = false;
        bool isfirst = true;
        IMyTextPanel panel;

        void Main(string argument)
        {
            if (isfirst2)
            {
                //beginningvel = ship.Velocity;                       
                isfirst2 = false;
            }
            if (isfirst)
            {
                Vector3D targ = new Vector3D(0, -60000, 0);

                remote = GridTerminalSystem.GetBlockWithName("Remote");
                timer = GridTerminalSystem.GetBlockWithName("Timer");
                ship.Multiplier = Multiplier;
                ship = new SingleAxisThrustShip(remote, GridTerminalSystem, this);
                // ship.planet = planet;       
                ship.planet = Planet.ALIEN;
                //ship.Mass = 959798 - ship.GetMassSummary();          
                ship.Target = (new Vector3D(-100, 0, 0)) + remote.GetPosition();
                //arm = new ArmController(ship, "Shoulder Yaw", "Shoulder Pitch", "Elbow", "Wrist Pitch", "Wrist Yaw");
                isfirst = false;
                isfirst2 = true;
                // panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCDebug");          
                //arm.extensionOffsetAngle = Math.PI / 2;
                targvel = remote.WorldMatrix.Forward * 10;
                //InteropManager.Add(new InterpOperation(arm, Vector3D.Transform(new Vector3D(10, 5, 0), arm.shoulderYaw.WorldMatrix), 0, 0));
                //InteropManager.Add(new InterpOperation(arm, remote.GetPosition(), 0, 0));
            }
            switch (argument)
            {
                case "Measure":
                    xs.Add(remote.GetPosition());
                    ds.Add(Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity()));
                    break;
                default:


                    if (!comfound)
                    {
                        // comfound = ship.TestForInitialCoM();          
                    }




                    if (xs.Count >= 2)
                    {
                       // Center = Planet.CalcCenterOfPlanet(xs, ds);
                    }
                    double poop = 0;
                    double pee = 0;
                    //test = arm.setArm(Vector3D.Transform(new Vector3D(10, 5, 0), arm.shoulderYaw.WorldMatrix), 0, 0);  
                    // Echo((Vector3D.Transform(arm.FindPos(out poop,out pee), MatrixD.Invert(arm.shoulderYaw.WorldMatrix))).ToString());  

                    //InteropManager.Tick();
                    //     //Vector3D.Transform(new Vector3D(3, 0, 0), arm.shoulderYaw.WorldMatrix)        


                    //new Vector3D(0,0, 9), arm.shoulderYaw.WorldMatrix)        

                    //ship.WriteToScreen(ship.GetDisplayString(), "LCDebug", false);          

                    Matrix orient;
                    remote.Orientation.GetMatrix(out orient);

                    MatrixD InvWorld = MatrixD.Invert(MatrixD.CreateFromDir(remote.WorldMatrix.
                    Forward, remote.WorldMatrix.Forward));

                    //  if (!test)
                    //  {
                    //      ship.SetDampeners(false);
                    ship.MaintainVelocity(Vector3D.Normalize((remote as IMyRemoteControl).GetNaturalGravity()) * 1.2);
                    // }
                    // else
                    //  {
                    //      ship.SetDampeners(true);
                    //  }
                    break;
            }
            //ship.set_oriented_gyros_planet(targ, new Vector3D(0, 0, 0), orient.Right);                           
            if ((timer as IMyFunctionalBlock).Enabled)
            {
                timer.GetActionWithName("TriggerNow").Apply(timer);
            }

        }

        public class Planet
        {
            public double LimitAltitude;
            public double HillParams;
            public Vector3D Center;
            public double Radius;
            public double Density;
            public static Planet EARTHLIKE = new Planet()
            {
                Center = new Vector3D(0, 0, 0),
                Radius = 60000,
                HillParams = .12,
                LimitAltitude = 2,
                Density = 1
            };
            public static Planet ALIEN = new Planet()
            {
                Center = new Vector3D(131072.46958, 131072.45268, 5731072.41439),
                Radius = 60000,
                HillParams = .12,
                LimitAltitude = 2,
                Density = 1.2
            };
            public double GetAirDensity(double altitude)
            {
                return (1 - ((altitude - Radius) / (Radius * 0.7 * HillParams * LimitAltitude))) * Density;
            }
            public double GetAtmosphericThrusterCoefficient(double altitude)
            {

                double ratio = (1 - ((altitude - Radius) / (Radius * 0.7 * HillParams * LimitAltitude))) * Density;

                return MathHelper.Clamp(ratio, 0, 1);
            }
            public double GetIonThrusterCoefficient(double altitude)
            {
                //(altitude - Radius) / (Radius * HillParams * LimitAltitude))
                double ratio = MathHelper.Clamp((-(GetAirDensity(altitude) - 1)), 0, 1) * 0.7 + 0.3;

                return MathHelper.Clamp(ratio, 0, 1);
            }
            public Planet() { }
            public static Vector3D CalcCenterOfPlanet(List<Vector3D> x, List<Vector3D> d, String Name = "LCDebug", int kek = 1)
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
                Vector3D Center = new Vector3D((mX + nX) / 2, (mY + nY) / 2, (mZ + nZ) / 2);

                return Center;
            }
        }

        public class Ship
        {
            #region Variable Declerations
            public int Multiplier = 1;
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
            public Planet planet;
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
            Ship target;
            public Vector3D Target;
            List<Vector3D> samples = new List<Vector3D>();
            int sampleCount;
            #endregion
            public void SetDampeners(bool ayylmao)
            {
                remote.SetValueBool("DampenersOverride", ayylmao);
            }
            public Vector3D Projection(Vector3D thi, Vector3D that)
            {
                // (scalar/scalar)*(vector) = (vector)                       
                return (Vector3D.Dot(that, thi) / Vector3D.Dot(that, that)) * that;
            }
            public static double GetTargetRotorAngle(IMyTerminalBlock rotor, Vector3D targ)
            {
                Vector3D targDir = Vector3D.Normalize(targ - rotor.GetPosition());
                targDir = Vector3D.Normalize(Vector3D.Reject(targDir, rotor.WorldMatrix.Up));
                targDir = Vector3D.Transform(targDir, MatrixD.Invert(rotor.WorldMatrix.GetOrientation()));
                double angleAway = Math.Atan2(targDir.GetDim(2), targDir.GetDim(0));
                double finalAng = angleAway;
                return finalAng;
            }
            public static double GetTargetRotorAngle(IMyTerminalBlock rotor, Vector3D targ, IMyTerminalBlock reference)
            {

                Vector3D targDir = Vector3D.Normalize(targ - reference.GetPosition());
                targDir = Vector3D.Normalize(Vector3D.Reject(targDir, rotor.WorldMatrix.Up));
                MatrixD state = MatrixD.Invert(rotor.WorldMatrix.GetOrientation());
                targDir = Vector3D.Transform(targDir, state);
                double angleAway = Math.Atan2(targDir.GetDim(2), targDir.GetDim(0));//Math.Atan2(Vector3D.Cross(targDir, rotor.WorldMatrix.Right).Length(), Vector3D.Dot(targDir, rotor.WorldMatrix.Right));                  
                double finalAng = angleAway;

                return finalAng;
            }

            public static void SetTurretFacing(IMyTerminalBlock turret, Vector3D dir)
            {
                Vector3D localHeading = Vector3D.Transform(dir, MatrixD.Invert(turret.WorldMatrix.GetOrientation()));
                IMyLargeTurretBase tur = turret as IMyLargeTurretBase;
                double Azimuth;
                double Elevation;
                Vector3D.GetAzimuthAndElevation(localHeading, out Azimuth, out Elevation);
                tur.Azimuth = (float)Azimuth;
                tur.Elevation = (float)Elevation;
                tur.SyncAzimuth();
                tur.SyncElevation();

            }
            public static Vector3D GetTurretFacing(IMyTerminalBlock turret)
            {
                IMyLargeTurretBase tur = turret as IMyLargeTurretBase;
                double Azimuth = tur.Azimuth;
                if (Azimuth > Math.PI) Azimuth -= Math.PI * 2;
                double x = Math.Sin(tur.Elevation + Math.PI / 2) * Math.Sin(Azimuth);
                double z = Math.Sin(tur.Elevation + Math.PI / 2) * Math.Cos(Azimuth);
                double y = Math.Cos(tur.Elevation + Math.PI / 2);

                Vector3D localHeading = new Vector3D(x, y, z);
                return Vector3D.Transform(localHeading, turret.WorldMatrix.GetOrientation());
            }
            public Dictionary<IMyTerminalBlock, ROTORPID> rotors = new Dictionary<IMyTerminalBlock, ROTORPID>();
            public bool PointRotor(IMyTerminalBlock rotor, Vector3D targ)
            {
                double targAng = GetTargetRotorAngle(rotor, targ) * 57.2958;
                if (!rotors.ContainsKey(rotor))
                {
                    rotors.Add(rotor, new ROTORPID(targAng, -10, 10, false));
                }
                rotors[rotor].SetPoint = targAng;
                double currentAngle = (rotor as IMyMotorStator).Angle * 57.2958;
                if (currentAngle > 180) currentAngle -= 360;

                double rpm = rotors[rotor].Compute(currentAngle);
                rotor.SetValueFloat("Velocity", (float)rpm);
                return false;
            }
            public bool PointRotor2(IMyTerminalBlock rotor, double ang, double speed = 10)
            {
                double targAng = ang * 57.2958;

                //if (!rotors.ContainsKey(rotor))   
                //  {   
                //    rotors.Add(rotor, new ROTORPID(targAng, -10, 10, false));   
                // }   

                //rotors[rotor].SetPoint = targAng;   
                double currentAngle = (rotor as IMyMotorStator).Angle * 57.2958;
                if (currentAngle > 180) currentAngle -= 360;
                double deltaAng = targAng - currentAngle;
                if (deltaAng > 180) deltaAng -= 360;
                if (deltaAng < -180) deltaAng += 360;
                double rpm = deltaAng > 0 ? speed : -speed;//rotors[rotor].Compute(currentAngle);   
                if (deltaAng < 0.1 && deltaAng > 0.1)
                {
                    rotor.SetValueFloat("Velocity", 0);
                    // rotors.Remove(rotor);   
                    return true;
                }
                rotor.SetValueFloat("Velocity", (float)rpm);
                return false;
            }
            public bool PointRotor(IMyTerminalBlock rotor, double ang, double speed = 10)
            {
                double targAng = ang * 57.2958;

                if (!rotors.ContainsKey(rotor))
                {
                    rotors.Add(rotor, new ROTORPID(targAng, -speed, speed, false));
                }

                rotors[rotor].SetPoint = targAng;
                double currentAngle = (rotor as IMyMotorStator).Angle * 57.2958;
                if (currentAngle > 180) currentAngle -= 360;
                double deltaAng = targAng - currentAngle;
                if (deltaAng > 180) deltaAng -= 360;
                if (deltaAng < -180) deltaAng += 360;
                double rpm = rotors[rotor].Compute(currentAngle);
                if (rotors[rotor].lastError > -0.1 && rotors[rotor].lastError < 0.1)
                {
                    rotor.SetValueFloat("Velocity", 0);
                    rotors.Remove(rotor);
                    return true;
                }
                rotor.SetValueFloat("Velocity", (float)rpm);
                return false;
            }
            public bool PointRotor(IMyTerminalBlock rotor, Vector3D targ, IMyTerminalBlock reference)
            {
                double targAng = GetTargetRotorAngle(rotor, targ, reference) * 57.2958;
                if (!rotors.ContainsKey(rotor))
                {
                    rotors.Add(rotor, new ROTORPID(targAng, -10, 10, false));
                }
                rotors[rotor].SetPoint = targAng;
                double currentAngle = (rotor as IMyMotorStator).Angle * 57.2958;
                if (currentAngle > 180) currentAngle -= 360;

                double rpm = rotors[rotor].Compute(currentAngle);
                rotor.SetValueFloat("Velocity", (float)rpm);
                return false;
            }
            public Vector3D localCoM = default(Vector3D);
            double getvelratio(PID pid, double velUp)
            {
                double test = 1 - velUp / pid.SetPoint;
                if (test < 0) test = Math.Abs(test * 2);
                return test;
            }
            double getothervelratio(PID pid, Vector3D uh, Vector3D dir, double velUp, double MaxVel)
            {
                double test = (1 - velUp / (MaxVel * Vector3D.Dot(remote.WorldMatrix.Up, dir)));
                if (test < 0) test = Math.Abs(test * 2);
                return test;
            }
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
            public void StoreDataInScreen(String Name)
            {
                IMyTextPanel panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(Name);
                panel.WritePublicTitle(localCoM + "," + planet.Center);
            }
            public void GetDataFromScreen(String Name)
            {
                IMyTextPanel panel = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(Name);
                String data = panel.GetPublicTitle();
                String[] datas = data.Split(",".ToCharArray());
                if (Vector3D.TryParse(datas[0], out localCoM)) localCoM = new Vector3D(0, 0, 0);
                Vector3D.TryParse(datas[1], out planet.Center);
            }
            public String GetDisplayString()
            {
                double dist = Ship.SphericalDist(remote.GetPosition(), Target, planet.Center, remote.GetPosition().Length());
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
                    var items = (reactors[i]).GetInventory(0).GetItems();
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
                //Me.Echo(blocks.Count);
                upThrusts = Where(blocks, thrust =>
                {
                    // Me.Echo((thrust.WorldMatrix.Backward == remote.WorldMatrix.Up).ToString());
                    return thrust.WorldMatrix.Forward == remote.WorldMatrix.Up && (thrust as IMyFunctionalBlock).Enabled;
                });
                downThrusts = Where(blocks, thrust => thrust.WorldMatrix.Forward == remote.WorldMatrix.Down && (thrust as IMyFunctionalBlock).Enabled);
                leftThrusts = Where(blocks, thrust => thrust.WorldMatrix.Forward == remote.WorldMatrix.Left && (thrust as IMyFunctionalBlock).Enabled);
                rightThrusts = Where(blocks, thrust => thrust.WorldMatrix.Forward == remote.WorldMatrix.Right && (thrust as IMyFunctionalBlock).Enabled);
                forThrusts = Where(blocks, thrust => thrust.WorldMatrix.Forward == remote.WorldMatrix.Forward && (thrust as IMyFunctionalBlock).Enabled);
                backThrusts = Where(blocks, thrust => thrust.WorldMatrix.Forward == remote.WorldMatrix.Backward && (thrust as IMyFunctionalBlock).Enabled);
                //  CalculateForce(upThrusts, out upThrust);
                // CalculateForce(downThrusts, out downThrust);
                //CalculateForce(leftThrusts, out leftThrust);
                // CalculateForce(rightThrusts, out rightThrust);
                //CalculateForce(forThrusts, out forThrust);
                // CalculateForce(backThrusts, out backThrust);
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
            public bool FlyTo(Vector3D target)
            {
                Vector3D gravity = new Vector3D(0, 0, 0);
                if ((remote as IMyRemoteControl).GetNaturalGravity() != null)
                {
                    gravity = (remote as IMyRemoteControl).GetNaturalGravity();
                }

                var remote1 = ((IMyShipController)remote).CalculateShipMass();
                float mass = (remote1.BaseMass + (remote1.TotalMass - remote1.BaseMass) / Multiplier);
                Vector3D force = ((target - remote.GetPosition()) - 5 * ((IMyShipController)remote).GetShipVelocities().LinearVelocity - gravity) * mass;
                if (((target - remote.GetPosition()).Length() < 0.1))
                {
                    thrust(new Vector3D(0));
                    return true;
                }
                Vector3D Local = Vector3D.Transform(force, MatrixD.Invert(remote.WorldMatrix.GetOrientation()));
                Me.Echo(Local.ToString());
                //Me.Echo(Local.ToString());
                thrust(Local);
                return false;
            }
            public void MaintainVelocity(Vector3D target)
            {
                Vector3D gravity = new Vector3D(0, 0, 0);
                if ((remote as IMyRemoteControl).GetNaturalGravity() != null)
                {
                    gravity = (remote as IMyRemoteControl).GetNaturalGravity();
                }

                var remote1 = ((IMyShipController)remote).CalculateShipMass();
                float mass = (remote1.BaseMass + (remote1.TotalMass - remote1.BaseMass) / Multiplier);
                Vector3D force = ((target) - ((IMyShipController)remote).GetShipVelocities().LinearVelocity - gravity) * mass;

                Vector3D Local = Vector3D.Transform(force, MatrixD.Invert(remote.WorldMatrix.GetOrientation()));
                Me.Echo(Local.ToString());
                //Me.Echo(Local.ToString());
                thrust(Local);

            }
            public void thrust(Vector3D dir)
            {
                if (dir.X < 0)
                {
                    ThrustNewtons(rightThrusts, (float)-dir.X);
                    ThrustNewtons(leftThrusts, 0);
                }
                else if (dir.X > 0)
                {
                    ThrustNewtons(leftThrusts, (float)dir.X);
                    ThrustNewtons(rightThrusts, 0);
                }
                else
                {
                    ThrustNewtons(leftThrusts, 0);
                    ThrustNewtons(rightThrusts, 0);
                }
                if (dir.Y < 0)
                {
                    ThrustNewtons(upThrusts, (float)-dir.Y);
                    ThrustNewtons(downThrusts, 0);
                }
                else if (dir.Y > 0)
                {
                    ThrustNewtons(downThrusts, (float)dir.Y);
                    ThrustNewtons(upThrusts, 0);
                }
                else
                {
                    ThrustNewtons(upThrusts, 0);
                    ThrustNewtons(downThrusts, 0);
                }
                if (dir.Z < 0)
                {
                    ThrustNewtons(backThrusts, (float)-dir.Z);
                    ThrustNewtons(forThrusts, 0);
                }
                else if (dir.Z > 0)
                {
                    ThrustNewtons(forThrusts, (float)dir.Z);
                    ThrustNewtons(backThrusts, 0);
                }
                else
                {
                    ThrustNewtons(forThrusts, 0);
                    ThrustNewtons(backThrusts, 0);
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
            public void SetValueNewtons(List<IMyTerminalBlock> blocks, string key, float value)
            {
                blocks.ForEach((block) =>
                {
                    float thrust = (float)(GetThrust(block) == largeAtmoThrust || GetThrust(block) == smallAtmoThrust || GetThrust(block) == smalllargeAtmoThrust || GetThrust(block) == smallsmallAtmoThrust ? GetThrust(block) * planet.GetAtmosphericThrusterCoefficient(Vector3D.Distance(remote.GetPosition(), planet.Center)) : GetThrust(block));
                    float Override = value * 100 / (blocks.Count * thrust);
                    block.SetValueFloat(key, Override);
                });
            }
            public Dictionary<IMyTerminalBlock, float> maxThrusts = new Dictionary<IMyTerminalBlock, float>();
            public float CalculateMaxThrust(List<IMyTerminalBlock> input)
            {
                float maxthrust = 0;
                foreach (IMyTerminalBlock block in input)
                {
                    IMyThrust thrust = block as IMyThrust;
                    float currentThrust = thrust.MaxThrust;
                    if (maxThrusts.ContainsKey(block)) {
                        maxThrusts[block] = currentThrust;

                    }
                    else
                    {
                        maxThrusts.Add(block, currentThrust);
                    }
                    maxthrust += currentThrust;
                    //Me.Echo(currentThrust.ToString());
                }
                //Me.Echo("Somethings wrong :/ "+maxthrust);
                return maxthrust;
            }
            public void ThrustNewtons(List<IMyTerminalBlock> inputs, float newtons)
            {

                //var transformedNewtons = Vector3D.Transform(newtons,orientationMatrix);
                //Me.Echo(inputs.Count.ToString());
                float MaxNewtons = CalculateMaxThrust(inputs);
                if (MaxNewtons == 0)
                {
                    MaxNewtons = 1;
                }
                foreach (IMyTerminalBlock block in inputs)
                {
                    IMyThrust thruster = block as IMyThrust;
                    if (MaxNewtons < newtons)
                    {
                        block.SetValueFloat("Override", 100);
                        //Me.Echo("I hate you! More! "+MaxNewtons+" "+newtons+" "+GetThrust(block));
                    }
                    else
                    {

                        float currentMaxThrust = maxThrusts[block];
                        if (currentMaxThrust == -1)
                        {
                            block.SetValueFloat("Override", 100);
                            //Me.Echo("I hate you!");
                        }
                        else
                        {
                            float ratio = currentMaxThrust / MaxNewtons;
                            float thrustPercentage = 0;
                            if (currentMaxThrust != 0)
                            {
                                thrustPercentage = (newtons / currentMaxThrust) * ratio * 100.0f;
                            }
                            if (thrustPercentage != 0 && thrustPercentage < 1)
                            {
                                // Me.Echo(thrustPercentage.ToString());
                                // thrustPercentage = 2f;
                            }
                            // Me.Echo(thrustPercentage.ToString());
                            block.SetValueFloat("Override", Math.Abs(thrustPercentage));
                        }
                    }
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
                    thrust(new Vector3D(0, 0, 0));
                    return true;
                }
                // Me.Echo(mod.ToString() + remVel.Length().ToString());                       
                thrust(localremVel * mod);
                return false;
            }
            //Credit to MMaster                                   
            public double GetMassSummary()
            {
                List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocks(Blocks);
                double used = 0;
                for (int i = 0; i < Blocks.Count; i++)
                {
                    for (int invId = 0; invId < 2; invId++)
                    {
                        var inv = Blocks[i].GetInventory(invId);
                        if (inv == null)
                            continue;

                        used += (double)inv.CurrentMass;
                    }
                }
                return used * 1000;
            }
            public double GetMassTemp()
            {
                return Mass + GetMassSummary();
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

                    vels = currentVelocity;

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

                lastPos = remote.GetPosition();
                Me = pro;
                PopulateThrusters();
                // GetStoredInScreen(Name);          
            }
            public bool SetForwardVelocity(double thrustLimit)
            {
                Vector3D vel = currentVelocity;
                //Me.Echo(vel.Length().ToString());                     
                if ((thrustLimit - vel.Length()) < 0.1 && (thrustLimit - vel.Length()) >= 0)
                {
                    if (!topKek)
                    {
                        thrust(new Vector3D(0, 0, 0.01f));
                        topKek = true;
                    }


                    return true;
                }
                else
                {
                    topKek = false;
                    if ((thrustLimit - vel.Length()) < 0)
                    {
                        thrust(new Vector3D(0, 0, 1));
                        return false;
                    }
                }
                thrust(new Vector3D(0, 0, -1));
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
        public struct PID
        {
            public float PropGains;
            public float IntGains;
            public float DerGains;
            public double SetPoint, Output, lastInput;
            public double ErrorSum;
            public double lastError;
            public double min, max;
            bool IsFirst;
            bool IsPaused;
            public double Error;
            public PID(double SetPoint, double min, double max)
            {
                this.SetPoint = SetPoint;
                this.min = min;
                this.max = max;
                ErrorSum = 0;
                lastError = 0;
                Output = 0;
                lastInput = 0;
                PropGains = 2;
                IntGains = 1f;
                DerGains = 2;
                IsPaused = false;
                IsFirst = true;
                Error = 0;
            }
            public double Compute(double input)
            {
                if (IsFirst || IsPaused)
                {
                    lastInput = input;
                    ErrorSum = Output;
                    if (ErrorSum > max) ErrorSum = max;
                    else if (ErrorSum < min) ErrorSum = min;
                    IsFirst = false;
                    if (IsPaused) return 0;
                }
                Error = SetPoint - input;
                double error = Error;
                ErrorSum += error * IntGains;
                if (ErrorSum > max) ErrorSum = max;
                else if (ErrorSum < min) ErrorSum = min;
                double dError = input - lastInput;
                Output = PropGains * error + ErrorSum - DerGains * dError;
                if (Output > max) Output = max;
                else if (Output < min) Output = min;
                lastError = error;
                lastInput = input;
                return Output;
            }
        }
        public class ROTORPID
        {
            public float PropGains;
            public float IntGains;
            public float DerGains;
            public double SetPoint, Output, lastInput;
            public double ErrorSum;
            public double lastError;
            public double min, max;
            bool IsFirst;
            bool IsPaused;
            bool Radians;
            public ROTORPID(double SetPoint, double min, double max, bool Radians)
            {
                this.SetPoint = SetPoint;
                this.min = min;
                this.max = max;
                ErrorSum = 0;
                lastError = 0;
                Output = 0;
                lastInput = 0;
                PropGains = 1;
                IntGains = 0.000f;
                DerGains = 0;
                IsPaused = false;
                IsFirst = true;
                this.Radians = Radians;
            }
            public double Compute(double input)
            {
                if (IsFirst || IsPaused)
                {
                    lastInput = input;
                    ErrorSum = Output;
                    if (ErrorSum > max) ErrorSum = max;
                    else if (ErrorSum < min) ErrorSum = min;
                    IsFirst = false;
                    if (IsPaused) return 0;
                }
                double error = SetPoint - input;
                if (Radians)
                {
                    if (error > Math.PI) error -= 2 * Math.PI;
                    if (error < -Math.PI) error += 2 * Math.PI;
                }
                else
                {
                    if (error > 180) error -= 360;
                    if (error < -180) error += 360;
                }

                ErrorSum += error * IntGains;
                if (ErrorSum > max) ErrorSum = max;
                else if (ErrorSum < min) ErrorSum = min;
                double dError = input - lastInput;
                Output = PropGains * error + ErrorSum - DerGains * dError;
                if (Output > max) Output = max;
                else if (Output < min) Output = min;
                lastError = error;
                lastInput = input;
                return Output;
            }
        }
        public class ArmController
        {
            public Ship ship;
            public IMyMotorStator shoulderYaw;
            public IMyMotorStator shoulderPitch;
            public IMyMotorStator Elbow;
            public IMyMotorStator wristPitch;
            public IMyMotorStator wristYaw;
            double sy_angle = 0;
            public double headHeight = 2 * 2.5;
            public double headLength = 3 * 2.5;
            double sp_angle = 0;
            double wp_angle = 0;
            double wy_angle = 0;
            double e_angle = 0;
            public double extensionOffsetAngle = 0;
            public bool wristYawIsLocal = false;
            double armLength = 5 * 2.5;
            bool init = true;
            public ArmController(Ship ship, String sy_name, String sp_name, String elbow_name, String wp_name, String wy_name, bool wristyawlocal = false)
            {
                this.ship = ship;
                this.shoulderYaw = (IMyMotorStator)ship.GridTerminalSystem.GetBlockWithName(sy_name);
                this.shoulderPitch = (IMyMotorStator)ship.GridTerminalSystem.GetBlockWithName(sp_name);
                this.Elbow = (IMyMotorStator)ship.GridTerminalSystem.GetBlockWithName(elbow_name);
                this.wristPitch = (IMyMotorStator)ship.GridTerminalSystem.GetBlockWithName(wp_name);
                this.wristYaw = (IMyMotorStator)ship.GridTerminalSystem.GetBlockWithName(wy_name);
                this.wristYawIsLocal = wristyawlocal;
                ship.Me.Echo("Finished init controller " + (bool)(shoulderPitch == null));
            }
            public ArmAngles GetCurrentState()
            {
                double C = (2 * armLength * armLength) - 2 * armLength * armLength * Math.Cos(Elbow.Angle);
                double deltaAng = shoulderPitch.Angle - (Math.PI + Elbow.Angle) / 2;
                return new ArmAngles(deltaAng, Elbow.Angle, shoulderYaw.Angle, shoulderPitch.Angle, wristYaw.Angle, wristPitch.Angle);
            }
            public Vector3D FindPos(out double WristPitch, out double WristYaw)
            {
                double elbow = -Elbow.Angle + Math.PI * 2;
                double C = Math.Sqrt((2 * armLength * armLength) - (2 * armLength * armLength * Math.Cos(elbow)));
                double deltaAng = shoulderPitch.Angle - (Math.PI - elbow) / 2;
                double WristPitch2 = (-wristPitch.Angle + Math.PI) - (shoulderPitch.Angle + elbow);
                WristPitch = WristPitch2 + Math.PI / 2;
                WristYaw = shoulderYaw.Angle - wristYaw.Angle;
                double WristPitch1 = WristPitch2 + extensionOffsetAngle;
                double wristOffset_x = Math.Cos(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Cos(WristYaw) * Math.Cos(WristPitch2) * headHeight;
                double wristOffset_z = Math.Sin(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Sin(WristYaw) * Math.Cos(WristPitch2) * headHeight;
                double wristOffset_y = Math.Sin(WristPitch1) * headLength + Math.Sin(WristPitch2) * headHeight;
                Vector3D output = new Vector3D(0, 0, 0);
                output.X = (Math.Cos(-shoulderYaw.Angle) * Math.Cos(deltaAng) * C) + wristOffset_x;
                output.Z = -(Math.Sin(-shoulderYaw.Angle) * Math.Cos(deltaAng) * C) + wristOffset_z;
                output.Y = Math.Sin(deltaAng) * C + wristOffset_y;
                ship.Me.Echo("stiff" + deltaAng * 57 + " " + C + " " + WristPitch * 57 + " " + Elbow.Angle * 57);
                return Vector3D.Transform(output, shoulderYaw.WorldMatrix);
            }
            public Vector3D FindPos(ArmAngles ang)
            {
                double C = Math.Sqrt((2 * armLength * armLength) - (2 * armLength * armLength * Math.Cos(ang.elbowAng - Math.PI)));
                double deltaAng = ang.deltaAng;
                double WristPitch = ang.wristPitchAng + ang.shoulderPitchAng - ang.elbowAng - Math.PI / 2;
                double WristYaw = ang.wristYawAng - ang.shoulderYawAng;
                double WristPitch1 = WristPitch + extensionOffsetAngle;
                double wristOffset_x = Math.Cos(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Cos(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_z = Math.Sin(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Sin(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_y = Math.Sin(WristPitch1) * headLength + Math.Sin(WristPitch) * headHeight;
                Vector3D output = new Vector3D(0, 0, 0);
                output.X = Math.Cos(-ang.shoulderYawAng) * Math.Cos(deltaAng) * C + wristOffset_x;
                output.Z = Math.Sin(-ang.shoulderYawAng) * Math.Cos(deltaAng) * C + wristOffset_z;
                output.Y = Math.Sin(deltaAng) * C + wristOffset_y;
                return output;
            }
            public void FindAngles(Vector3D worldtarg, double WristPitch, double WristYaw)
            {

                Vector3D tar = Vector3D.Transform(worldtarg, MatrixD.Invert(shoulderYaw.WorldMatrix));
                tar.Z = -tar.Z;
                double WristPitch1 = WristPitch + extensionOffsetAngle;
                double wristOffset_x = Math.Cos(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Cos(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_z = Math.Sin(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Sin(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_y = Math.Sin(WristPitch1) * headLength + Math.Sin(WristPitch) * headHeight;
                tar.X -= wristOffset_x;
                tar.Z -= wristOffset_z;
                tar.Y -= wristOffset_y;

                sy_angle = -Math.Atan2(tar.Z, tar.X);
                wy_angle = wristYawIsLocal ? WristYaw : sy_angle + WristYaw;

                double newX = Math.Sqrt(Math.Pow(tar.X, 2) + Math.Pow(tar.Z, 2));
                double c = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(tar.Y, 2));
                e_angle = -Math.Acos((2 * armLength * armLength - c * c) / (2 * armLength * armLength));

                double deltaAng = Math.Atan(tar.Y / newX);
                sp_angle = (Math.PI + e_angle) / 2 + deltaAng;

                wp_angle = -(sp_angle - e_angle - WristPitch);
                if (double.IsNaN(e_angle)) { e_angle = 3.14; sp_angle = deltaAng; wp_angle = 3.14; }
                ship.Me.Echo("Delta Ang: " + deltaAng * 57); ship.Me.Echo("wrist yaw: " + wp_angle);
                ship.Me.Echo(tar.ToString() + " " + e_angle * 57 + " " + sy_angle * 57 + " " + wp_angle * 57 + " " + deltaAng * 57);
            }
            public ArmAngles FindAnglesState(Vector3D worldtarg, double WristPitch, double WristYaw)
            {

                Vector3D tar = Vector3D.Transform(worldtarg, MatrixD.Invert(shoulderYaw.WorldMatrix));
                tar.Z = -tar.Z;
                double WristPitch1 = WristPitch + extensionOffsetAngle;
                double wristOffset_x = Math.Cos(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Cos(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_z = Math.Sin(WristYaw) * Math.Cos(WristPitch1) * headLength + Math.Sin(WristYaw) * Math.Cos(WristPitch) * headHeight;
                double wristOffset_y = Math.Sin(WristPitch1) * headLength + Math.Sin(WristPitch) * headHeight;
                tar.X -= wristOffset_x;
                tar.Z -= wristOffset_z;
                tar.Y -= wristOffset_y - 1;

                double sy_angle = -Math.Atan2(tar.Z, tar.X);
                double wy_angle = wristYawIsLocal ? WristYaw : sy_angle + WristYaw;

                double newX = Math.Sqrt(Math.Pow(tar.X, 2) + Math.Pow(tar.Z, 2));
                double c = Math.Sqrt(Math.Pow(newX, 2) + Math.Pow(tar.Y, 2));
                double e_angle = -Math.Acos((2 * armLength * armLength - c * c) / (2 * armLength * armLength));
                double deltaAng = Math.Atan(tar.Y / newX);
                double sp_angle = (Math.PI + e_angle) / 2 + deltaAng;

                double wp_angle = -(sp_angle - e_angle - WristPitch);
                return new ArmAngles(deltaAng, e_angle, sy_angle, sp_angle, wy_angle, wp_angle);

                //ship.Me.Echo(tar.Length() + " " + e_angle * 57 + " " + sy_angle * 57 + " " + wp_angle * 57 + " " + deltaAng * 57);      
            }
            public bool setArm(Vector3D targ, double WristPitch, double WristYaw, double speed = 10)
            {
                double WristPitchnew = WristPitch + -90;
                //ship.Me.Echo("set angles");         
                FindAngles(targ, WristPitchnew * (Math.PI / 180), WristYaw * (Math.PI / 180));

                bool a = ship.PointRotor(shoulderPitch, sp_angle, speed);
                bool b = ship.PointRotor(shoulderYaw, sy_angle, speed);
                bool c = ship.PointRotor(Elbow, e_angle, speed);
                bool d = ship.PointRotor(wristPitch, wp_angle, speed);
                bool e = ship.PointRotor(wristYaw, wy_angle, speed);
                if (a && b && c && d && e) return true;
                else return false;


            }
        }
        public struct ArmAngles
        {
            public double deltaAng;
            public double elbowAng;
            public double shoulderYawAng;
            public double shoulderPitchAng;
            public double wristYawAng;
            public double wristPitchAng;
            public ArmAngles(double deltaAng, double elbowAng, double shoulderYawAng, double shoulderPitchAng, double wristYawAng, double wristPitchAng)
            {
                this.deltaAng = deltaAng;
                this.elbowAng = elbowAng;
                this.shoulderYawAng = shoulderYawAng;
                this.shoulderPitchAng = shoulderPitchAng;
                this.wristYawAng = wristYawAng;
                this.wristPitchAng = wristPitchAng;
            }
        }
        public class InterpOperation
        {
            public ArmController arm;
            public double stepSize = 0.3;
            public double InterpState = 0;
            public double stepAmount;
            public ArmAngles EndState;
            public ArmAngles CurrentState;
            public Vector3D CurrentPos;
            public Vector3D EndPos;
            public double EndWristPitch;
            public double EndWristYaw;
            public double StartWristPitch;
            public double StartWristYaw;
            public bool initialized = false;
            public InterpOperation(ArmController arm, Vector3D worldtarg, double WristPitch, double WristYaw)
            {

                this.arm = arm;
                CurrentState = arm.GetCurrentState();
                EndState = arm.FindAnglesState(worldtarg, WristPitch, WristYaw);
                EndPos = worldtarg;
                CurrentPos = arm.FindPos(out StartWristPitch, out StartWristYaw);
                //CurrentPos += arm.wristYaw.WorldMatrix.Up * arm.headHeight;   
                stepAmount = stepSize / Vector3D.Distance(CurrentPos, EndPos);
                // StartWristPitch = arm.wristPitch.Angle + arm.shoulderPitch.Angle - arm.Elbow.Angle;

                // StartWristYaw = arm.wristYaw.Angle - arm.shoulderYaw.Angle;

                EndWristPitch = WristPitch;
                EndWristYaw = WristYaw;
            }
            public void Init()
            {
                CurrentPos = arm.FindPos(out StartWristPitch, out StartWristYaw);
                stepAmount = stepSize / Vector3D.Distance(CurrentPos, EndPos);
                initialized = true;
            }
            public double lerp(double a, double b, double x)
            {
                return a + (b - a) * (x);
            }
            public Vector3D lerp(Vector3D a, Vector3D b, double x)
            {
                return a + (b - a) * (x);
            }
            public bool Tick()
            {

                bool ready = arm.setArm(CurrentPos, lerp(StartWristPitch, EndWristPitch, InterpState), lerp(StartWristYaw, EndWristYaw, InterpState), 30);
                CurrentPos = lerp(CurrentPos, EndPos, stepAmount);
                if (ready)
                {
                    if (Vector3D.Distance(CurrentPos, EndPos) < 0.05)
                    {
                        return true;
                    }
                    //InterpState+=stepAmount;
                    //stepAmount=stepSize/Vector3D.Distance(CurrentPos, EndPos);


                }
                return false;
            }
        }
        public static class InteropManager
        {
            public static List<InterpOperation> queue = new List<InterpOperation>();
            public static bool Tick()
            {
                // for (int i = 0; i < queue.Count; i++)
                //{
                if (queue.Count == 0) return false;
                if (!queue[0].initialized)
                {
                    queue[0].Init();
                }
                if (queue[0].Tick())
                {
                    queue.Remove(queue[0]);
                    return true;

                }

                return false;
                // }
            }
            public static void Add(InterpOperation x)
            {
                if (queue.Contains(x))
                {
                    return;
                }
                else
                {
                    queue.Add(x);
                }
                
                
            }
        }
        #endregion
        // Code o Heidenhain Klartext Language (CNC milling machine control) https://youtu.be/ER54WS7_Rsw?t=46  http://www.heidenhain.co.uk/en_UK/products-and-applications/cnc-controls/itnc-530/ 

        
        
    }
}
