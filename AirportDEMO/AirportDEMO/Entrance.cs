using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirportDEMO
{
	public class Entrance
	{
		int baseProcessingTime = 3;
		public Passenger CurrentPassenger
		{
			get
			{
				return currentPassenger;
			}
			set
			{
				timer = TimeSpan.Zero;
				currentPassenger = value;
				switch (currentPassenger.PassengerType)
				{
					case PassengerType.VIPWOLuggage:
					case PassengerType.NormalWOLuggage:
						timeOut = TimeSpan.FromSeconds(baseProcessingTime);
						break;
					case PassengerType.VIPWLuggage:
					case PassengerType.NormalWLuggage:
						timeOut = TimeSpan.FromSeconds(baseProcessingTime * 3);
						break;
					default:
						break;
				}
			}
		}

		public TimeSpan Timer
		{
			get { return timer; }
			set { timer = value; }
		}

		public bool IsFinishedProcessing
		{
			get { return timer >= timeOut; }
		}

		private Passenger currentPassenger;
		private TimeSpan timer;
		private TimeSpan timeOut;

		public void Update(GameTime gameTime)
		{
			if (currentPassenger !=null)
			{
				if (timer > timeOut)
				{
					timer = timeOut;
					CurrentPassenger.GetOutOfTheQueue();
					currentPassenger = null;
				}
				else timer += gameTime.ElapsedGameTime;
			}
		}
	}
}
