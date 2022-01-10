namespace FordAPI.Helper
{
    #region Using Directives
    
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    #endregion

    #region SqlHelper

    /// <summary>
    /// The sql helper.
    /// </summary>
    public sealed class SqlHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlHelper"/> class.
        /// </summary>
        public SqlHelper()
        {
        }

        /// <summary>
        /// Gets connection string.
        /// </summary>
        public static string GetConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            }
        }

        /// <summary>
        /// Gets connection string.
        /// </summary>
        public static string GetDMSConnectionString
        {
            get
            {
                
                //return ConfigurationManager.ConnectionStrings["DmsContentEsCountries"].ToString();
                //return ConfigurationManager.ConnectionStrings["PackTrackConnection"].ToString();
                return ConfigurationManager.ConnectionStrings["DMSContentConnection"].ToString();
            }
        }

        /// <summary>
        /// Gets connection string.
        /// </summary>
        public static string GetDMSConnectionStringForEsCountry
        {
            get
            {
                return ConfigurationSettings.AppSettings["DmsContentEsCountries"].ToString();
            }
        }

        #region ExecuteNonQuery

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <returns>
        /// Integer value.
        /// </returns>
        public static int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(GetConnectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="commandParameters">
        /// The command parameters.
        /// </param>
        /// <returns>
        /// Integer value.
        /// </returns>
        public static int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            string connectionString = GetConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored prcedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(string connectionString, string storedProcedureName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// The execute non query.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// Integer value.
        /// </returns>
        public static int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            string connectionString = GetConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            if (mustCloseConnection)
            {
                connection.Close();
            }

            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string storedProcedureName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Finally, execute the command
            int retval = cmd.ExecuteNonQuery();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Replace singe quotes.
        /// </summary>
        /// <param name="sourceString">
        /// The source string.
        /// </param>
        /// <returns>
        /// Returns a string without quotes string.
        /// </returns>
        public static string ReplaceSingleQuotes(string sourceString)
        {
            try
            {
                if (sourceString != null)
                {
                    return "'" + sourceString.Replace("'", "''") + "'";
                }
                else
                {
                    return "''";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The replace quotes.
        /// </summary>
        /// <param name="sourceString">
        /// The string.
        /// </param>
        /// <returns>
        /// Returns a string without quotes.
        /// </returns>
        public static string ReplaceQuotes(string sourceString)
        {
            try
            {
                if (sourceString != null)
                {
                    return "'" + sourceString.Replace("'", "''''") + "'";
                }
                else
                {
                    return "''";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>An int representing the number of rows affected by the command.</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataset

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// The execute data set.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <returns>
        /// Returns dataset.
        /// </returns>
        public static DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(GetConnectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// The execute data set.
        /// </summary>
        /// <param name="conStr">
        /// The connection string.
        /// </param>
        /// <param name="sourceSql">
        /// The source sql.
        /// </param>
        /// <returns>
        /// Returns dataset.
        /// </returns>
        public static DataSet ExecuteDataset(string conStr, string sourceSql)
        {
            try
            {
                if (conStr == null || conStr.Length <= 0)
                {
                    throw new ApplicationException("Unable to connect to the database. Missing Connection string in Web.Config");
                }
                else
                {
                    SqlDataAdapter sqlDa;
                    DataSet ds;

                    using (SqlConnection sqlConn = new SqlConnection(conStr))
                    {
                        sqlDa = new SqlDataAdapter(sourceSql, sqlConn);
                        ds = new DataSet();
                        if (sqlDa.SelectCommand != null)
                        {
                            sqlDa.SelectCommand.CommandTimeout = 0;
                        }

                        sqlDa.Fill(ds);
                        if (sqlConn.State == ConnectionState.Open)
                        {
                            sqlConn.Close();
                        }

                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Teh execute dataset.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <param name="commandParameters">
        /// The command parameters.
        /// </param>
        /// <returns>
        /// Returns dataset.
        /// </returns>
        public static DataSet ExecuteDataset(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            string connectionString = GetConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(string connectionString, string storedProcedureName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// The execute dataset.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// Returns dataset.
        /// </returns>
        public static DataSet ExecuteDataset(string storedProcedureName, params object[] parameterValues)
        {
            string connectionString = GetConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// The execute dataset.
        /// </summary>
        /// <param name="storedProcedureName">
        /// The stored procedure name.
        /// </param>
        /// <param name="parameterValues">
        /// The parameter values.
        /// </param>
        /// <returns>
        /// Returns dataset.
        /// </returns>
        public static DataSet ExecuteDMSDataset(string connectionString, string storedProcedureName, params object[] parameterValues)
        {
            //string connectionString = "Data Source=10.34.9.82;Initial Catalog=DMSContent;User ID=svcEndeca;Pwd=xhUUhrtAgbAFGAvN;";
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (var da = new SqlDataAdapter(cmd))
            {
                var ds = new DataSet();

                //// Fill the DataSet using default values for DataTable names, etc
                try
                {
                    da.Fill(ds);
                }
                catch (Exception)
                {
                    throw;
                }
                

                //// Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                {
                    connection.Close();
                }

                //// Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, string storedProcedureName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            using (var da = new SqlDataAdapter(cmd))
            {
                var ds = new DataSet();

                // Fill the DataSet using default values for DataTable names, etc
                da.Fill(ds);

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                // Return the dataset
                return ds;
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>A dataset containing the resultset generated by the command.</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteDataset(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteDataset(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteDataset

        #region ExecuteScalar

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">
        /// The command type.
        /// </param>
        /// <param name="commandText">
        /// The command text.
        /// </param>
        /// <returns>
        /// Returns scalar value.
        /// </returns>
        public static object ExecuteScalar(CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(GetConnectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="commandType">The command type.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns>Returns scalar value.</returns>
        public static object ExecuteScalar(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            string connectionString = GetConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // Create & open a SqlConnection, and dispose of it after we are done
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure name.</param>
        /// <param name="parameterValues">The prameter values.</param>
        /// <returns>Returns scalar value.</returns>
        public static object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            string connectionString = GetConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();

            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();
            //////object retval = commandParameters[1].Value;

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if (mustCloseConnection)
            {
                connection.Close();
            }

            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="connection">A valid SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlConnection connection, string storedProcedureName, params object[] parameterValues)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            // Create a command and prepare it for execution
            var cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Execute the command & return the results
            object retval = cmd.ExecuteScalar();

            // Detach the SqlParameters from the command object, so they can be used again
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <param name="transaction">A valid SqlTransaction.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure.</param>
        /// <returns>An object containing the value in the 1x1 resultset generated by the command.</returns>
        public static object ExecuteScalar(SqlTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction != null && transaction.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            // If we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                // PPull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, storedProcedureName);

                // Assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                // Call the overload that takes an array of SqlParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                // Otherwise we can just call the SP without params
                return ExecuteScalar(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteScalar

        #region private utility methods & constructors

        /// <summary>
        /// The create parameter.
        /// </summary>
        /// <param name="paramname">
        /// The parameter name.
        /// </param>
        /// <param name="paramtype">
        /// The parameter type.
        /// </param>
        /// <param name="paramsize">
        /// The parameter size.
        /// </param>
        /// <param name="paramdirection">
        /// The parameter direction.
        /// </param>
        /// <param name="paramvalue">
        /// The parameter value.
        /// </param>
        /// <returns>
        /// Return sql parameter.
        /// </returns>
        public static SqlParameter CreateParameter(string paramname, SqlDbType paramtype, int paramsize, ParameterDirection paramdirection, string paramvalue)
        {
            SqlParameter myParameter = new SqlParameter();

            myParameter.ParameterName = paramname;
            if (paramvalue == string.Empty)
            {
                myParameter.Value = DBNull.Value;
            }
            else
            {
                myParameter.Value = paramvalue;
            }

            myParameter.SqlDbType = paramtype;
            myParameter.Size = paramsize;
            myParameter.Direction = paramdirection;
            return myParameter;
        }
        
        /// <summary>
        /// The attach parameters.
        /// </summary>
        /// <param name="command">
        /// The command argument.
        /// </param>
        /// <param name="commandParameters">
        /// The command parameters.
        /// </param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }

                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// This method assigns dataRow column values to an array of SqlParameters.
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values.</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values.</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data
                return;
            }

            int i = 0;

            // Set the parameters values
            foreach (SqlParameter commandParameter in commandParameters)
            {
                // Check the parameter name
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                {
                    throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", i, commandParameter.ParameterName));
                }

                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                {
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                }

                i++;
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters.
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values.</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned.</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data
                return;
            }

            // We must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Length)
            {
               //// throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // If the current array value derives from IDbDataParameter, then assign its Value property
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared.</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command.</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'.</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.).</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required.</param>
        /// <param name="mustCloseConnection"><c>True</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;
            command.CommandTimeout = 100000;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                }

                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }
        #endregion private utility methods & constructors
    }
    #endregion

    #region SqlHelperParameterCache

    /// <summary>
    /// The sql helper paramter cache.
    /// </summary>
    public sealed class SqlHelperParameterCache
    {
        /// <summary>
        /// The paramCache.
        /// </summary>
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Prevents a default instance of the <see cref="SqlHelperParameterCache"/> class from being created.
        /// </summary>
        private SqlHelperParameterCache()
        {
        }

        #region caching functions

        /// <summary>
        /// Add parameter array to the cache.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached.</param>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// Retrieve a parameter array from the cache.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="commandText">The stored procedure name or T-SQL command.</param>
        /// <returns>An array of SqlParamters.</returns>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }
        #endregion caching functions

        #region Parameter Discovery Functions

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <returns>An array of SqlParameters.</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string storedProcedureName)
        {
            return GetSpParameterSet(connectionString, storedProcedureName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure.
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results.</param>
        /// <returns>An array of SqlParameters.</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string storedProcedureName, bool includeReturnValueParameter)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            using (var connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, storedProcedureName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure.
        /// </summary>
        /// <param name="connection">A valid SqlConnection object.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <returns>An array of SqlParameters.</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string storedProcedureName)
        {
            return GetSpParameterSet(connection, storedProcedureName, false);
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure.
        /// </summary>
        /// <remarks>
        /// This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results.</param>
        /// <returns>An array of SqlParameters.</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string storedProcedureName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, storedProcedureName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// Retrieves the set of SqlParameters appropriate for the stored procedure.
        /// </summary>
        /// <param name="connection">A valid SqlConnection object.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results.</param>
        /// <returns>An array of SqlParameters.</returns>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string storedProcedureName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            string hashKey = connection.ConnectionString + ":" + storedProcedureName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : string.Empty);

            var cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] sqlParameters = DiscoverSpParameterSet(connection, storedProcedureName, includeReturnValueParameter);
                paramCache[hashKey] = sqlParameters;
                cachedParameters = sqlParameters;
            }

            return CloneParameters(cachedParameters);
        }
        #endregion Parameter Discovery Functions

        #region private methods, variables, and constructors
        /// <summary>
        /// Resolve at run time the appropriate set of SqlParameters for a stored procedure.
        /// </summary>
        /// <param name="connection">A valid SqlConnection object.</param>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter.</param>
        /// <returns>The parameter array discovered.</returns>
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string storedProcedureName, bool includeReturnValueParameter)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (string.IsNullOrEmpty(storedProcedureName))
            {
                throw new ArgumentNullException("storedProcedureName");
            }

            var cmd = new SqlCommand(storedProcedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            // Init the parameters with a DBNull value
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }

            return discoveredParameters;
        }

        /// <summary>
        /// Deep copy of cached SqlParameter array.
        /// </summary>
        /// <param name="originalParameters">The original parameters.</param>
        /// <returns>Returns sql paratemeter array.</returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion private methods, variables, and constructors
    }
    #endregion

    #region ModHelper
    /// <summary>
    /// The ModHelper.
    /// </summary>
    public sealed class ModHelper
    {
        /// <summary>
        /// The sql parameter.
        /// </summary>
        /// <param name="paramname">The parameter name.</param>
        /// <param name="paramtype">The parameter type.</param>
        /// <param name="paramsize">The parameter size.</param>
        /// <param name="paramdirection">The parameter direction.</param>
        /// <param name="paramvalue">The parameter value.</param>
        /// <returns>Returns sql parameter.</returns>
        public static SqlParameter CreateParameter(string paramname, SqlDbType paramtype, int paramsize, ParameterDirection paramdirection, object paramvalue)
        {
            var myParameter = new SqlParameter();
            myParameter.ParameterName = paramname;
            myParameter.Value = paramvalue ?? DBNull.Value;

            myParameter.SqlDbType = paramtype;
            myParameter.Size = paramsize;
            myParameter.Direction = paramdirection;
            return myParameter;
        }

        /// <summary>
        /// The create parameter.
        /// </summary>
        /// <param name="paramname">The parameter name.</param>
        /// <param name="paramtype">The parameter type.</param>
        /// <param name="paramsize">The parameter size.</param>
        /// <param name="paramdirection">The parameter direction.</param>
        /// <returns>Returns sql parameter.</returns>
        public static SqlParameter CreateParameter(string paramname, SqlDbType paramtype, int paramsize, ParameterDirection paramdirection)
        {
            var myParameter = new SqlParameter();
            myParameter.ParameterName = paramname;

            myParameter.SqlDbType = paramtype;
            myParameter.Size = paramsize;
            myParameter.Direction = paramdirection;
            return myParameter;
        }
    }
    #endregion
}
