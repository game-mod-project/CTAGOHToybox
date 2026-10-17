; == mid WAR STANDARD RESOURCES ==
(define "standard_all"
	{teams
		{a ;// PLAYER DEFENDER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_a"}
			(include "global_parameters/unitlimits.inc")
			{chargeFactors
				{value "*=1.00"}
			}
			{balancingFactorBonus	;// only affects players on team A (not bots)
				{chargeDelay 1}		;// adjusts the initial delay only
				{charge 1.0}		;// adjusts the interval only
			}
			(include "global_parameters/last_stand/budget_defender.inc" s(%start_a) f(%finish_a) d(%doctrine))
		}
		{b ;// AI ATTACKER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_b"}
			(include "global_parameters/unitlimits.inc")
			{chargeFactors
				{value "*=1.00"}
			}
			(include "global_parameters/last_stand/budget_attacker.inc" s(%start_b) f(%finish_b) d(%doctrine))
		}
	}

	{bots
		(include "global_parameters/last_stand/bots_standard.inc" period("mid") start_a(%start_a) finish_a(%finish_a) start_b(%start_b) finish_b(%finish_b) doctrine(%doctrine) cp_a(%cp_a) cp_b(%cp_b))
	}
)


; == MID WAR CUSTOM RESOURCES ==
(define "custom_all"
	{common
		{chargeFactors
			{value "*=1.00"}
			{customize "0;0.25;0.5;1;2"}
		}
	}
	{teams
		{a ;// PLAYER DEFENDER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_a"}
			(include "global_parameters/unitlimits.inc")
			(include "global_parameters/last_stand/budget_defender_custom.inc" s(%start_a) f(%finish_a) d(%doctrine))
		}
		{b ;// AI ATTACKER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_b"}
			(include "global_parameters/unitlimits.inc")
			(include "global_parameters/last_stand/budget_defender_custom.inc" s(%start_b) f(%finish_b) d(%doctrine))
		}
	}

	{bots
		(include "global_parameters/last_stand/bots_custom.inc" period("mid") start_a(%start_a) finish_a(%finish_a) start_b(%start_b) finish_b(%finish_b) doctrine(%doctrine) cp_a(%cp_a) cp_b(%cp_b))
	}
)


; == MID WAR TEST ==
(define "test_mid"
	{teams
		{a ;// PLAYER DEFENDER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_a"}
			{unitLimits ""}
			{chargeFactors
				{value "*=0"}
			}
			(include "global_parameters/last_stand/budget_defender.inc" s(%start_a) f(%finish_a) d(%doctrine))
		}
		{b ;// AI ATTACKER TEAM
			{unitset
				{value "mid^conquestonly|doctrineonly"}
			}
			{commandPoints "*=%cp_b"}
			{unitLimits ""}
			{chargeFactors
				{value "*=0"}
			}
			(include "global_parameters/last_stand/budget_attacker.inc" s(%start_b) f(%finish_b) d(%doctrine))
		}
	}

	{bots
		(include "global_parameters/last_stand/bots_standard.inc" period("mid") start_a(%start_a) finish_a(%finish_a) start_b(%start_b) finish_b(%finish_b) doctrine(%doctrine) cp_a(%cp_a) cp_b(%cp_b))
	}
)

;------------------------------------------------------------------------------------------------------------
{presets
	{"s:mid_standard_80"
		{gameMode "standard"}
		{unitMode "mid"}
		{specMode "all_normal"}
		{apply "only_roster_mid"}
		("standard_all"			start_a(25000) finish_a(82500) start_b(500) finish_b(3600) doctrine(200) cp_a(0:4000,900:5000,1800:6000) cp_b(0:120,900:160,1800:220))
		{ranked}
	}
;------------------------------------------------------------------------------------------------------------

; // custom games here

	{"c:mid_custom_60"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_60"}
		{apply "only_roster_mid"}
		("custom_all"			start_a(25000) finish_a(82500) start_b(500) finish_b(3600) doctrine(200) cp_a(0:3000,900:4000,1800:5000) cp_b(0:120,900:160,1800:220))
	}
	{"c:mid_custom_80"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_80"}
		{apply "only_roster_mid"}
		("custom_all"			start_a(27500) finish_a(110000) start_b(550) finish_b(4800) doctrine(200) cp_a(0:4000,900:5000,1800:6000) cp_b(0:140,900:180,1800:240))
	}
	{"c:mid_custom_100"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_100"}
		{apply "only_roster_mid"}
		("custom_all"			start_a(30000) finish_a(137500) start_b(600) finish_b(5000) doctrine(200) cp_a(0:5000,900:6000,1800:7000) cp_b(0:160,900:200,1800:260))
	}
	{"c:mid_custom_120"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_120"}
		{apply "only_roster_mid"}
		("custom_all"			start_a(32500) finish_a(165000) start_b(650) finish_b(7200) doctrine(200) cp_a(0:6000,900:7000,1800:8000) cp_b(0:180,900:220,1800:280))
	}

;------------------------------------------------------------------------------------------------------------

	{"s:test_mid"
		{gameMode "test"}
		{unitMode "mid"}
		{specMode "all_normal"}
		{apply "only_roster_mid"}
		("test_mid"			start_a(2500000) finish_a(2500000) start_b(50000) finish_b(50000) doctrine(2000) cp_a(50000) cp_b(1000))
	}
}