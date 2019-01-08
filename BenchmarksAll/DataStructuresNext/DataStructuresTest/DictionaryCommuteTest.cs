using System;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataStructures;
using DataStructures.Utility;
using PexAPIWrapper;
namespace DataStructures.Comm.Test
{
    [PexClass(typeof(DataStructures.Dictionary<int, int>))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class DictionaryCommuteTest
    {
        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityRemoveRemoveComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int y)
        {
            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);
            PexAssume.IsTrue(-101 < x && x < 101);
            PexAssume.IsTrue(-101 < y && y < 101);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();

            AssumePrecondition.IsTrue(true);
            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));


            bool r1 = false, r2 = false;
            bool re1 = false, re2 = false;

            // First test
            r1 = s1.Remove(x);
            re1 = s1.Remove(y);

            // Second test 
            re2 = s2.Remove(y);
            r2 = s2.Remove(x);

            //NotpAssume.IsTrue(r1 == r2 && re1 == re2 && eq.Equals(s1,s2));
            PexAssert.IsTrue(r1 == r2 && re1 == re2 && eq.Equals(s1, s2));
        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityRemoveAddComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int y, int y1)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(-101 < x && x < 101);
            PexAssume.IsTrue(-101 < y && y < 101);
            PexAssume.IsTrue(-101 < y1 && y1 < 101);

            AssumePrecondition.IsTrue((((0 * s1.Count + -1 * x + 1 * y + 0 * y1 <= 0)) && ((((0 * s1.Count + -1 * x + 1 * y + 0 * y1 <= -1)) && (true)) || (!((0 * s1.Count + -1 * x + 1 * y + 0 * y1 <= -1)) && (false)))) || (!((0 * s1.Count + -1 * x + 1 * y + 0 * y1 <= 0)) && (true)));

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_y1", y1);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(y1)", s1.ContainsValue(y1));

            bool r1 = false, r2 = false;
            bool first = false, second = false;


            // First test
            r1 = s1.Remove(x);
            s1.Add(y, y1);



            // Second test 
            s2.Add(y, y1);
            r2 = s2.Remove(x);





            //NotpAssume.IsTrue(r1 == r2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(r1 == r2 && eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityAddAddComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int x1, int y, int y1)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(-101 < x && x < 101);
            PexAssume.IsTrue(-101 < x1 && x1 < 101);
            PexAssume.IsTrue(-101 < y && y < 101);
            PexAssume.IsTrue(-101 < y1 && y1 < 101);

            AssumePrecondition.IsTrue(true);

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_x1", x1);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_y1", y1);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(x1)", s1.ContainsValue(x1));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(y1)", s1.ContainsValue(y1));

            bool first = false, second = false;


            // First test
            s1.Add(x, x1);
            s1.Add(y, y1);



            // Second test 
            s2.Add(y, y1);
            s2.Add(x, x1);





            //NotpAssume.IsTrue(eq.Equals(s1, s2));
            PexAssert.IsTrue(eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityGetGetComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int y)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(-101 < x && x < 101);
            PexAssume.IsTrue(-101 < y && y < 101);

            AssumePrecondition.IsTrue(true);

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));

            int g1 = -1, g2 = -3;
            int ge1 = -1, ge2 = -3;
            bool first = false, second = false;


            // First test
            g1 = s1[x];
            ge1 = s1[y];


            first = true;


            // Second test 
            ge2 = s2[y];
            g2 = s2[x];






            //NotpAssume.IsTrue(ge1 == ge2 && g1 == g2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(ge1 == ge2 && g1 == g2 && eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityGetAddComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int y, int y1)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(-101 < x && x < 101);
            PexAssume.IsTrue(-101 < y && y < 101);
            PexAssume.IsTrue(-101 < y1 && y1 < 101);
            int val = -1;

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_y", y1);
            PexObserve.ValueForViewing("$input_val", s1.TryGetValue(x, out val) ? val : y1 - 7);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(y1)", s1.ContainsValue(y1));
            PexObserve.ValueForViewing("$input_s1.TryGetValue(x, out val)", s1.TryGetValue(x, out val));


            AssumePrecondition.IsTrue((((s1.ContainsKey(x))) && (true)) || (!((s1.ContainsKey(x))) && ((((s1.ContainsKey(y))) && (false)) || (!((s1.ContainsKey(y))) && ((((0 * s1.Count + -1 * x + 1 * y + 0 * y1 + 0 * val <= -1)) && (true)) || (!((0 * s1.Count + -1 * x + 1 * y + 0 * y1 + 0 * val <= -1)) && ((((0 * s1.Count + -1 * x + 1 * y + 0 * y1 + 0 * val <= 0)) && (false)) || (!((0 * s1.Count + -1 * x + 1 * y + 0 * y1 + 0 * val <= 0)) && (true)))))))));


            int g1 = -1, g2 = -3;
            bool first = false, second = false;
            bool firstKey = false, secondKey = false;

            // First test
            g1 = s1[x];
            s1.Add(y, y1);





            // Second test 
            s2.Add(y, y1);
            g2 = s2[x];









            //NotpAssume.IsTrue(g1 == g2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(g1 == g2 && eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativityGetRemoveComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int y)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);
            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            int val = -3;
            PexAssume.IsTrue(x > -101 && x < 101);
            PexAssume.IsTrue(y > -101 && y < 101);
            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));

            AssumePrecondition.IsTrue((((s1.ContainsKey(y))) && (false)) || (!((s1.ContainsKey(y))) && (true)));


            int g1 = -1, g2 = -3;
            bool r1 = false, r2 = false;
            bool first = false, second = false;


            // First test
            g1 = s1[x];
            r1 = s1.Remove(y);


            first = true;

            // Second test 
            r2 = s2.Remove(y);
            g2 = s2[x];






            //NotpAssume.IsTrue(r1 == r2 && g1 == g2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(r1 == r2 && g1 == g2 && eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativitySetSetComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int x1, int y, int y1)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(x > -101 && x < 101);
            PexAssume.IsTrue(y > -101 && y < 101);
            PexAssume.IsTrue(x1 > -101 && x1 < 101);
            PexAssume.IsTrue(y1 > -101 && y1 < 101);
            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_x1", x1);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_y", y1);

            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(x1)", s1.ContainsValue(x1));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(y1)", s1.ContainsValue(y1));


            AssumePrecondition.IsTrue((((s1.ContainsValue(y1))) && ((((s1.ContainsValue(x1))) && (true)) || (!((s1.ContainsValue(x1))) && ((((s1.ContainsKey(y))) && (false)) || (!((s1.ContainsKey(y))) && (true)))))) || (!((s1.ContainsValue(y1))) && (false)));


            bool firstKey = false, secondKey = false;
            bool first = false, second = false;


            // First test
            s1[x] = x1;
            s1[y] = y1;




            // Second test 
            s2[y] = y1;
            s2[x] = x1;









            //NotpAssume.IsTrue(eq.Equals(s1, s2));
            PexAssert.IsTrue(eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativitySetAddComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int x1, int y, int y1)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);
            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(x > -101 && x < 101);
            PexAssume.IsTrue(y > -101 && y < 101);
            PexAssume.IsTrue(x1 > -101 && x1 < 101);
            PexAssume.IsTrue(y1 > -101 && y1 < 101);
            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_x1", x1);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_y", y1);

            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(x1)", s1.ContainsValue(x1));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(y1)", s1.ContainsValue(y1));

            AssumePrecondition.IsTrue(true);

            bool firstKey = false, secondKey = false;
            bool first = false, second = false;

            // First test
            s1[x] = x1;
            s1.Add(y, y1);





            // Second test 
            s1.Add(y, y1);
            s2[x] = x1;









            //NotpAssume.IsTrue(eq.Equals(s1, s2));
            PexAssert.IsTrue(eq.Equals(s1, s2));

        }


        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativitySetRemoveComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int x1, int y)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);

            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();
            PexAssume.IsTrue(x > -101 && x < 101);
            PexAssume.IsTrue(y > -101 && y < 101);
            PexAssume.IsTrue(x1 > -101 && x1 < 101);

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_x1", x1);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(x1)", s1.ContainsValue(x1));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));


            AssumePrecondition.IsTrue((((s1.ContainsKey(y))) && (false)) || (!((s1.ContainsKey(y))) && ((((s1.ContainsKey(x))) && (true)) || (!((s1.ContainsKey(x))) && ((((0 * s1.Count + -1 * x + 0 * x1 + 1 * y <= -1)) && (true)) || (!((0 * s1.Count + -1 * x + 0 * x1 + 1 * y <= -1)) && ((((0 * s1.Count + -1 * x + 0 * x1 + 1 * y <= 0)) && (false)) || (!((0 * s1.Count + -1 * x + 0 * x1 + 1 * y <= 0)) && (true)))))))));


            bool r1 = false, r2 = false;
            bool firstKey = false, secondKey = false;
            bool first = false, second = false;

            // First test
            s1[x] = x1;
            r1 = s1.Remove(y);




            // Second test 
            r2 = s1.Remove(y);
            s2[x] = x1;









            //NotpAssume.IsTrue(r1 == r2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(r1 == r2 && eq.Equals(s1, s2));

        }

        //[PexMethod(TestEmissionFilter = PexTestEmissionFilter.All)]
        [PexMethod]
        public void PUT_CommutativitySetGetComm([PexAssumeUnderTest]DataStructures.Dictionary<int, int> s1, int x, int x1, int y)
        {

            Dictionary<int, int> s2 = new Dictionary<int, int>(s1, s1.Comparer);
            DictionaryEqualityComparer<int, int> eq = new DictionaryEqualityComparer<int, int>();

            PexObserve.ValueForViewing("$input_s1.Count", s1.Count);
            PexObserve.ValueForViewing("$input_x", x);
            PexObserve.ValueForViewing("$input_x1", x1);
            PexObserve.ValueForViewing("$input_y", y);
            PexObserve.ValueForViewing("$input_s1.ContainsKey(x)", s1.ContainsKey(x));
            PexObserve.ValueForViewing("$input_s1.ContainsValue(x1)", s1.ContainsValue(x1));
            PexObserve.ValueForViewing("$input_s1.ContainsKey(y)", s1.ContainsKey(y));

            AssumePrecondition.IsTrue(true);

            int r1 = -3, r2 = -3;
            bool firstKey = false, secondKey = false;
            bool first = false, second = false;

            // First test
            s1[x] = x1;
            r1 = s1[y];




            // Second test 
            r2 = s1[y];
            s2[x] = x1;








            //NotpAssume.IsTrue(r1 == r2 && eq.Equals(s1, s2));
            PexAssert.IsTrue(r1 == r2 && eq.Equals(s1, s2));

        }

    }
}