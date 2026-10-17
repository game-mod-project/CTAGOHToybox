
	(include "/properties/cannon_mgun__ctb_high_mg_mg42_6fe7b97f.ext")
	(include "/properties/shadow/tank.inc" scale(0.3))
	{extension "mg42_lafette.mdl"}
	{props "vision_lev00_can" "cannon_sandbag_standard" "detect_cannon_aa" "sq_mg"}

	{boarder
		{anm "gunner"
			{forward {end "pose_dshk_aa_gunner"}}
		}
		{anm "commander"
			{forward {end "pose_dshk_aa_commander"}}
		}
		{anm "driver1"
			{forward {end "cannon_maxim_move" 10}}
		}
		{anm "driver2"
			{forward {end "walk_1hand" 10}}
		}
	}

	{extender "cannon"
		{animation
		;// "gunner" "commander"
			{switch "close"}
			{move "cannon_maxim_move" "walk_1hand"}
			;{fire "mg_maxim_gunner_fire" "mg_maxim_commander_fire"}
			{aim "mg_dshk_aa_gunner_aim" "mg_dshk_aa_commander_aim"}
			{reload "mg_dshk_aa_gunner_reload" "mg_dshk_aa_commander_reload"}
		}
		{move {workers "driver1" "driver2" minWorkers 1} {obstacle "close"}}
	}

	{extender "inventory"
		{weapon "mgun" {mask "mountable"}}
		{box
			{item "ammo mgun_ger belt" 5000}
			{item "sandbag_vehicle" 4}
		}
	}

	{Weaponry
		{place "mgun"
			{type "main"}
			{gunner "gunner"}
			{charger "gunner"}
			{LinkBone "mgun"}
			{LinkAnimation "off"}
			("abm_mgun")
			{Rotate}
		}
	}

	{mass 32}
	{Chassis
		("mobility_cannon"
			speed(12) ;----------------------------------Max speed.
			weight(32) ;---------------------------------Weight in Kilograms.
		)
	}

	{bone "basis"
		{speed2 20}
	}
	{bone "gun_rot"
		{limits -10 15}
		{speed2 60}
	}
	{bone "mgun"
		{limits -5 3}
		{speed2 60}
	}
	{bone "turret"
		{limits 13 13}
		{speed2 60}
	}
