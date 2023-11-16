﻿using Anurgenlib;
using CansolveANK.CansolveModel;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CansolveANK.AnkurLibservises
{
    public class AnkurServises : ICan
    {
        public IMongoCollection<Cansolve> sampleData { get; set; }
        public AnkurServises(IOptions<MongoSocket> connect)
        {
            MongoClient client = new MongoClient(connect.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(connect.Value.DatabaseName);
            sampleData = database.GetCollection<Cansolve>(connect.Value.CollectionName);
        }
        /// <summary>
        /// @AnkDataraw
        /// </summary>
        /// <param name="EventTime"></param>
        /// <param name="EndEvenTime"></param>
        /// <returns></returns>
        public async Task<List<Cansolve>> GetByEvenTimeAsync(DateTime EventTime , DateTime EndEvenTime , string[] TagName)
        {

            var filter = Builders<Cansolve>.Filter.Gte(x => x.EventTime, EventTime) &
                     Builders<Cansolve>.Filter.Lte(x => x.EventTime, EndEvenTime) & Builders<Cansolve>.Filter.In("TagName", TagName);

            var result = await sampleData.Find(filter).ToListAsync();

            return result;
        }

        


        public Task<Cansolve> GetByIdAsync(string TagName)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// @AnkurMall_ExAdform_Avi
        /// </summary>
        /// <param name="startEVENTtIME"></param>
        /// <param name="endTime"></param>
        /// <param name="TagName"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public async Task<List<AggregationModelResult>> GetAvgValue(DateTime startEVENTtIME, DateTime endTime, string[] TagName, long frequency)
        {
            var connectionstring = "mongodb://10.2.10.5:27017";
            string databaseName = "RealTime";
            string CollectionsName = "CansolvData";
            if (startEVENTtIME == DateTime.MinValue)
            {
                return null;
            }
           
            var res = new CansolvJfrog(connectionstring, databaseName, CollectionsName);
            var resultList = await Task.Run(() => res.CalculateAverageForTimeInterval(startEVENTtIME, endTime, TagName, frequency)); 

          
            var result = resultList.Select(aggregationModel => new AggregationModelResult
            {
                //Id = aggregationModel.Id,
                Value = aggregationModel.Average,
                EventTime = aggregationModel.EventTime,
                TagName = aggregationModel.TagName
            }).OrderBy(item => item.EventTime).ToList();

            return result;
        }

        public async Task<List<Cansolve>> GetAsyncValueDos()
        {
            var res = await sampleData.Find(_=>true).ToListAsync(); 
            return res; 
        }
    }/// <summary>
/// @MongoSocket
/// </summary>
    public class MongoSocket
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
