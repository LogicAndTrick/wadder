using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wadder.Bsp
{
    public class Bsp
    {
        public int Version { get; set; }
        public List<BspLump> Lumps { get; set; }

        public Bsp(Stream stream)
        {
            Lumps = new List<BspLump>();

            using (var br = new BinaryReader(stream, Encoding.ASCII, true))
            {
                Version = br.ReadInt32();
                if (Version != 30) throw new Exception("Only BSP v30 is supported");

                foreach (LumpType type in Enum.GetValues(typeof(LumpType)))
                {
                    Lumps.Add(new BspLump
                    {
                        Type = type,
                        Offset = br.ReadInt32(),
                        Length = br.ReadInt32()
                    });
                }
            }
        }

        public BspLump GetLump(LumpType type)
        {
            return Lumps.First(x => x.Type == type);
        }
    }
}
