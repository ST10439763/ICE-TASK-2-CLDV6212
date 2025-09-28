using Azure;
using Azure.Data.Tables;
using TablesQuickStartMVC.Models;

namespace ABC_Retail.Services
{
    public class TableService
    {
        private readonly TableServiceClient _tableService;
        private TableClient _table;

        public TableService(IConfiguration cfg)
        {
            var conn = cfg["AzureStorage:ConnectionString"];
            _tableService = new TableServiceClient(conn);
            _tableService.CreateTableIfNotExists(typeof(Product).Name);
            _table = _tableService.GetTableClient(typeof(Product).Name);
        }

        public async Task ConfigureTableClient(string tableName)
        {
            try
            {
                await _tableService.CreateTableIfNotExistsAsync(tableName);
                _table = _tableService.GetTableClient(tableName);
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Table " + tableName + " could not be accessed nor created", e);
            }
        }

        public async Task AddEntityAsync<T>(T entity) where T : class?, ITableEntity
        {
            if (typeof(T).Name != _table.Name)
            {
                await ConfigureTableClient(typeof(T).Name);
            }

            if (string.IsNullOrEmpty(entity.PartitionKey) || string.IsNullOrEmpty(entity.RowKey))
            {
                throw new ArgumentException("Partion Key And RowKey must be set.");
            }

            try
            {
                await _table.AddEntityAsync<T>(entity);
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Error adding entity to Table Storage", e);
            }
        }

        public async Task DeleteAsync<T>(string partitionKey, string rowKey) where T : class?, ITableEntity
        {
            if (typeof(T).Name != _table.Name)
            {
                await ConfigureTableClient(typeof(T).Name);
            }

            try
            {
                await _table.DeleteEntityAsync(partitionKey, rowKey);
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Error deleting entity from Table Storage", e);
            }
        }

        public async Task UpdateEntityAsync<T>(T entity) where T : class?, ITableEntity
        {
            if (typeof(T).Name != _table.Name)
            {
                await ConfigureTableClient(typeof(T).Name);
            }

            if (string.IsNullOrEmpty(entity.PartitionKey) || string.IsNullOrEmpty(entity.RowKey))
            {
                throw new ArgumentException("Partion Key And RowKey must be set.");
            }

            try
            {
                await _table.UpdateEntityAsync(entity, ETag.All);
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Error updating entity to Table Storage", e);
            }
        }

        public async Task<List<T>> ListAllAsync<T>() where T : class?, ITableEntity
        {
            if (typeof(T).Name != _table.Name)
            {
                await ConfigureTableClient(typeof(T).Name);
            }

            List<T> listQuery = new List<T>();
            try
            {
                await foreach (T item in _table.QueryAsync<T>())
                {
                    listQuery.Add(item);
                }
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Error listing entities from Table Storage", e);
            }

            return listQuery;
        }

        public async Task<T> GetEntity<T>(string partionKey, string rowKey) where T : class?, ITableEntity
        {
            if (typeof(T).Name != _table.Name)
            {
                await ConfigureTableClient(typeof(T).Name);
            }

            T entity;
            try
            {
                entity = _table.GetEntity<T>(partionKey, rowKey);
            }
            catch (RequestFailedException e)
            {
                throw new InvalidOperationException("Error getting entitie from Table Storage", e);
            }
            return entity;
        }
    }
}

