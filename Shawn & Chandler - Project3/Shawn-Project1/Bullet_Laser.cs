//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	public class Laser : Bullet
	{
		float angle;
		Vector3 endPoint;
		bool alive;
		
		public float Angle
		{
			get { return angle; }
		}
		
		public Laser(Texture2D texture, GraphicsContext graphics, Vector3 position, float angle, int damage,
		             Vector4 color)
			: base (texture, graphics, position, angle, 0, damage, 1)
		{
			this.angle = angle;
			alive = true;
			sprite.Center = new Vector2(.5f, 1);
			sprite.Rotation = angle + FMath.PI / 2;
			sprite.SetColor(color.X, color.Y, color.Z, color.W);
			sprite.Scale = new Vector2(1, graphics.Screen.Width / sprite.Height);
			endPoint = new Vector3(position.X + FMath.Cos(angle) * sprite.Height, 
			                       position.Y + FMath.Sin(angle) * sprite.Height, 0);
		}
		
		public override GameObject[] Update()
		{
			if (!alive)
				Kill();
			alive = false;
			return new GameObject[0];
		}
		
		public bool Collides(Vector3 position, float radius)
		{
			if (State == EntityState.Alive)
			{
				Vector3 startPoint = sprite.Position, closestPoint;
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
				else
					return false;
			}
			return false;
		}
	}
}

