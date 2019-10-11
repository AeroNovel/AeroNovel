using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length >= 2)
        {
            switch (args[0].ToLower())
            {
                case "epub":
                    if (Directory.Exists(args[1]))
                    {
                        Epub e = AeroNovelEpub.GenEpub.Gen(args[1]);
                        e.filename = e.title;
                        e.Save("");
                    }
                    break;
                case "txt":
                    if (Directory.Exists(args[1]))
                    {
                        GenTxt.Gen(args[1]);
                    }
                    break;
                case "bbcode":
                    if (Directory.Exists(args[1]))
                    {
                        GenBbcode.Gen(args[1]);
                    }
                    break;
                case "restore":
                    if (Directory.Exists(args[1]))
                    {
                        Publish.Restore(args[1]);
                    }
                    break;
                case "epub2comment":
                    if (File.Exists(args[1]))
                    {
                        Epub2Comment.Proc(args[1]);
                    }
                    break;
                default:
                Log.log("[Warn]Nothing happens. Usage:epub/txt/bbcode/restore/epub2comment");
                break;
            }
        }else
        {
            Log.log("[Warn]Usage:epub/txt/bbcode/restore/epub2comment");
        }
    }
}
public class AeroNovel
{
    public static string filename_reg="([0-9][0-9])(.*?)\\.[a]{0,1}txt";
}

