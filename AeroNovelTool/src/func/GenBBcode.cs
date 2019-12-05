using System;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Collections.Generic;
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

            const string reg_noteref = "\\[note\\]";
            const string reg_notecontent = "\\[note=(.*?)\\]";
            const string reg_img = "\\[img\\](.*?)\\[\\/img\\]";
            const string reg_illu = "\\[illu\\](.*?)\\[\\/illu\\]";
            const string reg_class = "\\[class=(.*?)\\](.*?)\\[\\/class\\]";
            const string reg_chapter = "\\[chapter=(.*?)\\](.*?)\\[\\/chapter\\]";
            Dictionary<string, string> reg_dic = new Dictionary<string, string>
            {
                //{"\\[align=(.*?)\\](.*?)\\[\\/align\\]","<p class=\"aligned\" style=\"text-align:$1\">$2</p>"},
                {reg_noteref,"[color=#00ffff][sup]注[/sup][/color]"},
                {reg_notecontent,"\r\n[align=right][size=1][color=#00ffff]$1[/color][/size][/align]"},
                {reg_img,"[图片：$1]"},
                {reg_illu,"[图片：$1]"},
                {reg_class,"$2"},
                {reg_chapter,"$2"},
                //{"\\[b\\](.*?)\\[\\/b\\]","<b>$1</b>"},
                {"\\[title\\](.*?)\\[\\/title\\]","[size=5]$1[/size]"},
                //{"\\[ruby=(.*?)\\](.*?)\\[\\/ruby\\]","<ruby>$2<rt>$1</rt></ruby>"},
                {"\\[pagebreak\\]",""},
                {"/\\*.*?\\*/",""},
                {"///.*",""},
                {"\\[emphasis\\](.*?)\\[\\/emphasis\\]","[b]$1[/b]"},
                //{"\\[s\\](.*?)\\[\\/s\\]","<s>$1</s>"},
                //{"\\[i\\](.*?)\\[\\/i\\]","<i>$1</i>"},
                //{"\\[color=(.*?)\\](.*?)\\[\\/color\\]","<span style=\"color:$1\">$2</span>"},
                //{"\\[size=(.*?)\\](.*?)\\[\\/size\\]","<span style=\"font-size:$1em\">$2</span>"}
            };
        string html = "";
        foreach (string line in txt)
        {
            if(line.StartsWith("##"))continue;
            string r = line;
            Match m = Regex.Match("", "1");
            do
            {
                foreach (var kv in reg_dic)
                {
                    m = Regex.Match(r,kv.Key);
                    if (m.Success)
                    {
                        Regex reg = new Regex(kv.Key);
                        switch (kv.Key)
                        {
                            default:
                                r = reg.Replace(r, kv.Value);
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

