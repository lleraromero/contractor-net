using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
	public class Mp3Player
	{
		public const int capacity = 50;
		public bool playing;
		public bool pause;
		public int currentSong;

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(playing || currentSong == 0);
			Contract.Invariant(!pause || playing);
			Contract.Invariant(currentSong >= 0);
			Contract.Invariant(currentSong <= capacity);
		}

		public Mp3Player()
		{
			//Contract.Ensures(!playing && !pause && currentSong == 0);

			playing = false;
			pause = false;
			currentSong = 0;
		}

		public void Play()
		{
			Contract.Requires(!playing || pause);
			//Contract.Ensures(playing && !pause);

			playing = true;
			pause = false;
		}

		public void Stop()
		{
			Contract.Requires(playing);
			//Contract.Ensures(!playing && !pause && currentSong == 0);

			playing = false;
			pause = false; // la ausencia de esta linea podria ser un error para ser detectado con la abstraccion
			currentSong = 0; // esta tambien podria faltar
		}

		public void Pause()
		{
			Contract.Requires(playing);
			//Contract.Ensures(pause == !Contract.OldValue(pause));

			pause = !pause;
		}

		public void Previous()
		{
			Contract.Requires(playing);
			Contract.Requires(currentSong > 0);
			//Contract.Ensures(currentSong + 1 == Contract.OldValue(currentSong));

			currentSong--;
		}

		public void Next()
		{
			Contract.Requires(playing);
			Contract.Requires(currentSong < capacity);
			//Contract.Ensures(currentSong == Contract.OldValue(currentSong) + 1);

			currentSong++;
		}
	}
}
