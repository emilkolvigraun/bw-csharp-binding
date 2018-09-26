using BWBinding.Common;

namespace BWBinding.Interfaces
{
    public interface IResponseHandler
    {
        Response result { get; }
        bool received { get; }
        void ResponseReceived(Response result);
    }
}
