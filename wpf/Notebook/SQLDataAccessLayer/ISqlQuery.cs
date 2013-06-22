using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLDataAccessLayer
{
    interface ISqlQuery
    {
        int InsertInto(string tableName, string[] columnsName, object[] values, string condition);
        int Update(string tableName, string[] columnsName, object[] values, string condition);        
        object[] SelectFrom(string tableName, string[] columnsName, string condition);
        int DeleteFrom(string tableName, string condition);
    }
}
