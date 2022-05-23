//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	/// <summary>
	/// Follows the player and stays a good distance away.
	/// Has a lot of health to block bullets.
	/// </summary>
	public class Shielder : Enemy
	{
		float distance, speed;
		private static Player player;
		
		public Shielder(Texture2D texture, GraphicsContext graphics, Vector3 position, float speed, float distance) 
			: this(texture, graphics, position, 65, speed, distance)
		{
			
		}
		
		public Shielder(Texture2D texture, GraphicsContext graphics, Vector3 position, int health, float speed, 
		                float distance) 
			: base(texture, graphics, position, health)
		{
			this.distance = distance;
			this.speed = speed;
			sprite.SetColor(0, 0, 1, 1);
		}
		
		public override GameObject[] Update ()
		{
			sprite.SetColor(0, 0, 1, 1);
			float angle = FMath.Atan2(player.Position.Y - sprite.Position.Y, player.Position.X - sprite.Position.X);
			Vector3 displacement = new Vector3(FMath.Cos(angle) * speed, FMath.Sin(angle) * speed, 0);
			sprite.Rotation = angle;
			
			if (sprite.Position.DistanceSquared(player.Position) > distance * distance)
				Move(displacement);
			
			return new GameObject[0];
		}
		
		public override void Hit (int damage)
		{
			base.Hit (damage);
			sprite.SetColor(0, 0, .5f, 1);
		}
		
		public static new void SetPlayer(ref Player trackedPlayer)
		{
			player = trackedPlayer;
		}
	}
}

