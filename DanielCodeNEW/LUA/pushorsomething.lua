hook.Add("PlayerUse","Push",function(ply,ent)
	if(ent:IsPlayer() and ply:GetActiveWeapon():GetClass()=="m9k_fists") then
		force = ent:GetPos():Sub(ply:GetPos())
		ent:SetVelocity(force:Mul(10))
	end
end)