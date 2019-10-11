using System.IO;
using System.Text.RegularExpressions;
public class EpubToNote
{
    public static void Proc(string path)
    {
        Directory.CreateDirectory("epub2note_output");
        Epub e = new Epub(path);
        e.items.ForEach(
            (i) =>
            {
                if (typeof(TextItem) == i.GetType() && i.fullName.EndsWith(".xhtml"))
                { ProcXHTML((TextItem)i); }
            }
            );
    }
    static void ProcXHTML(TextItem i)
    {
        Log.log("[Info]"+i.fullName);
        string name=Path.GetFileNameWithoutExtension(i.fullName);
        string r=i.data.Replace("\r","").Replace("\n","");
        Match m=Regex.Match(r,"<body(.*)</body>");
        if(!m.Success){Log.log("[Error]body?");return;}
        r=m.Groups[0].Value;
        XFragment f=new XFragment(r,0);
        string txt="";
        string counter="";
        f.parts.ForEach((p)=>
        {
            if(p.GetType()==typeof(XText)){txt+=Util.Trim(p.originalText);counter+=Util.Trim(p.originalText);}
            if(p.GetType()==typeof(XTag))
            {
                XTag p0=(XTag)p;
                if(p.type==PartType.tag_start&&p0.tagname=="rt"){txt+="(";}
                if(p.type==PartType.tag_end&&p0.tagname=="rt"){txt+=")";}
                 if(p.type==PartType.tag_start&&p0.tagname=="p"){txt+="//";}
                if(p.type==PartType.tag_end&&p0.tagname=="p"){txt+="\r\n";}
                if(p.type==PartType.tag_end&&p0.tagname=="div"){txt+="\r\n";}
            }
        });
        if(Util.Trim(counter).Length>0)
        File.WriteAllText("epub2note_output/"+name+".txt",txt);
        

    }


}