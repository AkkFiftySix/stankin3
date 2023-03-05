﻿using CsvHelper;
using stankin3.Data;
using stankin3.Models;
using System.Globalization;

namespace stankin3.Job
{
    public class Job
    {
        private readonly DataContext _context;
        private readonly HttpClient _client;
        private readonly ILogger<Job> _logger;

        public Job(DataContext context,
            HttpClient client,
            ILogger<Job> logger)
        {
            _context = context;
            _client = client;
            _logger = logger;
        }

        public async Task FillRatesAsync()
        {
            try
            {
                var url = $"https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt?date={DateTime.Now.ToString("dd.MM.yyyy")}";
                var res = await _client.GetAsync(url);
                var resStr = await res.Content.ReadAsStringAsync();
                resStr = resStr.Replace('|', ',');

                using (var reader = new StringReader(resStr))
                {
                    reader.ReadLine();

                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<CurrentRate>().ToArray();
                        var rate = records.First(r => r.Code == "USD").Rate;
                        await _context.Rates.AddAsync(new Rate { Date = DateTime.Now, Value = rate });
                        await _context.SaveChangesAsync();
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Job error");
            }
        }
    }
}