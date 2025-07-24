using Assets.Models;
using Dapper;
using System.Data;
namespace Assets.DataAccess
{
    public class AssetAdditionRepository
    {
        private readonly IDbConnection _connection;
        private readonly AssetRepository _assetRepo;
        private readonly JournalEntryRepository _journalRepo;
        public AssetAdditionRepository(IDbConnection connection, AssetRepository assetRepo, JournalEntryRepository journalRepo)
        {
            _connection = connection;
            _assetRepo = assetRepo;
            _journalRepo = journalRepo;
        }
        public List<AssetAddition> GetAll()
        {
            string sql = @"
                SELECT aa.AdditionId, aa.AssetId, aa.AdditionDate, aa.AdditionValue, aa.Description,
                       a.AssetName
                FROM AssetAdditions aa
                INNER JOIN Assets a ON aa.AssetId = a.AssetId
                ORDER BY aa.AdditionId DESC";
            return _connection.Query<AssetAddition>(sql).AsList();
        }
        public AssetAddition GetById(int additionId)
        {
            string sql = "SELECT * FROM AssetAdditions WHERE AdditionId = @Id";
            return _connection.QueryFirstOrDefault<AssetAddition>(sql, new { Id = additionId });
        }
        public void Insert(AssetAddition addition)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string insertSql = @"
                    INSERT INTO AssetAdditions (AssetId, AdditionDate, AdditionValue, Description)
                    VALUES (@AssetId, @AdditionDate, @AdditionValue, @Description);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newId = _connection.ExecuteScalar<int>(insertSql, addition, transaction);
                addition.AdditionId = newId;
                var asset = _assetRepo.GetById(addition.AssetId);
                _journalRepo.InsertDoubleEntry(
                    _connection,
                    transaction,
                    addition.AdditionDate,
                    $"إضافة على الأصل: {asset.AssetName}",
                    asset.AssetAccountId.ToString(),
                    asset.PurchaseAccountId.ToString(),
                    addition.AdditionValue,
                    0,
                    addition.AdditionId
                );
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Update(AssetAddition addition)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string updateSql = @"
                    UPDATE AssetAdditions SET
                        AssetId = @AssetId,
                        AdditionDate = @AdditionDate,
                        AdditionValue = @AdditionValue,
                        Description = @Description
                    WHERE AdditionId = @AdditionId";

                _connection.Execute(updateSql, addition, transaction);

                string deleteJournal = "DELETE FROM JournalEntries WHERE AdditionId = @AdditionId";
                _connection.Execute(deleteJournal, new { AdditionId = addition.AdditionId }, transaction);
                var asset = _assetRepo.GetById(addition.AssetId);
                _journalRepo.InsertDoubleEntry(
                    _connection,
                    transaction,
                    addition.AdditionDate,
                    $"تعديل إضافة على الأصل: {asset.AssetName}",
                    asset.AssetAccountId.ToString(),
                    asset.PurchaseAccountId.ToString(),
                    addition.AdditionValue,
                    0,
                    addition.AdditionId
                );
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public void Delete(int additionId)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string deleteJournal = "DELETE FROM JournalEntries WHERE AdditionId = @AdditionId";
                _connection.Execute(deleteJournal, new { AdditionId = additionId }, transaction);
                string deleteAddition = "DELETE FROM AssetAdditions WHERE AdditionId = @AdditionId";
                _connection.Execute(deleteAddition, new { AdditionId = additionId }, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
