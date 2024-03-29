﻿using CommonTypes;
using DSTMServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTMServices
{
    public class DebugService
    {
        private MasterService masterService;
        public DebugService(MasterService masterS)
        {

            masterService = masterS;
        }

        public bool Status()
        {
            IMaster master = masterService.Master;
            try
            {
                bool result = master.Status();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool Fail(String serverUrl)
        {
            try
            {
                getServerProxyFromUrl(serverUrl).Fail(); //ancar excepcao se ja fez fail
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool Freeze(String serverUrl)
        {
            try
            {
                getServerProxyFromUrl(serverUrl).Freeze(); //ancar excepcao se ja fez fail
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool Recover(String serverUrl)
        {
            try
            {
                getServerProxyFromUrl(serverUrl).Recover(); //ancar excepcao se ja fez fail
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        private IServer getServerProxyFromUrl(String urlString)
        {

            return (IServer)Activator.GetObject(typeof(IServer), urlString); //recentemente comentado MasterService

        }

    }
}
