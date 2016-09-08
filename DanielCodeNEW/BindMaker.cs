using System;
using System.IO;
using System.Collections.Generic;
public static class BindMaker{
public static bool cancel = false;
public static IEnumerable<PhysObject> PhysStep(PhysObject phys){
	Vector2 pos = phys.pos;
	Vector2 vel= phys.vel;
	int r=0;
	Vector2 gravity=new Vector2(0,-10);
	while(cancel ==false&&r<11){
		vel+=gravity;
		phys.pos=pos+vel;

		pos=phys.pos;
		phys.vel=vel;
		r++;
		yield return phys;
	}
}
public static void Main(string[] args){
	PhysObject physobject = new PhysObject(new Vector2(0,100),new Vector2(0,100));
	foreach(PhysObject n in PhysStep(physobject)){
		Console.WriteLine("Position: "+n.pos.x+":"+n.pos.y);
	}
	Console.ReadLine();
}
}
public class Vector2{
	public int x;
	public int y;
	public Vector2(int x, int y){
		this.x=x;
		this.y=y;
	}
	public static Vector2 operator +(Vector2 a,Vector2 b){
		return new Vector2(a.x+b.x,a.y+b.y);
	}
	public static Vector2 operator -(Vector2 a,Vector2 b){
		return new Vector2(a.x-b.x,a.y-b.y);
	}
}
public class PhysObject{
	public static List<PhysObject> objects = new List<PhysObject>();
	public Vector2 pos;
	public Vector2 vel;
	public PhysObject(Vector2 pos,Vector2 vel){
		this.pos=pos;
		this.vel=vel;
		objects.Add(this);
	}
}
public class Orbit{
	public float w;
	public float e;
}