using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Wadder.Bsp
{
    public class BspEntityLump : BspLump
    {
        public string EntityData { get; set; }
        public List<BspEntity> Entities { get; set; }

        public BspEntityLump(BspLump lump, Stream stream)
        {
            Type = lump.Type;
            Offset = lump.Offset;
            Length = lump.Length;

            using (var br = new BinaryReader(stream, Encoding.ASCII, true))
            {
                br.BaseStream.Seek(Offset, SeekOrigin.Begin);
                var bytes = br.ReadBytes(Length);
                EntityData = Encoding.ASCII.GetString(bytes);
            }

            Entities = new List<BspEntity>();

            BspEntity current = null;
            foreach (var line in EntityData.Split('\n'))
            {
                if (line == "{") Entities.Add(current = new BspEntity());
                else if (line == "}") current = null;
                else if (current != null)
                {
                    var split = line.SplitWithQuotes();
                    if (split.Length == 2) current.Values[split[0]] = split[1];
                }
            }
        }
    }
}