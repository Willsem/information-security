using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CompressHuffman
{
    public class HuffmanCompress
    {
        private readonly string _fileName;
        private readonly string _tableName;

        private Dictionary<byte, uint> _countBytes;

        public HuffmanCompress(string fileName, string tableName)
        {
            _fileName = fileName;
            _tableName = tableName;
            _countBytes = new Dictionary<byte, uint>(256);
        }

        public void Execute()
        {
            byte[] bytes = File.ReadAllBytes(_fileName);
            foreach (byte b in bytes)
            {
                if (_countBytes.ContainsKey(b))
                    _countBytes[b]++;
                else
                    _countBytes[b] = 1;
            }

            List<Tree.Node> nodes = new List<Tree.Node>();
            foreach (KeyValuePair<byte, uint> symbol in _countBytes)
            {
                Tree.Node node = new Tree.Node();
                node.Count = symbol.Value;
                node.Symbol = symbol.Key;
                nodes.Add(node);
            }

            while (nodes.Count > 1)
            {
                nodes.Sort((f, s) => f.Count.CompareTo(s.Count));

                Tree.Node left = nodes[0];
                Tree.Node right = nodes[1];
                nodes.Remove(left);
                nodes.Remove(right);

                Tree.Node node = new Tree.Node();
                node.Count = left.Count + right.Count;
                node.Left = left;
                node.Right = right;

                nodes.Add(node);
            }

            Tree tree = new Tree(nodes[0]);
            var table = tree.GetTable();

            List<byte> zip = new List<byte>();
            foreach (byte sb in bytes)
            {
                foreach (byte b in table[sb])
                {
                    zip.Add(b);
                }
            }

            byte[] zipWrite = new byte[zip.Count / 8 + 1];
            for (int i = 0; i < zip.Count; i += 8)
            {
                byte b = 0;
                for (int j = 0; j < 8; ++j)
                {
                    b <<= 1;

                    if (i + j < zip.Count)
                        b += zip[i + j];
                    else
                        b += (byte)0;
                }
                zipWrite[i / 8] = b;
            }

            File.WriteAllBytes(_fileName + ".zip", zipWrite);

            File.WriteAllText(_tableName, JsonConvert.SerializeObject(table));
        }
    }
}
