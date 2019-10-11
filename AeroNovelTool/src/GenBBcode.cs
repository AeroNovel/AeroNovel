using System;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;

class GenBbcode
{
    public static void Gen(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        Directory.CreateDirectory("bbcode_output");
        foreach (string f in files)
        {
            Match m = Regex.Match(Path.GetFileName(f), AeroNovel.filename_reg);
            if (!m.Success) continue;
            //string no = m.Groups[1].Value;
            //string chaptitle = m.Groups[2].Value;
            string[] lines = File.ReadAllLines(f);
            string body = Body(lines);
            string outpath="bbcode_output\\"+Path.GetFileNameWithoutExtension(f)+".txt";
            File.WriteAllText(outpath, body);
            Console.WriteLine(outpath);
        }
    }
    public static string Body(string[] txt)
    {
        var regs = new string[]{
                //"\\([align=.*?\\].*?\\[\\/align\\])",
                "\\[note\\]",
                "\\[note=(.*?)\\]",
                //"\\[img\\](.*?)\\[\\/img\\]",
                //"\\[b\\](.*?)\\[\\/b\\]",
                "\\[title\\](.*?)\\[\\/title\\]",
                //"\\[ruby=(.*?)\\](.*?)\\[\\/ruby\\]",
                "\\[pagebreak\\]",
                "/\\*.*?\\*/",
                "///.*"
                };

        var repls = new string[]{
                //"$1",
                "[color=#00ffff][sup]注[/sup][/color]",
                "\r\n[align=right][size=1][color=#00ffff]$1[/color][/size][/align]",
                //"[图片：$1]",
                //"$1",
                "[size=5]$1[/size]",
                //"$2（$1）",
                "",
                "",
                ""
                };

        string html = "";
        foreach (string line in txt)
        {
            string r = line;
            Match m = Regex.Match("", "1");
            do
            {
                for (int i = 0; i < regs.Length; i++)
                {
                    m = Regex.Match(r, regs[i]);
                    if (m.Success)
                    {
                        Regex reg = new Regex(regs[i]);
                        switch (i)
                        {
                            default:
                                r = reg.Replace(r, repls[i]);
                                break;
                        }
                        break;
                    }

                }
            } while (m.Success);
            if(!r.StartsWith("[align"))
            if (r.Length > 0)
                switch (r[0])
                {
                    case '「':
                    case '『':
                    case '＜':
                    case '《':
                        r = "　" + r;
                        break;
                    default:
                        r = "　　" + r;
                        break;
                }
            if (r.EndsWith("[/align]"))
                html += r;
            else
                html += r + "\r\n";
        }

        return html;
    }

}

