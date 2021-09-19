using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace JassScriptOptimizer
{
    class Program
    {

        public struct vartypevalue
        {
            public string type;
            public string value;
            public int strid;
        }

        public struct vartypevalue2
        {
            public string type;
            public string value;
            public int strid;
            public bool found;
            public int foundcount;
        }

        public struct funcstr
        {
            public string funcname;
            public string[] funcpseudocalls;
            public int strid;
            public int endid;
            public bool used;
        }

        static List<int> commentlines = new List<int>();

        public static void LogAdd(string text)
        {
            if (!File.Exists(".\\leaklist.txt"))
            {
                File.Create(".\\leaklist.txt").Close();
            }

            List<string> outdata = new List<string>(File.ReadAllLines(".\\leaklist.txt"));
            outdata.Add(text);
            // Console.WriteLine( text );
            File.WriteAllLines(".\\leaklist.txt", outdata.ToArray());
        }

        public static void VLogAdd(string text)
        {
            if (!File.Exists(".\\UnusedLocals.txt"))
            {
                File.Create(".\\UnusedLocals.txt").Close();
            }

            List<string> outdata = new List<string>(File.ReadAllLines(".\\UnusedLocals.txt"));
            outdata.Add(text);
            // Console.WriteLine( text );
            File.WriteAllLines(".\\UnusedLocals.txt", outdata.ToArray());
        }


        public static void XLogAdd(string text)
        {
            if (!File.Exists(".\\UnusedFunctions.txt"))
            {
                File.Create(".\\UnusedFunctions.txt").Close();
            }

            List<string> outdata = new List<string>(File.ReadAllLines(".\\UnusedFunctions.txt"));
            outdata.Add(text);
            // Console.WriteLine( text );
            File.WriteAllLines(".\\UnusedFunctions.txt", outdata.ToArray());
        }

        public static void XDLogAdd(string text)
        {
            if (!File.Exists(".\\UnusedGlobals.txt"))
            {
                File.Create(".\\UnusedGlobals.txt").Close();
            }

            List<string> outdata = new List<string>(File.ReadAllLines(".\\UnusedGlobals.txt"));
            outdata.Add(text);
            // Console.WriteLine( text );
            File.WriteAllLines(".\\UnusedGlobals.txt", outdata.ToArray());
        }


        static void ScanThisFuncList(int id)
        {
            functionstoscan[id].used = true;
            for (int i = 0; i < functionstoscan[id].funcpseudocalls.Length; i++)
            {
                string pseudocallstr = functionstoscan[id].funcpseudocalls[i];
                for (int n = 0; n < functionstoscan.Length; n++)
                {
                    if (functionstoscan[n].used)
                        continue;


                    if (functionstoscan[n].funcname == pseudocallstr)
                    {
                        ScanThisFuncList(n);
                    }
                }
            }
        }

        static funcstr[] functionstoscan;

        static void SaveUnusedFunctionss(string[] JassData)
        {
            string commentline = @"^\s*//";
            string funcstartregex = @"^\s*function\s+(\w+)\s+";
            string funcendregex = @"^\s*endfunction";


            //function Fcv

            List<funcstr> functionstoscanx = new List<funcstr>();


            for (int i = 0; i < JassData.Length; i++)
            {
                Match IsComment = Regex.Match(JassData[i], commentline);
                if (IsComment.Success)
                    continue;

                Console.Write("\r " + Convert.ToInt32((Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) / 1.05f) + "%  ");


                Match GetStartFunc = Regex.Match(JassData[i], funcstartregex);
                if (GetStartFunc.Success)
                {
                    funcstr tmpfuncstr = new funcstr();
                    tmpfuncstr.funcname = GetStartFunc.Groups[1].Value;
                    tmpfuncstr.used = false;
                    tmpfuncstr.strid = i;

                    List<string> funcpseudocalls = new List<string>();
                    // i++;
                    int n = i;
                    for (; n < JassData.Length; n++)
                    {
                        IsComment = Regex.Match(JassData[n], commentline);
                        if (IsComment.Success)
                            continue;

                        Match GetEndFunc = Regex.Match(JassData[n], funcendregex);
                        if (GetEndFunc.Success)
                        {
                            i = n;
                            tmpfuncstr.endid = i + 1;
                            break;
                        }
                        //Search type 1 
                        {
                            Regex regex = new Regex(@"call\s+(\w+)");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 2 
                        {
                            Regex regex = new Regex(@",\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }


                        //Search type 3 
                        {
                            Regex regex = new Regex(@"\(\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 4 
                        {
                            Regex regex = new Regex(@"=\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 5 
                        {
                            Regex regex = new Regex(@"\""(\w+)\""");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 6 
                        {
                            Regex regex = new Regex(@"function\s+(\w+)");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }


                        //Search type 7 
                        {
                            Regex regex = new Regex(@"^if\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 8 
                        {
                            Regex regex = new Regex(@"\Wif\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 9 
                        {
                            Regex regex = new Regex(@"return\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 10 
                        {
                            Regex regex = new Regex(@"\+\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }


                        //Search type 11 
                        {
                            Regex regex = new Regex(@"and\s+(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 12 
                        {
                            Regex regex = new Regex(@"or\s+(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 13 
                        {
                            Regex regex = new Regex(@"\[\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }


                        //Search type 14 
                        {
                            Regex regex = new Regex(@"^elseif\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 15
                        {
                            Regex regex = new Regex(@"\Welseif\s*(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                        //Search type 16
                        {
                            Regex regex = new Regex(@"\W(\w+)\s*\(");
                            foreach (Match match in regex.Matches(JassData[n]))
                            {
                                bool needadd = true;
                                for (int v = 0; v < funcpseudocalls.Count; v++)
                                {
                                    if (funcpseudocalls[v] == match.Groups[1].Value)
                                    {
                                        needadd = false;
                                    }
                                }
                                if (needadd)
                                    funcpseudocalls.Add(match.Groups[1].Value);
                            }
                        }

                    }
                    tmpfuncstr.funcpseudocalls = funcpseudocalls.ToArray();
                    functionstoscanx.Add(tmpfuncstr);

                }

            }
            functionstoscan = functionstoscanx.ToArray();
            XLogAdd("Found functions in script: " + functionstoscan.Length);

            int mainid = -1;

            for (int i = 0; i < functionstoscan.Length; i++)
            {
                if (functionstoscan[i].funcname == "main")
                {
                    mainid = i;
                    XLogAdd("All okay. Function main found!");
                }
            }

            Console.Write("\r 97%  ");


            if (mainid != -1)
            {
                ScanThisFuncList(mainid);

                Console.Write("\r 99%  ");

                for (int n = 0; n < functionstoscan.Length; n++)
                {
                    if (functionstoscan[n].used == false)
                    {
                        XLogAdd("Warning. Function " + functionstoscan[n].funcname + " never used.");
                        for (int i = functionstoscan[n].strid; i < functionstoscan[n].endid; i++)
                        {
                            commentlines.Add(i);
                        }
                    }
                }

                Console.Write("\r 100%  ");
            }
        }


        static void SaveUnusedLocalVars(string[] JassData)
        {
            string funcstartregex = @"^\s*function\s+(\w+)\s+";
            string funcendregex = @"^\s*endfunction";

            string getlocalname = @"^\s*local\s+(\w+)\s+(\w+)";
            string getlocalarrayname = @"^\s*local\s+(\w+)\s+array\s+(\w+)";
            string commentline = @"^\s*//";

            int allvars = 0;
            int foundvars = 0;
            int allfunctions = 0;
            int funcstartline = 0;
            for (int i = 0; i < JassData.Length; i++)
            {
                Match IsComment = Regex.Match(JassData[i], commentline);
                if (IsComment.Success)
                    continue;

                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                Match GetStartFunc = Regex.Match(JassData[i], funcstartregex);
                if (GetStartFunc.Success)
                {
                    funcstartline = i;
                    i++;
                    allfunctions++;
                    List<vartypevalue2> localslist = new List<vartypevalue2>();
                    int endlocalid = 0;

                    vartypevalue2[] localslistout;
                    int n = i;

                    for (; n < JassData.Length; n++)
                    {
                        IsComment = Regex.Match(JassData[n], commentline);
                        if (IsComment.Success)
                            continue;

                        Match GetEndFunc = Regex.Match(JassData[n], funcendregex);
                        if (GetEndFunc.Success)
                        {
                            localslistout = localslist.ToArray();

                            for (int z = i; z < n; z++)
                            {
                                for (int x = 0; x < localslistout.Length; x++)
                                {
                                    if (localslistout[x].strid == z)
                                        continue;
                                    if (localslistout[x].found)
                                        continue;

                                    string foundvarname = @"\W" + localslistout[x].value + @"\W";
                                    if (Regex.Match(JassData[z], foundvarname).Success)
                                    {
                                        foundvars++;
                                        localslistout[x].found = true;
                                        localslistout[x].foundcount++;
                                        if (localslistout[x].foundcount == 1)
                                        {
                                            string checknull = @"\s*set\s+" + localslistout[x].value + @"\s*=";

                                            if (Regex.Match(JassData[z], checknull).Success)
                                                localslistout[x].found = false;
                                            else
                                            {
                                                checknull = @"\s*set\s+" + localslistout[x].value + @"\s*\[.*?=";
                                                if (Regex.Match(JassData[z], checknull).Success)
                                                    localslistout[x].found = false;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        foundvarname = @"\W" + localslistout[x].value + @"$";
                                        if (Regex.Match(JassData[z], foundvarname).Success)
                                        {
                                            foundvars++;
                                            localslistout[x].found = true;
                                            localslistout[x].foundcount++;
                                        }
                                        /*else
                                        {
                                            foundvarname = @"^" + localslistout [ x ].value + @"\W";
                                            if ( Regex.Match( JassData [ z ] , foundvarname ).Success )
                                            {
                                                foundvars++;
                                                localslistout [ x ].found = true;
                                            }
                                        }*/

                                        if (localslistout[x].foundcount == 1)
                                        {
                                            string checknull = @"\s*set\s+" + localslistout[x].value + @"\s*=";

                                            if (Regex.Match(JassData[z], checknull).Success)
                                                localslistout[x].found = false;
                                            else
                                            {
                                                checknull = @"\s*set\s+" + localslistout[x].value + @"\s*\[.*?=";
                                                if (Regex.Match(JassData[z], checknull).Success)
                                                    localslistout[x].found = false;

                                            }
                                        }
                                    }
                                }
                            }

                            bool firsttime = true;
                            for (int x = 0; x < localslistout.Length; x++)
                            {
                                if (!localslistout[x].found)
                                {
                                    if (firsttime)
                                    {
                                        firsttime = false;
                                        VLogAdd( /*"[" + funcstartline + "] " +*/ "Function: " + GetStartFunc.Groups[1] + " Warning. Found unused local variable!");
                                    }
                                    commentlines.Add(localslistout[x].strid);
                                    VLogAdd( /*"[" + localslistout [ x ].strid + "] Var " +*/ localslistout[x].value + " type " + localslistout[x].type + " declared but not used!");
                                }
                            }

                            i = n;

                            break;

                        }
                        else if (endlocalid == 0)
                        {
                            Match GetLocalName = Regex.Match(JassData[n], getlocalarrayname);
                            if (GetLocalName.Success)
                            {
                                vartypevalue2 tmpvar = new vartypevalue2();
                                tmpvar.strid = n;
                                tmpvar.type = GetLocalName.Groups[1].Value;
                                tmpvar.value = GetLocalName.Groups[2].Value;
                                tmpvar.found = false;
                                tmpvar.foundcount = 0;
                                allvars++;
                                localslist.Add(tmpvar);
                            }
                            else
                            {
                                GetLocalName = Regex.Match(JassData[n], getlocalname);
                                if (GetLocalName.Success)
                                {
                                    vartypevalue2 tmpvar = new vartypevalue2();
                                    tmpvar.strid = n;
                                    tmpvar.type = GetLocalName.Groups[1].Value;
                                    tmpvar.value = GetLocalName.Groups[2].Value;
                                    tmpvar.found = false;
                                    tmpvar.foundcount = 0;
                                    allvars++;
                                    localslist.Add(tmpvar);
                                }
                                else
                                {
                                    endlocalid = n;
                                }
                            }


                        }
                    }
                }

            }

        }


        static void LeakScanner(string[] JassData)
        {

            List<string> NullLocals = new List<string>();

            if (File.Exists(".\\checkleak.txt"))
            {
                string[] leaklist = File.ReadAllLines(".\\checkleak.txt");
                foreach (string str in leaklist)
                    NullLocals.Add(str);
            }
            else
            {
                NullLocals.Add("agent");
                NullLocals.Add("event");
                //   NullLocals.Add( "player" );
                NullLocals.Add("widget");
                NullLocals.Add("unit");
                NullLocals.Add("destructable");
                NullLocals.Add("item");
                NullLocals.Add("ability");
                NullLocals.Add("buff");
                NullLocals.Add("force");
                //    NullLocals.Add( "group" );
                NullLocals.Add("trigger");
                NullLocals.Add("triggercondition");
                NullLocals.Add("triggeraction");
                NullLocals.Add("timer");
                NullLocals.Add("location");
                NullLocals.Add("region");
                NullLocals.Add("rect");
                NullLocals.Add("boolexpr");
                NullLocals.Add("sound");
                NullLocals.Add("conditionfunc");
                NullLocals.Add("filterfunc");
                NullLocals.Add("unitpool");
                NullLocals.Add("itempool");
                NullLocals.Add("race");
                NullLocals.Add("alliancetype");
                NullLocals.Add("racepreference");
                NullLocals.Add("gamestate");
                NullLocals.Add("igamestate");
                NullLocals.Add("fgamestate");
                NullLocals.Add("playerstate");
                NullLocals.Add("playerscore");
                NullLocals.Add("playergameresult");
                NullLocals.Add("unitstate");
                NullLocals.Add("aidifficulty");

                NullLocals.Add("eventid");
                NullLocals.Add("gameevent");
                NullLocals.Add("playerevent");
                NullLocals.Add("playerunitevent");
                NullLocals.Add("unitevent");
                NullLocals.Add("limitop");
                NullLocals.Add("widgetevent");
                NullLocals.Add("dialogevent");
                NullLocals.Add("unittype");

                NullLocals.Add("gamespeed");
                NullLocals.Add("gamedifficulty");
                NullLocals.Add("gametype");
                NullLocals.Add("mapflag");
                NullLocals.Add("mapvisibility");
                NullLocals.Add("mapsetting");
                NullLocals.Add("mapdensity");
                NullLocals.Add("mapcontrol");
                NullLocals.Add("playerslotstate");
                NullLocals.Add("volumegroup");
                NullLocals.Add("camerafield");
                NullLocals.Add("camerasetup");
                NullLocals.Add("playercolor");
                NullLocals.Add("placement");
                NullLocals.Add("startlocprio");
                NullLocals.Add("raritycontrol");
                NullLocals.Add("blendmode");
                NullLocals.Add("texmapflags");
                NullLocals.Add("effect");
                NullLocals.Add("effecttype");
                NullLocals.Add("weathereffect");
                NullLocals.Add("terraindeformation");
                NullLocals.Add("fogstate");
                NullLocals.Add("fogmodifier");
                NullLocals.Add("dialog");
                NullLocals.Add("button");
                NullLocals.Add("quest");
                NullLocals.Add("questitem");
                NullLocals.Add("defeatcondition");
                NullLocals.Add("timerdialog");
                NullLocals.Add("leaderboard");
                NullLocals.Add("multiboard");
                NullLocals.Add("multiboarditem");
                NullLocals.Add("trackable");
                NullLocals.Add("gamecache");
                NullLocals.Add("version");
                NullLocals.Add("itemtype");
                //     NullLocals.Add( "texttag" );
                NullLocals.Add("attacktype");
                NullLocals.Add("damagetype");
                NullLocals.Add("weapontype");
                NullLocals.Add("soundtype");
                NullLocals.Add("lightning");
                NullLocals.Add("pathingtype");
                NullLocals.Add("image");
                NullLocals.Add("ubersplat");
                NullLocals.Add("hashtable");
            }
            string funcstartregex = @"\s*function\s+(\w+)\s+";


            string funcendregex = @"\s*endfunction";

            int currentfuncstartid = 0;
            int currentfuncendid = 0;
            string currentfuncname = string.Empty;
            string commentline = @"^\s*//";
            for (int i = 0; i < JassData.Length; i++)
            {

                Match IsComment = Regex.Match(JassData[i], commentline);
                if (IsComment.Success)
                    continue;


                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                Match getstartfunc = Regex.Match(JassData[i], funcstartregex);

                if (getstartfunc.Success)
                {
                    currentfuncname = getstartfunc.Groups[1].Value;
                    currentfuncstartid = i;
                }
                Match getendfunc = Regex.Match(JassData[i], funcendregex);
                if (getendfunc.Success)
                {
                    currentfuncendid = i;
                    List<vartypevalue> localslist = new List<vartypevalue>();
                    foreach (string currentlocaltype in NullLocals)
                    {
                        for (int x = currentfuncstartid; x < currentfuncendid; x++)
                        {
                            string localnameregex = @"^\s*local\s+" + currentlocaltype + @"\s+(\w+)";
                            Match GetCurrentLocalName = Regex.Match(JassData[x], localnameregex);
                            if (GetCurrentLocalName.Success)
                            {
                                vartypevalue tmpvartval = new vartypevalue();
                                tmpvartval.type = currentlocaltype;
                                tmpvartval.value = GetCurrentLocalName.Groups[1].Value;
                                tmpvartval.strid = x;

                                localslist.Add(tmpvartval);
                                break;
                            }
                        }
                    }

                    foreach (vartypevalue currentlocal in localslist)
                    {
                        bool nullfound = false;
                        for (int x = currentfuncstartid; x < currentfuncendid; x++)
                        {
                            string checknull = @"\s*set\s+" + currentlocal.value + @".*?=\s*null";

                            if (Regex.Match(JassData[x], checknull).Success)
                            {

                                nullfound = true;
                            }
                            else
                            {
                                string checknotnull = @"\s*set\s+" + currentlocal.value + @"\s*=";
                                if (Regex.Match(JassData[x], checknotnull).Success)
                                {
                                    nullfound = false;
                                }
                                else
                                {
                                    checknull = @"\s*set\s+" + currentlocal.value + @"\s*\[.*?=\s*null";
                                    if (Regex.Match(JassData[x], checknull).Success)
                                    {
                                        nullfound = true;
                                    }
                                    else
                                    {
                                        checknull = @"\s*set\s+" + currentlocal.value + @"\s*\[.*?=";
                                        if (Regex.Match(JassData[x], checknotnull).Success)
                                        {
                                            nullfound = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (nullfound == false)
                        {
                            LogAdd( /*"Str id №:" + currentlocal.strid + */". Function :\"" + currentfuncname + "\"");
                            LogAdd("Local: \"" + currentlocal.value + "\" type (:" + currentlocal.type + ") leak found.");
                        }

                    }




                }


            }
        }


        static void ScanUnusedGlobals(string[] JassData)
        {
            string globalstartregex = @"^\s*globals";
            string globalendregex = @"^\s*endglobals";
            string commentline = @"(^\s*//)|(^\s*$)";

            //integer array UnitModelScaleUnitIdList
            string getvarnamearray = @"^\s*(\w+)\s+array\s+(\w+).*";

            string getvarname = @"^\s*(\w+)\s+(\w+).*";
            List<vartypevalue2> localslist = new List<vartypevalue2>();
            vartypevalue2[] globalslistarrays = null;
            int i = 0;

            for (; i < JassData.Length; i++)
            {

                //if (JassData[i].IndexOf("UnitModelScaleUnitIdList") > -1)
                //{
                //    Console.WriteLine("YES0");
                //    Console.WriteLine(JassData[i]);

                //}
                if (Regex.Match(JassData[i], commentline).Success)
                    continue;

                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                if (Regex.Match(JassData[i], globalstartregex).Success)
                {


                    int n = i;
                    for (; n < JassData.Length; n++)
                    {


                        Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(n) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                        if (Regex.Match(JassData[n], globalendregex).Success)
                        {
                            i = n + 1;
                            globalslistarrays = localslist.ToArray();
                            n = JassData.Length;
                            i = JassData.Length;
                            break;
                        }


                        Match GetGlobal = Regex.Match(JassData[n], getvarnamearray);
                        if (GetGlobal.Success)
                        {
                            vartypevalue2 vvar = new vartypevalue2();
                            vvar.type = GetGlobal.Groups[1].Value;
                            vvar.value = GetGlobal.Groups[2].Value;
                            vvar.strid = n;
                            vvar.found = false;
                            vvar.foundcount = 0;
                            localslist.Add(vvar);
                        }
                        else
                        {
                            GetGlobal = Regex.Match(JassData[n], getvarname);
                            if (GetGlobal.Success)
                            {
                                vartypevalue2 vvar = new vartypevalue2();
                                vvar.type = GetGlobal.Groups[1].Value;
                                vvar.value = GetGlobal.Groups[2].Value;
                                vvar.strid = n;
                                vvar.found = false;
                                vvar.foundcount = 0;
                                localslist.Add(vvar);
                            }
                        }
                    }


                    break;
                }
            }

            if (globalslistarrays != null)
            {
                XDLogAdd("Found " + globalslistarrays.Length + " globals.");


                int counttoprintpercent = 1000;


                for (; i < JassData.Length; i++)
                {
                    if (Regex.Match(JassData[i], commentline).Success)
                        continue;

                    counttoprintpercent--;
                    if (counttoprintpercent == 0)
                    {
                        counttoprintpercent = 1000;
                        Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");
                    }

                    for (int x = 0; x < globalslistarrays.Length; x++)
                    {
                        if (globalslistarrays[x].found)
                            continue;

                        if (JassData[i].IndexOf(globalslistarrays[x].value, 0, StringComparison.Ordinal) > -1)
                        {

                            if (Regex.Match(JassData[i], @"\W" + globalslistarrays[x].value + @"\W").Success)
                            {
                                globalslistarrays[x].found = true;
                                globalslistarrays[x].foundcount++;
                            }
                            else if (Regex.Match(JassData[i], @"\W" + globalslistarrays[x].value + @"$").Success)
                            {
                                globalslistarrays[x].found = true;
                                globalslistarrays[x].foundcount++;
                            }

                            if (globalslistarrays[x].foundcount == 1)
                            {


                                string checknull = @"\s*set\s+" + globalslistarrays[x].value + @"\s*=";

                                if (Regex.Match(JassData[i], checknull).Success)
                                    globalslistarrays[x].found = false;
                                else
                                {
                                    checknull = @"\s*set\s+" + globalslistarrays[x].value + @"\s*\[.*?=";
                                    if (Regex.Match(JassData[i], checknull).Success)
                                        globalslistarrays[x].found = false;

                                }

                            }

                        }
                    }
                }
                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");


                for (int x = 0; x < globalslistarrays.Length; x++)
                {
                    if (globalslistarrays[x].found)
                        continue;
                    else
                    {
                        XDLogAdd("Warning. Global " + globalslistarrays[x].value + " (type:" + globalslistarrays[x].type + ")  declared but not used!");
                    }
                }


            }
            else
                XDLogAdd("Fatal error in scanner...");

        }


        static void ScanErrors(string[] JassData)
        {
            string LoopStr = @"\s*loop";
            string EndLoopStr = @"\s*endloop";
            string ReturnStr = @"\s*return";
            string ExitwhenStr = @"\s*exitwhen";

            string globalstartregex = @"^\s*globals";
            string globalendregex = @"^\s*endglobals";
            string commentline = @"^\s*//|^\s*$";

            //integer array UnitModelScaleUnitIdList
            string getvarnamearray = @"^\s*(\w+)\s+array\s+(\w+).*";

            string getvarname = @"^\s*(\w+)\s+(\w+).*";
            List<vartypevalue2> localslist = new List<vartypevalue2>();
            vartypevalue2[] globalslistarrays = null;
            int i = 0;
            int startdata = 0;

            for (; i < JassData.Length; i++)
            {

                //if (JassData[i].IndexOf("UnitModelScaleUnitIdList") > -1)
                //{
                //    Console.WriteLine("YES0");
                //    Console.WriteLine(JassData[i]);

                //}
                if (Regex.Match(JassData[i], commentline).Success)
                    continue;

                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                if (Regex.Match(JassData[i], globalstartregex).Success)
                {


                    int n = i;
                    for (; n < JassData.Length; n++)
                    {


                        Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(n) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                        if (Regex.Match(JassData[n], globalendregex).Success)
                        {
                            startdata = i;
                            globalslistarrays = localslist.ToArray();
                            n = JassData.Length;
                            i = JassData.Length;
                            break;
                        }


                        Match GetGlobal = Regex.Match(JassData[n], getvarnamearray);
                        if (GetGlobal.Success)
                        {
                            vartypevalue2 vvar = new vartypevalue2();
                            vvar.type = GetGlobal.Groups[1].Value;
                            vvar.value = GetGlobal.Groups[2].Value;
                            vvar.strid = n;
                            vvar.found = false;
                            vvar.foundcount = 0;
                            localslist.Add(vvar);
                        }
                        else
                        {
                            GetGlobal = Regex.Match(JassData[n], getvarname);
                            if (GetGlobal.Success)
                            {
                                vartypevalue2 vvar = new vartypevalue2();
                                vvar.type = GetGlobal.Groups[1].Value;
                                vvar.value = GetGlobal.Groups[2].Value;
                                vvar.strid = n;
                                vvar.found = false;
                                vvar.foundcount = 0;
                                localslist.Add(vvar);
                            }
                        }
                    }


                    break;
                }
            }

            if (globalslistarrays != null)
            {
                XDLogAdd("Found " + globalslistarrays.Length + " globals.");
                for (int n = startdata; n < JassData.Length; n++)
                {



                }

            }


        }

        static void Main(string[] args)
        {

            try
            {
                File.Delete(".\\UnusedFunctions.txt");
            }
            catch
            {

            }
            try
            {
                File.Delete(".\\UnusedGlobals.txt");
            }
            catch
            {

            }
            try
            {
                File.Delete(".\\UnusedLocals.txt");
            }
            catch
            {

            }
            try
            {
                File.Delete(".\\leaklist.txt");
            }
            catch
            {

            }
            Console.WriteLine("Jass script warning helper v0.4b!");
            Console.WriteLine("Scan problems in script. Enter path to war3map.j without quotes:");
            string[] JassData = File.ReadAllLines(Console.ReadLine());
            List<string> FixedJassData = new List<string>(0);

            Console.WriteLine("Read script...");
            for (int i = 0; i < JassData.Length; i++)
            {
                Console.Write("\r " + Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(JassData.Length) * 100.0f) + "%  ");

                JassData[i] = Regex.Replace(JassData[i], @"^\s*(\w+)\s+array\s+(.*?)$", "$1 $2");
                JassData[i] = Regex.Replace(JassData[i], @"^\s*(\w+)\s+(\w+)\s+array\s+(.*?)$", "$1 $2 $3");
                JassData[i] = Regex.Replace(JassData[i], @"^\s*constant\s+(.*?)$", "$2");

                if (Regex.Match(JassData[i], @"\s*/").Success
                    || Regex.Match(JassData[i], @"^\s*$").Success
                    )
                    continue;

                FixedJassData.Add(JassData[i]);
            }

            JassData = FixedJassData.ToArray();

            Console.WriteLine("\rRead completed!");

            Console.WriteLine("Scanning: unused functions...");
            SaveUnusedFunctionss(JassData);
            Console.WriteLine("\rScan completed!");
            Console.WriteLine("Saved to UnusedFunctions.txt");

            Console.WriteLine("Scanning: unused global vars...");
            ScanUnusedGlobals(JassData);
            Console.WriteLine("\rScan completed!");
            Console.WriteLine("Saved to UnusedGlobals.txt");


            Console.WriteLine("Scanning: unused local vars...");
            SaveUnusedLocalVars(JassData);
            Console.WriteLine("\rScan completed!");
            Console.WriteLine("Saved to UnusedLocals.txt");


            Console.WriteLine("Scanning: leaks...");
            LeakScanner(JassData);
            Console.WriteLine("\rScan completed!");
            Console.WriteLine("Saved to leaklist.txt");

            Console.WriteLine("Scanning: errors...");
            ScanErrors(JassData);
            Console.WriteLine("\rScan completed!");
            Console.WriteLine("Saved to errors.txt");

            Console.WriteLine("\rPress any key to exit.!");
            Console.ReadLine();
        }

    }
}
