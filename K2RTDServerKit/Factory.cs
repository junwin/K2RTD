//-----------------------------------------------------------------------
// <copyright file="Factory.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace K2RTDServerKit
{
    /// <summary>
    /// Provides a way to create the RTDServer and dependancies - we use this beacuse
    /// for packages like MATLAB we need load a single private assembly using the path
    /// and we then will expect the factory to load any other dependancies as needed.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile Factory s_instance;

        string m_BinPath = "";

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        private IRTDServer m_Server = null;

        /// <summary>
        /// Get a instance of the factory
        /// </summary>
        /// <returns></returns>
        public static Factory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new Factory();
                    }
                }
            }
            return s_instance;
        }

        protected Factory()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
        }

        /// <summary>
        /// Get the RTD Server component - we need this to use the RTD kit
        /// </summary>
        /// <param name="binPath"></param>
        /// <returns></returns>
        public IRTDServer GetRTDServer(string binPath)
        {
            if (m_Server == null)
            {
                loadRTDAssemblies(binPath);
            }
            return m_Server;
        }

        /// <summary>
        /// Load assemblies
        /// </summary>
        /// <param name="binPath">path to the kit binaries, empty use location of this assembly</param>
        private void loadRTDAssemblies(string binPath)
        {
            try
            {
                if(binPath.Length ==0)
                {
                    // get the location of the current assembly i.e. K2RTDServerKit, assume dependancies are there
                    string[] reqAssyDef = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',');
                    int reqDllNamePos = System.Reflection.Assembly.GetExecutingAssembly().Location.IndexOf(reqAssyDef[0]);
                    binPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, reqDllNamePos - 1);
                }


                //m_PlugInLog.Info("DynamicLoad:" + myPath);
                string mainDllPath = binPath + @"\K2RTDRequestHandler.dll";
                
                //string mainDllPath = binPath + @"\K2RTDRequestHandler.dll";
                m_Server = DynamicLoad(mainDllPath);


            }
            catch
            {
            }
        }

        /// <summary>
        /// Called if we need to resolve some dependancy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Reflection.Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            System.Reflection.Assembly assy4 = null;
            try
            {
                // attempt to load from the binpath or the requesting assmebly path
                string[] assyDef = args.Name.Split(',');
                string[] reqAssyDef = args.RequestingAssembly.FullName.Split(',');
                int reqDllNamePos = args.RequestingAssembly.Location.IndexOf(reqAssyDef[0]);
                string reqDllPath = args.RequestingAssembly.Location.Substring(0, reqDllNamePos - 1);
                string DllPath = "";
                if (m_BinPath.Length > 0)
                {
                    DllPath = m_BinPath + @"\" + assyDef[0] + ".dll";
                }
                else
                {
                    DllPath = reqDllPath + @"\" + assyDef[0] + ".dll";
                }

                assy4 = System.Reflection.Assembly.LoadFrom(DllPath);
            }
            catch
            {
            }
            return assy4;
        }


        /// <summary>
        /// Dynamically load some plugin (visible or non visible)
        /// </summary>
        /// <param name="uid">user id </param>
        /// <param name="path">path to the plugin</param>
        /// <returns></returns>
        public IRTDServer  DynamicLoad(string myPath)
        {

            IRTDServer myPlugIn = null;
            try
            {
                //m_PlugInLog.Info("DynamicLoad:" + myPath);
                System.Reflection.Assembly myAssy = System.Reflection.Assembly.LoadFile(myPath);

                System.Type myIF;
                Object oTemp;
                foreach (System.Type myType in myAssy.GetTypes())
                {
                    myIF = myType.GetInterface("K2RTDServerKit.IRTDServer");
                    if (myIF != null)
                    {
                        // Will call the constructor - guess you need to invoke the instance
                        // method
                        oTemp = myAssy.CreateInstance(myType.ToString());
                        myPlugIn = oTemp as K2RTDServerKit.IRTDServer;
                        if (myPlugIn != null)
                        {
                           
                        }
                    }
                }
                return myPlugIn;
            }
            catch (Exception myE)
            {
               // m//_PlugInLog.Error("DynamicLoad", myE);
               // m_PlugInLog.Error("DynamicLoad:" + myPath);
                return null;
            }
        }
    }
}
