//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace ShawnProject1
{
	public class SpiralBullet : Bullet
	{
		int splitDamage, splitDuration;
		float splitAngle, splitAngleChange;
		Texture2D texture;
		GraphicsContext graphics;
		Timer splitTimer;
		
		public SpiralBullet (Texture2D texture, GraphicsContext graphics, Vector3 position, float angle, float speed, int damage, 
		                     int duration, int splitDelay, int splitDamage, int splitDuration, float splitAngle) 
					   : base(texture, graphics, position, angle, speed, damage, duration)
		{
			this.splitDamage = splitDamage;
			this.texture = texture;
			this.graphics = graphics;
			this.splitDuration = splitDuration;
			splitTimer = new Timer(splitDelay);
			this.splitAngle = angle;
			splitAngleChange = splitAngle;
			sprite.Rotation = angle + FMath.PI / 2;
		}
	
		public override GameObject[] Update ()
		{
			List<GameObject> items = new List<GameObject>();
			
			Move();
			splitTimer.Update();
			
			if (splitTimer.Status == TimerStatus.JustCompleted)
			{
				splitTimer.Reset();
				Bullet bullet = new Bullet(texture, graphics, sprite.Position, splitAngle, 7.5f, splitDamage, 
				                           splitDuration);
				items.Add(bullet);
				splitAngle += splitAngleChange;
			}
			
			return items.ToArray();
		}
	}
}

