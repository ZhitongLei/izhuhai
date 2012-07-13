using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using iZhuHai.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace iZhuHai.Model
{
    public class User
    {
        private SQLHelper sqlHelper = new SQLHelper();

        public bool IsUserEntryExisted(String email)
        {
            String[] parameters = { "@email" };
            String[] values = { email };

            return IsEntryExisted("SP_QueryUser", parameters, values);
        }

        public bool AddUser(string email,string userName,string personProfile)
        {
            String[] parameters = { "@email","@userName","@personalProfile" };
            String[] values = { email,userName,personProfile };
            sqlHelper.Open();
            int rows=sqlHelper.ExecuteStoredProcedure("SP_InsertUser", parameters, values);
            sqlHelper.Close();
            return rows > 0;
        }

        private bool IsEntryExisted(string storedProduceName, IEnumerable<string> parameters,
                                           IEnumerable<string> values)
        {
            bool isEntryExisted = false;
            sqlHelper.Open();
            SqlDataReader dataReader = sqlHelper.Query(storedProduceName, parameters, values);
            if (dataReader.HasRows)
                isEntryExisted = true;
            sqlHelper.Close();

            return isEntryExisted;
        }

    }
}
