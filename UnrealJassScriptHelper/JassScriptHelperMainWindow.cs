using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.Collections.Concurrent;

namespace UnrealJassScriptHelper
{
    public partial class JassScriptHelperMainWindow : Form
    {
        public JassScriptHelperMainWindow()
        {
            InitializeComponent();
        }

        FileIniDataParser iniparser = new FileIniDataParser();
        IniData config = new IniData();
        const string configfilename = "JassSriptHelper.ini";
        bool NeedCleanupArgs = false;

        Object LockObject = new Object();


        int FuncNeedForCreateNewGlobal = 3;

        void SaveConfig()
        {


            // Save all path
            config["Path"]["inputscript"] = inputscript.Text;
            config["Path"]["OutOptimizedScript"] = OutOptimizedScript.Text;
            config["Path"]["LogLeakDetected"] = LogLeakDetected.Text;
            config["Path"]["LoadLeakForScan"] = LoadLeakForScan.Text;
            config["Path"]["LogUnusedGlobals"] = LogUnusedGlobals.Text;
            config["Path"]["LogUnusedFunctions"] = LogUnusedFunctions.Text;
            config["Path"]["LogUnusedLocals"] = LogUnusedLocals.Text;
            config["Path"]["OutOptimizeHelper"] = OutOptimizeHelper.Text;
            config["Path"]["OptimizeFuncUsageFile"] = OptimizeFuncUsageFile.Text;
            // Save checked options
            config["Options"]["WriteOptimizedScript"] = WriteOptimizedScript.Checked ? "y" : "n";
            config["Options"]["WriteLeakList"] = WriteLeakList.Checked ? "y" : "n";
            config["Options"]["HaveCustomLeakList"] = HaveCustomLeakList.Checked ? "y" : "n";
            config["Options"]["WriteUnusedGlobals"] = WriteUnusedGlobals.Checked ? "y" : "n";
            config["Options"]["WriteUnusedFunctions"] = WriteUnusedFunctions.Checked ? "y" : "n";
            config["Options"]["LogUnusedLocals"] = WriteUnusedLocals.Checked ? "y" : "n";
            config["Options"]["WriteOptimizations"] = WriteOptimizations.Checked ? "y" : "n";
            config["Options"]["NeedSaveCommentLines"] = NeedSaveCommentLines.Checked ? "y" : "n";
            config["Options"]["SaveDebugInfo"] = SaveDebugInfo.Checked ? "y" : "n";
            // Save other options
            config["HiddenOptions"]["CleanupArgs"] = NeedCleanupArgs ? "y" : "n";
            config["HiddenOptions"]["FuncNeedForCreateNewGlobal"] = FuncNeedForCreateNewGlobal.ToString();

            iniparser.WriteFile(configfilename, config, Encoding.UTF8);
        }


        void LoadConfig()
        {
            if (!File.Exists(configfilename))
                SaveConfig();
            config = iniparser.ReadFile(configfilename);
            // Read all path
            inputscript.Text = config["Path"]["inputscript"];
            OutOptimizedScript.Text = config["Path"]["OutOptimizedScript"];
            LogLeakDetected.Text = config["Path"]["LogLeakDetected"];
            LoadLeakForScan.Text = config["Path"]["LoadLeakForScan"];
            LogUnusedGlobals.Text = config["Path"]["LogUnusedGlobals"];
            LogUnusedFunctions.Text = config["Path"]["LogUnusedFunctions"];
            LogUnusedLocals.Text = config["Path"]["LogUnusedLocals"];
            OutOptimizeHelper.Text = config["Path"]["OutOptimizeHelper"];
            OptimizeFuncUsageFile.Text = config["Path"]["OptimizeFuncUsageFile"];
            // Read checked options
            WriteOptimizedScript.Checked = config["Options"]["WriteOptimizedScript"].ToLower() == "y" ? true : false;
            WriteLeakList.Checked = config["Options"]["WriteLeakList"].ToLower() == "y" ? true : false;
            HaveCustomLeakList.Checked = config["Options"]["HaveCustomLeakList"].ToLower() == "y" ? true : false;
            WriteUnusedGlobals.Checked = config["Options"]["WriteUnusedGlobals"].ToLower() == "y" ? true : false;
            WriteUnusedFunctions.Checked = config["Options"]["WriteUnusedFunctions"].ToLower() == "y" ? true : false;
            WriteUnusedLocals.Checked = config["Options"]["LogUnusedLocals"].ToLower() == "y" ? true : false;
            WriteOptimizations.Checked = config["Options"]["WriteOptimizations"].ToLower() == "y" ? true : false;
            NeedSaveCommentLines.Checked = config["Options"]["NeedSaveCommentLines"].ToLower() == "y" ? true : false;
            SaveDebugInfo.Checked = config["Options"]["SaveDebugInfo"].ToLower() == "y" ? true : false;
            // Read other options
            NeedCleanupArgs = config["HiddenOptions"]["CleanupArgs"].ToLower() == "y" ? true : false;
            FuncNeedForCreateNewGlobal = 3;
            int.TryParse(config["HiddenOptions"]["FuncNeedForCreateNewGlobal"], out FuncNeedForCreateNewGlobal);
            if (FuncNeedForCreateNewGlobal <= 0)
                FuncNeedForCreateNewGlobal = 0;
            if (FuncNeedForCreateNewGlobal >= 10)
                FuncNeedForCreateNewGlobal = 10;

        }

        struct Variable
        {
            public string Type;
            public string Name;
            public string RawData;
            public string RawDataWithoutString;
            public int Line;
            public bool IsArray;
            public bool UsedInCode;
            public bool Cleaned;
            public bool Changed;
            public int FunctionId;
            public string[] commentlines;
        }

        List<Variable> GlobalVariables = new List<Variable>();


