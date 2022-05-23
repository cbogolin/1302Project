using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	public class Particle : GameObject
	{
		private float friction, rotationalFriction, rotation;
		private Vector3 position, movement;
		private Timer duration;
		private EntityState state;
		
		public EntityState State
		{
			get { return state; }
		}
		public Vector2 Center
		{
			get { return sprite.Center; }
			set { sprite.Center = value;}
		}
		
		public Particle(Texture2D texture, GraphicsContext graphics, Vector3 position, int duration) : base(texture, graphics, position)
		{
			state = EntityState.Alive;
			this.position = position;
			this.duration = new Timer(duration);
		}
		public Particle(Texture2D texture, GraphicsContext graphics, Vector3 position, int duration, float angle) 
						: this(texture, graphics, position, duration)
		{
			sprite.Rotation = angle;
		}
		public Particle(Texture2D texture, GraphicsContext graphics, Vector3 position, int duration, float angle, Vector3 movement, 
		                float friction) 
						: this(texture, graphics, position, duration, angle)
		{
			this.movement = movement;
			this.friction = friction;
			movement = new Vector3(FMath.Cos(angle), FMath.Sin(angle), 0);
		}
		public Particle(Texture2D texture, GraphicsContext graphics, Vector3 position, int duration, float angle, float movementAngle, 
		                float speed, float friction) 
						: this(texture, graphics, position, duration, angle)
		{
			this.friction = friction;
			movement = new Vector3(FMath.Cos(movementAngle) * speed, FMath.Sin(movementAngle) * speed, 0);
		}
		public Particle(Texture2D texture, GraphicsContext graphics, Vector3 position, int duration, float angle, float movementAngle, 
		                float speed, float friction, float rotation, float rotationalFriction) 
						: this(texture, graphics, position, duration, angle, movementAngle, speed, friction)
		{
			this.rotationalFriction = rotationalFriction;
			this.rotation = rotation;
		}
		
		public override GameObject[] Update ()
		{
			position += movement;
			sprite.Position = position;
			
			#region movement
			if (movement.X != 0)
			{
				if (movement.X > 0)
				{
					movement.X -= friction;
					if (movement.X <= 0)
						movement.X = 0;
				}
				else 
				{
					movement.X += friction;
					if (movement.X >= 0)
						movement.X = 0;
				}
			}
			if (movement.Y != 0)
			{
				if (movement.Y > 0)
				{
					movement.Y -= friction;
					if (movement.Y <= 0)
						movement.Y = 0;
				}
				else 
				{
					movement.Y += friction;
					if (movement.Y >= 0)
						movement.Y = 0;
				}
			}
			#endregion
			
			#region rotation
			if (rotation != 0)
			{
				sprite.Rotation += rotation;
				if (rotation < 0)
				{
					rotation += rotationalFriction;
					if (rotation >= 0)
						rotation = 0;
				}
				else
				{
					rotation -= rotationalFriction;
					if (rotation <= 0)
						rotation = 0;
				}
			}
			#endregion
			
			duration.Update();
			if(duration.Status == TimerStatus.JustCompleted)
				state = EntityState.Dead;
			
			return new GameObject[0];
		}
		
		public override void Render ()
		{
			sprite.Render();
		}
	}
}

