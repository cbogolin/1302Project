//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;

namespace ShawnProject1
{
	public class Bullet : GameObject
	{
		private int damage;
		private Vector3 movement;
		private Timer duration;
		private EntityState state;
		
		public override Vector3 Position
		{
			get { return sprite.Position; }
		}
		public int Damage
		{
			get { return damage; }
		}
		public EntityState State
		{
			get { return state; }
		}
		
		public Bullet (Texture2D texture, GraphicsContext graphics, Vector3 position) : base(texture, graphics, position)
		{}
		
		public Bullet (Texture2D texture, GraphicsContext graphics, Vector3 position, float angle, 
		               float speed, int damage, int duration) 
					   : this(texture, graphics, position)
		{
			this.damage = damage;
			this.duration = new Timer(duration);
			movement = new Vector3(FMath.Cos(angle) * speed, FMath.Sin(angle) * speed, 0);
			sprite.Rotation = angle + FMath.PI / 2;
			state = EntityState.Alive;
		}
	
		public override GameObject[] Update ()
		{
			Move();
			duration.Update();
			if (duration.Status == TimerStatus.JustCompleted)
				state = EntityState.Dead;
			return new GameObject[0];
		}
		
		public void Move()
		{
			sprite.Position += movement;
		}
		
		public virtual GameObject[] Hit()
		{
			state = EntityState.Dead;
			return new GameObject[0];
		}
		
		public void Kill()
		{
			state = EntityState.Dead;
		}
		
		public override void Render ()
		{
			sprite.Render();
		}
	}
}