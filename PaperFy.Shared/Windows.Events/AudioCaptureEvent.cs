using System;

namespace PaperFy.Shared.Windows.Events
{
	public class AudioCaptureEvent
	{
		public byte[] Buffer { get; }

		public int Sequence { get; }

		public Guid SessionID { get; }

		public AudioCaptureEvent(Guid sessionID, int sequence, byte[] buffer)
		{
			Buffer = buffer;
			Sequence = sequence;
			SessionID = sessionID;
		}
	}
}
