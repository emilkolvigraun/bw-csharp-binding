using BWBinding.Common;

namespace BWBinding.Interfaces
{
    public interface IMessageHandler
    {
        void ResultReceived(Message message);
    }
}
