using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using $safeprojectname$.Domain.Contracts;
using $safeprojectname$.Domain.Entity;
using $safeprojectname$.Domain.Infra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using $safeprojectname$.Domain.Infra.OracleObjects;

namespace $safeprojectname$.Domain.Managers
{
    public class PersonManager : BaseOracleDao, IPersonManager
    {
        /// <summary>
        /// Personal Manager - Rules and Access Repository
        /// </summary>
        /// <param name="appSettings"></param>
        public PersonManager(IOptions<OracleSettings> appSettings) : base(appSettings)
        {
            this.DefaultQuery = @"SELECT * FROM TPI_PERSON WHERE 1 = 1";
            this.DefaultKeyQuery = @"SELECT ID, FIRSTNAME, LASTNAME, AGE, TYPE, ACTIVE, USERNAME, PHONE, EMAIL, REGISTER, PHOTO, AREA, DATEREGISTER FROM TPI_PERSON WHERE 1 = 1";
            this.DefaultInsert = @"INSERT INTO TPI_PERSON (Id, FirstName, LastName, Age, Type, Active, Username, Phone, Email, Register, Photo, Area, DateRegister) 
                                     VALUES (null, :FirstName, :LastName, :Age, :Type, :Active, :UserName, :Phone, :Email, :Register, :Photo, :Area, :DateRegister)
                                     returning ID into :Id";
            this.DefaultUpdate = @"UPDATE TPI_PERSON SET FirstName = :FirstName, LastName = :LastName, Age = :Age,
                                            Type = :Type, Active = :Active, Username = :Username, Phone = :Phone, Email = :Email, Register = :Register,
                                            Photo = :Photo, Area = :Area 
                                            WHERE ID = :ID"; ;
            this.DefaultDelete = @"DELETE TPI_PERSON WHERE ID = :ID";
        }

        /// <summary>
        /// Get all Persons
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<Person>>> GetAllAsync()
        {
            var result = new Result<IEnumerable<Person>>();

            try
            {
                using (IDbConnection dbCon = base.DbConnection)
                {
                    if (dbCon.State == ConnectionState.Closed)
                        dbCon.Open();

                    result.Content = await dbCon.QueryAsync<Person>(DefaultQuery);
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to get All Persons: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Get person by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<Person>> GetByIdAsync(object id)
        {
            var result = new Result<Person>();

            try
            {
                using (IDbConnection dbCon = base.DbConnection)
                {
                    if (dbCon.State == ConnectionState.Closed)
                        dbCon.Open();

                    var resultDb = await dbCon.QueryAsync<Person>(DefaultKeyQuery + " AND ID = :ID", new { ID = id });
                    result.Content = resultDb.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to get Person by Id: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Get all persons by procedure.
        /// </summary>
        /// <returns></returns>
        public async Task<Result<IEnumerable<Person>>> GetAllPersonByProcedure()
        {
            var result = new Result<IEnumerable<Person>>();

            try
            {
                using (IDbConnection dbCon = base.DbConnection)
                {
                    if (dbCon.State == ConnectionState.Closed)
                        dbCon.Open();

                    var parameters = new OracleDynamicParameters();
                    parameters.Add("@p_recordset", dbType: OracleDbType.RefCursor, direction: ParameterDirection.Output);
                    result.Content = await SqlMapper.QueryAsync<Person>(base.DbConnection, "PersonProcedure", param: parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to get All Persons By Procedure: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Create new person
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Result<int>> CreateAsync(Person entity)
        {
            var result = new Result<int>();

            try
            {
                //put here any business rules
                //not insert rules about validation fields
                if (await ExistWithThisRegisterAsync(entity.Register))
                    result.BusinessErrorMessages.Add("Person already registered with this register");

                if (!result.HasBusinessErrors)
                {
                    entity.DateRegister = DateTime.Now;

                    using (IDbConnection dbCon = base.DbConnection)
                    {
                        if (dbCon.State == ConnectionState.Closed)
                            dbCon.Open();

                        var param = new DynamicParameters();
                        param.Add(name: "FirstName", value: entity.FirstName, direction: ParameterDirection.Input);
                        param.Add(name: "LastName", value: entity.LastName, direction: ParameterDirection.Input);
                        param.Add(name: "Age", value: entity.Age, direction: ParameterDirection.Input);
                        param.Add(name: "Type", value: entity.Type, direction: ParameterDirection.Input);
                        param.Add(name: "Active", value: entity.Active, direction: ParameterDirection.Input);
                        param.Add(name: "UserName", value: entity.UserName, direction: ParameterDirection.Input);
                        param.Add(name: "Phone", value: entity.Phone, direction: ParameterDirection.Input);
                        param.Add(name: "Email", value: entity.Email, direction: ParameterDirection.Input);
                        param.Add(name: "Register", value: entity.Register, direction: ParameterDirection.Input);
                        param.Add(name: "Photo", value: entity.Photo, direction: ParameterDirection.Input);
                        param.Add(name: "Area", value: entity.Area, direction: ParameterDirection.Input);
                        param.Add(name: "DateRegister", value: entity.DateRegister, direction: ParameterDirection.Input);
                        param.Add(name: "Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await dbCon.QueryFirstOrDefaultAsync<int>(DefaultInsert, param);

                        result.Content = param.Get<int>("Id");
                        result.Success = result.Content > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to insert new Person: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Update a Person
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Result<bool>> UpdateAsync(Person entity)
        {
            var result = new Result<bool>();

            try
            {
                //put here any business rules
                //not insert rules about validation fields

                if (!result.HasBusinessErrors)
                {
                    using (IDbConnection dbCon = base.DbConnection)
                    {
                        if (dbCon.State == ConnectionState.Closed)
                            dbCon.Open();

                        var parans = new DynamicParameters();
                        parans.Add("ID", entity.ID);
                        parans.Add("FirstName", entity.FirstName);
                        parans.Add("LastName", entity.LastName);
                        parans.Add("Age", entity.Age);
                        parans.Add("Type", entity.Type);
                        parans.Add("Active", entity.Active);
                        parans.Add("UserName", entity.UserName);
                        parans.Add("Phone", entity.Phone);
                        parans.Add("Email", entity.Email);
                        parans.Add("Register", entity.Register);
                        parans.Add("Photo", entity.Photo);
                        parans.Add("Area", entity.Area);

                        result.Content = await dbCon.ExecuteAsync(DefaultUpdate, parans) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to insert new Person: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Delete a Person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<bool>> DeleteAsync(object id)
        {
            var result = new Result<bool>();

            try
            {
                using (IDbConnection dbCon = base.DbConnection)
                {
                    if (dbCon.State == ConnectionState.Closed)
                        dbCon.Open();

                    var parameters = new DynamicParameters();
                    parameters.Add("ID", id);

                    result.Content = await dbCon.ExecuteAsync(DefaultDelete, parameters) > 0;
                }
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = (string.Format("Exception to delete new Person: {0}", ex.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Find Person by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(object id)
        {
            using (IDbConnection dbCon = base.DbConnection)
            {
                if (dbCon.State == ConnectionState.Closed)
                    dbCon.Open();

                return await dbCon.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM TPI_PERSON WHERE ID = :ID", new { id });
            }
        }

        /// <summary>
        /// Find Person by Register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public async Task<bool> ExistWithThisRegisterAsync(object register)
        {
            using (IDbConnection dbCon = base.DbConnection)
            {
                if (dbCon.State == ConnectionState.Closed)
                    dbCon.Open();

                return await dbCon.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM TPI_PERSON WHERE REGISTER = :Register", new { register });
            }
        }
    }
}
