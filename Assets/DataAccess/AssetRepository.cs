using Assets.Models;
using Dapper;
using System.Data;
namespace Assets.DataAccess
{
    public class AssetRepository
    {
        private readonly IDbConnection _connection;
        private readonly JournalEntryRepository _journalRepo;
        public AssetRepository(IDbConnection connection, JournalEntryRepository journalRepo)
        {
            _connection = connection;
            _journalRepo = journalRepo;
        }
        public List<Asset> GetAll()
        {
            return _connection.Query<Asset>("SELECT * FROM Assets ORDER BY AssetId DESC ").ToList();
        }
        public Asset GetById(int assetId)
        {
            string sql = "SELECT * FROM Assets WHERE AssetId = @Id";
            return _connection.QueryFirstOrDefault<Asset>(sql, new { Id = assetId });
        }
        public string GetLastAssetNumber()
        {
            string sql = "SELECT TOP 1 AssetNumber FROM Assets WHERE AssetNumber LIKE 'AST%' ORDER BY AssetId DESC";
            return _connection.QueryFirstOrDefault<string>(sql);
        }
        public Asset Insert(Asset asset)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string insertSql = @"
                    INSERT INTO Assets 
                    (AssetName, AssetDate, PurchaseValue, AssetAccountId, PurchaseAccountId)
                    VALUES 
                    (@AssetName, @AssetDate, @PurchaseValue, @AssetAccountId, @PurchaseAccountId);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                int newId = _connection.ExecuteScalar<int>(insertSql, asset, transaction);
                asset.AssetId = newId;
                string numberSql = "SELECT AssetNumber FROM Assets WHERE AssetId = @AssetId";
                asset.AssetNumber = _connection.ExecuteScalar<string>(numberSql, new { asset.AssetId }, transaction);
                _journalRepo.InsertDoubleEntry(
                    _connection,
                    transaction,
                    asset.AssetDate,
                    $"شراء أصل: {asset.AssetName}",
                    asset.AssetAccountId.ToString(),
                    asset.PurchaseAccountId.ToString(),
                    asset.PurchaseValue,
                    asset.AssetId
                );
                transaction.Commit();
                return asset;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void Update(Asset asset)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string updateSql = @"
                    UPDATE Assets SET 
                        AssetName = @AssetName,
                        AssetDate = @AssetDate,
                        PurchaseValue = @PurchaseValue,
                        AssetAccountId = @AssetAccountId,
                        PurchaseAccountId = @PurchaseAccountId
                    WHERE AssetId = @AssetId";
                _connection.Execute(updateSql, asset, transaction);
                string deleteJournal = "DELETE FROM JournalEntries WHERE AssetId = @AssetId";
                _connection.Execute(deleteJournal, new { AssetId = asset.AssetId }, transaction);
                _journalRepo.InsertDoubleEntry(
                    _connection,
                    transaction,
                    asset.AssetDate,
                    $"تحديث أصل: {asset.AssetName}",
                    asset.AssetAccountId.ToString(),
                    asset.PurchaseAccountId.ToString(),
                    asset.PurchaseValue,
                    asset.AssetId
                );
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
        public void Delete(int assetId)
        {
            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                string deleteAdditions = "DELETE FROM AssetAdditions WHERE AssetId = @AssetId";
                _connection.Execute(deleteAdditions, new { AssetId = assetId }, transaction);
                string deleteJournal = "DELETE FROM JournalEntries WHERE AssetId = @AssetId";
                _connection.Execute(deleteJournal, new { AssetId = assetId }, transaction);
                string deleteAsset = "DELETE FROM Assets WHERE AssetId = @AssetId";
                _connection.Execute(deleteAsset, new { AssetId = assetId }, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
