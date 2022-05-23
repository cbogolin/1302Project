//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace ShawnProject1
{
	public class Swing : Bullet
	{
		private float size, angle, spread;
		private bool alive;
		
		public Swing (Texture2D texture, GraphicsContext graphics, Vector3 position, float angle, int damage, float size,
		              float spread) 
			: base(texture, graphics, position, angle, 0, damage, 1)
		{
			this.size = size;
			this.angle = angle;
			this.spread = spread;
			alive = false;
		}
		
		public override GameObject[] Hit ()
		{
			return new GameObject[0];
		}
		
		public override GameObject[] Update ()
		{
			if (!alive)
				Kill();
			alive = false;
			return new GameObject[0];
		}
		
		public bool Collides(Vector3 position, float radius)
		{
			//if (State == EntityState.Alive)
			{
				float enemyAngle = FMath.Atan2(position.Y - Position.Y, position.X - Position.X);
				if (enemyAngle > angle - spread && enemyAngle < angle + spread && Position.Distance(position) < size + radius)
					return true;
				for (int i = 0; i < 2; i++)
				{
					Vector3 startPoint = sprite.Position, closestPoint;
					Vector3 endPoint = new Vector3(startPoint.X + FMath.Cos(angle + (spread * (2 * i - 1))) * size, 
				                       			   startPoint.Y + FMath.Sin(angle + (spread * (2 * i - 1))) * size, 0);
					float xDistance = endPoint.X - startPoint.X, yDistance = endPoint.Y - startPoint.Y;
				
				    float distance = ((position.X - startPoint.X) * xDistance + (position.Y - startPoint.Y) * yDistance) /
				        (xDistance * xDistance + yDistance * yDistance);
				
				    if (distance < 0)
				    {
				        xDistance = position.X - startPoint.X;
				        yDistance = position.Y - startPoint.Y;
				    }
				    else if (distance > 1)
				    {
				        xDistance = position.X - endPoint.X;
				        yDistance = position.Y - endPoint.Y;
				    }
				    else
				    {
						closestPoint = new Vector3(startPoint.X + distance * xDistance, 
						                           startPoint.Y + distance * yDistance, 0);
				        xDistance = position.X - closestPoint.X;
				        yDistance = position.Y - closestPoint.Y;
				    }
					
					if (Math.Sqrt(xDistance * xDistance + yDistance * yDistance) < radius)
				    	return true;
				}
			}
			return false;
		}
		
		public override void Render()
		{
		}
	}
}

