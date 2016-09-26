using System;
using System.Collections.Generic;

namespace WeddingApp
{
    public class TestData
    {
        private static Dictionary<Type, object> mTestData = new Dictionary<Type, object>();

        public static void Add(object _testData)
        {
            mTestData.Add(_testData.GetType(), _testData);
        }

        public static object Fill(object _runtimeData)
        {
            return mTestData[_runtimeData.GetType()];
        }
    }
}
