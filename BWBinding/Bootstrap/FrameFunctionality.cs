using BWBinding.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using BWBinding.Common;
using BWBinding.Exceptions;

namespace Bootstrap
{
    class FrameFunctionality
    {
        public FrameFunctionality()
        {
            // Testing PopFirstValue
            FrameUtils frameUtils = new FrameUtils(Command.SUBSCRIBE, 123456789);
            frameUtils.AddVSKeyPairGetUtils("string_test", "true");
            frameUtils.AddVSKeyPairGetUtils("bytes_test", Encoding.UTF8.GetBytes("false"));

            Frame frame = frameUtils.Build();
            string falseValue = Encoding.UTF8.GetString(frame.PopFirstValue("bytes_test"));
            string trueValue = Encoding.UTF8.GetString(frame.PopFirstValue("string_test"));

            Console.WriteLine("false : " + falseValue);
            Console.WriteLine("true : " + trueValue);
            Console.WriteLine(" ");

            // Testing CorruptedFrameException
            try
            {
                byte[] frameContent = Encoding.UTF8.GetBytes("helo 00000000000 0000000410 foobar\nend\n");

                MemoryStream ns = new MemoryStream(frameContent);
                Frame.ReadFromStream(ns);
            }
            catch (CorruptedFrameException ex)
            {
                // Successfull
                Console.WriteLine("Thrown CorruptedFrameException");
            }
            Console.WriteLine(" ");

            // Testing Reading Verifying Signing Key Pair Frame
            string vsString = "pute 0000000099 0000001337\n" + "kv testKey 9\n" + "testValue\n" + "kv blahbla2 10\n" + "blahblahb2\n" + "kv lawlKey 6\n" + "foobar\n" + "end\n";
            byte[] vsBytes = Encoding.UTF8.GetBytes(vsString);
            MemoryStream vsStream = new MemoryStream(vsBytes);
            Frame csFrame = Frame.ReadFromStream(vsStream);

            Console.WriteLine("Command: " + Command.PUT_ENTITY + " = " + csFrame.command);
            Console.WriteLine("Sequence number: " + "1337" + " = " + csFrame.sequenceNumber);
            Console.WriteLine("Number of VS key pais: " + "3" + " = " + csFrame.vsKeyPairs.Count);
            Console.WriteLine("Value from testKey: " + "testValue" + " = " + Encoding.UTF8.GetString(csFrame.PopFirstValue("testKey")));
            Console.WriteLine("Empty list: false" + " = " + csFrame.routingObjects.Any());

            //Testing Writing VSKeyPair
            FrameUtils writerUtils = new FrameUtils(Command.PUBLISH, 9876);
            writerUtils.AddVSKeyPairGetUtils("testKey1", "testValue1");
            writerUtils.AddVSKeyPairGetUtils("testKey2", "testValue2");
            Frame writerFrame = writerUtils.Build();
            MemoryStream ns1 = new MemoryStream();
            Stream bs1 = ns1;
            writerFrame.Write(bs1);
            Console.WriteLine("");
            string frameRetreived = Encoding.UTF8.GetString(ns1.ToArray());
            string expectedFrameStr = "publ 0000000000 0000009876\n" + "kv testKey1 10\n" + "testValue1\n" + "kv testKey2 10\n" + "testValue2\n" + "end\n";
            if (frameRetreived.Equals(expectedFrameStr))
            {
                Console.WriteLine("Writing test: They are equal.");
            }
        }
    }
}
