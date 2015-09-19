package com.thedanieldude.test.client.render.items;

import com.thedanieldude.test.Main;
import com.thedanieldude.test.items.ModItems;

import net.minecraft.client.Minecraft;
import net.minecraft.client.resources.model.ModelResourceLocation;
import net.minecraft.item.Item;

public final class ItemRenderRegister {
	public static void registerItemRenderers(){
		Minecraft.getMinecraft().getRenderItem().getItemModelMesher().register(ModItems.testItem,0,new ModelResourceLocation("test:test","inventory"));
	}

}
