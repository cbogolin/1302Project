//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;
using System.Collections.Generic;

namespace ShawnProject1
{
	public class Player : GameObject
	{
		private int activeGun, health;
		private float speed, angle;
		private Vector3 position;
		private List<Gun> guns;
		private GraphicsContext graphics;
		private Timer semiAutoTimer, invulnTimer;
		
		private static Random ran = new Random();
		
		public int Health
		{
			get { return health; }
		}
		public Gun SelectedGun
		{
			get 
			{ 
				if (guns.Count > 0)
					return guns[activeGun]; 
				return null;
			}
		}
		public TimerStatus status
		{
			get { return invulnTimer.Status; }
		}
		public Gun[] Guns
		{
			get { return guns.ToArray(); }
		}
		public override Vector3 Position
		{
			get { return position; }
		}
		
		public Player (Texture2D texture, GraphicsContext graphics, Vector3 position) : base (texture, graphics, position)
		{
			this.graphics = graphics;
			this.position = position;
			sprite.Scale = new Vector2(50 / sprite.Width, 50 / sprite.Height);
			guns = new List<Gun>();
			speed = 5;
			angle = 0;
			activeGun = 0;
			health = 5;
			invulnTimer = new Timer(45);
			semiAutoTimer = new Timer(4);
		}
		
		public override GameObject[] Update()
		{
			return null;
		}
		
		public GameObject[] Update(GamePadData gamePadData)
		{
			bool shootSemiAuto = false;
			List<GameObject> items = new List<GameObject>();
			
			Vector2 movement = Vector2.Zero, aiming = Vector2.Zero;
			
			foreach(Gun g in guns)
				items.AddRange(g.Update(position, angle));
			
			invulnTimer.Update();
			if (invulnTimer.Status == TimerStatus.JustCompleted)
			{
				sprite.SetColor(1, 1, 1, 1);
			}
			
			#region movement
			if ((gamePadData.Buttons & GamePadButtons.Triangle) != 0)
				movement.Y--;
			if ((gamePadData.Buttons & GamePadButtons.Cross) != 0)
				movement.Y++;
			if ((gamePadData.Buttons & GamePadButtons.Circle) != 0)
				movement.X++;
			if ((gamePadData.Buttons & GamePadButtons.Square) != 0)
				movement.X--;
			
			if (movement.Y != 0 || movement.X != 0)
				Move(movement);
			#endregion
			
			#region aiming
			if (semiAutoTimer.Status == TimerStatus.Incomplete) 
			{
				if ((gamePadData.ButtonsDown & GamePadButtons.Up) != 0) 
					shootSemiAuto = true;
				else if ((gamePadData.ButtonsDown & GamePadButtons.Down) != 0)
					shootSemiAuto = true;
				else if ((gamePadData.ButtonsDown & GamePadButtons.Left) != 0)
					shootSemiAuto = true;
				else if ((gamePadData.ButtonsDown & GamePadButtons.Right) != 0)
					shootSemiAuto = true;
			}
			
			if ((gamePadData.Buttons & GamePadButtons.Down) != 0)
				aiming.Y++;
			if ((gamePadData.Buttons & GamePadButtons.Up) != 0)
				aiming.Y--;
			if ((gamePadData.Buttons & GamePadButtons.Right) != 0)
				aiming.X++;
			if ((gamePadData.Buttons & GamePadButtons.Left) != 0)
				aiming.X--;
			
			if (aiming.X != 0 || aiming.Y != 0)
			{
				Aim (aiming);
				if (guns.Count > 0)
				{
					if (SelectedGun.Status == GunStatus.CanShoot)
					{
						if (SelectedGun.FireType == FireType.Automatic || SelectedGun.FireType == FireType.Burst)
							items.AddRange(SelectedGun.Shoot(angle));
						
						//Gives the user time to aim diagonal shots before firing
						else
						{
							if (shootSemiAuto)
								semiAutoTimer.Update();
							if (semiAutoTimer.Status != TimerStatus.Incomplete)
								semiAutoTimer.Update();
							if (semiAutoTimer.Status == TimerStatus.JustCompleted)
							{
								items.AddRange(SelectedGun.Shoot(angle));
								semiAutoTimer.Reset();
							}
						}
					}
				}
			}
			
			#endregion
			
			#region swapping weapons
			if (guns.Count > 0)
			{
				if (SelectedGun.Status == GunStatus.CanShoot || SelectedGun.Status == GunStatus.Waiting || 
				    SelectedGun.Status == GunStatus.OutOfAmmo)
				{
					if ((gamePadData.ButtonsDown & GamePadButtons.Select) != 0)
						activeGun = 0;
					else
					{
						int currentIndex = activeGun, indexChange = 0;
						if (gamePadData.ButtonsDown == GamePadButtons.L)
							indexChange--;
						if (gamePadData.ButtonsDown == GamePadButtons.R)
							indexChange++;
						
						if (indexChange != 0)
							do
							{
								activeGun += indexChange;
								if (activeGun >= guns.Count)
									activeGun = 0;
								else if (activeGun < 0)
									activeGun = guns.Count - 1;
							}while (activeGun != currentIndex && SelectedGun.Status == GunStatus.OutOfAmmo);
					}
				}
			}
			#endregion
			
			return items.ToArray();
		}
		
		private void Move(Vector2 movement)
		{
			float movementAngle = FMath.Atan2(movement.Y, movement.X);
			position.X += FMath.Cos(movementAngle) * speed;
			position.Y += FMath.Sin(movementAngle) * speed;
			sprite.Position = position;
			
			float width = sprite.Width, height = sprite.Height;
			
			if (position.X - width / 2 < 0)
				position.X = width / 2;
			else if (position.X > graphics.Screen.Width - width / 2)
				position.X = graphics.Screen.Width - width / 2;
			if (position.Y - height / 2 < 0)
				position.Y = height / 2;
			else if (position.Y > graphics.Screen.Height - height / 2)
				position.Y = graphics.Screen.Height - height / 2;
		}
		
		private void Aim(Vector2 aiming)
		{
			angle = FMath.Atan2(aiming.Y, aiming.X);
			sprite.Rotation = angle;
		}
		
		public void Hit()
		{
			if (invulnTimer.Status == TimerStatus.Completed || invulnTimer.Status == TimerStatus.JustCompleted)
			{
				health--;
				sprite.SetColor(1, 1, 1, .5f);
				invulnTimer.Reset();
			}
		}
		
		public void AddGun(Gun gun)
		{
			if (guns.Count > 0 && gun is HandGun)
			{
				if (guns[0].Name != "Handgun")
				{
					if (!(guns[0] is HandGun))
					{
						activeGun++;
						guns.Add(gun);
						for (int i = guns.Count - 1; i > 0; i--)
						{
							guns[i] = guns[i - 1];
						}
					}
					guns[0] = gun;
				}
			}
				
			else
				guns.Add(gun);
		}
		
		public override void Render ()
		{
			sprite.Render();
		}
	}
}