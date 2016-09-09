///////////////////////// 
// How to use:  
// Press "1" to start calculation of planet center and fly horizontaly for 5-10 seconds.  
// Press "2" to stop script  
// Press "3" to stop planet center recalculation and continue only bearing and planetary coordinates calculation.  
// You can place this tool on rover, mining ship or satellite.  
// You need to place these blocks:  
 
// block            name                   settings  
// Timer            "TimerClock"           trigger Programmable block  
// Program block    "Programmable block"   copy script there  
// Remote Control   "RemCon"  
// Text Panel       "TP"                   set font size to 1.3, select "show public text" option  
 
// Script arguments for 1, 2, 3 buttons: "Start", "Stop", "Continue". 
// The only thing, you should keep in mind, script works only in gravity range. 
/////////////////////////// 
 
//-----User defined vars------ 
const int Clock=15;  // defines how frequantly will your text panel be updated    
const string TimerName= "TimerClock"; // timer block name  
const string TPName= "TP"; // text panel name 
const string RemConName= "RemCon"; // remote control name 
//----------------- 
 
IMyTimerBlock Timer;    
IMyTextPanel TP;    
IMyRemoteControl RemCon;    
int TickCount;     
bool On, RecalcCenter;    
Vector3D FirstPos, FirstGravVector, LastPos, LastGravVector, PlanetCenter, AltVector, AltReject, FReject, VectNord, LReject, GravVectorNorm; 
double Alpha, Alpha2, Altitude, Latitude, Longitude, Bearing; 
 
const string CompassStr="|NW|-320--325--330--335--340--345--350--355--|N|--005--010--015--020--025--030--035--040--|NE|-050--055--060--065--070--075--080--085--|E|--095--100--105--110--115--120--125--130--|SE|-140--145--150--155--160--165--170--175--|S|--185--190--195--200--205--210--215--220--|SW|-230--235--240--245--250--255--260--265--|W|--275--280--285--290--295--300--305--310--|NW|-320--325--330--335--340--345--350--355--|N|--005--010--015--020--025--030--035--040--"; 
string Compass; 
//---------------    
    
void Main(string argument){    
	if (Timer==null)    
		Timer = GridTerminalSystem.GetBlockWithName(TimerName) as IMyTimerBlock;      
	if (TP==null)		   
		TP = GridTerminalSystem.GetBlockWithName(TPName) as IMyTextPanel;		  
	if (RemCon==null)		   
		RemCon = GridTerminalSystem.GetBlockWithName(RemConName) as IMyRemoteControl;			 
	if (argument=="Start")    
	{    
		On=true;   
		Alpha2=0; 
		Alpha=0; 
		FirstPos = RemCon.GetPosition(); 
		FirstGravVector = RemCon.GetNaturalGravity(); 
		RecalcCenter=true; 
	}		    
	if (argument=="Stop")    
	{    
		On=false; 
		RecalcCenter=false;		 
	}    
	if (argument=="Continue")    
	{    
		On=true;    
		RecalcCenter=false; 
	}    
 
		TickCount++;      
		if ((TickCount % Clock)==0)      
		{    
			LastPos = RemCon.GetPosition(); 
			LastGravVector = RemCon.GetNaturalGravity(); 
			GravVectorNorm = Vector3D.Normalize(LastGravVector); 
			   FReject = Vector3D.Reject(RemCon.WorldMatrix.Forward, GravVectorNorm); 
			   VectNord = Vector3D.Reject(new Vector3D(0,-1,0), GravVectorNorm); 
			if (Math.Acos(Vector3D.Dot(RemCon.WorldMatrix.Down, GravVectorNorm))<(Math.PI/2)) 
			   LReject = Vector3D.Reject(RemCon.WorldMatrix.Right, GravVectorNorm); 
			else 
			   LReject = Vector3D.Reject(RemCon.WorldMatrix.Left, GravVectorNorm); 
			if (LReject.GetDim(1)>0) 
				Bearing = Math.Acos(Vector3D.Dot(Vector3D.Normalize(FReject), Vector3D.Normalize(VectNord)))*180/Math.PI; 
			else 
				Bearing = 360 - Math.Acos(Vector3D.Dot(Vector3D.Normalize(FReject), Vector3D.Normalize(VectNord)))*180/Math.PI;			 
				Compass = CompassStr.Substring((int)Bearing+31, 31); 
			if (RecalcCenter) 
			{ 
				Alpha = Math.Acos(Vector3D.Dot(Vector3D.Normalize(LastGravVector), Vector3D.Normalize(FirstGravVector)));				 
				if (Alpha > Alpha2) 
				{ 
					Alpha2=Alpha; 
					PlanetCenter=TargetGPS(FirstPos, FirstPos+FirstGravVector, LastPos, LastPos+LastGravVector); 
				} 
			} 
			AltVector = LastPos - PlanetCenter; 
			Altitude=AltVector.Length(); 
			AltReject=new Vector3D(AltVector.GetDim(0), 0, AltVector.GetDim(2)); 
			Latitude = Math.Acos(Vector3D.Dot(Vector3D.Normalize(AltVector), Vector3D.Normalize(AltReject)))*180/Math.PI; 
			if (AltVector.GetDim(1)>0) 
				Latitude=-Latitude; 
			Longitude = Math.Acos(Vector3D.Dot(Vector3D.Normalize(AltReject), new Vector3D(0,0,1)))*180/Math.PI; 
			if (AltReject.GetDim(0)<0) 
				Longitude=-Longitude;			 
			string Output=" My Pos"; 
            Output += "\n X: " + Math.Round(LastPos.GetDim(0)) + "\n Y: " + Math.Round(LastPos.GetDim(1)) + "\n Z: " + Math.Round(LastPos.GetDim(2)); 
			Output+= "\n Bearing: " + Math.Round(Bearing, 2); 
			Output+= "\n" + Compass; 
			Output+= "\n Altitude: " + Math.Round(Altitude, 2); 
			Output+= "\n Latitude: " + Math.Round(Latitude, 4); 
			Output+= "\n Longitude: " + Math.Round(Longitude, 4);			 
			//Output+= "\n Angle: " + Math.Round((Alpha*180/Math.PI), 2) + " / " + Math.Round((Alpha2*180/Math.PI), 2);  
			Output+= "\n Planet center"; 
            Output += "\n X: " + Math.Round(PlanetCenter.GetDim(0)) + "\n Y: " + Math.Round(PlanetCenter.GetDim(1)) + "\n Z: " + Math.Round(PlanetCenter.GetDim(2)); 
			TextOutput(TP, Output); 
			Timer.GetActionWithName("TriggerNow").Apply(Timer);    
		}    
		else{      
			if (On)    
				Timer.GetActionWithName("TriggerNow").Apply(Timer);          
		}  	    
}    
 
