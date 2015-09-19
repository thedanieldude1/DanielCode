package com.thedanieldude.test;
import net.minecraftforge.fml.common.Mod;
import net.minecraftforge.fml.common.Mod.EventHandler;
import net.minecraftforge.fml.common.Mod.Instance;
import net.minecraftforge.fml.common.SidedProxy;
import net.minecraftforge.fml.common.event.FMLInitializationEvent;
import net.minecraftforge.fml.common.event.FMLPostInitializationEvent;
import net.minecraftforge.fml.common.event.FMLPreInitializationEvent;
@Mod(modid = Main.MODID,name = Main.MODNAME, version = Main.VERSION)
public class Main {
	public static final String MODNAME = "Test Mod";
	public static final String MODID = "test";
	public static final String VERSION = "1.0";
	@SidedProxy(clientSide="com.thedanieldude.test.ClientProxy",serverSide="com.thedanieldude.test.ServerProxy")
	public static CommonProxy proxy;
	@Instance
	public static Main instance = new Main(); //Instance for easy access to the mod
	@EventHandler
	public void preInit(FMLPreInitializationEvent e){ //called at the very beginning of the startup routine. In this method we should read your config file, create Blocks, Items, etc. and register them with the GameRegistry.
		this.proxy.preInit(e);
	}
	
	@EventHandler
	public void init(FMLInitializationEvent e){ //In this method we can build up data structures, add Crafting Recipes and register new event handler.
		this.proxy.init(e);
	}
	
	@EventHandler
	public void postInit(FMLPostInitializationEvent e){ //. Its used to communicate with other mods and adjust your setup based on this.
		this.proxy.postInit(e);
	}
}
