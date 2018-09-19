using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BWBinding.Common;
using BWBinding.Exceptions;
using BWBinding.Interfaces;
using BWBinding.Utils;

namespace BWBinding.Observer
{
    class BossWaveListener
    {
        public void Run(NetworkStream inputStream)
        {
            Console.WriteLine("Running from another Thread...");
            try
            {
                while (true)
                {
                    Frame frame = Frame.ReadFromStream(inputStream);
                    int sequenceNumber = frame.sequenceNumber;
                    Command command = frame.command;
                    switch (command)
                    {
                        case Command.RESULT:
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
                                string reason;

                            }
                            break;
                        default:
                            Console.WriteLine(CommandUtils.GetCode(command));
                            break;
                    }
                }
            } catch(Exception ex) when (ex is CorruptedFrameException || ex is SocketException || ex is IOException)
            {
                if (ex is IOException)
                {
                    throw new SystemException("Failed to read Frame.\n", ex);
                }
            }
        }
    }
}