public void TextOutput(IMyTextPanel tp, string output = "") 
{ 
	if (tp != null) 
	{ 
		tp.ShowTextureOnScreen(); 
		if (output != "") 
		{ 
			tp.WritePublicText(output); 
		} 
		tp.ShowPublicTextOnScreen(); 
		tp.GetActionWithName("OnOff_On").Apply(tp); 
	} 
} 
 
Vector3D TargetGPS(Vector3D FirstPos, Vector3D FirstGravVector, Vector3D LaststPos, Vector3D LastGravVector) 
{ 
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
AB2=(bX-aX)*(bX-aX)+(bY-aY)*(bY-aY)+(bZ-aZ)*(bZ-aZ); 
SCxAB=(cX-sX)*(bX-aX)+(cY-sY)*(bY-aY)+(cZ-sZ)*(bZ-aZ); 
ASxAB=(sX-aX)*(bX-aX)+(sY-aY)*(bY-aY)+(sZ-aZ)*(bZ-aZ); 
ABxSC=(bX-aX)*(cX-sX)+(bY-aY)*(cY-sY)+(bZ-aZ)*(cZ-sZ); 
SC2=(cX-sX)*(cX-sX)+(cY-sY)*(cY-sY)+(cZ-sZ)*(cZ-sZ); 
ASxSC=(sX-aX)*(cX-sX)+(sY-aY)*(cY-sY)+(sZ-aZ)*(cZ-sZ); 
 
Mk=((ASxAB*SC2)-(ASxSC*SCxAB))/((AB2*SC2)-(ABxSC*SCxAB)); 
Nk=((AB2*ASxSC)-(ABxSC*ASxAB))/((AB2*SC2)-(ABxSC*SCxAB)); 
 
mX=aX+((bX-aX)*Mk); 
mY=aY+((bY-aY)*Mk); 
mZ=aZ+((bZ-aZ)*Mk); 
 
nX=sX+((sX-cX)*Nk); 
nY=sY+((sY-cY)*Nk); 
nZ=sZ+((sZ-cZ)*Nk); 
 
return new Vector3D((mX+nX)/2, (mY+nY)/2, (mZ+nZ)/2); 
}
