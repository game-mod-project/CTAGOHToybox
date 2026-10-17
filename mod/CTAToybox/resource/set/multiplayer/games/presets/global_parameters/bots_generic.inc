{easy
	{unitset
		{value %period}
	}
	{balancingFactorBonus
		{cp 0.016}
		{mpStart 0.022}
		{mpFinish 0.016}
		{payback 0.8}
		{chargeDelay 1.0}	;// adjusts the initial delay only
		{charge 1.0}		;// adjusts the interval only
	}
	{unitScore 1.2}
}
{normal
	{unitset
		{value %period}
	}
	{balancingFactorBonus
		{cp 0.018}
		{mpStart 0.022}
		{mpFinish 0.018}
		{payback 0.90}
		{chargeDelay 1.0}	;// adjusts the initial delay only
		{charge 1.0}		;// adjusts the interval only
	}
	{unitScore 1.0}
}
{hard
	{unitset
		{value %period}
	}
	{balancingFactorBonus
		{cp 0.02}
		{mpStart 0.022}
		{mpFinish 0.025}
		{payback 3.50}
		{chargeDelay 1.0}	;// adjusts the initial delay only
		{charge 1.0}		;// adjusts the interval only
	}
	{unitScore 0.8}
}
{heroic
	{unitset
		{value %period}
	}
	{balancingFactorBonus
		{cp 0.022}
		{mpStart 0.022}
		{mpFinish 0.035}
		{payback 4.00}
		{chargeDelay 1.0}	;// adjusts the initial delay only
		{charge 1.0}		;// adjusts the interval only
	}
	{unitScore 0.5}
}
