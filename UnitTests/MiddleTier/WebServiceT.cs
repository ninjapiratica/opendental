﻿using OpenDentBusiness;
using System;

namespace UnitTests
{
    public class WebServiceT
    {

        public static bool ConnectToMiddleTier(string serverURI, string userName, string password, string productVersion, bool isOracle)
        {
            if (isOracle)
            {
                return false;
            }
            RemotingClient.ServerURI = serverURI;
            try
            {
                Userod user = Security.LogInWeb(userName, password, "", productVersion, false);
                Security.CurUser = user;
                Security.PasswordTyped = password;
                RemotingClient.MiddleTierRole = MiddleTierRole.ClientMT;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

    }
}
