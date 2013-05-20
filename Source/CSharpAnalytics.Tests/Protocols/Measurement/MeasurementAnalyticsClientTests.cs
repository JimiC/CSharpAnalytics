﻿using System;
using System.Collections.Generic;
using System.Linq;
using CSharpAnalytics.Activities;
using CSharpAnalytics.Protocols.Measurement;
using CSharpAnalytics.Sessions;
#if WINDOWS_STORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CSharpAnalytics.Test.Protocols.Measurement
{
    [TestClass]
    public class MeasurementAnalyticsClientTests
    {
        [TestMethod]
        public void MeasurementAnalyticsClient_Replays_Tracked_Activities_After_Configured()
        {
            var actual = new List<Uri>();

            var client = new MeasurementAnalyticsClient();
            client.Track(new AppViewActivity("The Big Screen"));
            client.Track(new AppViewActivity("Silk Screen"));

            MeasurementTestHelpers.ConfigureForTest(client, actual.Add);
            
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_Track_Ends_AutoTimedEventActivity()
        {
            var client = new MeasurementAnalyticsClient();
            var autoTimedEvent = new AutoTimedEventActivity("Category", "Variable");

            client.Track(autoTimedEvent);

            Assert.IsNotNull(autoTimedEvent.EndedAt);
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomDimension_By_Int_Is_Sent()
        {
            var actual = new List<Uri>();
            var client = new MeasurementAnalyticsClient();
            MeasurementTestHelpers.ConfigureForTest(client, actual.Add);

            client.SetCustomDimension(5, "DimensionFive");
            client.TrackAppView("Test View");

            Assert.AreEqual(1, actual.Count);
            StringAssert.Contains(actual[0].Query, "cd5=DimensionFive");
        }

        private enum CustomDimensions
        {
            Eight = 8
        };

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomDimension_By_Enum_Is_Sent()
        {
            var actual = new List<Uri>();
            var client = new MeasurementAnalyticsClient();
            MeasurementTestHelpers.ConfigureForTest(client, actual.Add);

            client.SetCustomDimension(CustomDimensions.Eight, "DimensionEight");
            client.TrackAppView("Test View");

            Assert.AreEqual(1, actual.Count);
            StringAssert.Contains(actual[0].Query, "cd8=DimensionEight");
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomMetric_By_Int_Is_Sent()
        {
            var actual = new List<Uri>();
            var client = new MeasurementAnalyticsClient();
            MeasurementTestHelpers.ConfigureForTest(client, actual.Add);

            client.SetCustomMetric(6, 6060);
            client.TrackAppView("Test View");

            Assert.AreEqual(1, actual.Count);
            StringAssert.Contains(actual[0].Query, "cm6=6060");
        }

        private enum CustomMetrics
        {
            Eleven = 11
        };

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomMetric_By_Enum_Is_Sent()
        {
            var actual = new List<Uri>();
            var client = new MeasurementAnalyticsClient();
            MeasurementTestHelpers.ConfigureForTest(client, actual.Add);

            client.SetCustomMetric(CustomMetrics.Eleven, 110110);
            client.TrackAppView("Test View");

            Assert.AreEqual(1, actual.Count);
            StringAssert.Contains(actual[0].Query, "cm11=110110");
        }

        private enum NotIntBacked : long
        {
            SomeValue
        }

#if WINDOWS_STORE
        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomDimension_Throws_ArgumentException_If_Enum_Not_Underlying_Int_Type()
        {
            Assert.ThrowsException<ArgumentException>(() => new MeasurementAnalyticsClient().SetCustomDimension(NotIntBacked.SomeValue, "OneTwoThree"));
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomDimension_Throws_ArgumentOutOfRangeException_If_Enum_Not_Defined()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MeasurementAnalyticsClient().SetCustomDimension((CustomDimensions) 99, "Ninety-Nine"));
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomMetric_By_Enum_Throws_ArgumentException_If_Not_Underlying_Int_Type()
        {
            Assert.ThrowsException<ArgumentException>(() => new MeasurementAnalyticsClient().SetCustomMetric(NotIntBacked.SomeValue, 123));
        }

        [TestMethod]
        public void MeasurementAnalyticsClient_SetCustomMetric_Throws_ArgumentOutOfRangeException_If_Enum_Not_Defined()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MeasurementAnalyticsClient().SetCustomMetric((CustomMetrics)99, 990990));
        }
#endif

#if NET45
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MeasurementAnalyticsClient_SetCustomDimension_Throws_ArgumentException_If_Enum_Not_Underlying_Int_Type()
        {
            new MeasurementAnalyticsClient().SetCustomDimension(NotIntBacked.SomeValue, "OneTwoThree");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MeasurementAnalyticsClient_SetCustomDimension_Throws_ArgumentOutOfRangeException_If_Enum_Not_Defined()
        {
            new MeasurementAnalyticsClient().SetCustomDimension((CustomDimensions) 99, "Ninety-Nine");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MeasurementAnalyticsClient_SetCustomMetric_By_Enum_Throws_ArgumentException_If_Not_Underlying_Int_Type()
        {
            new MeasurementAnalyticsClient().SetCustomMetric(NotIntBacked.SomeValue, 123);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MeasurementAnalyticsClient_SetCustomMetric_Throws_ArgumentOutOfRangeException_If_Enum_Not_Defined()
        {
            new MeasurementAnalyticsClient().SetCustomMetric((CustomMetrics)99, 990990);
        }
#endif
    }
}