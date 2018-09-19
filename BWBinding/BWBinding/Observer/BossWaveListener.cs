using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BWBinding.Common;
using BWBinding.Control;
using BWBinding.Exceptions;
using BWBinding.Interfaces;

namespace BWBinding.Observer
{
    class BossWaveListener
    {
        public void Run()
        {
            try
            {
                Frame frame = Frame.ReadFromStream(Controller.Instance.inputStream);
                int sequenceNumber = frame.sequenceNumber;
                Command command = frame.command;
                switch (command)
                {
                    case Command.RESULT:
                        IMessageHandler messageHandler;
                        lock (Controller.Instance.messageLock)
                        {
                            messageHandler = Controller.Instance.messageHandlers[sequenceNumber];
                        }
                        IListResultHandler listResultHandler;
                        lock (Controller.Instance.resultLock)
                        {
                            listResultHandler = Controller.Instance.resultHandlers[sequenceNumber];
                        }
                        if (messageHandler != null)
                        {
                            string uri = Encoding.UTF8.GetString(frame.PopFirstValue("uri"));
                            string from = Encoding.UTF8.GetString(frame.PopFirstValue("from"));
                            bool unpack = true;
                            byte[] bytes = frame.PopFirstValue("unpack");
                            if (bytes != null)
                            {
                                unpack = bool.Parse(Encoding.UTF8.GetString(bytes));
                            }
                            Message message;
                            if (unpack)
                            {
                                message = new Message(from, uri, frame.payloadObjects, frame.routingObjects);
                            }
                            else
                            {
                                message = new Message(from, uri, null, null);
                            }
                            messageHandler.ResultReceived(message);
                        }
                        else if (listResultHandler != null)
                        {
                            string finishedStr = Encoding.UTF8.GetString(frame.PopFirstValue("finished"));
                            bool finished = bool.Parse(finishedStr);
                            if (finished)
                            {
                                listResultHandler.finish();
                            }
                            else
                            {
                                string child = Encoding.UTF8.GetString(frame.PopFirstValue("child"));
                                listResultHandler.Result(child);
                            }
                        }
                        break;
                    case Command.RESPONSE:
                        IResponseHandler responseHandler;
                        lock (Controller.Instance.responseLock)
                        {
                            responseHandler = Controller.Instance.responseHandlers[sequenceNumber];
                        }
                        if (responseHandler != null)
                        {
                            string status = Encoding.UTF8.GetString(frame.PopFirstValue("status"));
                            string reason = null;
                            if (!status.Equals("okay"))
                            {
                                reason = Encoding.UTF8.GetString(frame.PopFirstValue("reason"));
                            }
                            responseHandler.ResponseReceived(new Response(status, reason));
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) when (ex is CorruptedFrameException || ex is SocketException || ex is IOException)
            {
                if (ex is IOException)
                {
                    throw new SystemException("Failed to read Frame.\n", ex);
                }
            }
        }
    }
}
