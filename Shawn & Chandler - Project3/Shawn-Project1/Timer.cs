//Shawn Carter

using System;

namespace ShawnProject1
{
	public class Timer
	{
		private int currentTime, maxTime;
		private TimerStatus status;
		
		public TimerStatus Status
		{
			get { return status; }
		}
		
		public int Duration
		{
			get { return maxTime; }
		}
		
		public Timer (int duration)
		{
			currentTime = 0;
			maxTime = duration;
			status = TimerStatus.Incomplete;
		}
		public Timer() : this(60)
		{}
		
		public void Update()
		{
			switch (status)
			{
			case TimerStatus.Incomplete:
				currentTime++;
				status = TimerStatus.StartedCounting;
				break;
			case TimerStatus.StartedCounting:
				currentTime++;
				status = TimerStatus.Counting;
				break;
			case TimerStatus.Counting:
				currentTime++;
				break;
			case TimerStatus.JustCompleted:
				status = TimerStatus.Completed;
				break;
			}
			if (currentTime == maxTime)
					status = TimerStatus.JustCompleted;
		}
		
		/// <summary>
		/// Resets the timer to 0 and the status to incomplete.
		/// </summary>
		public void Reset()
		{
			currentTime = 0;
			status = TimerStatus.Incomplete;
		}
	}
}

