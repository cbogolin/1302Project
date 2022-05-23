//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	/// <summary>
	/// Follows the player and inflicts contact damage.
	/// </summary>
	public class Tracker : Enemy
	{
		private static Player player;
		private float speed;
		
		public Tracker(Texture2D texture, GraphicsContext graphics, Vector3 position, float speed) 
			: this(texture, graphics, position, speed, 20)
		{
			
		}
		
		public Tracker(Texture2D texture, GraphicsContext graphics, Vector3 position, float speed, int health) 
			: base(texture, graphics, position, health)
		{
			this.speed = speed;
			sprite.SetColor(0, 1, 0, 1);
		}
		
		public override GameObject[] Update ()
		{
			sprite.SetColor(0, 1, 0, 1);
			
			float angle = FMath.Atan2(player.Position.Y - sprite.Position.Y, player.Position.X - sprite.Position.X);
			Vector3 displacement = new Vector3(FMath.Cos(angle) * speed, FMath.Sin(angle) * speed, 0);
			sprite.Rotation = angle;
			
			if (sprite.Position.DistanceSquared(player.Position) > 4f)
				Move(displacement);
			
			return new GameObject[0];
		}
		
		public override void Hit (int damage)
		{
			base.Hit (damage);
			sprite.SetColor(0, .5f, 0, 1);
		}
		
		public static void SetPlayer(ref Player trackedPlayer)
		{
			player = trackedPlayer;
		}
	}
}

