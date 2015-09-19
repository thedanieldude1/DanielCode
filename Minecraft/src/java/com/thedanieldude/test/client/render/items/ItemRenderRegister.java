package com.thedanieldude.test.client.render.items;

import com.thedanieldude.test.Main;
import com.thedanieldude.test.items.ModItems;

import net.minecraft.client.Minecraft;
import net.minecraft.client.resources.model.ModelResourceLocation;
import net.minecraft.item.Item;

public final class ItemRenderRegister {
	public static void registerItemRenderers(){
		regItemRender(ModItems.testItem);
	}
	public static void regItemRender(Item item){
		Minecraft.getMinecraft().getRenderItem().getItemModelMesher().register(item,0,new ModelResourceLocation(Main.MODID+":"+item.getUnlocalizedName(),"inventory"));
	}
}
