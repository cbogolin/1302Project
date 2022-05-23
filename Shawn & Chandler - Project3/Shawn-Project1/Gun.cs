//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ShawnProject1
{
	[Serializable]
	public class Gun : GameObject
	{
		private int damage, burstCount, currentBurst, ammo, ammoMax;
		private float angle, accuracy;
		private string name;
		private Timer burstDelay, rateOfFire;
		private Texture2D bulletTexture, casingTexture, groundTexture;
		private Vector3 position;
		private GraphicsContext graphics;
		private GunStatus status;
		private FireType fireType;
		
		private Random ran = new Random();
		
		public int Damage
		{
			get { return damage; }
			set { damage = value; }
		}
		public int BurstCount
		{
			get { return burstCount; }
			set { burstCount = value; }
		}
		public int CurrentBurst
		{
			get { return currentBurst; }
			set { currentBurst = value; }
		}
		public int Ammo
		{
			get { return ammo; }
			set { ammo = value; }
		}
		public int AmmoMax
		{
			get { return ammoMax; }
			set { ammoMax = value; }
		}
		public float Angle
		{
			get { return angle; }
			set { angle = value; }
		}
		public float Accuracy
		{
			get { return accuracy; }
			set { accuracy = value; }
		}
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		public Timer BurstDelay
		{
			get { return burstDelay; }
			set { burstDelay = value; }
		}
		public Timer RateOfFire
		{
			get { return rateOfFire; }
			set { rateOfFire = value; }
		}
		public Texture2D BulletTexture
		{
			get { return bulletTexture; }
			set { bulletTexture = value; }
		}
		public Texture2D CasingTexture
		{
			get { return casingTexture; }
			set { casingTexture = value; }
		}
		public Vector3 Position
		{
			get { return sprite.Position; }
			set { sprite.Position = value; }
		}
		public GraphicsContext Graphics
		{
			get { return graphics; }
			set { graphics = value; }
		}
		public GunStatus Status
		{
			get { return status; }
			set { status = value; }
		}
		public FireType FireType
		{
			get { return fireType; }
			set { fireType = value; }
		}
		public Random Ran
		{
			get { return ran; }
		}
		
		public Gun (Texture2D texture, GraphicsContext graphics, Vector3 position) : base(texture, graphics, position)
		{
			groundTexture = texture;
		}
		
		public override GameObject[] Update ()
		{
			return new GameObject[0];
		}
		public virtual GameObject[] Update(Vector3 position, float angle)
		{
			return new GameObject[0];
		}
		public virtual GameObject[] Shoot(float angle)
		{
			return new GameObject[0];
		}
		public override void Render ()
		{
			sprite.Render();
		}
		public virtual void Unchanber()
		{
			status = GunStatus.Waiting;
		}
		public static Gun Clone(Gun gun)
		{
			if (gun is HandGun)
			{
				HandGun newGun = gun as HandGun;
				return new HandGun(gun.groundTexture, gun.graphics, gun.position, gun.name, gun.damage,
				                   gun.burstCount, gun.ammoMax, gun.accuracy, gun.burstDelay.Duration, 
				                   gun.rateOfFire.Duration, newGun.Reload.Duration, gun.fireType, 
				                   gun.bulletTexture, gun.casingTexture);
			}
			return null;
		}
		
		
		/// <summary>
		/// Creates a gun from a file.
		/// </summary>
		/// <returns>
		/// Gun created from the file.
		/// </returns>
		/// <param name='graphics'>
		/// Graphics.
		/// </param>
		/// <param name='filePath'>
		/// File path.
		/// </param>
		public static Gun FromFile(GraphicsContext graphics, string filePath)
		{
			Gun gun = new Gun(new Texture2D(@"\Application\assets\images\Antibody.png", false), graphics, Vector3.Zero);
			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				do
				{
					line = reader.ReadLine();
				} while (line.Contains("###") && line != "");
					
				switch (line.ToLower())
				{
				case "assaultrifle":
					gun = AssaultRifle.Create(graphics, filePath);
					break;
				case  "shotgun":
					gun = ShotGun.Create(graphics, filePath);
					break;
				case "spiralgun":
					gun = SpiralGun.Create(graphics, filePath);
					break;
				case "lasergun":
					gun = LaserGun.Create(graphics, filePath);
					break;
				case "discolaser":
					gun = DiscoLaser.Create(graphics, filePath);
					break;
				case "minigun":
					gun = MiniGun.Create(graphics, filePath);
					break;
				case "handgun":
					gun = HandGun.Create(graphics, filePath);
					break;
				case "smg":
					gun = SMG.Create(graphics, filePath);
					break;
				case "melee":
					gun = Melee.Create(graphics, filePath);
					break;
				}
			}
			return gun;
		}
	}
}