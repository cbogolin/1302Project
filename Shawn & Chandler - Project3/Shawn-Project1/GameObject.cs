//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	[Serializable]
	public abstract class GameObject
	{
		protected Sprite sprite;
		
		public virtual float Top
		{
			get { return sprite.Position.Y - sprite.Height / 2; }
		}
		public virtual float Bottom
		{
			get { return sprite.Position.Y + sprite.Height / 2; }
		}
		public virtual float Left
		{
			get { return sprite.Position.X - sprite.Width / 2; }
		}
		public virtual float Right
		{
			get { return sprite.Position.X + sprite.Width / 2; }
		}
		public virtual float Width
		{
			get { return sprite.Width; }
		}
		public virtual float Height
		{
			get { return sprite.Height; }
		}
		public virtual float Radius
		{
			get 
			{ 
				if (Width > Height)
					return Height / 2;
				return Width / 2;
			}
		}
		public virtual Vector3 Position
		{
			get { return sprite.Position; }
		}
		
		public GameObject (Texture2D texture, GraphicsContext graphics, Vector3 position)
		{
			sprite = new Sprite(graphics, texture);
			sprite.Position = position;
			sprite.Center = new Vector2(.5f);
		}
		
		public abstract GameObject[] Update();
		public abstract void Render();
	}
}

