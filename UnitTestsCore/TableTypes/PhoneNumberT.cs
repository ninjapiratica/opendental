﻿using CodeBase;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTestsCore
{
    public class PhoneNumberT
    {

        public static List<PhoneNumber> GetAll()
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<PhoneNumber>>(MethodBase.GetCurrentMethod());
            }
            return OpenDentBusiness.Crud.PhoneNumberCrud.SelectMany("SELECT * FROM phonenumber");
        }

        public static string GetRandomPhoneNumber()
        {
            return (ODRandom.Next(0, 2) > 0 ? "1" : "") + $"({ODRandom.Next(100, 999)}) {ODRandom.Next(100, 999)}-{ODRandom.Next(1000, 9999)}";
        }

        public static void ClearPhoneNumberTable()
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod());
                return;
            }
            DataCore.NonQ("DELETE FROM phonenumber");
        }

    }
}
