using Application.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Application.Helper
{
    public class DataBingding
    {
        private static IDbConnection connection;
        public DataBingding()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        public static URL GetFacebook()
        {
            using (connection)
            {
                var module = connection.QueryFirstOrDefault<URL>(string.Format(@"
                     SELECT [ID]
                           ,[NAME]
                           ,[DESCRIP]
                     FROM [dbo].[URL]
                     WHERE [ID] = 2
                "));
                return module;
            }
        }

        public static URL GetYotube()
        {
            using (connection)
            {
                var module = connection.QueryFirstOrDefault<URL>(string.Format(@"
                     SELECT [ID]
                           ,[NAME]
                           ,[DESCRIP]
                     FROM [dbo].[URL]
                     WHERE [ID] = 1
                "));
                return module;
            }
        }

        public static URL GetInstagram()
        {
            using (connection)
            {
                var module = connection.QueryFirstOrDefault<URL>(string.Format(@"
                     SELECT [ID]
                           ,[NAME]
                           ,[DESCRIP]
                     FROM [dbo].[URL]
                     WHERE [ID] = 3
                "));
                return module;
            }
        }

        public static OPTIONAL GetIntro()
        {
            using (connection)
            {
                var module = connection.QueryFirstOrDefault<OPTIONAL>(string.Format(@"
                    SELECT [ID]
                            ,[NAME]
                            ,[CONTENTS]
                    FROM [dbo].[Optional]
                    WHERE [ID] = 1
                "));
                return module;
            }
        }
        
        public static IEnumerable<SLIDER> GetSlider()
        {
            using (connection)
            {
                var modules = connection.Query<SLIDER>(string.Format(@"
                    SELECT [ID]
                          ,[NAME]
                          ,[URI]
                    FROM [dbo].[SLIDER]
                "));
                return modules;
            }
        }
    }
}