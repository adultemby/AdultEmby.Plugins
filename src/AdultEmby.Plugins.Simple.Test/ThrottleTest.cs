using System;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.TestLogging;
using MediaBrowser.Model.Logging;
using Xunit;

namespace AdultEmby.Plugins.Simple.Test
{
    public class ThrottleTest
    {
        [Theory]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(500)]
        [InlineData(1000)]
        public void GetNext(int interval)
        {
            var cancellationToken = new CancellationToken();
            var timeSpan = TimeSpan.FromMilliseconds(interval);
            var throttle = new Throttle(timeSpan, "", LogManager());
            TimeSpan delay1, delay2, delay3, delay4;
            throttle.GetNext(out delay1, cancellationToken);
            throttle.GetNext(out delay2, cancellationToken);
            Thread.Sleep(interval / 2);
            throttle.GetNext(out delay3, cancellationToken);
            throttle.GetNext(out delay4, cancellationToken);
            Assert.Equal(delay1, TimeSpan.Zero);
            AssertInRange(timeSpan, delay2, 1.0);
            AssertInRange(timeSpan, delay3, 1.5);
            AssertInRange(timeSpan, delay4, 2.5);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(200)]
        public async Task AwaitGetNext(int interval)
        {
            var cancellationToken = new CancellationToken();
            var timeSpan = TimeSpan.FromMilliseconds(interval);
            var throttle = new Throttle(timeSpan, "", LogManager());
            TimeSpan delay1, delay2, delay3;
            await throttle.GetNext(out delay1, cancellationToken);
            Assert.Equal(TimeSpan.Zero, delay1);
            await throttle.GetNext(out delay2, cancellationToken);
            AssertInRange(timeSpan, delay2, 1);
            await throttle.GetNext(out delay3, cancellationToken);
            AssertInRange(timeSpan, delay2, 1);
        }

        private static void AssertInRange(
            TimeSpan expected,
            TimeSpan actual,
            double multiplier,
            double fudgeFactor = 0.25)
        {
            var expectedMs = expected.TotalMilliseconds;
            var actualMs = actual.TotalMilliseconds;
            var low = expectedMs * (multiplier - fudgeFactor);
            var high = expectedMs * (multiplier + fudgeFactor);
            Assert.InRange(actualMs, low, high);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
