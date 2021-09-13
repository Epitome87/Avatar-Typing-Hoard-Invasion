#region File Description
//-----------------------------------------------------------------------------
// ParticleEngine.cs
// Matt McGrath, modified version of Whitaker's tutorial.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.ParticleSystem
{
     public class ParticleEngine
     {
          #region Fields

          private Random random;
          private Vector2 emitterLocation;
          private List<Particle> particles;
          private List<Texture2D> textures;

          #endregion

          #region Properties

          public Vector2 EmitterLocation
          {
               get { return emitterLocation; }
               set { emitterLocation = value; }
          }

          #endregion

          #region Initialization

          public ParticleEngine(List<Texture2D> textures, Vector2 location)
          {
               EmitterLocation = location;
               this.textures = textures;
               this.particles = new List<Particle>();
               random = new Random();
          }

          #endregion

          #region Update

          public void Update()
          {
               int total = 10;

               for (int i = 0; i < total; i++)
               {
                    particles.Add(GenerateNewParticle());
               }

               for (int particle = 0; particle < particles.Count; particle++)
               {
                    particles[particle].Update();
                    if (particles[particle].LifeSpan <= 0)
                    {
                         particles.RemoveAt(particle);
                         particle--;
                    }
               }
          }

          #endregion

          #region Draw

          public void Draw(SpriteBatch spriteBatch)
          {
               spriteBatch.Begin();
               for (int index = 0; index < particles.Count; index++)
               {
                    particles[index].Draw(spriteBatch);
               }
               spriteBatch.End();
          }

          #endregion

          #region Helper Methods

          private Particle GenerateNewParticle()
          {
               Texture2D texture = textures[random.Next(textures.Count)];
               Vector2 position = EmitterLocation;
               Vector2 velocity = new Vector2(
                                       1f * (float)(random.NextDouble() * 2 - 1),
                                       1f * (float)(random.NextDouble() * 2 - 1));
               float angle = 0;
               float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
               Color color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
               float size = (float)random.NextDouble();
               int ttl = 20 + random.Next(40);

               return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
          }

          #endregion
     }
}
