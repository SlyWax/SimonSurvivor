//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class BallSequenceGenerator
	{
		public IList<BallColor> sequence { get; private set; }
		private Random random;

		public BallSequenceGenerator ()
		{
			sequence = new List<BallColor> ();
			random = RandomSingleton.getInstance().random;
		}

		public BallSequenceGenerator addNewBall() {
			BallColor color;
			do {
				color = GenerateRandomColor();
			} while (!IsAcceptable(color));
			sequence.Add (color);
			return this;
		}

		private BallColor GenerateRandomColor() {
			var availableColors = Enum.GetValues(typeof(BallColor)).Cast<BallColor>();
			return availableColors.ElementAt (random.Next (availableColors.Count ()));
		}

		private bool IsAcceptable(BallColor color) {
			return (sequence.Count < 2 || !(sequence.Reverse().Take(2).All(c => c == color)));
		}

        public BallSequenceGenerator reset()
        {
            sequence = new List<BallColor>();
            return this;
        }

        public BallColor getLastColor()
        {
            return sequence[sequence.Count - 1];
        }
	}
}

