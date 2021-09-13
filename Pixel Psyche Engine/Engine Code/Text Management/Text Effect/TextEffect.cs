#region File Description
//-----------------------------------------------------------------------------
// TextEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.Text
{
     /// <remarks>
     /// Defines a base class which allows one to customize the way in which 
     /// a Text Object is presented and displayed to the screen.
     /// </remarks>
     public abstract class TextEffect
     {
          #region Fields

          protected string text;
          protected float runTime;
          protected float step;

          #endregion

          #region Properties

          /// <summary>
          /// A String representing the Text to be displayed.
          /// </summary>
          public string Text
          {
               get { return text; }
               set { text = value; }
          }

          /// <summary>
          /// Amount of milliseconds the TextEffect should take to complete
          /// its custom display process.
          /// </summary>
          public float RunTime
          {
               get { return runTime; }
               set { runTime = value; }
          }

          /// <summary>
          /// Helper timing variable used to internally calculate the amount of
          /// milliseconds that each "step" (whose definition depends on the TextEffect
          /// type) is to last.
          /// </summary>
          protected float Step
          {
               get { return step; }
               set { step = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// TextEffect Constructor.
          /// </summary>
          /// <param name="runTime">How long (milliseconds) the effect runs.</param>
          
          protected TextEffect(float runTime)
          {
               this.RunTime = runTime;
          }

          #endregion

          #region Virtual Methods

          /// <summary>
          /// Override this method with the custom TextEffect's 
          /// Initialize method.
          /// </summary>
          public virtual void Initialize() { }

          /// <summary>
          /// Override this method with the custom TextEffect's Update logic.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public virtual void Update(GameTime gameTime, TextObject textObj) { }

          /// <summary>
          /// Override this method with the custom TextEffect's Draw logic.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          /// <param name="textObj">The Text Object to Draw.</param>
          public virtual void Draw(GameTime gameTime, TextObject textObj) { }

          #endregion
     }
}