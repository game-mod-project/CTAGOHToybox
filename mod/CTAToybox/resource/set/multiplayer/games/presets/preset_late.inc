; == late WAR STANDARD RESOURCES ==
(define "standard_all"
	{common
		{unitset
			{value "late^conquestonly|doctrineonly"}
		}
		{commandPoints "*=%cp"}
		(include "global_parameters/unitlimits.inc")
		{chargeFactors
			{value "*=1.00"}
		}
		(include "global_parameters/budget.inc" s(%start) f(%finish) d(%doctrine))
	}
	{bots
		(include "global_parameters/bots_generic.inc" period("late"))
	}
)

; == late WAR CUSTOM RESOURCES ==
(define "custom_all"
	{common
		{unitset
			{value "late^conquestonly|doctrineonly"}
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
		(include "global_parameters/bots_generic.inc" period("late"))
	}
)

;------------------------------------------------------------------------------------------------------------
{presets
	{"s:late_standard_100"
		{gameMode "standard"}
		{unitMode "late"}
		{specMode "all_normal"}
		{apply "only_roster_late"}
		("standard_all"			start(10000) finish(300000) doctrine(200) cp(0:5000,900:7500,1500:10000))
		{ranked}
	}
;------------------------------------------------------------------------------------------------------------	

	{"c:late_custom_80"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_80"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(200000) doctrine(200) cp(0:4000,900:6500,1500:9000))
	}
	{"c:late_custom_100"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_100"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(225000) doctrine(200) cp(0:5000,900:7500,1500:10000))
	}
	{"c:late_custom_120"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_120"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(250000) doctrine(200) cp(0:6000,900:8500,1500:11000))
	}
	{"c:late_custom_140"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_140"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(275000) doctrine(200) cp(0:7000,900:9500,1500:12000))
	}
	{"c:late_custom_160"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_160"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(300000) doctrine(200) cp(0:8000,900:10500,1500:13000))
	}
	{"c:late_custom_180"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_180"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(325000) doctrine(200) cp(0:9000,900:11500,1500:14000))
	}
	{"c:late_custom_200"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_200"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(350000) doctrine(200) cp(0:10000,900:12500,1500:15000))
	}
	{"c:late_custom_250"
		{gameMode "custom"}
		{unitMode "late"}
		{specMode "cp_250"}
		{apply "only_roster_late"}
		("custom_all"			start(10000) finish(375000) doctrine(200) cp(0:12500,900:15000,1500:17500))
	}
;------------------------------------------------------------------------------------------------------------
}