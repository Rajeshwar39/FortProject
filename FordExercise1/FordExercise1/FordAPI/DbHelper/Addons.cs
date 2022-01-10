using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FordAPI.Models;
using System.Data;
//using System.Web.Mvc;

using Newtonsoft.Json;
using System.Globalization;

using AutoMapper;
using System.Reflection;
using FordAPI.Helper;
using System.Configuration;


using System.Web.Http.Cors;

using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Web.Http.Description;

using System.Dynamic;
using System.Text;
using System.Web;
using Newtonsoft;

namespace FordAPI.DbHelper
{
    public class AddonsClass
    {
        public static string UserConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["UserConnectionString"].ToString();
            }
        }
        public DataSet GetUsers()
        {
            List<UsersList> uList = new List<UsersList>();

            return SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_GetUsers");

        }
        public DataSet DeleteCitiesInfo(string username, string city, string country)
        {
            List<UsersList> uList = new List<UsersList>();
            SqlParameter[] sqlParams = {
                   new SqlParameter("@username", username.Trim()),
                     new SqlParameter("@cityname", city.Trim()),
                       new SqlParameter("@country", country.Trim())
            };
            return SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_DeleteCities");

        }
        public DataSet ShowCitiesInfo()
        {
            List<UsersList> uList = new List<UsersList>();

            return SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_ShowCities");

        }

        public DataSet addUserinfo(string username, string password, string email)
        {
            SqlParameter[] sqlParams = {
                   new SqlParameter("@username", username.Trim()),
                     new SqlParameter("@password", password.Trim()),
                       new SqlParameter("@email", email.Trim())
            };
            return SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_AddUsers", sqlParams);

        }
        public DataSet addcityinfo(string username, string city, string country)
        {
            SqlParameter[] sqlParams = {
                   new SqlParameter("@username", username.Trim()),
                     new SqlParameter("@cityname", city.Trim()),
                       new SqlParameter("@country", country.Trim())
            };
            return SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_AddCities", sqlParams);

        }
    }
}