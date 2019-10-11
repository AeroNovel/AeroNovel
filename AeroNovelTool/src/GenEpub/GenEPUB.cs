using System;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Collections.Generic;
namespace AeroNovelEpub
{
    public class GenEpub
    {
        static List<string> txt_nums=new List<string>();
        static List<string> txt_titles=new List<string>();
        static List<string> xhtml_names=new List<string>();
        public static Epub Gen(string dir)
        {
            Epub epub = new Epub("template.zip");
            string uid = "urn:uuid:" + Guid.NewGuid().ToString();
            string xhtml_temp;
            {
                TextItem t = epub.GetItem<TextItem>("OEBPS/Text/template.xhtml");
                xhtml_temp = t.data;
                epub.items.Remove(t);
            }
            string spine = "";
            string items = "";

            GenHtml genHtml = new GenHtml();
            

            string[] files = Directory.GetFiles(dir);
            foreach (string f in files)
            {
                Match m = Regex.Match(Path.GetFileName(f),AeroNovel.filename_reg);
                if (!m.Success) { continue; }
                string no = m.Groups[1].Value;
                string chaptitle = m.Groups[2].Value;
                string[] lines = File.ReadAllLines(f);
                string body = genHtml.Gen(lines);
                if (f.Contains("info.txt"))
                {
                    body = "<div class=\"info\" epub:type=\"acknowledgements\">" + body + "<p>AeroNovelTool EPUB生成器by AE " + DateTime.Now + "</p>" +
                    "<p>推荐使用阅读器:<br/>Apple Books<br/>Microsoft Edge<br/>Kindle(使用Kindlegen 转换)<br/></p>" +
                    "</div>";
                    //File.WriteAllText("info.txt",body);
                }
                string xhtml = xhtml_temp.Replace("{❤title}", chaptitle).Replace("{❤body}", body);
                string name="t"+no+".xhtml";
                if(Regex.Match(Path.GetFileNameWithoutExtension(f),"[a-zA-Z0-9]").Success)
                {
                    name="t"+Path.GetFileNameWithoutExtension(f)+".xhtml";
                }
                
                TextItem i=new TextItem("OEBPS/Text/"+name,xhtml);
                epub.items.Add(i);
                items += string.Format("<item id=\"{0}\" href=\"Text/{0}\" media-type=\"application/xhtml+xml\"/>\n",name);
                spine += string.Format("<itemref idref=\"{0}\"/>", name);
                Log.log("[Info]Proc Text No." + no+": "+chaptitle);

                txt_nums.Add(no);
                txt_titles.Add(chaptitle);
                xhtml_names.Add(name);
            }

            string img = Path.Combine(dir, "Images");
            if (Directory.Exists(img))
            {
                foreach (var f in Directory.GetFiles(img))
                {
                    if (f.Contains(".jpg"))
                    {
                        NormalItem i=new NormalItem("OEBPS/Images/"+Path.GetFileName(f),File.ReadAllBytes(f));
                        epub.items.Add(i);
                        items += string.Format("<item id=\"{0}\" href=\"Images/{0}\" media-type=\"image/jpeg\"/>\n", Path.GetFileName(f));
                        Log.log("[Info]Add image: "+f);
                    }

                }
            }

            string meta = File.ReadAllText(Path.Combine(dir, "meta.txt"));
            meta = meta.Replace("{urn:uuid}", uid);
            string title = Regex.Match(meta, "<dc:title.*?>(.*?)</dc:title>").Groups[1].Value;

            TextItem toc=epub.GetItem<TextItem>("OEBPS/toc.ncx");
            toc.data = GenTOC(File.ReadAllLines(Path.Combine(dir, "toc.txt")), uid, title,toc.data);

            TextItem opf = epub.GetItem<TextItem>("OEBPS/content.opf");
            opf.data= string.Format(opf.data, meta, items, spine);
            epub.ReadMeta();
            return epub;
        }
        public static string GenTOC(string[] lines, string uid, string title, string template)
        {
            //string temp=File.ReadAllText("template/toc.txt");
            string r = "";
            List<string> label = new List<string>();
            int depth = 1;
            int count = 0;
            foreach (string line in lines)
            {
                Match m = Regex.Match(line, "\\[(.*?)\\]");
                if (m.Success)
                {
                    string tag = m.Groups[1].Value;
                    if (tag[0] == '/')
                    {
                        label.RemoveAt(label.Count - 1);
                        r += "</navPoint>\n";
                    }
                    else
                    {
                        label.Add(tag);
                        if (depth < label.Count + 1) { depth = label.Count + 1; }
                        count++;
                        r += string.Format("<navPoint id=\"navPoint-{0}\" playOrder=\"{0}\"><navLabel><text>{1}</text></navLabel><content src=\"dummylink\"/>\n", count, tag);

                        m = Regex.Match(line.Substring(m.Index + m.Length), "([0-9][0-9])");
                        if (m.Success)
                        {
                            int index=txt_nums.IndexOf(m.Groups[1].Value);
                            string link = "Text/" +xhtml_names[index];
                            r = r.Replace("dummylink", link);
                        }
                    }
                    continue;
                }

                m = Regex.Match(line, "([0-9][0-9])");
                if (m.Success)
                {
                    count++;
                    int index=txt_nums.IndexOf(m.Groups[1].Value);
                    string link = "Text/" +xhtml_names[index];
                    r += string.Format("<navPoint id=\"navPoint-{0}\" playOrder=\"{0}\"><navLabel><text>{1}</text></navLabel><content src=\"{2}\"/></navPoint>\n", count, txt_titles[index], link);
                    r = r.Replace("dummylink", link);
                }

            }
            return string.Format(template, uid, depth, title, r);
        }
    }

}