using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PasswordSafe.Data.Biz;
using System.Data.SqlServerCe;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;

namespace PasswordSafe.Data.DAL
{
    /// <summary>
    /// Data Access Layer to exchange data from business objects with the Database.
    /// </summary>
    internal class Context : IDisposable
    {
        internal Context(string connectionString)
            : base()
        {
            if (connectionString != null) Open(connectionString);
        }

        private void Open(string connectionString)
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
            if (!string.IsNullOrEmpty(connectionString))
            {
                connection = new SqlCeConnection(connectionString);
            }
            if (connection != null && connection.State == ConnectionState.Closed) connection.Open();
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        #endregion

        public bool IsConnected
        {
            get
            {
                return Connection != null && Connection.State == ConnectionState.Open;
            }
        }

        private SqlCeConnection connection;

        /// <summary>
        /// Gets the Connection String for the database.
        /// </summary>
        public SqlCeConnection Connection
        {
            get
            {
                return connection;
            }
        }

        /// <summary>
        /// Executes a T-SQL with parameters.
        /// </summary>
        /// <param name="command">The T-SQL command to execute.</param>
        /// <param name="param">The parameters.</param>
        /// <returns>The number data affected.</returns>
        protected int Execute(string command, params object[] param)
        {
            if (Connection.State == System.Data.ConnectionState.Closed) Connection.Open();
            using (SqlCeCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = command;
                int id = 1;
                foreach (object value in param)
                {
                    cmd.Parameters.AddWithValue("@v" + id.ToString(), value != null ? value : DBNull.Value);
                    id++;
                }
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Reads data from database and converts it into business objects.
        /// </summary>
        /// <typeparam name="T">The type of business object.</typeparam>
        /// <param name="command">The T-SQL command to read from database.</param>
        /// <param name="convert">A <see cref="T:Func"/> that converts a <see cref="T:DataRow"/> to <typeparamref name="T"/>.</param>
        /// <param name="param">The parameters for the T-SQL.</param>
        /// <returns>An enumeration of <typeparamref name="T"/></returns>
        protected IEnumerable<T> ReadData<T>(string command, Func<DataRow, T> convert, params object[] param)
        {
            if (Connection.State == System.Data.ConnectionState.Closed) Connection.Open();
            lock (this)
            {
                using (DataTable table = new DataTable())
                {
                    using (SqlCeCommand cmd = Connection.CreateCommand())
                    {
                        cmd.CommandText = command;
                        int id = 1;
                        foreach (object value in param)
                        {
                            cmd.Parameters.AddWithValue("@v" + id.ToString(), value);
                            id++;
                        }
                        using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(cmd))
                        {
                            adapter.Fill(table);
                        }
                    }
                    return table.AsEnumerable().Select(f => convert(f));
                }
            }
        }

        private int GetIdentity()
        {
            if (Connection.State == System.Data.ConnectionState.Closed) Connection.Open();
            using (SqlCeCommand cmd = Connection.CreateCommand())
            {
                cmd.CommandText = "select @@IDENTITY;";
                return (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void CreateFavorite(int passwordId, int folderId)
        {
            int n = Execute("insert into Favorite (PasswordId,folderId) VALUES (@v1,@v2)", passwordId, folderId);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        public void DeleteFavorite(int passwordId, int folderId)
        {
            int n = Execute("delete from Favorite where passwordId=@v1 and folderId=@v2", passwordId, folderId);
            if (n != 1) throw new DBEntityNotUpdatedException();

        }

        public IEnumerable<int> GetExistingFaves(int passwordId)
        {

            return ReadData<int>("select FolderId from Favorite where PasswordId=@v1",
                r => (int)r[0],
                passwordId);
        }

        private string TableNameFromFieldType(FieldType type)
        {
            switch (type)
            {
                case FieldType.Text:
                case FieldType.Password:
                    return "StringField";

                case FieldType.Int:
                case FieldType.Bool:
                case FieldType.Separator:
                    return "IntField";

                case FieldType.Date:
                case FieldType.Time:
                    return "DateField";

                case FieldType.Memo:
                    return "MemoField";

                default:
                    throw new NotImplementedException();
            }
        }

        public void UpdateField(Field field)
        {
            string sql = string.Format("update {0} set Name=@v1, [Value]=@v2,[Order]=@v3", TableNameFromFieldType(field.Type));

            object p1 = null;
            object p2 = null;
            if (field.HasRange)
            {
                sql += ",[MinRange]=@v5, [MaxRange]=@v6";
                p1 = CheckOptValue(field.MinRange, field);
                p2 = CheckOptValue(field.MaxRange, field);
            }
            if (field.HasLines)
            {
                sql += ",[MinLines]=@v5,[MaxLines]=@v6";
                p1 = field.MinLines;
                p2 = field.MaxLines;
            }
            sql += " where Id=@v4";

            int n = Execute(
                sql,
                field.Name, CheckValue(field.Value, field), field.Order, field.Id, p1, p2);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        public void UpdateTemplateField(TemplateField field)
        {
            string sql = "update Template set [Type]=@v1, [Name]=@v2,  [Order]=@v3, [MinRange]=@v4, [MaxRange]=@v5 where [Id]=@v6";

            int n = Execute(
                sql,
                (int)field.Type, field.Name, field.Order, field.MinRange, field.MaxRange, field.Id);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        public int CreateTemplateField(TemplateField field)
        {
            int n = Execute(
                "Insert into Template ([Name], [CategoryId], [Type], [Order]) values (@v1, @v2, @v3, 3200)",
                field.Name, field.Category.Id, (int)field.Type, field.Order);

            if (n != 1) throw new DBEntityNotUpdatedException();
            int id = GetIdentity();
            field.Id = id;
            return id;
        }

        private readonly DateTime minDate = new DateTime(1753, 1, 1);
        private readonly DateTime maxDate = new DateTime(9999, 12, 31);

        private object CheckValue(object value, Field field)
        {
            switch (field.Type)
            {
                case FieldType.Text:
                case FieldType.Password:
                case FieldType.Memo:
                    if (value is string) return (string)value; else return "";

                case FieldType.Separator:
                case FieldType.Int:
                    if (value is int) return (int)value; else return default(int);

                case FieldType.Bool:
                    if (value is int)
                    {
                        return ((int)value) == 0 ? 0 : 1;
                    }
                    else return 0;

                case FieldType.Time:
                case FieldType.Date:
                    if (value is DateTime)
                    {
                        DateTime dt = (DateTime)value;
                        if (dt < minDate) dt = minDate;
                        if (dt > maxDate) dt = maxDate;
                        return dt;
                    }
                    else return maxDate;

                default:
                    throw new NotImplementedException();
            }
        }

        private object CheckOptValue(object value, Field field)
        {
            switch (field.Type)
            {
                case FieldType.Text:
                case FieldType.Password:
                case FieldType.Memo:
                    if (value is string) return (string)value; else return null;

                case FieldType.Separator:
                case FieldType.Int:
                    if (value is int) return (int)value; else return null;


                case FieldType.Time:
                case FieldType.Date:
                    if (value is DateTime)
                    {
                        DateTime dt = (DateTime)value;
                        if (dt < minDate) dt = minDate;
                        if (dt > maxDate) dt = maxDate;
                        return dt;
                    }
                    else return null;

                default:
                    return null;
            }
        }

        public int CreateField(Field field)
        {
            int n = Execute(
                string.Format("Insert into {0} (Name,[Value],PasswordId,Type,[Order]) values (@v1,@v2,@v3,@v4,@v5)", TableNameFromFieldType(field.Type)),
                field.Name, CheckValue(field.Value, field), field.Password.Id, (short)field.Type, (short)field.Order);

            if (n != 1) throw new DBEntityNotUpdatedException();
            return GetIdentity();
        }

        public void DeleteField(int id, FieldType type)
        {
            int n = Execute(
                string.Format("delete from {0}  where Id=@v1", TableNameFromFieldType(type)),
                id);
        }

        public IEnumerable<int> GetFieldIds(int passwordId, FieldType type)
        {
            return ReadData<int>(
                string.Format("select Id from {0} where PasswordId=@v1 order by [Order]", TableNameFromFieldType(type)),
                r => r.Field<int>("Id"),
                passwordId);
        }

        private Category ToCategory(DataRow row, Category parent)
        {
            return new Category(
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                row.Field<short>("Order"),
                parent);
        }

        private Category ToCategory2(DataRow row)
        {
            return new Category(
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                row.Field<short>("Order"),
                row.Field<int?>("ParentId"));
        }

        private Password ToPassword(DataRow row, Category category)
        {
            return new Password(
                category,
                row.Field<int>("Id"),
                row.Field<string>("Name"));
        }

        private Password ToPassword2(DataRow row)
        {
            return new Password(
                row.Field<int>("CategoryId"),
                row.Field<int>("Id"),
                row.Field<string>("Name"));
        }


        public Folder ToFolder(DataRow row, Folder parent)
        {
            var folder = new Folder(
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                parent);

            folder.Order = row.Field<short>("Order");
            return folder;
        }

        public Folder ToFolder2(DataRow row)
        {
            var folder = new Folder(
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                row.Field<int?>("ParentId"));
            folder.Order = row.Field<short>("Order");
            return folder;
        }


        public Field ToField(DataRow row, Password password)
        {
            Field field = new Field(password,
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                (FieldType)row.Field<short>("Type"),
                row["Value"],
                row.Field<short>("Order"));

            if (field.HasRange)
            {
                try
                {
                    field.SetRange(row["MinRange"], row["MaxRange"]);
                }
                catch (Exception)
                {
                }
            }
            if (field.HasLines)
            {
                try
                {
                    field.SetLines(row.Field<int?>("MinLines"), row.Field<int?>("MaxLines"));
                }
                catch (Exception)
                {
                }
            }
            return field;
        }

        public TemplateField ToTemplateField(DataRow row, Category category)
        {
            return new TemplateField(
                row.Field<int>("Id"),
                row.Field<string>("Name"),
                (FieldType)row.Field<short>("Type"),
                category,
                row.Field<string>("MinRange"),
                row.Field<string>("MaxRange"),
                row.Field<short>("Order")
                );
        }

        public IEnumerable<Category> GetRootCategories()
        {
            if (!IsDbAvailable()) return Enumerable.Empty<Category>();
            return ReadData<Category>(
                "select * from Category where ParentId is null order by [Order],Name",
                r => ToCategory(r, null));
        }

        private bool IsDbAvailable()
        {
            return IsConnected;
        }

        public IEnumerable<Category> GetCategories(Category parent)
        {
            return ReadData<Category>(
                "select * from Category where ParentId=@v1 order by [Order],Name",
                r => ToCategory(r, parent),
                parent.Id);
        }

        public void UpdatePassword(Password password)
        {
            int n = Execute("update Password set Name=@v1,Modified=@v2, [Order]=@v3 where Id=@v4",
                password.Name,
                DateTime.Now,
                password.Order,
                password.Id);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        public IEnumerable<Password> GetPasswordsByCategory(Category category)
        {
            return ReadData<Password>("select * from Password where CategoryId=@v1 order by [Order],Name",
                r => ToPassword(r, category),
                category.Id);
        }

        public IEnumerable<Folder> GetFolderByPasswordId(int passwordId)
        {
            return ReadData<Folder>("select f.* from Folder as f join Favorite as x on x.FolderId=f.Id where x.PasswordId=@v1",
                r => ToFolder2(r), passwordId);
        }

        public IEnumerable<Folder> GetFoldersByParentFolder(Folder parent)
        {
            return ReadData<Folder>("select * from Folder where ParentId=@v1 order by [Order],Name",
                r => ToFolder(r, parent),
                parent.Id);
        }

        public IEnumerable<Password> GetPasswordsByFolderId(int folderId)
        {
            return ReadData<Password>("select p.* from Password as p join Favorite as f on f.PasswordId=p.Id where f.FolderId=@v1",
                r => ToPassword2(r),
                folderId);
        }

        public void DeletePassword(int passwordId)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                Execute("delete from StringField where PasswordId=@v1", passwordId);
                Execute("delete from IntField where PasswordId=@v1", passwordId);
                Execute("delete from MemoField where PasswordId=@v1", passwordId);
                Execute("delete from DateField where PasswordId=@v1", passwordId);
                Execute("delete from Favorite where PasswordId=@v1", passwordId);
                int n = Execute("delete from Password where Id=@v1", passwordId);
                // if (n != 1) throw new DBEntityNotUpdatedException();
                transaction.Commit();
            }
        }

        public IEnumerable<Folder> GetRootFolders()
        {

            if (!IsDbAvailable()) return Enumerable.Empty<Folder>();
            return ReadData<Folder>("select * from Folder where ParentId is null order by [Order],Name",
                r => ToFolder(r, null));
        }

        public IEnumerable<Field> GetFields(Password password)
        {
            var f1 = GetFields(password, FieldType.Text).ToArray();
            var f2 = GetFields(password, FieldType.Int).ToArray();
            var f3 = GetFields(password, FieldType.Date).ToArray();
            var f4 = GetFields(password, FieldType.Memo).ToArray();

            var result = f1.Union(f2).Union(f2).Union(f3).Union(f4).OrderBy(f => f.Order).ToArray();
            return result;
        }

        private IEnumerable<Field> GetFields(Password password, FieldType fieldType)
        {
            return ReadData<Field>(string.Format("select  * from {0} where PasswordId=@v1", TableNameFromFieldType(fieldType)),
                r => ToField(r, password),
                password.Id);
        }

        public Category GetCategoryById(int categoryId)
        {
            var c = ReadData<Category>("select  * from Category where Id=@v1",
                r => ToCategory2(r),
                categoryId);

            return c.FirstOrDefault();
        }

        public Folder GetFolderById(int folderId)
        {
            var folders = ReadData<Folder>("select  * from Folder where Id=@v1",
                r => ToFolder2(r),
                folderId);

            return folders.FirstOrDefault();
        }

        public Folder CreateFolder(Folder folder)
        {
            int n = Execute("insert into Folder ([Order], [ParentId]) values (3200, @v1)", folder.Parent.Id);
            if (n != 1) throw new DBEntityNotUpdatedException();
            int id = GetIdentity();
            folder.Id = id;
            return folder;
        }

        public void DeleteFolder(int folderId)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                Execute("delete from Favorite where FolderId=@v1", folderId);
                Execute("delete from Folder where Id=@v1", folderId);
                transaction.Commit();
            }
        }

        public Category CreateCategory(Category category)
        {
            int n = Execute("insert into Category ([Order], [ParentId], [Name]) values (@v1, @v2, @v3)", (short)category.Order, category.Parent.Id, category.Name);
            if (n != 1) throw new DBEntityNotUpdatedException();
            int id = GetIdentity();
            category.Id = id;
            return category;
        }

        public void DeleteCategory(int categoryId)
        {
            using (var scope = Connection.BeginTransaction())
            {
                Execute("delete from Template where [CategoryId]=@v1", categoryId);
                Execute("delete from Category where [Id]=@v1", categoryId);
                scope.Commit();
            }
        }


        public void CreatePassword(Password password)
        {
            DateTime now = DateTime.Now;
            int n = Execute("insert into Password (Name,CategoryId,Created,Modified,[Order]) values(@v1,@v2,@v3,@v4,32700)",
                password.Name,
                password.Category.Id,
                now, now);

            if (n != 1) throw new DBEntityNotUpdatedException();
            password.Id = GetIdentity();
        }

        public IEnumerable<TemplateField> GetTemplates(Category category)
        {
            return ReadData<TemplateField>("select * from Template where CategoryId=@v1",
                r => ToTemplateField(r, category),
                category.Id).OrderBy(f => f.Order);
        }

        public void DeleteTemplateField(int id)
        {
            Execute("delete from Template where Id=@v1", id);
        }

        public Password GetPassword(int passwordId)
        {
            return ReadData<Password>("select  * from Password where Id=@v1",
                r => ToPassword2(r),
                passwordId).FirstOrDefault();
        }

        public IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        public void UpdateCategory(Category category)
        {
            int n = Execute("update Category set Name=@v1, [Order]=@v2 where Id=@v3",
                category.Name,
                category.Order,
                category.Id);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        internal void UpdateFolder(Folder folder)
        {
            int n = Execute("update Folder set Name=@v1, [Order]=@v2 where Id=@v3",
                folder.Name,
                folder.Order,
                folder.Id);
            if (n != 1) throw new DBEntityNotUpdatedException();
        }

        public IEnumerable<int> GetTemplateFields(int categoryId)
        {
            return ReadData<int>(
                "select Id from Template where CategoryId=@v1",
                row => row.Field<int>("Id"),
                categoryId);
        }

        internal bool ChangePassword(string connectionString, string oldPassword, string newPassword)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                builder.Password = oldPassword;
                SqlCeConnection con = new SqlCeConnection(builder.ConnectionString);
                SqlCeEngine engine = new SqlCeEngine(builder.ConnectionString);
                builder.Encrypt = true;

                builder.Password = newPassword;
                engine.Compact(builder.ConnectionString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
