; == mid WAR STANDARD RESOURCES ==
(define "standard_all"
	{common
		{unitset
			{value "mid^conquestonly|doctrineonly"}
		}
		{commandPoints "*=%cp"}
		(include "global_parameters/unitlimits.inc")
		{chargeFactors
			{value "*=1.00"}
		}
		(include "global_parameters/budget.inc" s(%start) f(%finish) d(%doctrine))
	}
	{bots
		(include "global_parameters/bots_generic.inc" period("mid"))
	}
)

; == mid WAR CUSTOM RESOURCES ==
(define "custom_all"
	{common
		{unitset
			{value "mid^conquestonly|doctrineonly"}
		}
		{commandPoints "*=%cp"}
		(include "global_parameters/unitlimits.inc")
		{chargeFactors
			{value "*=1.00"}
			{customize "0;0.25;0.5;1;2"}
		}
		(include "global_parameters/budget_custom.inc" s(%start) f(%finish) d(%doctrine))
	}
	{bots
		(include "global_parameters/bots_generic.inc" period("mid"))
	}
)

;------------------------------------------------------------------------------------------------------------
{presets
	{"s:mid_standard_100"
		{gameMode "standard"}
		{unitMode "mid"}
		{specMode "all_normal"}
		{apply "only_roster_mid"}
		("standard_all"			start(10000) finish(165000) doctrine(200) cp(0:5000,900:7500,1800:10000))
		{ranked}
	}
;------------------------------------------------------------------------------------------------------------

	{"c:mid_custom_80"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_80"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(140000) doctrine(200) cp(0:4000,900:6500,1800:9000))
	}
	{"c:mid_custom_100"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_100"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(165000) doctrine(200) cp(0:5000,900:7500,1800:10000))
	}
	{"c:mid_custom_120"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_120"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(190000) doctrine(200) cp(0:6000,900:8500,1800:11000))
	}
	{"c:mid_custom_140"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_140"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(215000) doctrine(200) cp(0:7000,900:9500,1800:12000))
	}
	{"c:mid_custom_160"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_160"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(240000) doctrine(200) cp(0:8000,900:10500,1800:13000))
	}
	{"c:mid_custom_180"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_180"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(265000) doctrine(200) cp(0:9000,900:11500,1800:14000))
	}
	{"c:mid_custom_200"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_200"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(290000) doctrine(200) cp(0:10000,900:12500,1800:15000))
	}
	{"c:mid_custom_250"
		{gameMode "custom"}
		{unitMode "mid"}
		{specMode "cp_250"}
		{apply "only_roster_mid"}
		("custom_all"			start(10000) finish(315000) doctrine(200) cp(0:12500,900:15000,1800:17500))
	}
;------------------------------------------------------------------------------------------------------------
}