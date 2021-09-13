#region File Description
//-----------------------------------------------------------------------------
// Particle.cs
// Matt McGrath, modified version of Whitaker's tutorial.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.ParticleSystem
{
     public class Particle
     {
          #region Fields

          protected Texture2D texture;
          protected Vector2 position;
          protected Vector2 velocity;
          protected float angle;
          protected float angularVelocity;
          protected Color color;
          protected float size;
          protected int timeToLive;

          #endregion

          #region Properties

          public Texture2D Texture 
          {
               get { return texture; }
               set { texture = value; }
          }

          public Vector2 Position 
          {
               get { return position; }
               set { position = value; }
          }

          public Vector2 Velocity 
          {
               get { return velocity; }
               set { velocity = value; }
          }

          public float Angle 
          {
               get { return angle; }
               set { angle = value; }
          }

          public float AngularVelocity 
          {
               get { return angularVelocity; }
               set { angularVelocity = value; }
          }

          public Color Color 
          {
               get { return color; }
               set { color = value; }
          }

          public float Size 
          {
               get { return size; }
               set { size = value; } 
          }

          public int LifeSpan 
          {
               get { return timeToLive; }
               set { timeToLive = value; } 
          }

          #endregion

          #region Initialization

          public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
              float angle, float angularVelocity, Color color, float size, int ttl)
          {
               Texture = texture;
               Position = position;
               Velocity = velocity;
               Angle = angle;
               AngularVelocity = angularVelocity;
               Color = color;
               Size = size;
               LifeSpan = ttl;
          }

          #endregion

          #region Update

          public virtual void Update()
          {
               LifeSpan--;
               Position += Velocity;
               Angle += AngularVelocity;
          }
          
          #endregion

          #region Draw

          public virtual void Draw(SpriteBatch spriteBatch)
          {
               Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
               Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

               spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                   Angle, origin, Size, SpriteEffects.None, 0f);
          }

          #endregion
     }
}
