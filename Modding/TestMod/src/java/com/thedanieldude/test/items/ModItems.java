package com.thedanieldude.test.items;

import net.minecraft.item.Item;
import net.minecraftforge.fml.common.registry.GameRegistry;

public final class ModItems {
	public static Item testItem;
	public static void createItems(){
		GameRegistry.registerItem(testItem=new TestItem("test"),"test");
	}
}
