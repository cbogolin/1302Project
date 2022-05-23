//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using System.IO;

namespace ShawnProject1
{
	public class Melee : Gun
	{
		private float size, spread;
		
		public Melee(GraphicsContext graphics, Vector3 position) 
			: base(new Texture2D(@"\Application\assets\images\guns\assaultrifle.png", false), graphics, position)
		{
			Graphics = graphics;
			Name = "Melee";
			Damage = 10;
			RateOfFire = new Timer(60);
			size = 100;
			spread = .5f;
			BulletTexture = new Texture2D(@"\Application\assets\images\bullets\556bullet.png", false);
			CasingTexture = new Texture2D(@"\Application\assets\images\particles\556casing.png", false);
		}
		
		private Melee(Texture2D texture, GraphicsContext graphics, Vector3 position, string name, int damage,
		              int rateOfFire, float size, float spread, Texture2D weaponTexture, Texture2D casingTexture) 
			: base(texture, graphics, position)
		{
			Graphics = graphics;
			Name = name;
			Damage = damage;
			RateOfFire = new Timer(rateOfFire);
			this.size = size;
			this.spread = spread;
			BulletTexture = weaponTexture;
			CasingTexture = casingTexture;
			Status = GunStatus.CanShoot;
		}
		
		public override GameObject[] Update ()
		{
			List<GameObject> items = new List<GameObject>();
			
			switch (Status)
			{
			case GunStatus.Waiting:
				RateOfFire.Update();
				if (RateOfFire.Status == TimerStatus.JustCompleted)
				{
					Status = GunStatus.CanShoot;
					RateOfFire.Reset();
				}
				break;
			case GunStatus.Shooting:
					Status = GunStatus.Waiting;
				break;
			}
			
			return items.ToArray();
		}
		
		public override GameObject[] Update (Vector3 position, float angle)
		{
			Position = position;
			Angle = angle;
			
			return Update();
		}
		
		public override GameObject[] Shoot (float angle)
		{
			List<GameObject> items = new List<GameObject>();
			
			Angle = angle;
			CurrentBurst++;
			Status = GunStatus.Shooting;
			
			float swingAngle = (float)(angle - .5);
			Particle swing = new Particle(BulletTexture, Graphics, Position, 3, angle, 0, 0, 0, .5f, 0);
			swing.Center = new Vector2(.5f, 1);
			
			Bullet bullet = new Swing(BulletTexture, Graphics, Position, angle, Damage, size, spread);
			
			items.Add(swing);
			items.Add(bullet);
			
			return items.ToArray();
		}
		
		public static Gun Create(GraphicsContext graphics, string filePath)
		{
			
			using (StreamReader reader = new StreamReader(filePath))
			{
				string[] info = new string[9];
				for (int i = 0; i < info.Length; i++)
				{
					string line;
					do
					{
						line = reader.ReadLine();
					}while (line.Contains("###"));
					info[i] = line;
				}
				
				Texture2D bulletTexture = new Texture2D(info[6], false), casingTexture = new Texture2D(info[7], false), 
					groundTexture = new Texture2D(info[8], false);
				
				return new Melee(groundTexture, graphics, Vector3.Zero, info[1], int.Parse(info[2]), int.Parse(info[3]), 
				                        int.Parse(info[4]), float.Parse(info[5]), bulletTexture, casingTexture);
			}
			
			return new AssaultRifle(graphics, Vector3.Zero);
		}
	}
}

