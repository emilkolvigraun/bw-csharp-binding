using BWBinding.Common;

namespace BWBinding.Interfaces
{
    public interface IMessageHandler
    {
        Message message { get; }
        bool received { get; }
        void ResultReceived(Message message);
    }
}
