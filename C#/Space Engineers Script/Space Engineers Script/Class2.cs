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
    class Class2 : MyGridProgram
    {
        // Code Input similar to Heidenhain Klartext Language (CNC milling machine control) https://youtu.be/ER54WS7_Rsw?t=46  http://www.heidenhain.co.uk/en_UK/products-and-applications/cnc-controls/itnc-530/ 

        long Zykluszeit = 25;               // Refresh timer in ms (should be in range of 25 - 100) 

        double Feed = 100;                    //Movement Speed. 10 = (1 Large Block)/s 

        double Gain = 1.1;                     //Gain of P Control Loop. (Motor Slow Down, higher is faster, should not be over 2.5) 

        double SpeedUpMult = 1.2;        // Path velocity Speed up multiplicator (must be > 1, higher is faster) 

        double PrecisionRadius = 0.04; // Radius of Ist to End Shpere where movement is stopped and 
        // move is considered finished 

        double MaxTurnSpeed = 30;   // will lower feed if any rotors calculated turning rate exceeds this value 

        double ToolDir = -1;            // -1 = Tool on bottom side (default), 1 = Tool on upper side (for arm hanging down) 

        double segmentlength = 60; // Length of arm in large blocks (1 block = 10 units, from rotor axis to rotor axis) 
        double ToolLength = 0;  // Length of wrist attachment from AR5WristTop axis to tip of attachment 
        double ToolHeight = 0;  // Height of attachment starting from AR5WristTop Rotor minus 10 units 
        // (0 is same height as AR1BaseTop Rotor) 

        double ManualStep = 10;           // Initial Step Size Setting for Manual operation 

        // #################### Blocks that need to be built and named like below ####################### 

        // Rotors on arm 
        string AR1BaseTopName = "AR1"; // BaseTop Rotor 
        string AR2BasePoleName = "AR2"; // BasePole Rotor 
        string AR3PoleName = "AR3"; // Pole Rotor 
        string AR4PoleWristName = "AR4"; // PoleWrist Rotor 
        string AR5WristTopName = "AR5"; // WristTop Rotor 

        // Timer Block for main control loop 
        string TBName = "TB IK Arm";

        // #################### Optional Blocks ####################### 

        //Lighting Block for moving indication (for easy Automation usage) 
        string ILName = "IL Arm";

        // Main Screen LCD Display (for Coordinate Display) 
        string MscreenName = "MainScreen";

        // Feed Screen LCD Display (for big display of Feed and Step Size) 
        string FscreenName = "FeedScreen";

        // easy Automation LCD Screen, Write Saved Positions to public screen (for SavePos command) 
        string AutScreenName = "easyAuto Screen";

        // Additional controlled rotor axis B (used for tool swifel axis) 
        string ARBAxisName = "ARB";
        string ARBTipName = "ARBTip"; // New Tip Rotor for attaching Tools with command "Attach/Detach Tip" (normally AR5 without Adapter) 


        // Additional controlled rotor axis C (used for indexing table) 
        string ARCAxisName = "ARC";

        // #################################################################################################### 




        //~ ####   Global Variables   #### 

        IMyTerminalBlock AR1BaseTop;
        IMyTerminalBlock AR2BasePole;
        IMyTerminalBlock AR3Pole;
        IMyTerminalBlock AR4PoleWrist;
        IMyTerminalBlock AR5WristTop;

        IMyTerminalBlock ARB;
        IMyTerminalBlock ARBTip;
        IMyTerminalBlock ARC;

        IMyTimerBlock TB;
        IMyLightingBlock IL;
        IMyTextPanel Mscreen, Fscreen, AutScreen;


        double BaseTopAng, WristTopAng, PoleAng, PoleWristAng, BasePoleAng;

        double MxStart, MyStart, MzStart; // Start Pos of Iteration (Start position) 
        double MxIst, MyIst, MzIst;          // Current Pos 
        double MxEnd, MyEnd, MzEnd;      // iterate = 1 --> Final Pos of Iteration 
        // iterate = 0 --> Target Pos 

        double MaStart, MaEnd; // wristangle interpolation vars 
        double MbStart, MbEnd; // B Axis 
        double McStart, McEnd; // C Axis 

        double ang1; // Ist Angles 
        double ang2;
        double ang3;
        double ang4;
        double ang5;

        double angB;
        double angC;


        decimal EndPosReached;

        double MxEndOrg, MyEndOrg, MzEndOrg; // Original Start position of argument (for backtracking when rotation is active) 

        string CurrentLine;
        string DebugLine;

        decimal i = 0;
        double IstToEndInterpol;
        double IstToEndInterpolIter;
        double GlobalTurnSpeedLimitFactor = 1;

        double EndGain = 1.1;                     // Final applied Gain after multiplying with dampening factor 

        double ManualDir = 1;
        double pi = Math.PI;

        DateTime TargetTime;

        public struct shift
        {
            public double x;
            public double y;
            public double z;
        };

        shift[] Shift = new shift[10]; // Create Datum Shift(Zero Offset) Array 
        int sNr = 0; // Current Datum Shift Number 

        shift Rot = new shift { x = 0, y = 0, z = 0 };

        //~ //// ######################      MAIN       ########################### 

        void Main(string argument)
        {
            argument = argument.Trim();
            if (i == 0)
            {
                Init();
                i++;
                MainScreen("Initialising finished.\nIf you see this, the Programming Block has been called\nwithout a parameter or the Timer Block is not setup correctly.\nIn this case you have to setup the IK Arm Timer Block to\nRun the IK Arm Programming Block and Trigger-Now itself.", false);
                if (argument == "")
                {
                    TB.ApplyAction("TriggerNow");
                    return;
                }
            }
            // ##### Reinit stuff ##### 
            if (argument == "")
            {

            }
            else if (argument == "timer")
            {
                MainScreen(i.ToString(), true);
                MainScreen(ElapsedTime.ToString(), true);
                i = 0;
            }
            else if (argument == "B On")
            {
                ARB = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARBAxisName); // B Axis 
                ARBTip = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARBTipName); // B Axis Tip 

                EndPosReached = 0;
            }
            else if (argument == "B Off")
            {
                ARB = null; // B Axis 
                ARBTip = null; // B Axis Tip 
                EndPosReached = 0;
            }
            else if (argument == "C On")
            {
                ARC = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARCAxisName); // C Axis 
                EndPosReached = 0;
            }
            else if (argument == "C Off")
            {
                ARC = null; // C Axis 
                EndPosReached = 0;
            }
            else if (argument == "Attach Tip")
            {
                if (ARBTip != null)
                {
                    ARBTip.ApplyAction("Attach");
                }
                else
                {
                    AR5WristTop.ApplyAction("Attach");
                }
            }
            else if (argument == "Detach Tip")
            {
                if (ARBTip != null)
                {
                    ARBTip.ApplyAction("Detach");
                }
                else
                {
                    AR5WristTop.ApplyAction("Detach");
                }
            }
            else
            {
                Debug("Current MxEnd: " + r2s(MxEnd) + ", MyEnd: " + r2s(MyEnd) + ", MzEnd: " + r2s(MzEnd), true);
                Debug("Restoring to MxEndOrg: " + r2s(MxEndOrg) + ", MyEndOrg: " + r2s(MyEndOrg) + ", MzEndOrg: " + r2s(MzEndOrg), true);

                MxEnd = MxEndOrg; MyEnd = MyEndOrg; MzEnd = MzEndOrg; // Restore original coord values before rotation 

                string[] ArgArray = argument.Split(' ');
                for (int z = 0; z < ArgArray.Length; z++)
                {
                    if (ArgArray[z] == "Stop")
                    {
                        EndPosReached = 1;
                        Stop();
                        MxStart = MxEnd;
                        MyStart = MyEnd;
                        MzStart = MzEnd;
                        MaStart = MaEnd;
                        MainScreen("Emergency Stop. Ende.", true);
                        if (IL != null) { IL.ApplyAction("OnOff_Off"); }
                        i = 1;
                        return;

                    }
                    else if (ArgArray[z] == "SavePos")
                    { // Write current position to easyAuto LCD public Screen 
                        if (AutScreen != null)
                        {
                            //AutScreen.WritePublicText("TryRun "+Me.CustomName+" \""+SaveOutp(MxEnd)+" "+SaveOutp(MyEnd)+" "+SaveOutp(MzEnd)+" "+SaveOutp(MaEnd) + "\"\ncheck\n", true); 
                            AutScreen.WritePublicText("L " + SaveOutp(MxEnd) + " " + SaveOutp(MyEnd) + " " + SaveOutp(MzEnd) + " " + SaveOutp(MaEnd) + "\n", true);
                            break;
                        }
                    }
                    bool isLetter1 = !String.IsNullOrEmpty(ArgArray[z]) && Char.IsLetter(ArgArray[z][0]); // Check if this command is something and first char is a Letter 
                    string com = "";
                    string value = ArgArray[z];
                    double value_s;
                    if (isLetter1)
                    {
                        bool isLetter2 = ArgArray[z].Length > 1 && Char.IsLetter(ArgArray[z][1]); // Check if second char is a letter 
                        bool isLetter3 = ArgArray[z].Length > 2 && Char.IsLetter(ArgArray[z][2]); // Check if third char is a letter 
                        int comlength = ((isLetter2) ? 1 : 0) + ((isLetter3) ? 1 : 0); // Calculate extra letters 

                        //~ MainScreen("ArgArray[z]: "+ArgArray[z],true); 
                        //~ MainScreen("isLetter2: "+isLetter2.ToString(),true); 
                        //~ MainScreen("isLetter3: "+isLetter3.ToString(),true); 
                        //~ MainScreen("comlength: "+comlength.ToString(),true); 

                        com = ArgArray[z].Substring(0, 1 + comlength); // Extract Command 
                        value = ArgArray[z].Remove(0, 1 + comlength); // The remaining must be the value 
                        MainScreen("value:" + value, true);

                        if (value == "+" || value == "-")
                        { // Manual Operations 
                            ManualDir = -1;
                            if (value == "+") { ManualDir = 1; }
                            if (com == "X") { MxEnd = MxEnd + ManualDir * ManualStep; }
                            else if (com == "Y") { MyEnd = MyEnd + ManualDir * ManualStep; }
                            else if (com == "Z") { MzEnd = MzEnd + ManualDir * ManualStep; }
                            else if (com == "A") { MaEnd = SmartAngle(MaEnd + ManualDir * ManualStep); }
                            else if (com == "B") { MbEnd = SmartAngle(MbEnd + ManualDir * ManualStep); }
                            else if (com == "C") { McEnd = SmartAngle(McEnd + ManualDir * ManualStep); }
                            else if (com == "S")
                            {
                                if (value == "+")
                                {
                                    ManualStep += 1;
                                }
                                else if (ManualStep > 1)
                                {
                                    ManualStep -= 1;
                                }
                            }
                            else if (com == "F")
                            {
                                if (value == "+")
                                {
                                    Feed += 10;
                                }
                                else if (Feed > 10)
                                {
                                    Feed -= 10;
                                }
                            }
                            else { break; }

                            EndPosReached = 0;

                        }
                        else
                        { // Goto Position Line 


                            value_s = Convert.ToDouble(value);
                            if (com == "X") { MxEnd = value_s + Shift[sNr].x; }
                            else if (com == "Y") { MyEnd = value_s + Shift[sNr].y; }
                            else if (com == "Z") { MzEnd = value_s + Shift[sNr].z; }
                            else if (com == "A") { MaEnd = SmartAngle(value_s); }
                            else if (com == "B") { MbEnd = SmartAngle(value_s); }
                            else if (com == "C") { McEnd = SmartAngle(value_s); }
                            else if (com == "IX") { MxEnd += value_s; }
                            else if (com == "IY") { MyEnd += value_s; }
                            else if (com == "IZ") { MzEnd += value_s; }
                            else if (com == "IA") { MaEnd = SmartAngle(MaEnd + value_s); }
                            else if (com == "SL") { segmentlength = value_s; Recalculate(); }
                            else if (com == "TL") { ToolLength = value_s; Recalculate(); continue; }
                            else if (com == "TH") { ToolHeight = value_s; Recalculate(); continue; }
                            else if (com == "TD") { ToolDir = value_s; }
                            else if (com == "F") { Feed = value_s; } // Feed 
                            else if (com == "S") { ManualStep = value_s; } // Step 
                            else if (com == "G") { Gain = value_s; } // Gain 
                            else if (com == "SU") { SpeedUpMult = value_s; } // SpeedUpMult 
                            else if (com == "PR") { PrecisionRadius = value_s; } // PrecisionRadius 
                            else if (com == "SX") { Shift[sNr].x = value_s; } // Datum Shift(Zero Offset) X 
                            else if (com == "SY") { Shift[sNr].y = value_s; } // Datum Shift(Zero Offset) Y 
                            else if (com == "SZ") { Shift[sNr].z = value_s; } // Datum Shift(Zero Offset) Z 
                            else if (com == "RX") { Rot.x = value_s; Rot.y = 0; Rot.z = 0; } // Rotation around X axis (Plane Y,Z) 
                            else if (com == "RY") { Rot.x = 0; Rot.y = value_s; Rot.z = 0; } // Rotation around Y axis (Plane X,Z) 
                            else if (com == "RZ") { Rot.x = 0; Rot.y = 0; Rot.z = value_s; } // Rotation around Z axis (Plane X,Y) 
                            else if (com == "IRX") { Rot.x += value_s; Rot.y = 0; Rot.z = 0; } // Rotation around X axis (Plane Y,Z) 
                            else if (com == "IRY") { Rot.x = 0; Rot.y += value_s; Rot.z = 0; } // Rotation around Y axis (Plane X,Z) 
                            else if (com == "IRZ") { Rot.x = 0; Rot.y = 0; Rot.z += value_s; } // Rotation around Z axis (Plane X,Y) 
                            else { continue; }
                            EndPosReached = 0;
                        }

                    }
                    else
                    { // value_s Without Axis 
                        value_s = Convert.ToDouble(value);
                        if (z == 0) { MxEnd = value_s + Shift[sNr].x; }
                        if (z == 1) { MyEnd = value_s + Shift[sNr].y; }
                        if (z == 2) { MzEnd = value_s + Shift[sNr].z; }
                        if (z == 3) { MaEnd = SmartAngle(value_s); }
                        if (z == 4) { Feed = value_s; }

                        EndPosReached = 0;
                    }
                }

                if (IL != null && EndPosReached == 0) { IL.ApplyAction("OnOff_On"); }

                if (EndPosReached == 0)
                { // issued Movement command 
                    CurrentLine = argument;
                    EndGain = Gain;
                    //IstToEndInterpolIter = 5*(SpeedUpMult-1); // Start value for Path Speed Up 

                    //CalculateIstPos(ang1, ang2, ang4, ang5); 
                    //MxStart = MxIst; MyStart = MyIst; MzStart = MzIst; // Start Pos of Iteration (Start position) 

                    MxEndOrg = MxEnd; MyEndOrg = MyEnd; MzEndOrg = MzEnd; // After applying new coordinates and shift, save this values for restoring them on the next argument call 
                    Debug("Saving MxEndOrg: " + r2s(MxEndOrg) + ", MyEndOrg: " + r2s(MyEndOrg) + ", MzEndOrg: " + r2s(MzEndOrg), true);


                    if (Rot.x != 0)
                    {
                        double MyEndShifted = MyEndOrg - Shift[sNr].y; // Coordiantes without the shift to use for rotation (MyEndOrg are the absolute coords including shift) 
                        double MzEndShifted = MzEndOrg - Shift[sNr].z;
                        double[] rot = RotateVector2d(MyEndShifted, MzEndShifted, Rot.x);
                        MyEnd = Shift[sNr].y + rot[0]; // Absolute End Coordiantes with shift and rotation for this call 
                        MzEnd = Shift[sNr].z + rot[1];
                    }
                    else if (Rot.y != 0)
                    {
                        double MxEndShifted = MxEndOrg - Shift[sNr].x;
                        double MzEndShifted = MzEndOrg - Shift[sNr].z;
                        double[] rot = RotateVector2d(MxEndShifted, MzEndShifted, Rot.y);
                        MxEnd = Shift[sNr].x + rot[0];
                        MzEnd = Shift[sNr].z + rot[1];
                    }
                    else if (Rot.z != 0)
                    {
                        double MxEndShifted = MxEndOrg - Shift[sNr].x;
                        Debug("Calc MxEndShifted(" + r2s(MxEndShifted) + ") = MxEndOrg(" + r2s(MxEndOrg) + ") - Shift[sNr].x(" + r2s(Shift[sNr].x) + ")", true);
                        double MyEndShifted = MyEndOrg - Shift[sNr].y;
                        Debug("Calc MyEndShifted(" + r2s(MyEndShifted) + ") = MyEndOrg(" + r2s(MyEndOrg) + ") - Shift[sNr].y" + r2s(Shift[sNr].y) + ")", true);

                        Debug("Calc RotateVector2d(" + r2s(MxEndShifted) + ")(MxEndOrg,MyEndShifted,Rot.z(" + r2s(Rot.z) + ")", true);

                        double[] rot = RotateVector2d(MxEndShifted, MyEndShifted, Rot.z);
                        MxEnd = Shift[sNr].x + rot[0];
                        MyEnd = Shift[sNr].y + rot[1];
                        Debug("Calculated rotation rot[0]: " + r2s(rot[0]) + ", rot[1]: " + r2s(rot[1]), true);
                        Debug("Calculated EndPos with Rot MxEnd: " + r2s(MxEnd) + ", MyEnd: " + r2s(MyEnd), true);

                    }
                }
                Debug("End of Argument parsing", true);

            }


            if (DateTime.Now.Ticks <= TargetTime.Ticks) { TB.ApplyAction("TriggerNow"); return; } // Wait for Zykluszeit ms to process the next cycle 
            TargetTime = DateTime.Now.AddTicks(Zykluszeit * TimeSpan.TicksPerMillisecond);


            // ##### Execute this every Zykluszeit ##### 

            double factor = 1;
            double factorAng = 1;



            if (EndPosReached == 0)
            {

                IstToEndInterpol = Feed * GlobalTurnSpeedLimitFactor * (Convert.ToDouble(Zykluszeit) / 1000);

                MainScreen(i.ToString(), false);
                FeedScreen("Feed " + r2s(Feed) + "\n" + "Step " + r2s(ManualStep), false);

                ang1 = GetRotorAngle(AR2BasePole);
                ang2 = GetRotorAngle(AR3Pole);
                ang3 = GetRotorAngle(AR4PoleWrist);
                ang4 = GetRotorAngle(AR1BaseTop);
                ang5 = GetRotorAngle(AR5WristTop);


                // Main forward IK function 
                CalculateIstPos(ang1, ang2, ang4, ang5);

                double Dist3DIstToStart = Math.Sqrt(Math.Pow(MxIst - MxStart, 2) + Math.Pow(MyIst - MyStart, 2) + Math.Pow(MzIst - MzStart, 2));

                double diff1 = (ang5 - (ang4 * (-ToolDir))) * (-ToolDir);
                double WristDiff = Math.Abs(SmartAngle(MaEnd - diff1));

                if ((AR5WristTop as IMyMotorStator).IsAttached == false)
                { // TipRotor is Detached 
                    WristDiff = 0;
                }


                double Dist3DIstToEnd = Math.Sqrt(Math.Pow(MxIst - MxEnd, 2) + Math.Pow(MyIst - MyEnd, 2) + Math.Pow(MzIst - MzEnd, 2));


                double Dist3DIstToEndAng = Dist3DIstToEnd + ((WristDiff > PrecisionRadius * 2) ? WristDiff / 2 : WristDiff); // Add WristDiff as an additional distance, so 
                // turning around the same point gets its path iterations too. 
                if (ARB != null)
                {
                    angB = GetRotorAngle(ARB);
                    Dist3DIstToEndAng += Math.Abs(SmartAngle(MbEnd - angB));
                }
                if (ARC != null)
                {
                    angC = GetRotorAngle(ARC);
                    Dist3DIstToEndAng += Math.Abs(SmartAngle(McEnd - angC));
                }


                if (Dist3DIstToEndAng <= PrecisionRadius)
                {
                    //IstToEndInterpolIter = IstToEndInterpol; 

                    EndGain = EndGain * 0.8; // Reducing Gain if its close to desired Endposition to avoid Feedback Build Up 
                    IstToEndInterpolIter = 5 * (SpeedUpMult - 1); // Start value for Path Speed Up 
                    if (EndGain < 0.27 * Gain)
                    { // Consider Move Finished after (in this case) 6 ideling cycles (6*Zykluszeit) 
                        Stop();

                        EndPosReached = 1;

                        MxStart = MxEnd;
                        MyStart = MyEnd;
                        MzStart = MzEnd;
                        MaStart = MaEnd;
                        MbStart = MbEnd;
                        McStart = McEnd;
                    }

                    //MainScreen("Endposition reached. Stop.", true) ; 
                    if (IL != null) { IL.ApplyAction("OnOff_Off"); }
                }
                else
                {
                    IstToEndInterpolIter *= SpeedUpMult * (Convert.ToDouble(Zykluszeit) / 1000) * 40;   // slowly approach IstToEndInterpol Value by multiplying with SpeedUpMult every Zykluszeit 
                    if (IstToEndInterpolIter > IstToEndInterpol) { IstToEndInterpolIter = IstToEndInterpol; } // Limit when reached desired value 

                    double NewStartDist = Dist3DIstToEndAng - IstToEndInterpolIter; // shorten linear path by IstToEndInterpolIter 
                    if (NewStartDist < 0) { NewStartDist = 0; } // end of path, cant go negative 
                    factor = NewStartDist / Dist3DIstToEndAng;

                    //	NewTargetPos = EndPos + [(Distance to CurrentPos) */shortened by IstToEndInterpolIter] 
                    MxStart = MxEnd + (MxStart - MxEnd) * factor;
                    MyStart = MyEnd + (MyStart - MyEnd) * factor;
                    MzStart = MzEnd + (MzStart - MzEnd) * factor;

                    if (Math.Abs(MaEnd - MaStart) > 180)
                    {
                        if (MaStart < 0)
                        {
                            MaStart = MaStart + 360;
                        }
                        else
                        {
                            MaStart = MaStart - 360;
                        }
                    }
                    MaStart = MaEnd + (MaStart - MaEnd) * factor;


                    if (ARB != null)
                    {
                        if (Math.Abs(MbEnd - MbStart) > 180)
                        {
                            if (MbStart < 0)
                            {
                                MbStart = MbStart + 360;
                            }
                            else
                            {
                                MbStart = MbStart - 360;
                            }
                        }
                        MbStart = MbEnd + (MbStart - MbEnd) * factor;
                    }
                    if (ARC != null)
                    {
                        if (Math.Abs(McEnd - McStart) > 180)
                        {
                            if (McStart < 0)
                            {
                                McStart = McStart + 360;
                            }
                            else
                            {
                                McStart = McStart - 360;
                            }
                        }
                        McStart = McEnd + (McStart - McEnd) * factor;
                    }



                    // Main inverse IK function 
                    CalculateAngles(MxStart, MyStart, MzStart);
                }

                //MainScreen("",false);// Clear MainScreen 

                ExecuteRegelkreis();

                MainScreen("Actual                                 Remaining" + "\n" +

                            "X " + FormOutp(MxIst - Shift[sNr].x) + "                          X " + FormOutp(MxEnd - MxIst) + "\n" +
                            "Y " + FormOutp(MyIst - Shift[sNr].y) + "                          Y " + FormOutp(MyEnd - MyIst) + "\n" +
                            "Z " + FormOutp(MzIst - Shift[sNr].z) + "                          Z " + FormOutp(MzEnd - MzIst) + "\n" +
                            "A " + FormOutp(SmartAngle(diff1)) + "                          A " + FormOutp(WristDiff) + "\n" +

                            ((ARB != null) ? "B " + FormOutp(SmartAngle(angB)) + "                          B " + FormOutp(SmartAngle(MbEnd - angB)) + "\n\n" : "") +
                            ((ARC != null) ? "C " + FormOutp(SmartAngle(angC)) + "                          C " + FormOutp(SmartAngle(McEnd - angC)) + "\n\n" : "") +
                            "\n" +
                            "F" + r2s(IstToEndInterpolIter / (Convert.ToDouble(Zykluszeit) / 1000)) + "  S" + r2s(ManualStep) + "  D" + r2s(ToolDir) + "            3D Distance: " + r2s(Dist3DIstToEnd) + "\n" +
                            "\n" +
                            "                       3D Following Error: " + r2s(Dist3DIstToStart) + "\n", false);

                MainScreen(CurrentLine + ((EndPosReached == 1) ? "" : "         ## Moving ##"), true);
                MainScreen(DebugLine, true);




                //~ MainScreen("IstToEndInterpolIter: "+IstToEndInterpolIter,true); 
                //~ MainScreen("Gain: "+Gain,true); 
                //~ MainScreen("SpeedUpMult: "+SpeedUpMult,true); 
                //~ MainScreen("PrecisionRadius: "+PrecisionRadius,true); 

                //~ MainScreen( "Angles    Start    ->    Ist\nBasePole   "+ Math.Round(BasePoleAng,1) +"   ->   "+Math.Round(ang1,1)+"\n"+ 
                //~ "Pole       "+ Math.Round(PoleAng,1) +"   ->   "+Math.Round(ang2,1)+"\n"+ 
                //~ "PoleWrist  "+ Math.Round(PoleWristAng,1) +"   ->   "+Math.Round(ang3,1)+"\n"+ 
                //~ "BaseTop    "+ Math.Round(BaseTopAng,1) +"   ->   "+Math.Round(ang4,1)+"\n"+ 
                //~ "WristTop   "+ Math.Round(WristTopAng,1)+"   ->   "+Math.Round(ang5,1), true); 

                //~ MainScreen( "EndPosReached:" + EndPosReached + ", factor: " + factor.ToString()+"\n"+ 
                //~ "Start Pos -> MxStart: " +r2s(MxStart)+ " MyStart: " +r2s(MyStart)+ " MzStart: " +r2s(MzStart)+ " MaStart: " +MaStart +"\n"+ 
                //~ "End Pos   -> MxEnd:   " +r2s(MxEnd)  + " MyEnd:   " +r2s(MyEnd)  + " MzEnd:   " +r2s(MzEnd)  + " MaEnd:   " +MaEnd   +"\n",true); 

                //~ MainScreen( "EndPosReached:" + EndPosReached + ", factor: " + factor.ToString()+"\n"+ 
                //~ "MbStart: " +r2s(MbStart)+ " McStart: " +r2s(McStart)+"\n"+ 
                //~ "MbEnd:   " +r2s(MbEnd)  + " McEnd:   " +r2s(McEnd)  +"\n",true); 

                i++;
                TB.ApplyAction("TriggerNow"); // Call itself 

            }
            else
            {

                return;
            }

        }

        string FormOutp(double inp)
        {
            return ((inp < 0) ? " -" : "+") + "   " + String.Format("{0,5}", Math.Abs(Math.Round(inp)));
        }

        string SaveOutp(double inp)
        {
            return ((Math.Round(inp) < 0) ? "-" : "") + String.Format("{0}", Math.Abs(Math.Round(inp)));
        }

        void ExecuteRegelkreis()
        {
            if (EndPosReached == 1) { Stop(); return; }
            GlobalTurnSpeedLimitFactor = 1;
            for (int i = 0; i <= 1; i++)
            {
                SetRotorZyklus(AR2BasePole, BasePoleAng, false);
                SetRotorZyklus(AR3Pole, PoleAng, false);
                SetRotorZyklus(AR4PoleWrist, PoleWristAng, false);
                SetRotorZyklus(AR1BaseTop, BaseTopAng, false);
                SetRotorZyklus(AR5WristTop, WristTopAng, false);

                if (ARB != null) { SetRotorZyklus(ARB, MbStart, false); }
                if (ARC != null) { SetRotorZyklus(ARC, McStart, false); }


                if (GlobalTurnSpeedLimitFactor >= 1) { break; } // If every Turnspeed Output is within MaxTurnSpeed then exit loop 
                // If not iterate through all Outputs and apply GlobalTurnSpeedLimitFactor 
                //MainScreen("GlobalTurnSpeedLimitFactor: "+GlobalTurnSpeedLimitFactor,true); 
            }

        }

        void Stop()
        {

            SetRotorZyklus(AR2BasePole, 0, true);
            SetRotorZyklus(AR3Pole, 0, true);
            SetRotorZyklus(AR4PoleWrist, 0, true);
            SetRotorZyklus(AR1BaseTop, 0, true);
            SetRotorZyklus(AR5WristTop, 0, true);
            if (ARB != null) { SetRotorZyklus(ARB, 0, true); }
            if (ARC != null) { SetRotorZyklus(ARC, 0, true); }
        }



        double SetRotorZyklus(IMyTerminalBlock rotor, double AngleSol, bool stop)
        {
            //double VelIst; 

            if (stop == true)
            {
                rotor.SetValue<float>("Velocity", 0);
                return 0;
            }
            if (!(rotor as IMyMotorStator).IsAttached)
            { // Rotor is Detached 
                rotor.SetValue<float>("Velocity", 0);
                return 0;
            }

            double AngleIst = GetRotorAngle(rotor);

            //VelIst = rotor.GetValue < double > ("Velocity"); 
            if (AngleIst < 0) { AngleIst += 360; }

            double Error = SmartAngle(AngleSol - AngleIst); // Difference from Actual Angle (Ist) to Target Angle (Soll) 

            if (Math.Abs(Error) < 0.01)
            {
                //Error = 0; 
            }

            // ######  Main PI Control Loop formula ######## 
            double Output = Error * EndGain;

            double LocalTurnSpeedLimitFactor = Math.Abs(MaxTurnSpeed / Output);

            if (LocalTurnSpeedLimitFactor < GlobalTurnSpeedLimitFactor) { GlobalTurnSpeedLimitFactor = LocalTurnSpeedLimitFactor; return 0; }
            Output *= GlobalTurnSpeedLimitFactor;

            rotor.SetValue<float>("Velocity", Convert.ToSingle(Output));

            return AngleIst;
        }

        double SmartAngle(double ang)
        {
            if (ang >= 180)
            {
                ang = ang - 360;
                SmartAngle(ang);
            }
            else if (ang < -180)
            {
                ang = 360 + ang;
                SmartAngle(ang);
            }
            return ang;
        }


        double GetRotorAngle(IMyTerminalBlock rotor)
        {

            IMyMotorStator rotori = rotor as IMyMotorStator;
            double AngleIst = (double)rotori.Angle * 180 / pi;
            if (rotor == AR5WristTop || rotor == AR4PoleWrist) { AngleIst -= ((ToolDir == 1) ? 180 : 0); }
            return SmartAngle(AngleIst);
        }


        void CalculateIstPos(double BasePoleAngl, double PoleAngl, double BaseTopAngl, double WristTopAngl)
        {

            if ((AR5WristTop as IMyMotorStator).IsAttached == false)
            { // TipRotor is Detached 
                WristTopAngl = (BaseTopAngl + MaStart) * (-ToolDir);
            }
            else
            {
                WristTopAngl = WristTopAngl * (-ToolDir);
            }

            var Px1 = Math.Cos(BasePoleAngl * pi / 180) * segmentlength;
            var Px2 = Math.Cos((BasePoleAngl - PoleAngl) * pi / 180) * segmentlength;

            var PxF = Px1 + Px2;

            MxIst = -Math.Cos(-BaseTopAngl * pi / 180) * PxF - (ToolLength * Math.Cos((WristTopAngl - BaseTopAngl) * pi / 180));
            MyIst = -Math.Sin(-BaseTopAngl * pi / 180) * PxF - (ToolLength * Math.Sin((WristTopAngl - BaseTopAngl) * pi / 180));

            var z1 = Math.Sin(BasePoleAngl * pi / 180) * segmentlength;
            var z2 = Math.Sin((BasePoleAngl - PoleAngl) * pi / 180) * segmentlength;
            MzIst = (z1 + z2) - ToolHeight + ((ToolDir == 1) ? 41.5 : 0);

        }


        void CalculateAngles(double MouseX, double MouseY, double MouseZ)
        {
            MouseX = MouseX + (ToolLength * Math.Cos(MaStart * pi / 180));
            MouseY = MouseY + (ToolLength * Math.Sin(MaStart * pi / 180));
            MouseZ = -MouseZ - ToolHeight + ((ToolDir == 1) ? 41.5 : 0);
            double TipX = MouseX;
            double TipY = MouseY;
            double TipZ = MouseZ;

            double lenXY = Math.Sqrt(Math.Pow(MouseX, 2) + Math.Pow(MouseY, 2)); //length from base to Mouse in XY View 


            double lenAE = Math.Sqrt(Math.Pow(lenXY, 2) + Math.Pow(MouseZ, 2)); //real length from base to Mouse in rotated arm plane view 
            //lenXY is the X Coordiante of the Tip in this view 
            double lenXYnew = lenXY;
            double lenAEnew = lenAE;
            if (lenAE > 2 * segmentlength)
            {

                lenAEnew = 2 * segmentlength;
                lenXYnew = Math.Sqrt(Math.Abs(Math.Pow(lenAEnew, 2) - Math.Pow(MouseZ, 2))); // New Length based on restricted legth of arm 
                double Vmult = lenXYnew / lenXY; // Shortening Multiplier for XY Coords 
                TipX = (MouseX) * Vmult; // Recalculate Restricted Tip 
                TipY = MouseY * Vmult;

            }

            double VPivotBaseX = -lenXYnew / 2;
            double VPivotBaseY = TipZ / 2;

            double halfLenAEnew = lenAEnew / 2;

            double BaseHeight = Math.Sqrt(Math.Pow(segmentlength, 2) - Math.Pow(halfLenAEnew, 2));

            double dirx = VPivotBaseX / halfLenAEnew; // gradient ratio X 
            double diry = VPivotBaseY / halfLenAEnew; // gradient ratio Y 

            double PoleX = (dirx * halfLenAEnew) - (diry * BaseHeight); // rotate 90° and add BaseHeight 
            double PoleY = (diry * halfLenAEnew) + (dirx * BaseHeight);



            // Angles 
            BasePoleAng = -Math.Atan2(PoleY, -PoleX) * 180 / pi;

            PoleAng = (Math.Atan2(TipZ - PoleY, -((-lenXYnew) - PoleX)) * 180 / pi + BasePoleAng);

            PoleWristAng = -(90 - (Math.Atan2(20, 0) * 180 / pi) - PoleAng + BasePoleAng);

            // Angles 
            BaseTopAng = -Math.Atan2(-TipY, -TipX) * 180 / pi;
            WristTopAng = (BaseTopAng + MaStart) * (-ToolDir);

        }

        void Recalculate()
        {
            ang1 = GetRotorAngle(AR2BasePole);
            ang2 = GetRotorAngle(AR3Pole);
            ang3 = GetRotorAngle(AR4PoleWrist);
            ang4 = GetRotorAngle(AR1BaseTop);
            ang5 = GetRotorAngle(AR5WristTop);

            CalculateIstPos(ang1, ang2, ang4, ang5);
            MxStart = MxIst; MyStart = MyIst; MzStart = MzIst; // Start Pos of Iteration (Start position) 
        }

        double[] RotateVector2d(double x, double y, double degrees)
        {
            double[] result = new double[2];
            result[0] = x * Math.Cos(degrees * pi / 180) - y * Math.Sin(degrees * pi / 180);
            result[1] = x * Math.Sin(degrees * pi / 180) + y * Math.Cos(degrees * pi / 180);
            return result;
        }

        void MainScreen(string text, bool append)
        {
            if (Mscreen == null) { return; }
            Mscreen.WritePublicText(text + "\n", append);
        }

        void FeedScreen(string text, bool append)
        {
            if (Fscreen == null) { return; }
            Fscreen.WritePublicText(text + "\n", append);
        }

        void Debug(string text, bool append)
        {
            IMyTextPanel screen;
            screen = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("DEBUG");
            if (screen == null) { return; }
            screen.WritePublicText(text + "\n", append);
        }

        string r2s(double input)
        {
            return Math.Round(input, 1).ToString();
        }

        void Init()
        {
            //Shift[0].x = 0; // Example for hard coded Datum Shift 
            //Shift[0].y = 0; 
            //Shift[0].z = 0; 

            i++;
            MainScreen("init", true);
            Debug("", false);

            // Rotors on arm 
            AR1BaseTop = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(AR1BaseTopName); // BaseTop Rotor 
            AR2BasePole = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(AR2BasePoleName); // BasePole Rotor 
            AR3Pole = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(AR3PoleName); // Pole Rotor 
            AR4PoleWrist = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(AR4PoleWristName); // PoleWrist Rotor 
            AR5WristTop = (IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(AR5WristTopName); // WristTop Rotor 

            ARB = null;//(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARBAxisName); // WristTop Rotor 
            ARBTip = null;//(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARBTipName); // WristTop Rotor 
            ARC = null;//(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(ARCAxisName); // WristTop Rotor 

            // Timer Block for main control loop 
            TB = (IMyTimerBlock)GridTerminalSystem.GetBlockWithName(TBName);

            // ####### Optional Blocks ####### 

            //Lighting Block for moving indication (for easy Automation usage) 
            IL = (IMyLightingBlock)GridTerminalSystem.GetBlockWithName(ILName);

            // Main Screen LCD Display (for Coordinate Display) 
            Mscreen = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(MscreenName);

            // Feed Screen LCD Display (for big display of Feed and Step Size) 
            Fscreen = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(FscreenName);

            // easy Automation LCD Screen, Write Saved Positions to public screen (for SavePos command) 
            AutScreen = (IMyTextPanel)GridTerminalSystem.GetBlockWithName(AutScreenName);


            if (AR1BaseTop == null) { throw new Exception("Error: AR1 BaseTop Block not found"); }
            if (AR2BasePole == null) { throw new Exception("Error: AR2 BasePole Block not found"); }
            if (AR3Pole == null) { throw new Exception("Error: AR3 Pole Block not found"); }
            if (AR4PoleWrist == null) { throw new Exception("Error: AR4 PoleWrist Block not found"); }
            if (AR5WristTop == null) { throw new Exception("Error: AR5 WristTop Block not found"); }

            if (TB == null) { throw new Exception("Error: Timer Block not found"); }

            if (Mscreen == null) { Echo("Warning: MainScreen Block not found"); }
            if (Fscreen == null) { Echo("Warning: FeedScreen Block not found"); }

            if (IL == null) { Echo("Warning: IL Arm Block not found"); }

            if (AutScreen == null) { Echo("Warning: easy Automation Screen Block not found"); }

            TargetTime = DateTime.Now.AddTicks(Zykluszeit * TimeSpan.TicksPerMillisecond);

            EndPosReached = 1;
            IstToEndInterpolIter = 5 * (SpeedUpMult - 1); // Start value for Path Speed Up 

            ang1 = GetRotorAngle(AR2BasePole);
            ang2 = GetRotorAngle(AR3Pole);
            ang3 = GetRotorAngle(AR4PoleWrist);
            ang4 = GetRotorAngle(AR1BaseTop);
            ang5 = GetRotorAngle(AR5WristTop);

            MaStart = SmartAngle((ang5 - (ang4 * (-ToolDir))) * (-ToolDir));
            MaEnd = MaStart;


            if (ARB != null)
            {
                angB = GetRotorAngle(ARB);
                MbStart = SmartAngle(angB);
                MbEnd = MbStart;
            }
            if (ARC != null)
            {
                angC = GetRotorAngle(ARC);
                McStart = SmartAngle(angC);
                McEnd = McStart;
            }


            //~ MainScreen("Ist Angles\nBasePole   "+  Math.Round(ang1,3)+"\n"+ 
            //~ "Pole       "+ Math.Round(ang2,3)+"\n"+ 
            //~ "PoleWrist  "+ Math.Round(ang3,3)+"\n"+ 
            //~ "BaseTop    "+ Math.Round(ang4,3)+"\n"+ 
            //~ "WristTop   "+ Math.Round(ang5,3)+"\n", true); 

            CalculateIstPos(ang1, ang2, ang4, ang5);
            //~ MainScreen("Ist Pos   -> MxIst: "+r2s(MxIst)+" MyIst: "+r2s(MyIst)+" MzIst: "+r2s(MzIst), true) ; 
            MxStart = MxIst; MyStart = MyIst; MzStart = MzIst; // Start Pos of Iteration (Start position) 
            //~ MainScreen("iStart Pos -> MxStart: "+r2s(MxStart)+" MyStart: "+r2s(MyStart)+" MzStart: "+r2s(MzStart), true) ; 
            MxEnd = MxIst; MyEnd = MyIst; MzEnd = MzIst;      // Final Pos of Iteration 

            MxEndOrg = MxIst; MyEndOrg = MyIst; MzEndOrg = MzIst;

            CalculateAngles(MxStart, MyStart, MzStart);

            //~ MainScreen("Start Angles\nBasePole   "+ Math.Round(BasePoleAng,1) +"\n"+ 
            //~ "Pole       "+ Math.Round(PoleAng,1) +"\n"+ 
            //~ "PoleWrist  "+ Math.Round(PoleWristAng,1) +"\n"+ 
            //~ "BaseTop    "+ Math.Round(BaseTopAng,1) +"\n"+ 
            //~ "WristTop   "+ Math.Round(WristTopAng,1)+"\n",true); 

        } 
 

    }
}
