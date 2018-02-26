﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Wadder.Bsp;

namespace Wadder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Wadder - a tool to automatically create wads from BSP files");
                Console.WriteLine(@"Usage: Wadder.exe [file1] [file2] ... [filen]");
                Console.WriteLine(@"Or drag a bsp file onto Wadder.exe in explorer");
                Environment.Exit(0);
            }
            int created = 0;

            foreach (var file in args)
            {
                var mapsDir = Path.GetDirectoryName(file);
                var modDir = Path.GetDirectoryName(mapsDir);
                var baseDir = Path.GetDirectoryName(modDir);
                var gameDir = Path.Combine(baseDir, "valve");

                if (!Directory.Exists(gameDir))
                {
                    Console.WriteLine($"Directory not found: {gameDir}");
                    continue;
                }

                var modWads = Directory.GetFiles(gameDir, "*.wad", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToList();
                var gameWads = Directory.GetFiles(modDir, "*.wad", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToList();
                var foundWads = modWads.Union(gameWads).ToList();

                if (File.Exists(file))
                {
                    Console.WriteLine($"Map: {Path.GetFileName(file)}");
                    using (var s = File.OpenRead(file))
                    {
                        var bsp = new Bsp.Bsp(s);
                        var bel = new BspEntityLump(bsp.GetLump(LumpType.Entities), s);

                        var ws = bel.Entities.FirstOrDefault(x => x.Get("classname") == "worldspawn");
                        if (ws != null)
                        {
                            var wadline = ws.Get("wad");
                            var wads = wadline.Split(';');
                            foreach (var wad in wads.Where(x => !String.IsNullOrWhiteSpace(x)))
                            {
                                var fi = new FileInfo(Path.Combine(Environment.CurrentDirectory, wad));
                                if (!foundWads.Contains(fi.Name, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    Console.WriteLine($"{fi.Name}: Not found, creating...");
                                    using (var o = File.OpenWrite(Path.Combine(modDir, fi.Name)))
                                    {
                                        using (var ew = EmptyWad())
                                        {
                                            ew.CopyTo(o);
                                        }
                                    }
                                    modWads.Add(fi.Name);
                                    foundWads.Add(fi.Name);
                                    created++;
                                }
                                else
                                {
                                    Console.WriteLine($"{fi.Name}: Found");
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Done. Created {created} wad(s).");
            Console.WriteLine("");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static Stream EmptyWad()
        {
            var ms = new MemoryStream();

            using (var bw = new BinaryWriter(ms, Encoding.ASCII, true))
            {
                bw.Write("WAD3");
                bw.Write(0u);
                bw.Write(12u);
            }

            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
