using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

namespace AeroNovelEpub
{
    public class GenHtml
    {
        GenEpub context;
        public GenHtml(GenEpub context)
        {
            this.context = context;
        }
        public string Gen(string[] txt)
        {
            string noteref_temp = "<a class=\"noteref\" epub:type=\"noteref\" href=\"#note{0}\" id=\"note_ref{0}\"><sup>[注]</sup></a>";
            int note_count = 0;
            List<string> notes = new List<string>();
            const string reg_noteref = "\\[note\\]";
            const string reg_notecontent = "\\[note=(.*?)\\]";
            const string reg_img = "\\[img\\](.*?)\\[\\/img\\]";
            const string reg_illu = "^\\[illu\\](.*?)\\[\\/illu\\]$";
            const string reg_imgchar = "\\[imgchar\\](.*?)\\[\\/imgchar\\]";
            const string reg_class = "\\[class=(.*?)\\](.*?)\\[\\/class\\]";
            const string reg_chapter = "\\[chapter=(.*?)\\](.*?)\\[\\/chapter\\]";
            Dictionary<string, string> reg_dic = new Dictionary<string, string>
            {
                {"^\\[align=(.*?)\\](.*?)\\[\\/align\\]$","<p class=\"aligned\" style=\"text-align:$1\">$2</p>"},
                {reg_noteref,""},
                {reg_notecontent,""},
                {reg_img,""},
                {reg_illu,""},
                {reg_imgchar,""},
                {reg_class,""},
                {reg_chapter,""},
                {"\\[b\\](.*?)\\[\\/b\\]","<b>$1</b>"},
                {"^\\[title\\](.*?)\\[\\/title\\]$","<p class=\"tagtitle\">$1</p>"},
                {"\\[ruby=(.*?)\\](.*?)\\[\\/ruby\\]","<ruby>$2<rt>$1</rt></ruby>"},
                {"^\\[pagebreak\\]$","<p class=\"pagebreak\"><br/></p>"},
                {"/\\*.*?\\*/",""},
                {"///.*",""},
                {"\\[emphasis\\](.*?)\\[\\/emphasis\\]","<span class=\"emph\">$1</span>"},
                {"\\[s\\](.*?)\\[\\/s\\]","<s>$1</s>"},
                {"\\[i\\](.*?)\\[\\/i\\]","<i>$1</i>"},
                {"\\[color=(.*?)\\](.*?)\\[\\/color\\]","<span style=\"color:$1\">$2</span>"},
                {"\\[size=(.*?)\\](.*?)\\[\\/size\\]","<span style=\"font-size:$1em\">$2</span>"},
                {"^\\[h1\\](.*?)\\[\\/h1\\]$","<h1>$1</h1>"},
                {"^\\[h2\\](.*?)\\[\\/h2\\]$","<h2>$1</h2>"},
                {"^\\[h3\\](.*?)\\[\\/h3\\]$","<h3>$1</h3>"},
                {"^\\[h4\\](.*?)\\[\\/h4\\]$","<h4>$1</h4>"},
                {"^\\[h5\\](.*?)\\[\\/h5\\]$","<h5>$1</h5>"},
                {"^\\[h6\\](.*?)\\[\\/h6\\]$","<h6>$1</h6>"}
            };


            string html = "";
            foreach (string line in txt)
            {
                if (line.StartsWith("##")) continue;
                if (line.StartsWith("#HTML:"))
                {
                    html += line.Substring("#HTML:".Length);
                    continue;
                }
                string r = EncodeHTML(line);
                Match m = Regex.Match("", "1");
                do
                {
                    foreach (var pair in reg_dic)
                    {
                        m = Regex.Match(r, pair.Key);
                        if (m.Success)
                        {
                            Regex reg = new Regex(pair.Key);
                            switch (pair.Key)
                            {
                                case reg_noteref://noteref
                                    r = reg.Replace(r, string.Format(noteref_temp, note_count), 1);
                                    note_count++;
                                    break;
                                case reg_notecontent://note
                                    notes.Add(m.Groups[1].Value);
                                    r = reg.Replace(r, "", 1);
                                    break;
                                case reg_img://img
                                    {
                                        string img_name = Path.GetFileName(m.Groups[1].Value);
                                        if (File.Exists(Path.Combine(context.img_path, img_name)))
                                        {
                                            Log.log("[Info]Image used:" + img_name);
                                            if (!context.img_names.Contains(img_name))
                                            {
                                                context.img_names.Add(img_name);
                                            }
                                        }
                                        else
                                        {
                                            Log.log("[Warn]Cannot find " + img_name);
                                        }
                                        string src = "../Images/" + img_name;
                                        string img_temp = "<img src=\"{0}\" alt=\"\"/>";
                                        r = reg.Replace(r, string.Format(img_temp, src), 1);
                                    }

                                    break;
                                case reg_illu://illu
                                    {
                                        string img_name = Path.GetFileName(m.Groups[1].Value);
                                        if (File.Exists(Path.Combine(context.img_path, img_name)))
                                        {
                                            Log.log("[Info]Illustation used:" + img_name);
                                            if (!context.img_names.Contains(img_name))
                                            {
                                                context.img_names.Add(img_name);
                                            }
                                        }
                                        else
                                        {
                                            Log.log("[Warn]Cannot find " + img_name);
                                        }
                                        string src = "../Images/" + img_name;
                                        string img_temp = "<div class=\"aligned illu\"><img class=\"illu\" src=\"{0}\" alt=\"\"/></div>";
                                        r = reg.Replace(r, string.Format(img_temp, src), 1);
                                    }
                                    break;
                                case reg_imgchar:
                                    {
                                        string img_name = Path.GetFileName(m.Groups[1].Value);
                                        if (File.Exists(Path.Combine(context.img_path, img_name)))
                                        {
                                            Log.log("[Info]Imagechar used:" + img_name);
                                            if (!context.img_names.Contains(img_name))
                                            {
                                                context.img_names.Add(img_name);
                                            }
                                        }
                                        else
                                        {
                                            Log.log("[Warn]Cannot find " + img_name);
                                        }
                                        string src = "../Images/" + img_name;
                                        string img_temp = "<img class=\"imgchar\" src=\"{0}\" alt=\"\"/>";
                                        r = reg.Replace(r, string.Format(img_temp, src), 1);
                                    }

                                    break;
                                case reg_class://class
                                    {

                                        if (m.Index == 0&&m.Length==line.Length)
                                        {
                                            r = reg.Replace(r, "<p class=\"$1\">$2</p>");

                                        }
                                        else
                                        {
                                            r = reg.Replace(r, "<span class=\"$1\">$2</span>");
                                        }
                                    }
                                    break;
                                case reg_chapter://chapter
                                    {
                                        string chapnum_s = m.Groups[1].Value;
                                        int chapnum;
                                        if (!int.TryParse(chapnum_s, out chapnum)) { Log.log("[Error]Bad chapter string:" + chapnum_s); continue; }

                                        int index = context.txt_nums.FindIndex(0, (s) => int.Parse(s) == chapnum);
                                        if (index < 0) { Log.log("[Error]Bad chapter number:" + chapnum); continue; }
                                        string path = context.xhtml_names[index];
                                        r = reg.Replace(r, "<a href=\"" + path + "\">$2</a>");
                                    }
                                    break;
                                default:
                                    r = reg.Replace(r, pair.Value);
                                    break;
                            }
                            break;
                        }

                    }
                } while (m.Success);
                if (r.Length == 0) { r = "<br/>"; }
                bool addp = true;
                string[] dont_addp_list = new string[]
                {"p","div","h1","h2","h3","h4","h5","h6"};
                foreach (var a in dont_addp_list)
                    if (Regex.Match(r, "<" + a + ".*>").Success)
                        addp = false;
                if (addp)
                {
                    if (r[0] == '（' || r[0] == '「' || r[0] == '『'||r[0]=='〈')
                    {
                        r = "<p class=\"drawout\">" + r + "</p>";
                    }
                    else
                        r = "<p>" + r + "</p>";
                }
                CheckUnprocessedTag(r);
                html += r + "\n";
            }
            if (notes.Count > 0)
            {
                html += "<aside class=\"notesection\" epub:type=\"footnote\">注释<br/>";
                string note_temp = "<aside epub:type=\"footnote\" id=\"note{0}\"><p class=\"footnote-p\"><a href=\"#note_ref{0}\">{2}</a>{1}</p></aside>\n";
                int count = 0;
                foreach (string note in notes)
                {
                    int div = note.IndexOf(':');
                    if (div > 0)
                    {
                        string noteref_text = note.Substring(0, div);
                        html = html.Replace(string.Format(noteref_temp, count), string.Format(noteref_temp.Replace("注", noteref_text), count));
                        string note_content = note.Substring(div + 1);
                        html += string.Format(note_temp, count, note_content, noteref_text + ":");
                    }
                    else
                        html += string.Format(note_temp, count, note, "注:");
                    count++;
                }
                html += "</aside>";
            }

            return html;
        }

        Regex reg_tag = new Regex("\\[(.{1,20}?)\\]");
        void CheckUnprocessedTag(string s)
        {
            var ms = reg_tag.Matches(s);
            foreach (Match m in ms)
            {
                if (Regex.Match(m.Groups[1].Value, "^[a-zA-Z0-9=]{1,20}$").Success)
                {
                    Log.log("[Warn]Unprocessed tag in line:“" + s+"”");
                }
            }


        }
        public static string EncodeHTML(string s)
        {
            return s.Replace("&", "&amp;");
        }
    }
}