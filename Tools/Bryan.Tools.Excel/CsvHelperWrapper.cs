using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CsvHelper;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using System.Linq;
using System.Globalization;

namespace Helpers
{
    public class CsvHelperWrapper
    {
        private readonly CsvConfiguration _configuration;
        private readonly string _path;

        public CsvHelperWrapper(string path) : this(path, false)
        {
        }
        public CsvHelperWrapper(string path, bool HasHeaderRecord)
        {
            _path = path;
            _configuration = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = HasHeaderRecord,
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = (it, index) => it.ToLower(),
                IgnoreQuotes = true,
            };
        }
        public CsvReader GetCsvReader()
        {
            FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new CsvReader(new StreamReader(fileStream), _configuration);
        }

        public CsvWriter GetCsvWriter(FileMode fileMode = FileMode.OpenOrCreate)
        {
            FileStream fileStream = new FileStream(_path, fileMode, FileAccess.Write, FileShare.Read);
            return new CsvWriter(new StreamWriter(fileStream), _configuration);
        }

        /// <summary>
        /// 批量读数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetRecords<T>()
        {
            using (var csv = GetCsvReader())
            {
                return csv.GetRecords<T>()?.ToList();
            }
        }

        /// <summary>
        /// 批量写数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        public void WriteRecords<T>(IEnumerable<T> records, FileMode fileMode = FileMode.OpenOrCreate)
        {
            using (var csv = GetCsvWriter())
            {
                csv.WriteRecords<T>(records);
            }
        }

        public async Task WriteRecord<T>(T record, FileMode fileMode = FileMode.OpenOrCreate)
        {
            using (var csv = GetCsvWriter(fileMode))
            {
                csv.WriteRecord<T>(record);
                await csv.NextRecordAsync();
            }
        }

        public async Task WriteHeader<T>(FileMode fileMode = FileMode.OpenOrCreate)
        {
            using (var csv = GetCsvWriter(fileMode))
            {
                csv.WriteHeader<T>();
                await csv.NextRecordAsync();
            }
        }

        /// <summary>
        /// 批量读数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetRecords<T>(T anonymousTypeDefinition)
        {
            using (var csv = GetCsvReader())
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        public class Goods
        {
            public string Asin { get; set; }
            public string Date { get; set; }
            public double? Rank { get; set; }
            public double? PredRank { get; set; }
            public double? RealRank { get; set; }
        }

        public class GoodsMap : ClassMap<Goods>
        {
            public GoodsMap()
            {
                Map(m => m.Asin).Name("asin");
                Map(m => m.Date).Name("day");
                Map(m => m.Rank).Name("sale");
            }
        }
    }
}