        struct OptimizeFuncUsage
        {
            public string vartype;
            public string funcname;
        }

        List<OptimizeFuncUsage> OptimizeFuncUsageList = new List<OptimizeFuncUsage>();

        const string RegexConstantOrLocal = @"^\s*constant\s+(.*?)$|^\s*local\s+(.*?)$";
        const string getvarnameandtype = @"^\s*(\w+)\s+array\s+(\w+)|^\s*(\w+)\s+(\w+)";
        const string getvarlistfromstr = @",\s*(\w+\s+array|\w+)\s+(\w+)";
        const string foundsetvalue = @"\W=\W|\w=\W";
        const string foundsetnullvalue = @"\W=\s*null|\w=\s*null";
        const string foundcallstart = @"^\s*call\s+";

        bool FirstCreatedCustomGlobal = true;


        Variable[] GetVariableListFromStringForFunc(string str, int lineid, int functionid)
        {
            List<Variable> outvarlist = new List<Variable>();
            Variable tmpvar = new Variable();
            tmpvar.UsedInCode = false;
            tmpvar.Cleaned = true;
            tmpvar.Changed = false;
            tmpvar.Line = lineid;
            string fixedstr = "," + str;

            foreach (Match m in Regex.Matches(fixedstr, getvarlistfromstr))
            {
                tmpvar.Name = m.Groups[2].Value;
                tmpvar.Type = Regex.Match(m.Groups[1].Value, @"(\w+)").Groups[1].Value; // skip array
                tmpvar.IsArray = m.Groups[1].Value.IndexOf(" array") > -1;
                tmpvar.RawData = m.Groups[0].Value;
                tmpvar.RawDataWithoutString = NormalizeStringForSearch(m.Groups[0].Value);
                outvarlist.Add(tmpvar);
            }

            return outvarlist.ToArray();
        }

        bool GetVariableFromString(string str, int lineid, ref Variable outvar, bool localhelper = false)
        {
            Match fixregex = Regex.Match(str, RegexConstantOrLocal);
            string fixedstring = str;
            if (fixregex.Success)
            {
                if (fixregex.Groups[1].Length == 0)
                    fixedstring = fixregex.Groups[2].Value;
                else
                    fixedstring = fixregex.Groups[1].Value;

            }
            else if (localhelper)
                return false;
            Match getvardata = Regex.Match(fixedstring, getvarnameandtype);
            if (getvardata.Success)
            {
                string varname, vartype = varname = string.Empty;
                if (getvardata.Groups[4].Length == 0)
                {
                    outvar.Name = getvardata.Groups[2].Value;
                    outvar.Type = getvardata.Groups[1].Value;
                    outvar.IsArray = true;
                }
                else
                {
                    outvar.Name = getvardata.Groups[4].Value;
                    outvar.Type = getvardata.Groups[3].Value;
                }
                outvar.Line = lineid;
                outvar.RawData = str;
                outvar.RawDataWithoutString = NormalizeStringForSearch(str);
                outvar.Cleaned = false;
                outvar.Changed = true;
                /*if (!Regex.Match(outvar.RawDataWithoutString, foundsetvalue).Success)
                {
                   
                }*/
                if (Regex.Match(outvar.RawDataWithoutString, foundsetnullvalue).Success)
                {
                    outvar.Cleaned = true;
                    outvar.Changed = false;
                }

            }
            else return false;


            return true;
        }

        struct Function
        {
            public string Name;
            public string Type;
            public Variable[] Args;
            public Variable[] LocalVars;
            public string[] RawData;
            public string[] RawDataWithoutString;
            public int Line;
            public bool UsedInCode;
            public bool typecast;
            public string[] commentlines;
            public int RefCount;
        }

        string NormalizeStringForSearch(string str)
        {
            return Regex.Replace(str, "\".*?\"", string.Empty);
        }

        List<Function> Functions = new List<Function>();
        List<string> DumpOutString = new List<string>();
        List<string> NullLocals = new List<string>();

        List<string> GlobalLogFile = new List<string>();

