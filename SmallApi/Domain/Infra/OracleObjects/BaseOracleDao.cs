using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace SmallApi.Domain.Infra.OracleObjects
{
    /// <summary>
    /// Base Access Repository
    /// </summary>
    public abstract class BaseOracleDao
    {
        private readonly OracleSettings _oracleSettings;

        private static Dictionary<string, Tuple<DateTime, string>> cache = new Dictionary<string, Tuple<DateTime, string>>();
        private static int cacheExpiration = 120;

        public string DefaultQuery { get; protected set; }
        public string DefaultKeyQuery { get; protected set; }
        public string DefaultInsert { get; protected set; }
        public string DefaultUpdate { get; protected set; }
        public string DefaultDelete { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oracleSettings"></param>
        public BaseOracleDao(IOptions<OracleSettings> oracleSettings)
        {
            _oracleSettings = oracleSettings.Value;
        }

        internal IDbConnection DbConnection
        {
            get
            {
                string conx = GetConnection(_oracleSettings.DirectoryServer,
                   Convert.ToInt32(_oracleSettings.DirectoryServerPort),
                   _oracleSettings.DefaultAdminContext,
                   _oracleSettings.ServiceName,
                   _oracleSettings.UserId,
                   _oracleSettings.Password);

                return new OracleConnection(conx);
            }
        }

        private static string GetConnection(string directoryServer, int directoryServerPort, string defaultAdminContext, string serviceName, string userId, string password)
        {
            string descriptor = ConnectionDescriptor(directoryServer, directoryServerPort, defaultAdminContext, serviceName);
            string connectionString = $"user id={userId};password={password};data source={descriptor}";

            return connectionString;
        }

        private static string ConnectionDescriptor(string directoryServer, int directoryServerPort, string defaultAdminContext, string serviceName)
        {
            var key = serviceName + defaultAdminContext;

            if (cache.ContainsKey(key))
            {
                TimeSpan difference = DateTime.Now - cache[key].Item1;
                if (difference.TotalSeconds < cacheExpiration)
                {
                    return cache[key].Item2;
                }
                else
                {
                    cache.Remove(key);
                }
            }

            try
            {
                LdapConnection conn = new LdapConnection();
                conn.Connect(directoryServer, directoryServerPort);
                conn.Bind(defaultAdminContext, "");
                ILdapSearchResults searchResults = conn.Search(defaultAdminContext,
                    LdapConnection.ScopeSub, string.Format("(&(objectclass=orclNetService)(cn={0}))", serviceName), null, false);

                while (searchResults.HasMore())
                {
                    var nextEntry = searchResults.Next();
                    nextEntry.GetAttributeSet();

                    var cnx = nextEntry.GetAttribute("orclNetDescString").StringValue;

                    cache.Add(key, Tuple.Create<DateTime, string>(DateTime.Now, cnx));
                    return cnx;
                }
            }
            catch (LdapException ldapEx)
            {
                ldapEx.ToString();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return string.Empty;
        }
    }
}
