using System;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;

class GenTxt
{
    public static void Gen(string dir)
    {

        string txt = "";

        string[] files = Directory.GetFiles(dir);
        foreach (string f in files)
        {
            Match m = Regex.Match(Path.GetFileName(f), AeroNovel.filename_reg);
            if (!m.Success) continue;
            string no = m.Groups[1].Value;
            string chaptitle = m.Groups[2].Value;
            string[] lines = File.ReadAllLines(f);
            string body = Body(lines);
            txt += chaptitle + "\r\n" + body + "\r\n=====================\r\n";
            Console.WriteLine("Added " + chaptitle);

        }
        File.WriteAllText("txt_output.txt", txt);

    }
    public static string Body(string[] txt)
    {
        var regs = new string[]{
                "\\[align=(.*?)\\](.*?)\\[\\/align\\]",//0
                "\\[note\\]",
                "\\[note=(.*?)\\]",
                "\\[img\\](.*?)\\[\\/img\\]",
                "\\[b\\](.*?)\\[\\/b\\]",
                "\\[title\\](.*?)\\[\\/title\\]",
                "\\[ruby=(.*?)\\](.*?)\\[\\/ruby\\]",
                "\\[pagebreak\\]",
                "/\\*.*?\\*/",
                "\\[emphasis\\](.*?)\\[\\/emphasis\\]"
                };

        var repls = new string[]{
                "$2",
                "[注]",
                "[$1]",
                "[图片：$1]",
                "$1",
                "$1",
                "$2（$1）",
                "",
                "",
                "$1"
                };

        string html = "";
        foreach (string line in txt)
        {
            string r = line;
            bool aligned=false;
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
                            case 0://align
                                r = reg.Replace(r, repls[i]);
                                switch (m.Groups[1].Value)
                                {
                                    case "right": r = "　　　　　　" + r; break;
                                    case "center": r = "　　　　" + r; break;
                                }
                                aligned=true;
                                break;
                            case 3://img
                                string src = Path.GetFileName(m.Groups[1].Value);
                                string img_temp = "图片：{0}";
                                r = reg.Replace(r, string.Format(img_temp, src), 1);
                                break;
                            default:
                                r = reg.Replace(r, repls[i]);
                                break;
                        }
                        break;
                    }

                }
            } while (m.Success);
            if (r.Length > 0&&!aligned)
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
            html += r + "\r\n";
        }
        return html;
    }

}

