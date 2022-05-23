//Shawn Carter

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	public class Ranger : Enemy
	{
		private float distance, speed;
		private Gun gun;
		
		private static List<Gun> guns;
		private static Player player;
		
		public Ranger(Texture2D texture, GraphicsContext graphics, Vector3 position) 
			: this(texture, graphics, position, 20, 2, 500, guns[ran.Next(guns.Count)])
		{
			
		}
		
		public Ranger(Texture2D texture, GraphicsContext graphics, Vector3 position, int health, float speed, 
		                float distance, Gun gun) : base(texture, graphics, position)
		{
			this.distance = distance;
			this.speed = speed;
			this.gun = Gun.Clone(gun);
			sprite.SetColor(1, 1, 0, 1);
			
			
//			if (gun is HandGun)
//			{
//				this.gun = new HandGun(graphics, position);
//				this.gun = (gun as HandGun).Clone(this.gun as HandGun);
//			}
			
			
			gun.Unchanber();
		}
		
		public override GameObject[] Update ()
		{
			sprite.SetColor(1, 1, 0, 1);
			List<GameObject> items = new List<GameObject>();
			float angle = FMath.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
			
			sprite.Rotation = angle;
			Vector3 displacement = new Vector3(FMath.Cos(angle) * speed, FMath.Sin(angle) * speed, 0);
			
			items.AddRange(gun.Update(Position, angle));
			
			if (sprite.Position.DistanceSquared(player.Position) > distance * distance)
				Move(displacement);
			else
				if (gun.Status == GunStatus.CanShoot)
					items.AddRange(gun.Shoot(angle));
			
			return items.ToArray();
		}
		
		public static void SetPlayer(ref Player trackedPlayer)
		{
			player = trackedPlayer;
		}
		
		public static void AddGun(Gun gun)
		{
			if (guns == null)
				guns = new List<Gun>();
			guns.Add(gun);
		}
		
		private void SetGun(Gun gun)
		{
			this.gun = gun;
		}
	}
}

