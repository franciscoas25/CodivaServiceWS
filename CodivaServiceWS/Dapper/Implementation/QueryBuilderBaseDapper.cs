using CodivaServiceWS.Connection;
using CodivaServiceWS.Dapper.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodivaServiceWS.Dapper.Implementation
{
    public class QueryBuilderBaseDapper<T> : IQueryBuilderBaseDapper<T> where T : class
    {
        //public IEnumerable<T> GetList(T entity)
        //{
        //    IDbConnection connection = CodivaServiceConnection.GetConnection();

        //    var result = connection.Query<T>($"SELECT * FROM {entity.GetType().Name}");

        //    CodivaServiceConnection.CloseConnection(connection);

        //    return result;
        //}

        //public T SingleOrDefault(T entity)
        //{
        //    IDbConnection connection = CodivaServiceConnection.GetConnection();

        //    var result = connection.QueryFirstOrDefault<T>($"SELECT * FROM {entity.GetType().Name}");

        //    CodivaServiceConnection.CloseConnection(connection);

        //    return result;
        //}

        //public int? ExecuteAction(string query)
        //{
        //    IDbConnection connection = CodivaServiceConnection.GetConnection();

        //    var result = connection.Execute(query);

        //    CodivaServiceConnection.CloseConnection(connection);

        //    return result;
        //}
    }
}
