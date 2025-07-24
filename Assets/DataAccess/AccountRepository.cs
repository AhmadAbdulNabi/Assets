using Assets.Models;
using Dapper;
using System.Data;
namespace Assets.DataAccess
{
    public class AccountRepository
    {
        private readonly IDbConnection _connection;
        private readonly JournalEntryRepository _journalRepo;
        public AccountRepository(IDbConnection connection, JournalEntryRepository journalRepo)
        {
            _connection = connection;
            _journalRepo = journalRepo;
        }
        public List<Account> GetAll()
        {
            return _connection.Query<Account>("SELECT * FROM Accounts").ToList();
        }
        public Account GetById(int AccountId)
        {
            string sql = "SELECT * FROM Accounts WHERE AccountId = @Id";
            return _connection.QueryFirstOrDefault<Account>(sql, new { Id = AccountId });
        }
        public Account Insert(Account account)
        {
            _connection.Open();
            try
            {
                string insertSql = @"
                    INSERT INTO Accounts 
                    (AccountName)
                    VALUES 
                    (@AccountName);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                int newId = _connection.ExecuteScalar<int>(insertSql, account);
                account.AccountId = newId;
                return account;
            }
            catch
            {
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void Update(Account account)
        {
            _connection.Open();
            try
            {
                string updateSql = @"
                    UPDATE Accounts SET 
                        AccountName = @AccountName
                    WHERE AccountId = @AccountId";
                _connection.Execute(updateSql, account);
            }
            catch
            {
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void Delete(int AccountId)
        {
            _connection.Open();

            try
            {
                string delete = "DELETE FROM Accounts WHERE AccountId = @AccountId";
                _connection.Execute(delete, new { AccountId = AccountId });
            }
            catch
            {
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
