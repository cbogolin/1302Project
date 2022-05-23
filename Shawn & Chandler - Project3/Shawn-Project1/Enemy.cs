//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	public class Enemy : GameObject
	{
		private int health;
		private const float HEIGHT = 50, WIDTH = 50, RADIUS = 50;
		private Vector3 position, movement;
		private GraphicsContext graphics;
		private EntityState state;
		
		protected static Random ran = new Random();
		
		public override float Top
		{
			get { return position.Y - HEIGHT / 2; }
		}
		public override float Bottom
		{
			get { return position.Y + HEIGHT / 2; }
		}
		public override float Left
		{
			get { return position.X - WIDTH / 2; }
		}
		public override float Right
		{
			get { return position.X + WIDTH / 2; }
		}
		public override float Width
		{
			get { return WIDTH; }
		}
		public override float Height
		{
			get { return HEIGHT; }
		}
		public override float Radius
		{
			get { return RADIUS; }
		}
		public override Vector3 Position
		{
			get { return position; }
		}
		public EntityState State
		{
			get { return state; }
		}
		
		public Enemy (Texture2D texture, GraphicsContext graphics, Vector3 position) : this(texture, graphics, position, 25)
		{
			
		}
		
		public Enemy (Texture2D texture, GraphicsContext graphics, Vector3 position, int health) : 
			base(texture, graphics, position)
		{
			this.position = position;
			this.graphics = graphics;
			this.movement = new Vector3(ran.Next(1, 11), ran.Next(1, 11), 0);
			this.health = health;
			
			sprite.Scale = new Vector2(WIDTH / sprite.Width, HEIGHT / sprite.Height);
			sprite.SetColor(1, 0, 0, 1);
		}
		
		public override GameObject[] Update ()
		{
			Move (movement);
			sprite.SetColor(1, 0, 0, 1);
			
			if (Top == 0 || Bottom == graphics.Screen.Height)
				movement.Y *= -1;
			if (Left == 0 || Right == graphics.Screen.Width)
				movement.X *= -1;
				
			sprite.Rotation = FMath.Atan2(movement.Y, movement.X);
			
			return new GameObject[0];
		}
		
		public virtual void Move(Vector3 displacement)
		{
			position += displacement;
			
			if (Left < 0)
				position.X = WIDTH / 2;
			else if (Right > graphics.Screen.Width)
				position.X = graphics.Screen.Width - WIDTH / 2;
			if (Top < 0)
				position.Y = HEIGHT / 2;
			else if (Bottom > graphics.Screen.Height)
				position.Y = graphics.Screen.Height - HEIGHT / 2;
			
			sprite.Position = position;
		}
		
		public virtual void Hit(int damage)
		{
			health -= damage;
			if (health <= 0)
				state = EntityState.Dead;
			sprite.SetColor(.5f, 0, 0, 1);
		}
		
		public override void Render ()
		{
			sprite.Render();
		}
		
		public static void SetPlayer(ref Player trackedPlayer)
		{
			Tracker.SetPlayer(ref trackedPlayer);
			Shielder.SetPlayer(ref trackedPlayer);
			Ranger.SetPlayer(ref trackedPlayer);
		}
	}
}

