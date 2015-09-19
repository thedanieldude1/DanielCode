package com.thedanieldude.test;

import com.thedanieldude.test.client.render.items.ItemRenderRegister;
import com.thedanieldude.test.items.ModItems;

import net.minecraftforge.fml.common.event.FMLInitializationEvent;
import net.minecraftforge.fml.common.event.FMLPostInitializationEvent;
import net.minecraftforge.fml.common.event.FMLPreInitializationEvent;

public class ClientProxy extends CommonProxy{
	public void preInit(FMLPreInitializationEvent e) {
		super.preInit(e);
    }

    public void init(FMLInitializationEvent e) {
    	super.init(e);
    	ItemRenderRegister.registerItemRenderers();
    }

    public void postInit(FMLPostInitializationEvent e) {
    	super.postInit(e);
    }
}
