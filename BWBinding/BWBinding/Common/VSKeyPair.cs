using System.IO;

namespace BWBinding.Common
{
    public class VSKeyPair
    {
        public string key { get; private set; }
        public byte[] value { get; private set; }

        public VSKeyPair(string key, byte[] value)
        {
            this.key = key;
            this.value = value;
        }

        public void Write(StreamWriter outputStream)
        {
            string header = string.Format("kv %s %d\n", key, value.Length);
            outputStream.Write(header);
            outputStream.Write(value);
            outputStream.Write('\n');
        }

    }
}
