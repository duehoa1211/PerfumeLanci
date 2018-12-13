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
        private IDbConnection connection;
        public DataBingding()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
        public URL GetFacebook()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
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

        public URL GetYotube()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
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

        public URL GetInstagram()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
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

        public OPTIONAL GetIntro()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
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
        
        public IEnumerable<SLIDER> GetSlider()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
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

        public OPTIONAL Logo()
        {
            using (connection = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                var modules = connection.QueryFirstOrDefault<OPTIONAL>(string.Format(@"
                    SELECT [ID]
                        ,[NAME]
                        ,[CONTENTS]
                    FROM [dbo].[Optional]
                    WHERE [ID] = 2
                "));
                return modules;
            }
        }
    }
}