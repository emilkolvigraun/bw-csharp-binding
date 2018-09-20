using BWBinding.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
            Console.WriteLine("");
            Console.WriteLine("Testing PopFirstValue:");
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
            Console.WriteLine("");
            Console.WriteLine("Testing CorruptedFrameException:");
            try
            {
                byte[] frameContent = Encoding.UTF8.GetBytes("helo 00000000000 0000000410 foobar\nend\n");

                MemoryStream ns = new MemoryStream(frameContent);
                Frame.ReadFromStream(ns);
            }
            catch (CorruptedFrameException ex)
            {
                // Successfull
                Console.WriteLine("thrown CorruptedFrameException");
            }

            // Testing Reading Verifying Signing Key Pair Frame
            Console.WriteLine("");
            Console.WriteLine("Testing Reading Verifying Signing Key Pair Frame:");
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
            Console.WriteLine("");
            Console.WriteLine("Testing Writing VSKeyPair:");
            FrameUtils writerUtils = new FrameUtils(Command.PUBLISH, 9876);
            writerUtils.AddVSKeyPairGetUtils("testKey1", "testValue1");
            writerUtils.AddVSKeyPairGetUtils("testKey2", "testValue2");
            Frame writerFrame = writerUtils.Build();
            MemoryStream ns1 = new MemoryStream();
            Stream bs1 = ns1;
            writerFrame.Write(bs1);
            string frameRetreived = Encoding.UTF8.GetString(ns1.ToArray());
            string expectedFrameStr = "publ 0000000000 0000009876\n" + "kv testKey1 10\n" + "testValue1\n" + "kv testKey2 10\n" + "testValue2\n" + "end\n";
            if (frameRetreived.Equals(expectedFrameStr))
            {
                Console.WriteLine("Writing VSK test: They are equal.");
            }

            //Testing Writing PayloadObject
            Console.WriteLine("");
            Console.WriteLine("Testing Writing PayloadObject:");
            FrameUtils powriterUtils = new FrameUtils(Command.SUBSCRIBE, 1234);
            PayloadType potype = new PayloadType(42);
            PayloadObject poo = new PayloadObject(potype, Encoding.UTF8.GetBytes("testPayload"));
            powriterUtils.AddPayloadObjectGetUtils(poo);
            Frame powriterFrame = powriterUtils.Build();
            MemoryStream ns2 = new MemoryStream();
            Stream bs2 = ns2;
            powriterFrame.Write(bs2);
            string poframeRetreived = Encoding.UTF8.GetString(ns2.ToArray());
            string poexpectedFrameStr = "subs 0000000000 0000001234\n" + "po :42 11\n" + "testPayload\n" + "end\n";
            if (poframeRetreived.Equals(poexpectedFrameStr))
            {
                Console.WriteLine("Writing PO test: They are equal.");
            }

            //Testing Writing RoutingObject
            Console.WriteLine("");
            Console.WriteLine("Testing Writing RoutingObject:");
            FrameUtils rowriterUtils = new FrameUtils(Command.PUBLISH, 9876);
            RoutingObject ro = new RoutingObject(45, Encoding.UTF8.GetBytes("relaxing"));
            rowriterUtils.AddRoutingObjectGetUtils(ro);
            Frame rowriterFrame = rowriterUtils.Build();
            MemoryStream ns3 = new MemoryStream();
            Stream bs3 = ns3;
            rowriterFrame.Write(bs3);
            string roframeRetreived = Encoding.UTF8.GetString(ns3.ToArray());
            string roexpectedFrameStr = "publ 0000000000 0000009876\n" + "ro 45 8\n" + "relaxing\n" + "end\n";
            if (roframeRetreived.Equals(roexpectedFrameStr))
            {
                Console.WriteLine("Writing RO test: They are equal.");
            }

            //Reading RoutingObject Frame
            Console.WriteLine("");
            Console.WriteLine("Testing Reading RoutingObject Frame:");
            string roframetoread = "dlpc 0000000987 0000000645\n" + "ro 255 6\n" + "testRO\n" + "end\n";
            byte[] roframeBytes = Encoding.UTF8.GetBytes(roframetoread);
            MemoryStream roStream = new MemoryStream(roframeBytes);
            Frame roFrame = Frame.ReadFromStream(roStream);

            Console.WriteLine("Command: " + Command.DEL_PREF_CHAIN + " = " + roFrame.command);
            Console.WriteLine("Sequence number: " + "645" + " = " + roFrame.sequenceNumber);
            Console.WriteLine("VSkeyPairCount: " + "0" + " = " + roFrame.vsKeyPairs.Count);
            Console.WriteLine("RoutinObjects.Count: 1" + " = " + roFrame.routingObjects.Count);
            Console.WriteLine("PayloadObjects.Count: 0" + " = " + roFrame.payloadObjects.Count);

            //Readin PayloadObject Frame
            Console.WriteLine("");
            Console.WriteLine("Testing Reading PayloadObject Frame:");
            string poframetoread = "make 0000000059 0000000999\n" + "po 1.2.3.4: 11\n" + "testPayload\n" + "end\n";
            byte[] poframeBytes = Encoding.UTF8.GetBytes(poframetoread);
            MemoryStream mempo = new MemoryStream(poframeBytes);
            Frame mepoframe = Frame.ReadFromStream(mempo);

            Console.WriteLine("Command: " + Command.MAKE_ENTITY + " = " + mepoframe.command);
            Console.WriteLine("Sequence number: " + "999" + " = " + mepoframe.sequenceNumber);
            Console.WriteLine("VSkeyPairCount: " + "0" + " = " + mepoframe.vsKeyPairs.Count);
            Console.WriteLine("RoutinObjects.Count: 0" + " = " + mepoframe.routingObjects.Count);
            Console.WriteLine("PayloadObjects.Count: 1" + " = " + mepoframe.payloadObjects.Count);

            PayloadType expectedType = new PayloadType(new byte[] { 1, 2, 3, 4 });
            byte[] expectedContents = Encoding.UTF8.GetBytes("testPayload");
            PayloadObject expectedPayload = new PayloadObject(expectedType, expectedContents);
            if (expectedPayload.Equals(mepoframe.payloadObjects[0]))
            {
                Console.WriteLine("Readin PO test: They are equal.");
            }
        }
    }
}
