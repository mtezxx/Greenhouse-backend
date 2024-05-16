using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DaoInterfaces;
using Domain.Entity;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.EfcDataAccess.DAOs
{
    public class MeasurementDaoTest
    {
        private DbContextOptions<EfcContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<EfcContext>();
            builder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllMeasurements()
        {
            var options = CreateNewContextOptions();

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);
                context.Add(new Temperature { Value = 22.1, Time = DateTime.Now, Type = "Temperature" });
                context.Add(new Temperature { Value = 23.5, Time = DateTime.Now, Type = "Temperature" });
                context.SaveChanges();
            }

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);
                var results = await dao.GetAllAsync();

                Assert.Equal(2, results.Count);
                Assert.All(results, item => Assert.IsType<Temperature>(item));
            }
        }

        [Fact]
        public async Task AddAsync_AddsMeasurementSuccessfully()
        {
            var options = CreateNewContextOptions();

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);
                var temperature = new Temperature { Value = 25.5, Time = DateTime.Now, Type = "Temperature" };

                var result = await dao.AddAsync(temperature);
                await context.SaveChangesAsync();

                Assert.Equal(25.5, result.Value);
                Assert.NotEqual(default, result.Id); // ID should be set by EF Core
            }

            using (var context = new EfcContext(options))
            {
                var measurementInDb = await context.Set<Temperature>().FirstOrDefaultAsync();
                Assert.NotNull(measurementInDb);
                Assert.Equal(25.5, measurementInDb.Value);
            }
        }

        [Fact]
        public async Task GetLatestAsync_ReturnsLatestMeasurement()
        {
            var options = CreateNewContextOptions();

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);
                context.Add(new Temperature { Value = 22.1, Time = DateTime.Now.AddHours(-1), Type = "Temperature" });
                context.Add(new Temperature { Value = 23.5, Time = DateTime.Now, Type = "Temperature" });
                context.SaveChanges();
            }

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);
                var result = await dao.GetLatestAsync("Temperature");

                Assert.NotNull(result);
                Assert.Equal(23.5, result.Value);
            }
        }

        [Fact]
        public async Task GetLatestAsync_ThrowsException_WhenNoMeasurementFound()
        {
            var options = CreateNewContextOptions();

            using (var context = new EfcContext(options))
            {
                var dao = new MeasurementDao<Temperature>(context);

                await Assert.ThrowsAsync<Exception>(() => dao.GetLatestAsync("Temperature"));
            }
        }
    }
}
