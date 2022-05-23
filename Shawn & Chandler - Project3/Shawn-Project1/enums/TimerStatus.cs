using System;

namespace ShawnProject1
{
	public enum TimerStatus
	{
		Completed,//Completed for more than one frame
		JustCompleted,//Completed this frame
		Counting,//Counting for more than one frame
		StartedCounting,//Started counting this frame
		Incomplete//Just reset/hasn't started couting
	}
}