        bool work = false;
        private void STARTSTART(object sender, DoWorkEventArgs e)
        {
            work = true;
            GlobalVariables.Clear();
            Functions.Clear();
            DumpOutString.Clear();
            NullLocals.Clear();
            GlobalLogFile.Clear();
            OptimizeFuncUsageList.Clear();
            FirstCreatedCustomGlobal = true;

            if (!File.Exists(inputscript.Text))
            {
                GlobalLogFile.Add("Scan stop. No input file found!");
                MessageBox.Show("First select input script file!", "Jass Script Helper error!");
                work = false;
                STARTSTARTSTART.Enabled = true;
                return;
            }

            // Read script file
            GlobalLogFile.Add("Read script file!");
            jassscripthelperstatus = "Load Script";
            string[] scriptdata = File.ReadAllLines(inputscript.Text);

            // Read leak vars for scan
            GlobalLogFile.Add("Read leak vars for scan");
            jassscripthelperstatus = "Load custom leak file";
            if (HaveCustomLeakList.Checked && File.Exists(LoadLeakForScan.Text))
            {
                NullLocals = new List<string>(File.ReadAllLines(LoadLeakForScan.Text));
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

            GlobalLogFile.Add("Scan for " + NullLocals.Count + " leaks type.");

            // Read global variables
            GlobalLogFile.Add("Read global variables");
            jassscripthelperstatus = "Load Globals";
            string globalstartregex = @"^\s*globals";
            string globalendregex = @"^\s*endglobals";
            string commentline = @"^\s*//|^\s*$";
            string commentdata = @"^(\s*.*?)(//.*?)$";

            List<string> commentlines = new List<string>();

            bool start = false;
            int i = 0;
            for (; i < scriptdata.Length; i++)
            {
                scriptprogressvalue = (int)((float)i / scriptdata.Length * 100.0f);

                if (Regex.Match(scriptdata[i], commentline).Success)
                {
                    commentlines.Add(scriptdata[i]);
                    continue;
                }

                Match commentdatamatch = Regex.Match(scriptdata[i], commentdata);

                if (commentdatamatch.Success)
                {
                    if (commentdatamatch.Groups[1].Value.IndexOf("\"") > -1
                        && commentdatamatch.Groups[2].Value.IndexOf("\"") > -1)
                    {
                        ;
                    }
                    else
                    {
                        commentlines.Add(commentdatamatch.Groups[2].Value);
                        scriptdata[i] = commentdatamatch.Groups[1].Value;
                    }
                }


                if (!start && Regex.Match(scriptdata[i], globalstartregex).Success)
                {
                    start = true;
                    continue;
                }


                if (start)
                {
                    if (Regex.Match(scriptdata[i], globalendregex).Success)
                        break;

                    Variable tmpvar = new Variable();
                    tmpvar.UsedInCode = false;
                    tmpvar.Cleaned = false;
                    if (GetVariableFromString(scriptdata[i], i, ref tmpvar))
                    {
                        tmpvar.FunctionId = -1;
                        tmpvar.commentlines = commentlines.ToArray();
                        commentlines.Clear();
                        GlobalVariables.Add(tmpvar);
                    }
                    else
                    {
                        MessageBox.Show("Found bad global variable at line:" + i);
                    }
                }

                if (!start)
                {
                    DumpOutString.Add(scriptdata[i]);
                }

            }
            i++;
            // Read functions
            jassscripthelperstatus = "Load Functions";
            string getfuncstart = @"^\s*(function|constant\s+function)\s+(.*?)\s+takes\s+(.*?)\s+returns\s+(.*)";

            Function tempfunc = new Function();
            tempfunc.UsedInCode = false;
            tempfunc.RefCount = 0;
            commentlines.Clear();
            start = false;
            List<string> funcrawdata = new List<string>();
            for (; i < scriptdata.Length; i++)
            {
                scriptprogressvalue = (int)((float)i / scriptdata.Length * 100.0f);

                if (Regex.Match(scriptdata[i], commentline).Success)
                {
                    commentlines.Add(scriptdata[i]);
                    continue;
                }

                Match commentdatamatch = Regex.Match(scriptdata[i], commentdata);

                if (commentdatamatch.Success)
                {
                    if (commentdatamatch.Groups[1].Value.IndexOf("\"") > -1
                        && commentdatamatch.Groups[2].Value.IndexOf("\"") > -1)
                    {
                        ;
                    }
                    else
                    {
                        commentlines.Add(commentdatamatch.Groups[2].Value);
                        scriptdata[i] = commentdatamatch.Groups[1].Value;
                    }
                }



                Match funcstartdata = Regex.Match(scriptdata[i], getfuncstart);
                if (funcstartdata.Success)
                {
                    start = true;
                    tempfunc.Name = funcstartdata.Groups[2].Value;

                    if (tempfunc.Name == "config" || tempfunc.Name == "main")
                        tempfunc.UsedInCode = true;
                    tempfunc.Args = GetVariableListFromStringForFunc(funcstartdata.Groups[3].Value, i, Functions.Count);
                    tempfunc.Type = funcstartdata.Groups[4].Value;
                    tempfunc.Line = i;
                    funcrawdata.Clear();
                }

                funcrawdata.Add(scriptdata[i]);

                if (Regex.Match(scriptdata[i], IsEndFunctionRegex).Success)
                {
                    if (start)
                    {
                        start = false;

                        tempfunc.RawData = funcrawdata.ToArray();
                        tempfunc.RawDataWithoutString = funcrawdata.ToArray();

                        List<Variable> tmpLocalVars = new List<Variable>();

                        Variable tmplocvar = new Variable();
                        for (int m = 1;
                            m < tempfunc.RawData.Length && GetVariableFromString(tempfunc.RawData[m], m, ref tmplocvar, true);
                            m++)
                        {
                            tmplocvar.FunctionId = Functions.Count;
                            tmpLocalVars.Add(tmplocvar);
                        }

                        bool foundtypecast = false;

                        foreach (Variable localvar in tmpLocalVars)
                        {
                            if (foundtypecast)
                                break;
                            foreach (Variable globalvar in GlobalVariables)
                            {
                                if (localvar.Name == globalvar.Name)
                                {
                                    foundtypecast = true;
                                    break;
                                }

                            }
                        }
                        tempfunc.typecast = foundtypecast;


                        for (int n = 0; n < tempfunc.RawDataWithoutString.Length; n++)
                            tempfunc.RawDataWithoutString[n] = NormalizeStringForSearch(tempfunc.RawDataWithoutString[n]);

                        tempfunc.LocalVars = tmpLocalVars.ToArray();
                        tempfunc.commentlines = commentlines.ToArray();
                        commentlines.Clear();
                        Functions.Add(tempfunc);
                    }

                }
                else if (!start)
                {
                    DumpOutString.Add(scriptdata[i]);
                }

            }

            // Scan unused globals
            if (WriteUnusedGlobals.Checked && LogUnusedGlobals.Text.Length > 0)
            {
                jassscripthelperstatus = "Search Unused Globals";
                ScanUnusedGlobals();
            }


            // Scan unused functions
            if (WriteUnusedFunctions.Checked && LogUnusedFunctions.Text.Length > 0)
            {
                jassscripthelperstatus = "Search Unused Functions";
                ScanUnusedFunctions();
            }

            // Scan unused locals // after scan unused functions!!!
            if (WriteUnusedLocals.Checked && LogUnusedLocals.Text.Length > 0)
            {
                jassscripthelperstatus = "Search Unused Locals";
                ScanUnusedLocals();
            }


            // Scan for leaks - After scan globals/locals/functions !!!
            if (WriteLeakList.Checked && LogLeakDetected.Text.Length > 0)
            {
                jassscripthelperstatus = "Search for leaks in code";
                ScanForLeaks();
            }

            // Load OptimizeFuncUsage data
            if (OptimizeFuncUsageFile.Text.Length > 0)
            {
                jassscripthelperstatus = "Load natives for optimize";
                string GetFuncUsageData = @"^\s*(\w+)\s*;\s*(.*?)\s*$";
                if (File.Exists(OptimizeFuncUsageFile.Text))
                {
                    OptimizeFuncUsage tmpFuncUsage = new OptimizeFuncUsage();
                    string[] tmpFuncUsageData = File.ReadAllLines(OptimizeFuncUsageFile.Text);
                    foreach (string s in tmpFuncUsageData)
                    {
                        Match tmpFuncUsageMatch = Regex.Match(s, GetFuncUsageData);
                        if (tmpFuncUsageMatch.Success)
                        {
                            tmpFuncUsage.vartype = tmpFuncUsageMatch.Groups[1].Value;
                            tmpFuncUsage.funcname = tmpFuncUsageMatch.Groups[2].Value;
                            OptimizeFuncUsageList.Add(tmpFuncUsage);
                        }
                        else
                        {
                            GlobalLogFile.Add("Error in loading '" + s + "' line in OptimizeFuncUsageFile");
                        }
                    }

                }
                GlobalLogFile.Add("Loaded " + OptimizeFuncUsageList.Count + " funcusagedata count.");
            }

            // Optimize functions, method: create GLOBAL var if func calls bigger than FuncNeedForCreateNewGlobal
            if (WriteOptimizations.Checked && OutOptimizeHelper.Text.Length > 0)
            {
                jassscripthelperstatus = "Start code optimization";
                OptimizeFunctions();
            }



            //....


            // Save optimized script

            if (WriteOptimizedScript.Checked && OutOptimizedScript.Text.Length > 0)
            {
                jassscripthelperstatus = "Save optimized script...";
                SaveOptimizedScript();
            }

            jassscripthelperstatus = "idle";
            work = false;
            STARTSTARTSTART.Enabled = true;

            File.WriteAllLines("JassScriptHelper.log", GlobalLogFile.ToArray());
        }

        const string IsReturnRegex = @"^\s*return";
        const string IsEndFunctionRegex = @"^\s*endfunction";

        void OptimizeFunctions()
        {
            int total_i = 0;

            ConcurrentStack<Variable> tmpNewGlobalVars = new ConcurrentStack<Variable>();
            ConcurrentStack<string> tmpOutOptimizeFile = new ConcurrentStack<string>();
            int creatednewglobals = 0;
            Parallel.For(0, Functions.Count, i =>
            {
                total_i++;
                scriptprogressvalue = (int)((float)total_i / Functions.Count * 100.0f);

                Function func = Functions[i];
                List<string> tmpNewRawData = new List<string>(func.RawData);

                foreach (OptimizeFuncUsage tmpFuncUsage in OptimizeFuncUsageList)
                {
                    string ExtractedFuncName = tmpFuncUsage.funcname + "()";
                    int FoundFuncUsage = 0;
                    int n = 0;

                    if (!SaveDebugInfo.Checked)
                    {
                        for (n = 1; n < tmpNewRawData.Count; n++)
                        {
                            if (!Regex.Match(tmpNewRawData[n], RegexConstantOrLocal).Success)
                            {
                                break;
                            }
                        }
                    }

                    int startfunc = n;

                    for (n = startfunc; n < tmpNewRawData.Count; n++)
                    {
                        if (tmpNewRawData[n].IndexOf(tmpFuncUsage.funcname, StringComparison.Ordinal) > -1)
                        {
                            if (Regex.Match(tmpNewRawData[n], foundcallstart + @"" + tmpFuncUsage.funcname + @"\s*\(\s*\)").Success)
                            {
                                continue;
                            }
                            Match tmpReplaceData = Regex.Match(tmpNewRawData[n], @"(\W)" + tmpFuncUsage.funcname + @"\s*\(\s*\)");
                            if (tmpReplaceData.Success)
                            {
                                FoundFuncUsage++;
                            }
                        }
                    }

                    if (FoundFuncUsage > FuncNeedForCreateNewGlobal)
                    {

                        int newglobalid = 0;
                        string newglobalname = func.Name + "_" + tmpFuncUsage.vartype + newglobalid;
                        bool NeedExitFromWhile = false;

                        while (!NeedExitFromWhile)
                        {
                            NeedExitFromWhile = true;
                            foreach (Variable var in tmpNewGlobalVars)
                            {
                                if (var.Name == newglobalname)
                                {
                                    newglobalid++;
                                    newglobalname = func.Name + "_" + tmpFuncUsage.vartype + newglobalid;
                                    NeedExitFromWhile = false;
                                }
                            }
                            foreach (Variable var in GlobalVariables)
                            {
                                if (var.Name == newglobalname)
                                {
                                    newglobalid++;
                                    newglobalname = func.Name + "_" + tmpFuncUsage.vartype + newglobalid;
                                    NeedExitFromWhile = false;
                                }
                            }
                        }

                        if (SaveDebugInfo.Checked)
                        {
                            tmpOutOptimizeFile.Push(ExtractedFuncName + " in " + func.Name + " function called " + FoundFuncUsage + " times.");
                        }
                        else
                            tmpOutOptimizeFile.Push("Function " + func.Name + " can be optimized by replace multiple call(" + FoundFuncUsage + ") " + ExtractedFuncName + " function by global variable!");


                        Variable newglovalvar = new Variable();
                        newglovalvar.Changed = true;
                        newglovalvar.UsedInCode = true;
                        newglovalvar.Type = tmpFuncUsage.vartype;
                        newglovalvar.Name = newglobalname;
                        newglovalvar.Line = func.Line;
                        newglovalvar.IsArray = false;
                        newglovalvar.FunctionId = i;
                        creatednewglobals++;

                        newglovalvar.commentlines = new string[0];

                        newglovalvar.RawDataWithoutString = newglovalvar.RawData = newglovalvar.Type + " " + newglovalvar.Name;
                        tmpNewGlobalVars.Push(newglovalvar);



                        tmpNewRawData.Insert(startfunc, "set " + newglobalname + " = " + ExtractedFuncName);

                        for (n = startfunc + 1; n < tmpNewRawData.Count; n++)
                        {
                            if (tmpNewRawData[n].IndexOf(tmpFuncUsage.funcname, StringComparison.Ordinal) > -1)
                            {
                                if (Regex.Match(tmpNewRawData[n], foundcallstart + @"" + tmpFuncUsage.funcname + @"\s*\(\s*\)").Success)
                                {
                                    continue;
                                }
                                Match tmpReplaceData = Regex.Match(tmpNewRawData[n], @"(\W)" + tmpFuncUsage.funcname + @"\s*\(\s*\)");
                                if (tmpReplaceData.Success)
                                {
                                    tmpNewRawData[n] = Regex.Replace(tmpNewRawData[n], @"(\W)" + tmpFuncUsage.funcname + @"\s*\(\s*\)",
                                        "$1" + newglobalname + " ");
                                }
                            }
                        }
                    }
                }
                func.RawData = tmpNewRawData.ToArray();
                Functions[i] = func;
            });



            GlobalVariables.AddRange(tmpNewGlobalVars.ToList());
            if (creatednewglobals > 0)
            {
                Variable tmpVar = GlobalVariables[GlobalVariables.Count - creatednewglobals];
                tmpVar.commentlines =
                    new string[1] { "//Next globals created by Jass Script Helper tool." };
                GlobalVariables[GlobalVariables.Count - creatednewglobals] = tmpVar;
            }

            if (OutOptimizeHelper.Text.Length > 0)
            {
                File.WriteAllLines(OutOptimizeHelper.Text, tmpOutOptimizeFile.ToArray());
            }


        }

        void ScanForLeaks()
        {
            int total_i = 0;

            Parallel.For(0, Functions.Count, i =>
            {
                total_i++;
                scriptprogressvalue = (int)((float)total_i / Functions.Count * 100.0f);



                for (int m = 0; m < Functions[i].LocalVars.Length; m++)
                {
                    Function func = Functions[i];
                    Variable var = func.LocalVars[m];
                    string isnullvar = @"^\s*set\s+" + var.Name + @".*?=\s*null";
                    string orisnotnull = @"^\s*set\s+" + var.Name + @"\W";

                    foreach (string s in func.RawDataWithoutString)
                    {
                        if (s.IndexOf(var.Name, StringComparison.Ordinal) > -1)
                        {
                            Match IsVarNotNull = Regex.Match(s, orisnotnull);
                            if (Regex.Match(s, isnullvar).Success && IsVarNotNull.Success)
                            {
                                //  var.Cleaned = true;
                                //  var.Changed = false;
                            }
                            else if (IsVarNotNull.Success)
                            {
                                //  var.Changed = true;
                                var.Cleaned = false;
                            }
                        }

                        if (s.IndexOf("return", StringComparison.Ordinal) > -1 && Regex.Match(s, IsReturnRegex).Success)
                        {
                            // if (var.Cleaned || var.Changed)
                            // {
                            //     var.Cleaned = false;
                            // }
                            // else
                            // {
                            if (!var.Cleaned)
                                break;
                            //}
                        }
                    }

                    func.LocalVars[m] = var;
                    Functions[i] = func;
                }

                if (NeedCleanupArgs)
                {
                    for (int m = 0; m < Functions[i].Args.Length; m++)
                    {
                        Function func = Functions[i];
                        Variable var = func.Args[m];
                        string isnullvar = @"^\s*set\s+" + var.Name + @".*?=\s*null";
                        string orisnotnull = @"^\s*set\s+" + var.Name + @"\W";

                        foreach (string s in func.RawDataWithoutString)
                        {
                            if (s.IndexOf(var.Name, StringComparison.Ordinal) > -1)
                            {
                                Match IsVarNotNull = Regex.Match(s, orisnotnull);
                                if (Regex.Match(s, isnullvar).Success && IsVarNotNull.Success)
                                {
                                    //  var.Cleaned = true;
                                    //  var.Changed = false;
                                }
                                else if (IsVarNotNull.Success)
                                {
                                    //  var.Changed = true;
                                    var.Cleaned = false;
                                }
                            }

                            if (s.IndexOf("return", StringComparison.Ordinal) > -1 && Regex.Match(s, IsReturnRegex).Success)
                            {
                                // if (var.Cleaned || var.Changed)
                                // {
                                //     var.Cleaned = false;
                                // }
                                // else
                                // {
                                if (!var.Cleaned)
                                    break;
                                //}
                            }
                        }


                        func.Args[m] = var;
                        Functions[i] = func;
                    }
                }
            });


            List<string> savelistofleaks = new List<string>();


            foreach (Function tmpFunc in Functions)
            {
                List<string> tmpleaks = new List<string>();
                if (!SaveDebugInfo.Checked)
                {
                    if ((tmpFunc.typecast || !tmpFunc.UsedInCode) && WriteUnusedFunctions.Checked)
                        continue;
                }

                foreach (Variable var in tmpFunc.LocalVars)
                {
                    if (!var.Cleaned && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                    {
                        tmpleaks.Add(var.Name + "::" + var.Type + "::" + (var.Line + tmpFunc.Line));
                    }
                }

                foreach (Variable var in tmpFunc.Args)
                {
                    if (!var.Cleaned && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                    {
                        tmpleaks.Add("Used arg leak? ::" + var.Name + "::" + var.Type + "::" + (var.Line + tmpFunc.Line));
                    }
                }

                if (tmpleaks.Count > 0)
                {
                    savelistofleaks.Add("In function:" + tmpFunc.Name + " found " + tmpleaks.Count + " leaks!");
                    savelistofleaks.AddRange(tmpleaks);
                }
                //  countfound++;

            }

            File.WriteAllLines(LogLeakDetected.Text, savelistofleaks.ToArray());

        }

        void ScanUnusedLocals()
        {
            int total_i = 0;

            Parallel.For(0, Functions.Count, i =>
            {
                total_i++;
                scriptprogressvalue = (int)((float)total_i / Functions.Count * 100.0f);

                for (int m = 0; m < Functions[i].LocalVars.Length; m++)
                {
                    Variable var = Functions[i].LocalVars[m];
                    string FuncNameForRegex = @"\W" + var.Name + @"\W|\W" + var.Name + @"\s*$";

                    for (int n = 1; n < Functions[i].RawDataWithoutString.Length; n++)
                    {
                        if (n == var.Line)
                            continue;
                        string tmpStr = Functions[i].RawDataWithoutString[n];
                        if (tmpStr.IndexOf(var.Name, StringComparison.Ordinal) > -1
                        && Regex.Match(tmpStr, FuncNameForRegex).Success)
                        {
                            var.UsedInCode = true;
                            Functions[i].LocalVars[m] = var;
                            break;
                        }
                    }
                }
                for (int m = 0; m < Functions[i].Args.Length; m++)
                {
                    Variable var = Functions[i].Args[m];
                    string FuncNameForRegex = @"\W" + var.Name + @"\W|\W" + var.Name + @"\s*$";

                    for (int n = 1; n < Functions[i].RawDataWithoutString.Length; n++)
                    {
                        string tmpStr = Functions[i].RawDataWithoutString[n];
                        if (tmpStr.IndexOf(var.Name, StringComparison.Ordinal) > -1
                        && Regex.Match(tmpStr, FuncNameForRegex).Success)
                        {
                            var.UsedInCode = true;
                            Functions[i].Args[m] = var;
                            break;
                        }
                    }
                }

            });



            List<string> outunusedlocalsdata = new List<string>();
            foreach (Function tmpFunc in Functions)
            {
                if (!SaveDebugInfo.Checked)
                {
                    if ((tmpFunc.typecast || !tmpFunc.UsedInCode) && WriteUnusedFunctions.Checked)
                        continue;
                }

                List<string> tmpoutlocaldata = new List<string>();
                foreach (Variable var in tmpFunc.Args)
                {
                    if (!var.UsedInCode)
                    {
                        tmpoutlocaldata.Add("Unused arg: " + var.Name + ":" + var.Line + 1);
                    }
                }

                foreach (Variable var in tmpFunc.LocalVars)
                {
                    if (!var.UsedInCode)
                    {
                        tmpoutlocaldata.Add(var.Name + ":" + (var.Line + tmpFunc.Line + 1));
                    }
                }

                if (tmpoutlocaldata.Count > 0)
                {
                    outunusedlocalsdata.Add("In function:" + tmpFunc.Name + " found " + tmpoutlocaldata.Count + " unused vars!");
                    outunusedlocalsdata.AddRange(tmpoutlocaldata.ToArray());
                }

            }

            File.WriteAllLines(LogUnusedLocals.Text, outunusedlocalsdata.ToArray());


        }

        const string regexifdetect = @"^\s*if";
        const string regexloopdetect = @"^\s*loop";

        void SaveOptimizedScript()
        {
            List<string> outdata = new List<string>();

            if (SaveDebugInfo.Checked)
                outdata.Add("//uncommented string dump:");

            outdata.AddRange(DumpOutString.ToArray());

            outdata.Add("globals");

            int unusedglobals = 0;

            foreach (Variable var in GlobalVariables)
            {
                if ((var.UsedInCode || !WriteUnusedGlobals.Checked) || !WriteUnusedGlobals.Checked)
                {
                    if (NeedSaveCommentLines.Checked && var.commentlines.Length > 0)
                        outdata.AddRange(var.commentlines);
                    outdata.Add(var.RawData);
                }
                else
                    unusedglobals++;
            }

            outdata.Add("endglobals");
            if (SaveDebugInfo.Checked)
                outdata.Add("//total globals:" + GlobalVariables.Count + ". Removed unused:" + unusedglobals);

            int unusedfunctions = 0;

            for (int n = 0; n < Functions.Count; n++)
            {
                Function func = Functions[n];

                if ((func.UsedInCode || !WriteUnusedFunctions.Checked) || func.typecast)
                {
                    if (SaveDebugInfo.Checked)
                        outdata.Add("//total lines : " + func.RawData.Length + ". Local vars:" + func.LocalVars.Length + ". RefCount:" + func.RefCount);
                    if (func.typecast)
                        outdata.Add("//Found typecast function. Local variable have same name as global variable!");


                    if (NeedSaveCommentLines.Checked && func.commentlines.Length > 0)
                        outdata.AddRange(func.commentlines);

                    for (int i = 0; i < func.RawData.Length; i++)
                    {
                        string s = func.RawData[i];

                        bool foundunusedlocal = false;
                        bool needskipthisline = false;
                        if (WriteUnusedLocals.Checked && !func.typecast)
                        {
                            foreach (Variable var in func.LocalVars)
                            {
                                if (!var.UsedInCode)
                                {
                                    if (s == var.RawData)
                                    {
                                        foundunusedlocal = true;
                                        break;
                                    }
                                }
                            }
                        }



                        if (WriteLeakList.Checked && !func.typecast)
                        {
                            foreach (Variable var in func.Args)
                            {
                                if (!var.Cleaned && !var.IsArray && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                                {
                                    string isnullvar = @"^\s*set\s+" + var.Name + @".*?=\s*null";

                                    if (Regex.Match(s, isnullvar).Success)
                                    {
                                        needskipthisline = true;
                                    }
                                }
                            }

                            foreach (Variable var in func.LocalVars)
                            {
                                if (!var.Cleaned && !var.IsArray && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                                {
                                    string isnullvar = @"^\s*set\s+" + var.Name + @".*?=\s*null";

                                    if (Regex.Match(s, isnullvar).Success)
                                    {
                                        needskipthisline = true;
                                    }
                                }
                            }


                            if (s.IndexOf("return", StringComparison.Ordinal) > -1 ||
                                s.IndexOf("endfunction", StringComparison.Ordinal) > -1)
                            {
                                if (Regex.Match(s, IsEndFunctionRegex).Success ||
                                    Regex.Match(s, IsReturnRegex).Success)
                                {
                                    if (func.RawData[i - 1].IndexOf("return", StringComparison.Ordinal) > -1
                                        && Regex.Match(func.RawData[i - 1], IsReturnRegex).Success)
                                    {
                                        ;
                                    }
                                    else
                                    {
                                        foreach (Variable var in func.Args)
                                        {
                                            if (!var.Cleaned && var.UsedInCode && !var.IsArray && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                                            {
                                                GlobalLogFile.Add("Fix leak in function:" + func.Name + ", arg as var:" + var.Name);

                                                string filldata = Regex.Match(outdata[outdata.Count - 1], @"^(\s*)").Groups[1].Value;

                                                if (Regex.Match(outdata[outdata.Count - 1], regexifdetect).Success)
                                                    filldata += "\t";
                                                if (Regex.Match(outdata[outdata.Count - 1], regexloopdetect).Success)
                                                    filldata += "\t";



                                                outdata.Add(filldata + "set " + var.Name + " = null");
                                            }
                                        }

                                        foreach (Variable var in func.LocalVars)
                                        {
                                            if (!var.Cleaned && var.UsedInCode && !var.IsArray && NullLocals.Contains(var.Type, StringComparer.Ordinal))
                                            {
                                                GlobalLogFile.Add("Fix leak in function:" + func.Name + ", var:" + var.Name);
                                                string filldata = Regex.Match(outdata[outdata.Count - 1], @"^(\s*)").Groups[1].Value;

                                                if (Regex.Match(outdata[outdata.Count - 1], regexifdetect).Success)
                                                    filldata += "\t";
                                                if (Regex.Match(outdata[outdata.Count - 1], regexloopdetect).Success)
                                                    filldata += "\t";

                                                outdata.Add(filldata + "set " + var.Name + " = null");
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        if (!foundunusedlocal && !needskipthisline)
                            outdata.Add(s);
                    }


                }
                else
                    unusedfunctions++;
            }

            if (SaveDebugInfo.Checked)
                outdata.Add("//total functions:" + Functions.Count + ". Removed unused:" + unusedfunctions);

            File.WriteAllLines(OutOptimizedScript.Text, outdata.ToArray());

        }

        void ScanUnusedFunctions()
        {
            scriptprogressvalue = 0;
            int currenti = 0;

            Parallel.For(0, Functions.Count, i =>
            {
                currenti++;
                scriptprogressvalue = (int)((float)currenti / Functions.Count * 100.0f);
                Function tmpFunc = Functions[i];
                string FuncNameWithQuotes = "\"" + tmpFunc.Name + "\"";
                string FuncNameForRegex = @"\W" + tmpFunc.Name + @"\W|\W" + tmpFunc.Name + @"\s*$";


                for (int n = 0; n < Functions.Count; n++)
                {
                    if (n == i)
                        continue;

                    for (int m = 1; m < Functions[n].RawData.Length; m++)
                    {
                        if (Functions[n].RawData[m].IndexOf(tmpFunc.Name, StringComparison.Ordinal) > -1)
                        {
                            if (Functions[n].RawData[m].IndexOf(FuncNameWithQuotes, StringComparison.Ordinal) > -1
                             || Regex.Match(Functions[n].RawDataWithoutString[m], FuncNameForRegex).Success)
                            {
                                tmpFunc.UsedInCode = true;
                                tmpFunc.RefCount++;
                            }
                        }

                    }
                }
                if (tmpFunc.UsedInCode)
                    Functions[i] = tmpFunc;
            });


            List<string> outunusedfunctionsdata = new List<string>();
            foreach (Function tmpFunc in Functions)
            {
                if (!tmpFunc.UsedInCode && WriteUnusedFunctions.Checked)
                {
                    outunusedfunctionsdata.Add(tmpFunc.Name + ":" + (tmpFunc.Line + 1));
                    //  countfound++;
                }
            }

            File.WriteAllLines(LogUnusedFunctions.Text, outunusedfunctionsdata.ToArray());
        }

        void ScanUnusedGlobals()
        {
            scriptprogressvalue = 0;

            int currentn = 0;

            Parallel.For(0, GlobalVariables.Count, i =>
            {
                currentn++;
                scriptprogressvalue = (int)((float)currentn / GlobalVariables.Count * 10.0f);
                if (GlobalVariables[i].UsedInCode && WriteUnusedGlobals.Checked)
                    return;

                Variable tmpvar = GlobalVariables[i];

                string findglobalinstr = @"\W" + tmpvar.Name + @"\W|\W" + tmpvar.Name + @"\s*$";

                for (int n = i + 1; n < GlobalVariables.Count; n++)
                {
                    if (GlobalVariables[n].RawDataWithoutString.IndexOf(tmpvar.Name, StringComparison.Ordinal) > -1 && Regex.Match(GlobalVariables[n].RawData, findglobalinstr).Success)
                    {
                        n = GlobalVariables.Count;
                        tmpvar.UsedInCode = true;
                        GlobalVariables[i] = tmpvar;
                        break;
                    }
                }
            });

            currentn = 0;

            Parallel.For(0, GlobalVariables.Count, n =>
            {
                currentn++;
                scriptprogressvalue = 10 + (int)((float)currentn / GlobalVariables.Count * 90.0f);

                if (GlobalVariables[n].UsedInCode && WriteUnusedGlobals.Checked)
                    return;
                Variable tmpvar = GlobalVariables[n];

                string findglobalinstr = @"\W" + tmpvar.Name + @"\W|\W" + tmpvar.Name + @"\s*$";


                for (int i = 0; i < Functions.Count; i++)
                {
                    foreach (string str in Functions[i].RawDataWithoutString)
                    {
                        if (str.IndexOf(tmpvar.Name, StringComparison.Ordinal) > -1 && Regex.Match(str, findglobalinstr).Success)
                        {
                            tmpvar.UsedInCode = true;
                            GlobalVariables[n] = tmpvar;
                            i = Functions.Count;
                            break;
                        }
                    }

                }

            });

            // int countfound = 0;
            List<string> outunusedglobalsdata = new List<string>();
            foreach (Variable var in GlobalVariables)
            {
                if (!var.UsedInCode && WriteUnusedGlobals.Checked)
                {
                    outunusedglobalsdata.Add(var.Name + ":" + (var.Line + 1));
                    //  countfound++;
                }
            }

            File.WriteAllLines(LogUnusedGlobals.Text, outunusedglobalsdata.ToArray());
        }

        private void STARTSTARTSTART_Click(object sender, EventArgs e)
        {
            //if (WriteOptimizations.Checked && WriteOptimizedScript.Checked)
            //    MessageBox.Show("Out war3map_opt.j script only for preview changes.\nPlease move changes to your script manually and\n don't try use optimized script in your map :)", "Warning!");
            if (WriteLeakList.Checked)
                MessageBox.Show("Out war3map_opt.j script only for preview changes.\nPlease move changes to your script manually,\nauto fix leak can be uninitialize local vars in your code!", "Warning!");
            if (work)
                return;
            STARTSTARTSTART.Enabled = false;

            var bgw = new BackgroundWorker();
            bgw.DoWork += STARTSTART;
            bgw.RunWorkerAsync();

        }

        private void OpenFileForJassScriptHelper(object sender, EventArgs e)
        {
            Button sender_btn = sender as Button;
            OpenFileDialog select_file = new OpenFileDialog();
            select_file.CheckFileExists = false;
            select_file.Filter = "Any File|*.*|Text File|*.txt|Jass Script|*.j";


            if (sender_btn == SelectInputScript)
            {
                select_file.FileName = inputscript.Text;
            }
            else if (sender_btn == SelectOutputScript)
            {
                select_file.FileName = OutOptimizedScript.Text;
            }
            else if (sender_btn == SelectLeakLogFile)
            {
                select_file.FileName = LogLeakDetected.Text;
            }
            else if (sender_btn == SelectLeakListFile)
            {
                select_file.FileName = LoadLeakForScan.Text;
            }
            else if (sender_btn == SelectUnusedGLobalsLogFile)
            {
                select_file.FileName = LogUnusedGlobals.Text;
            }
            else if (sender_btn == SelectUnusedFunctionsLogFile)
            {
                select_file.FileName = LogUnusedFunctions.Text;
            }
            else if (sender_btn == SelectUnusedLocalsLogFile)
            {
                select_file.FileName = LogUnusedLocals.Text;
            }
            else if (sender_btn == SelectUnusedOptimizationsLogFile)
            {
                select_file.FileName = OutOptimizeHelper.Text;
            }

            if (select_file.ShowDialog() == DialogResult.OK)
            {

                if (sender_btn == SelectInputScript)
                {
                    inputscript.Text = select_file.FileName;
                }
                else if (sender_btn == SelectOutputScript)
                {
                    OutOptimizedScript.Text = select_file.FileName;
                }
                else if (sender_btn == SelectLeakLogFile)
                {
                    LogLeakDetected.Text = select_file.FileName;
                }
                else if (sender_btn == SelectLeakListFile)
                {
                    LoadLeakForScan.Text = select_file.FileName;
                }
                else if (sender_btn == SelectUnusedGLobalsLogFile)
                {
                    LogUnusedGlobals.Text = select_file.FileName;
                }
                else if (sender_btn == SelectUnusedFunctionsLogFile)
                {
                    LogUnusedFunctions.Text = select_file.FileName;
                }
                else if (sender_btn == SelectUnusedLocalsLogFile)
                {
                    LogUnusedLocals.Text = select_file.FileName;
                }
                else if (sender_btn == SelectUnusedOptimizationsLogFile)
                {
                    OutOptimizeHelper.Text = select_file.FileName;
                }
            }
        }

        int scriptprogressvalue = 0;
        int scriptprogresslastvalue = 0;

        string jassscripthelperstatus = "idle";
        string jassscripthelperlaststatus = "idle";

        private void UpdateProgressBar_Tick(object sender, EventArgs e)
        {
            if (scriptprogressvalue != scriptprogresslastvalue)
            {
                ScriptProcessProgress.Value = scriptprogresslastvalue = scriptprogressvalue;
            }

            if (jassscripthelperstatus != jassscripthelperlaststatus)
            {
                StatusLabel.Text = jassscripthelperlaststatus = jassscripthelperstatus;
            }

            ScriptProcessProgress.Invalidate();
            // ScriptProcessProgress.Update();
        }

        private void AboutBtn_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Can be search unused data, leaks, optimizations\nand save result to script.", "Jass Script Helper by Karaulov");
        }

        private void JassScriptHelperMainWindow_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }

        private void JassScriptHelperMainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }
    }
}
