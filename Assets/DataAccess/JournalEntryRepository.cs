using Assets.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace Assets.DataAccess
{
    public class JournalEntryRepository
    {
        private readonly IDbConnection _connection;
        public JournalEntryRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public void InsertDoubleEntry(
         IDbConnection conn,
         IDbTransaction transaction,
         DateTime date,
         string description,
         string debitAccountId,
         string creditAccountId,
         decimal amount,
         int? assetId = null, int? AdditionId = null)
        {
            string sql = @"
        INSERT INTO JournalEntries
        (EntryDate, Description, AccountId, Debit, Credit, AssetId , AdditionId)
        VALUES 
        (@EntryDate, @Description, @AccountId, @Debit, @Credit, @AssetId , @AdditionId)";

            var entries = new[]
            {
        new {
            EntryDate = date,
            Description = description,
            AccountId = debitAccountId,
            Debit = amount,
            Credit = 0m,
            AssetId = assetId ,
            AdditionId = AdditionId
        },
        new {
            EntryDate = date,
            Description = description,
            AccountId = creditAccountId,
            Debit = 0m,
            Credit = amount,
            AssetId = assetId ,
            AdditionId = AdditionId
        }
    };

            conn.Execute(sql, entries, transaction);
        }
        public List<JournalEntry> GetAll()
        {
            string sql = @"
            SELECT 
                je.EntryId,
                je.EntryDate,
                je.Description,
                je.AccountId,
                a.AccountName,
                je.Debit,
                je.Credit
            FROM JournalEntries je
            JOIN Accounts a ON je.AccountId = a.AccountId
            ORDER BY EntryId DESC";
            return _connection.Query<JournalEntry>(sql).AsList();
        }
    }
}
