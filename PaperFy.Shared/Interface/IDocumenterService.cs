using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
	public interface IDocumenterService : IDisposable
	{
		const int MaximumScreenshots = 200;

		bool IsIdle { get; }

		bool IsRunning { get; }

		bool IsTranscribingAudio { get; }

		long? LastMaximized { get; set; }

        DocumentingState State { get; }

		void CancelDocumenting();

		void PauseDocumenting();

		void ResumeDocumenting();

		void StartDocumenting(bool IsTranscribingAudio);

		Task StopDocumenting();
	}
}
